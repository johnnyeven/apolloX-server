using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using MySql.Data.MySqlClient;

namespace LoginServer
{
    class Program
    {
        const int CONTROLLER_INFO = 0;
        const int ACTION_LOGIN = 1;
        const int ACTION_LOGOUT = 2;
        const int ACTION_QUICK_START = 8;
        const int ACTION_REQUEST_CHARACTER = 4;

        const int ACK_CONFIRM = 1;
        const int ACK_ERROR = 0;
        const int ORDER_CONFIRM = 2;

        const int TYPE_INT = 0;
        const int TYPE_LONG = 1;
        const int TYPE_STRING = 2;
        const int TYPE_FLOAT = 3;

        private static MySqlConnection productDbConnection;
        private static MySqlConnection dbConnection;
        private static MySqlConnection insertConnection;

        [STAThread]
        static void Main(string[] args)
        {
            String connectionString = "Data Source=localhost;Initial Catalog=pulse_db_platform;User ID=root;Password=84@41%%wi96^4";
            dbConnection = new MySqlConnection(connectionString);
            dbConnection.Open();

            ThreadStart starter = new ThreadStart(listen);
            Thread listenThread = new Thread(starter);
            listenThread.Start();

            Thread.Sleep(100);
            Console.ReadLine();
        }

        public static void listen()
        {
            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Any, 9040);
            Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            server.Bind(ipEndPoint);
            server.Listen(10);
            while (true)
            {
                Console.WriteLine("等待客户端登陆连接...");
                Socket client = server.Accept();
                IPEndPoint clientEnpPoint = client.RemoteEndPoint as IPEndPoint;
                Console.WriteLine("客户端已连接, IP:" + clientEnpPoint.Address + ", 端口:" + clientEnpPoint.Port);
                Thread acceptThread = new Thread(new ParameterizedThreadStart(accept));
                acceptThread.Start(client);
            }
        }

        public static void accept(object arg)
        {
            Socket client = (Socket)arg;
            int receiveDataLength;
            byte[] receiveData;
            while (true)
            {
                receiveData = new byte[5120];
                receiveDataLength = client.Receive(receiveData);
                if (receiveDataLength == 0)
                {
                    break;
                }

                if (receiveDataLength > 0)
                {
                    int controller, action;

                    //取得操作数
                    controller = receiveData[0] >> 4;
                    action = receiveData[0] & 15;

                    if (controller == CONTROLLER_INFO)
                    {
                        if (action == ACTION_LOGIN)
                        {
                            requestLogin(receiveData, receiveDataLength, client);
                        }
                        else if (action == ACTION_REQUEST_CHARACTER)
                        {
                            requestCharacter(receiveData, receiveDataLength, client);
                        }
                        else if (action == ACTION_QUICK_START)
                        {
                            requestQuickStart(receiveData, receiveDataLength, client);
                        }
                    }
                }
            }
        }
        /*
        static void Main(string[] args)
        {
            String connectionString = "Data Source=localhost;Initial Catalog=wooha_character_db;User ID=root;Password=100200";
            dbConnection = new MySqlConnection(connectionString);
            dbConnection.Open();

            int receiveDataLength;
            byte[] receiveData;

            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Any, 9040);
            Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            server.Bind(ipEndPoint);
            server.Listen(10);

            while (true)
            {
                Console.WriteLine("等待客户端登陆连接...");
                Socket client = server.Accept();
                IPEndPoint clientEnpPoint = client.RemoteEndPoint as IPEndPoint;
                Console.WriteLine("客户端已连接, IP:" + clientEnpPoint.Address + ", 端口:" + clientEnpPoint.Port);

                while (true)
                {
                    receiveData = new byte[5120];
                    receiveDataLength = client.Receive(receiveData);
                    if (receiveDataLength == 0)
                    {
                        break;
                    }

                    if (receiveDataLength > 0)
                    {
                        int controller, action;

                        //取得操作数
                        controller = receiveData[0] >> 4;
                        action = receiveData[0] & 15;

                        if (controller == CONTROLLER_INFO)
                        {
                            if (action == ACTION_LOGIN)
                            {
                                requestLogin(receiveData, receiveDataLength, client);
                            }
                            else if (action == ACTION_REQUEST_CHARACTER)
                            {
                                requestCharacter(receiveData, receiveDataLength, client);
                            }
                        }
                    }
                }
                Console.WriteLine("客户端已断开");
                Console.WriteLine();
                client.Close();
            }
        }
        */

        static void requestQuickStart(byte[] receiveData, int receiveDataLength, Socket client)
        {
            IPEndPoint clientEnpPoint = client.RemoteEndPoint as IPEndPoint;
            int gameId = int.MinValue;
            for (int i = 1; i < receiveDataLength; )
            {
                int length = BitConverter.ToInt32(new byte[] { receiveData[i + 3], receiveData[i + 2], receiveData[i + 1], receiveData[i] }, 0);
                int type = receiveData[i + 4];
                switch (type)
                {
                    case TYPE_INT:
                        if (gameId == int.MinValue)
                        {
                            gameId = BitConverter.ToInt32(new byte[] { receiveData[i + 8], receiveData[i + 7], receiveData[i + 6], receiveData[i + 5] }, 0);
                        }
                        break;
                }
                i += (length + 5);
            }
            Console.WriteLine("[QuickStart] GameId: " + gameId);
            if (gameId != int.MinValue)
            {
                String guid = System.Guid.NewGuid().ToString("N");
                String name = "Guest" + guid;
                String pass = GetMD5(guid);
                int serverId = int.MinValue;
                Console.WriteLine("[QuickStart] Name: " + name + ", Pass: " + pass);

                String connectionString = "Data Source=localhost;Initial Catalog=pulse_db_game;User ID=root;Password=84@41%%wi96^4";
                productDbConnection = new MySqlConnection(connectionString);
                productDbConnection.Open();

                MySqlCommand command = new MySqlCommand();
                command.Connection = productDbConnection;
                command.CommandText = "select * from game_server where game_id=" + gameId + " and server_recommend=1";
                MySqlDataReader serverResult = command.ExecuteReader();
                if (serverResult.HasRows)
                {
                    serverResult.Read();
                    serverId = serverResult.GetInt32("account_server_id");
                }
                else
                {
                    serverResult.Close();
                    command.CommandText = "select * from game_server where game_id=" + gameId + " order by account_count desc";
                    serverResult = command.ExecuteReader();
                    if (serverResult.Read())
                    {
                        serverId = serverResult.GetInt32("account_server_id");
                    }
                }
                serverResult.Close();
                serverResult.Dispose();
                productDbConnection.Close();
                productDbConnection.Dispose();
                
		        if(name != "" && pass != "")
                {
                    command.Connection = dbConnection;
                    command.CommandText = "insert into pulse_account(account_name, account_pass) values ('" + name + "', '" + pass + "')";
                    command.ExecuteNonQuery();
                    int insertId = (int)command.LastInsertedId;
/*
					$jsonData = Array(
							'message'	=>	ACK_SUCCESS,
							'user'		=>	$user,
							'account_id'=>	$accountId,
							'nick_name'	=>	$nickName
					);
 */
                    CServerPackage ackSuccess = new CServerPackage();
                    ackSuccess.success = ACK_CONFIRM;
                    ackSuccess.controller = CONTROLLER_INFO;
                    ackSuccess.action = ACTION_QUICK_START;
                    ackSuccess.param.Add(new Object[] { 4, insertId });
                    ackSuccess.param.Add(new Object[] { name.Length, name });
                    ackSuccess.param.Add(new Object[] { pass.Length, pass });

                    sendPackage(client, ackSuccess);
                }
            }
        }

        static void requestLogin(byte[] receiveData, int receiveDataLength, Socket client)
        {
            IPEndPoint clientEnpPoint = client.RemoteEndPoint as IPEndPoint;
            String userName = "";
            String userPass = "";
            for (int i = 1; i < receiveDataLength; )
            {
                int length = BitConverter.ToInt32(new byte[] { receiveData[i + 3], receiveData[i + 2], receiveData[i + 1], receiveData[i] }, 0);
                int type = receiveData[i + 4];
                switch (type)
                {
                    case TYPE_STRING:
                        if (userName == "")
                        {
                            userName = Encoding.UTF8.GetString(receiveData, i + 5, length);
                        }
                        else if (userPass == "")
                        {
                            userPass = Encoding.UTF8.GetString(receiveData, i + 5, length);
                        }
                        break;
                }
                i += (length + 5);
            }
            Console.WriteLine("UserName: " + userName + ", UserPass: " + userPass);
            if (userName != "" && userPass != "")
            {
                MySqlCommand command = new MySqlCommand();
                command.Connection = dbConnection;
                command.CommandText = "select * from pulse_account where account_name='" + userName + "' and account_pass='" + userPass + "'";
                MySqlDataReader dataReader = command.ExecuteReader();
                if (dataReader.HasRows)
                {
                    dataReader.Close();
                    Console.WriteLine("验证通过");

                    command.CommandText = "select account_id from pulse_account where account_name='" + userName + "'";
                    int userId = Convert.ToInt32(command.ExecuteScalar());

                    String authKey = System.Guid.NewGuid().ToString("B");
                    long timestamp = ConvertDateTimeInt(DateTime.Now);
                    command.CommandText = "insert into login_certificate values (" + userId + ", '" + clientEnpPoint.Address.ToString() + "', '" + authKey + "', " + timestamp + ")";
                    command.ExecuteNonQuery();
                    //负载均衡 获取空闲的GameServer 并指定IP
                    String serverIP = "loverjohnny.3322.org";
                    int serverPort = 9050;

                    CServerPackage ackSuccess = new CServerPackage();
                    ackSuccess.success = ACK_CONFIRM;
                    ackSuccess.controller = CONTROLLER_INFO;
                    ackSuccess.action = ACTION_LOGIN;
                    ackSuccess.param.Add(new Object[] { authKey.Length, authKey });
                    ackSuccess.param.Add(new Object[] { 4, userId });
                    ackSuccess.param.Add(new Object[] { serverIP.Length, serverIP });
                    ackSuccess.param.Add(new Object[] { 4, serverPort });

                    sendPackage(client, ackSuccess);
                }
                else
                {
                    Console.WriteLine("验证失败");
                    CServerPackage ackPackage = new CServerPackage();
                    ackPackage.success = ACK_ERROR;
                    ackPackage.controller = CONTROLLER_INFO;
                    ackPackage.action = ACTION_LOGIN;

                    sendPackage(client, ackPackage);
                }
            }
        }

        static void requestCharacter(byte[] receiveData, int receiveDataLength, Socket client)
        {
            IPEndPoint clientEnpPoint = client.RemoteEndPoint as IPEndPoint;
            String authKey = "";
            int accountId = int.MinValue;
            for (int i = 1; i < receiveDataLength; )
            {
                int length = BitConverter.ToInt32(new byte[] { receiveData[i + 3], receiveData[i + 2], receiveData[i + 1], receiveData[i] }, 0);
                int type = receiveData[i + 4];
                switch (type)
                {
                    case TYPE_STRING:
                        authKey = Encoding.UTF8.GetString(receiveData, i + 5, length);
                        break;
                    case TYPE_INT:
                        accountId = BitConverter.ToInt32(new byte[] { receiveData[i + 8], receiveData[i + 7], receiveData[i + 6], receiveData[i + 5] }, 0);
                        break;
                }
                i += (length + 5);
            }
            Console.WriteLine("AuthKey: " + authKey + ", AccountId: " + accountId);
            if (authKey != "" && accountId != int.MinValue)
            {
                MySqlCommand command = new MySqlCommand();
                command.Connection = dbConnection;
                command.CommandText = "delete from login_certificate where account_id=" + accountId + " and account_ip='" + clientEnpPoint.Address.ToString() + "' and auth_key='" + authKey + "'";
                command.ExecuteNonQuery();

                command.CommandText = "select * from game_character where account_id=" + accountId;
                MySqlDataReader dataReader = command.ExecuteReader();
                if (dataReader.HasRows)
                {
                    String connectionString = "Data Source=localhost;Initial Catalog=wooha_character_db;User ID=root;Password=100200";
                    insertConnection = new MySqlConnection(connectionString);
                    insertConnection.Open();

                    while (dataReader.Read())
                    {
                        Console.WriteLine("找到一个角色");
                        CServerPackage package = new CServerPackage();
                        package.success = ACK_CONFIRM;
                        package.controller = CONTROLLER_INFO;
                        package.action = ACTION_REQUEST_CHARACTER;

                        String guid = dataReader.GetString("GUID");
                        package.param.Add(new Object[] { guid.Length, guid });
                        String characterName = dataReader.GetString("character_name");
                        package.param.Add(new Object[] { Encoding.UTF8.GetBytes(characterName).Length, characterName });
                        String resourceId = dataReader.GetString("resource_id");
                        package.param.Add(new Object[] { resourceId.Length, resourceId });
                        int direction = dataReader.GetInt32("character_direction");
                        package.param.Add(new Object[] { 4, direction });
                        int posX = dataReader.GetInt32("character_posx");
                        package.param.Add(new Object[] { 4, posX });
                        int posY = dataReader.GetInt32("character_posy");
                        package.param.Add(new Object[] { 4, posY });
                        int speed = dataReader.GetInt32("character_speed");
                        package.param.Add(new Object[] { 4, speed });
                        int healthMax = dataReader.GetInt32("character_health_max");
                        package.param.Add(new Object[] { 4, healthMax });
                        int health = dataReader.GetInt32("character_health");
                        package.param.Add(new Object[] { 4, health });
                        int manaMax = dataReader.GetInt32("character_mana_max");
                        package.param.Add(new Object[] { 4, manaMax });
                        int mana = dataReader.GetInt32("character_mana");
                        package.param.Add(new Object[] { 4, mana });
                        int energyMax = dataReader.GetInt32("character_energy_max");
                        package.param.Add(new Object[] { 4, energyMax });
                        int energy = dataReader.GetInt32("character_energy");
                        package.param.Add(new Object[] { 4, energy });
                        int attackRange = dataReader.GetInt32("character_attackrange");
                        package.param.Add(new Object[] {4, attackRange});
                        float attackSpeed = dataReader.GetFloat("character_attackspeed");
                        package.param.Add(new Object[] {4, attackSpeed});

                        authKey = System.Guid.NewGuid().ToString("B");
                        package.param.Add(new Object[] { authKey.Length, authKey });
                        long timestamp = ConvertDateTimeInt(DateTime.Now);

                        command.Connection = insertConnection;
                        command.CommandText = "insert into character_certificate values('" + guid + "', '" + clientEnpPoint.Address.ToString() + "', '" + authKey + "', " + timestamp + ")";
                        command.ExecuteNonQuery();

                        sendPackage(client, package);
                    }
                }
                else
                {
                    Console.WriteLine("没有角色，需要创建");
                    CServerPackage package = new CServerPackage();
                    package.success = ACK_ERROR;
                    package.controller = CONTROLLER_INFO;
                    package.action = ACTION_REQUEST_CHARACTER;

                    sendPackage(client, package);
                }
                dataReader.Close();
                insertConnection.Close();
                insertConnection.Dispose();
            }
        }

        static void sendPackage(Socket socket, CServerPackage package)
        {
            int dataLength = 0;

            byte[] result = new byte[1024];
            result[4] = (byte)package.success;
            result[5] = (byte)package.controller;
            result[6] = (byte)package.action;
            dataLength += 3;

            int resultOffset = 7;
            for (int i = 0; i < package.param.Count; i++)
            {
                object[] parameter = (object[])package.param[i];

                int length = (int)parameter[0];
                byte[] lengthBytes = BitConverter.GetBytes(length);
                lengthBytes.CopyTo(result, resultOffset);
                dataLength += 4;
                resultOffset += 4;

                if (parameter[1].GetType() == typeof(String))
                {
                    result[resultOffset] = TYPE_STRING;
                    dataLength += 1;
                    resultOffset += 1;

                    byte[] bytes = Encoding.UTF8.GetBytes((String)parameter[1]);
                    bytes.CopyTo(result, resultOffset);
                    dataLength += length;
                    resultOffset += length;
                }
                else if (parameter[1].GetType() == typeof(int))
                {
                    result[resultOffset] = TYPE_INT;
                    dataLength += 1;
                    resultOffset += 1;

                    byte[] bytes = BitConverter.GetBytes((int)parameter[1]);
                    bytes.CopyTo(result, resultOffset);
                    dataLength += 4;
                    resultOffset += 4;
                }
                else if (parameter[1].GetType() == typeof(float))
                {
                    result[resultOffset] = TYPE_FLOAT;
                    dataLength += 1;
                    resultOffset += 1;
                    byte[] bytes = BitConverter.GetBytes((float)parameter[1]);
                    bytes.CopyTo(result, resultOffset);
                    dataLength += 4;
                    resultOffset += 4;
                }
                else if (parameter[1].GetType() == typeof(long))
                {
                    result[resultOffset] = TYPE_LONG;
                    dataLength += 1;
                    resultOffset += 1;
                    byte[] bytes = BitConverter.GetBytes((long)parameter[1]);
                    bytes.CopyTo(result, resultOffset);
                    dataLength += 8;
                    resultOffset += 8;
                }
            }
            byte[] packageLength = BitConverter.GetBytes(dataLength);
            packageLength.CopyTo(result, 0);
            dataLength += 4;
            socket.Send(result, dataLength, SocketFlags.None);
        }

        /// <summary>
        /// 将c# DateTime时间格式转换为Unix时间戳格式
        /// </summary>
        /// <param name="time">时间</param>
        /// <returns>long</returns>
        public static long ConvertDateTimeInt(System.DateTime time)
        {
            //double intResult = 0;
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));
            //intResult = (time- startTime).TotalMilliseconds;
            long t = (time.Ticks - startTime.Ticks) / 10000;            //除10000调整为13位
            return t;
        }

        public static string GetMD5(string str)
        {
            string str1 = "";
            byte[] data = Encoding.GetEncoding("utf-8").GetBytes(str);
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] bytes = md5.ComputeHash(data);
            for (int i = 0; i < bytes.Length; i++)
            {
                str1 += bytes[i].ToString("x2");
            }
            return str1;
        }
    }
}

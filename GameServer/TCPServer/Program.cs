using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using MySql.Data.MySqlClient;

using Wooha.Objects;
using Wooha.Utils;
using Wooha.InitData;
using Continuum.Threading;
using WPFLuaFramework;
using Wooha.Api;
using System.Runtime.InteropServices;

namespace GameServer
{
    class Program
    {
        public static CObjectCollection objectList;
        public static ObjectsData objectsData;
        private static MySqlConnection dbConnection;

        const int CONTROLLER_BATTLE = 3;
        const int CONTROLLER_MSG = 2;
        const int CONTROLLER_MOVE = 1;
        const int CONTROLLER_INFO = 0;
        const int NPC_CONTROLLER_BATTLE = 13;
        const int NPC_CONTROLLER_MOVE = 11;
        /*
         * Action
         */
        //MOVE
        const int ACTION_MOVETO = 0;
        const int ACTION_MOVE = 1;
        //MSG
        const int ACTION_PUBLIC_MSG = 0;
        const int ACTION_PRIVATE_MSG = 1;
        //BATTLE
        const int ACTION_ATTACK = 0;
        const int ACTION_PREPARE_ATTACK = 2;
        const int ACTION_UNDERATTACK = 1;
        const int ACTION_SING = 3;
        //INFO
        const int ACTION_CAMERAVIEW_OBJECT_LIST = 0;
        const int ACTION_CHANGE_ACTION = 3;
        const int ACTION_LOGIN = 1;
        const int ACTION_LOGOUT = 2;
        const int ACTION_INIT_CHARACTER = 6;
        const int ACTION_CHANGE_DIRECTION = 7;

        const int ACK_CONFIRM = 1;
        const int ACK_ERROR = 0;
        const int ORDER_CONFIRM = 2;

        const int TYPE_INT = 0;
        const int TYPE_LONG = 1;
        const int TYPE_STRING = 2;
        const int TYPE_FLOAT = 3;
        const int TYPE_BOOL = 4;

        [STAThread]
        static void Main(String[] args)
        {
            objectsData = new ObjectsData();
            objectList = new CObjectCollection();
            String connectionString = "Data Source=localhost;Initial Catalog=pulse_db_game;User ID=root;Password=84@41%%wi96^4";
            dbConnection = new MySqlConnection(connectionString);
            dbConnection.Open();
            /*
            initCollection();
            LuaFramework frame = new LuaFramework();
            frame.BindLuaAPIFunction(new testApi());
            frame.BindLuaAPIFunction(new InitApi());
            frame.BindLuaAPIFunction(new MapApi());
            frame.ExecuteFile("main.lua");
            */
            ThreadStart tStart = new ThreadStart(listen);
            Thread serverThread = new Thread(tStart);
            serverThread.Start();

            Thread.Sleep(100);
            Console.ReadLine();
        }
        
        static void listen()
        {
            IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 9050);//本机预使用的IP和端口
            Socket newsock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            newsock.Bind(ipep);//绑定
            newsock.Listen(10);//监听
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("等待客户端连接...");
                Socket client = newsock.Accept();//当有可用的客户端连接尝试时执行，并返回一个新的socket,用于与客户端之间的通信
                IPEndPoint clientip = (IPEndPoint)client.RemoteEndPoint;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("客户端已连接, IP:" + clientip.Address + ", 端口:" + clientip.Port);
                Thread acceptThread = new Thread(new ParameterizedThreadStart(receiveData));
                acceptThread.Start(client); 
            }
        }

        /*
        static void Main(string[] args)
        {
            objectList = new CObjectCollection();
            String connectionString = "Data Source=localhost;Initial Catalog=wooha_character_db;User ID=root;Password=100200";
            dbConnection = new MySqlConnection(connectionString);
            dbConnection.Open();
            initCollection();

            IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 9050);//本机预使用的IP和端口
            Socket newsock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            newsock.Bind(ipep);//绑定
            newsock.Listen(10);//监听

            unsafe
            {
                IOCPThreadPool pThreadPool = new IOCPThreadPool(0, 5, 10, new IOCPThreadPool.USER_FUNCTION(IOCPThreadFunction));

                while (true)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("等待客户端连接...");
                    Socket client = newsock.Accept();//当有可用的客户端连接尝试时执行，并返回一个新的socket,用于与客户端之间的通信
                    pThreadPool.PostEvent(client);
                    //Thread acceptThread = new Thread(new ParameterizedThreadStart(receiveData));
                    //acceptThread.Start(client);
                }

                Thread.Sleep(10);
                pThreadPool.Dispose();
            }
            Console.ReadKey();
        }
        */
        public unsafe static void IOCPThreadFunction(int iValue, Socket client)
        {
            IPEndPoint clientip = (IPEndPoint)client.RemoteEndPoint;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("客户端已连接, IP:" + clientip.Address + ", 端口:" + clientip.Port);
            try
            {
                Console.WriteLine("Value: {0}", iValue);
            }
            catch (Exception pException)
            {
                Console.WriteLine(pException.Message);
            }
        }

        /*
        static void Main(string[] args)
        {
            objectList = new CObjectCollection();
            String connectionString = "Data Source=localhost;Initial Catalog=wooha_character_db;User ID=root;Password=100200";
            dbConnection = new MySqlConnection(connectionString);
            dbConnection.Open();
            initCollection();

            byte[] data = new byte[1024];//用于缓存客户端所发送的信息,通过socket传递的信息必须为字节数组
            IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 9050);//本机预使用的IP和端口
            Socket newsock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            newsock.Bind(ipep);//绑定
            newsock.Listen(10);//监听
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("waiting for a client");
                Socket client = newsock.Accept();//当有可用的客户端连接尝试时执行，并返回一个新的socket,用于与客户端之间的通信
                IPEndPoint clientip = (IPEndPoint)client.RemoteEndPoint;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("connect with client:" + clientip.Address + " at port:" + clientip.Port);
                string welcome = "welcome here!";
                data = Encoding.ASCII.GetBytes(welcome);
                //client.Send(data, data.Length, SocketFlags.None);//发送信息
                receiveData(client);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Disconnected from" + clientip.Address);
                client.Close();
            }
            newsock.Close();
        }
        */

        static public void receiveData(object arg)
        {
            Socket client = (Socket)arg;
            IPEndPoint clientip = (IPEndPoint)client.RemoteEndPoint;
            while (true)
            {
                //用死循环来不断的从客户端获取信息
                byte[] data = new byte[1024];//用于缓存客户端所发送的信息,通过socket传递的信息必须为字节数组
                int recv;//用于表示客户端发送的信息长度
                data = new byte[10240];
                try
                {
                    recv = client.Receive(data);
                }
                catch (SocketException err)
                {
                    Console.WriteLine(err.Message);
                    return;
                }

                if (recv == 0)//当信息长度为0，说明客户端连接断开
                {
                    break;
                }
                int controller, action;

                //取得操作数
                controller = data[0] >> 4;
                action = data[0] & 15;

                switch (controller)
                {
                    case CONTROLLER_INFO:
                        switch (action)
                        {
                            case ACTION_CAMERAVIEW_OBJECT_LIST:
                                requestObjectList(data, recv, client);
                                break;
                            case ACTION_CHANGE_ACTION:
                                changeAction(data, recv);
                                break;
                            case ACTION_INIT_CHARACTER:
                                initCharacterData(data, recv, client);
                                break;
                            case ACTION_CHANGE_DIRECTION:
                                changeDirection(data, recv);
                                break;
                        }
                        break;
                    case CONTROLLER_MOVE:
                        switch (action)
                        {
                            case ACTION_MOVETO:
                                parseMoveTo(data, recv);
                                break;
                            case ACTION_MOVE:
                                parseMove(data, recv);
                                break;
                        }
                        break;
                    case CONTROLLER_BATTLE:
                        switch (action)
                        {
                            case ACTION_SING:
                                parseSing(data, recv);
                                break;
                            case ACTION_ATTACK:
                                parseAttack(data, recv, client);
                                break;
                        }
                        break;
                    case NPC_CONTROLLER_BATTLE:
                        switch (action)
                        {
                            case ACTION_ATTACK:
                                parseNPCAttack(data, recv, client);
                                break;
                        }
                        break;
                }
            }
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("客户端断开连接，地址：" + clientip.Address);
        }

        static public void initCollection()
        {
            MySqlCommand comm = new MySqlCommand();
            comm.Connection = dbConnection;
            comm.CommandText = "select * from game_account";
            MySqlDataReader result = comm.ExecuteReader();
            while (result.Read())
            {
                CCharacterObject character = new CCharacterObject();
                character.ObjectType = CObjectType.PLAYER;
                character.ObjectId = result.GetString("GUID");
                character.Level = result.GetInt32("character_level");
                character.CharacterName = result.GetString("character_name");
                character.ResourceId = result.GetString("resource_id");
                character.Action = result.GetInt32("character_action");
                character.Direction = result.GetInt32("character_direction");
                character.PosX = result.GetInt32("character_posx");
                character.PosY = result.GetInt32("character_posy");
                character.Speed = result.GetInt32("character_speed");
                character.HealthMax = result.GetInt32("character_health_max");
                character.Health = result.GetInt32("character_health");
                character.ManaMax = result.GetInt32("character_mana_max");
                character.Mana = result.GetInt32("character_mana");
                character.EnergyMax = result.GetInt32("character_energy_max");
                character.Energy = result.GetInt32("character_energy");
                character.AttackRange = result.GetInt32("character_attackrange");
                character.AttackSpeed = result.GetFloat("character_attackspeed");
                objectList.Add(character);
            }
            result.Close();
        }

        static public void initCharacterData(byte[] data, int recv, Socket socket)
        {
            String guid = "";
            String authKey = "";

            for (int i = 1; i < recv; )
            {
                int length = BitConverter.ToInt32(new byte[] { data[i + 3], data[i + 2], data[i + 1], data[i] }, 0);
                int type = data[i + 4];

                if (guid == "")
                {
                    guid = Encoding.UTF8.GetString(data, i + 5, length);
                }
                else if (authKey == "")
                {
                    authKey = Encoding.UTF8.GetString(data, i + 5, length);
                }

                i += (length + 5);
            }
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Init Character Data, Guid: " + guid + ", AuthKey: " + authKey);

            IPEndPoint clientip = (IPEndPoint)socket.RemoteEndPoint;
            MySqlCommand comm = new MySqlCommand();
            comm.Connection = dbConnection;
            comm.CommandText = "delete from character_certificate where GUID='" + guid + "' and account_ip='" + clientip.Address.ToString() + "' and auth_key='" + authKey + "'";
            if (comm.ExecuteNonQuery() > 0)
            {
                comm.CommandText = "select * from game_character where GUID='" + guid + "'";
                MySqlDataReader dataReader = comm.ExecuteReader();
                if (dataReader.HasRows)
                {
                    CGameObject player = objectList[guid];
                    player.client = socket;

                    if(dataReader.Read())
                    {
                        CServerPackage package = new CServerPackage();
                        package.success = ACK_CONFIRM;
                        package.controller = CONTROLLER_INFO;
                        package.action = ACTION_INIT_CHARACTER;

                        package.param.Add(new Object[] { guid.Length, guid });
                        int characterLevel = dataReader.GetInt32("character_level");
                        package.param.Add(new Object[] { 4, characterLevel });
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
                        package.param.Add(new Object[] { 4, attackRange });
                        float attackSpeed = dataReader.GetFloat("character_attackspeed");
                        package.param.Add(new Object[] { 4, attackSpeed });

                        sendPackage(socket, package);
                    }
                }
                dataReader.Close();
            }
        }

        static public void parseMoveTo(byte[] data, int recv)
        {
            String playerId = "";
            int x = int.MinValue;
            int y = int.MinValue;
            for (int i = 1; i < recv; )
            {
                int length = BitConverter.ToInt32(new byte[] { data[i + 3], data[i + 2], data[i + 1], data[i] }, 0);
                int type = data[i + 4];
                switch (type)
                {
                    case TYPE_INT:
                        byte[] intNumber = new byte[] { data[i + 8], data[i + 7], data[i + 6], data[i + 5] };

                        if (x == int.MinValue)
                        {
                            x = BitConverter.ToInt32(intNumber, 0);
                        }
                        else if (y == int.MinValue)
                        {
                            y = BitConverter.ToInt32(intNumber, 0);
                        }
                        break;
                    case TYPE_STRING:
                        playerId = Encoding.UTF8.GetString(data, i + 5, length);
                        break;
                }
                i += (length + 5);
            }
            if (playerId != "")
            {
                CCharacterObject player = objectList[playerId] as CCharacterObject;
                if (x != int.MinValue)
                {
                    player.TargetX = x;
                }
                if (y != int.MinValue)
                {
                    player.TargetY = y;
                }
                objectList[playerId] = player;

                foreach(CGameObject o in objectList)
                {
                    if (o.client != null && o != player)
                    {
                        CServerPackage package = new CServerPackage();
                        package.success = ACK_CONFIRM;
                        package.controller = NPC_CONTROLLER_MOVE;
                        package.action = ACTION_MOVETO;
                        package.param.Add(new object[] { player.ObjectId.Length, player.ObjectId });
                        package.param.Add(new object[] { 4, player.TargetX });
                        package.param.Add(new object[] { 4, player.TargetY });
                        sendPackage(o.client, package);
                    }
                }

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Target: x = " + player.TargetX.ToString() + ", y = " + player.TargetY.ToString());
            }
        }

        static public void parseMove(byte[] data, int recv)
        {
            String playerId = "";
            int x = int.MinValue;
            int y = int.MinValue;
            for (int i = 1; i < recv; )
            {
                int length = BitConverter.ToInt32(new byte[] { data[i+3], data[i+2], data[i+1], data[i] }, 0);
                int type = data[i + 4];
                switch (type)
                {
                    case TYPE_INT:
                        byte[] intNumber = new byte[] { data[i + 8], data[i + 7], data[i + 6], data[i + 5] };

                        if (x == int.MinValue)
                        {
                            x = BitConverter.ToInt32(intNumber, 0);
                        }
                        else if (y == int.MinValue)
                        {
                            y = BitConverter.ToInt32(intNumber, 0);
                        }
                        break;
                    case TYPE_STRING:
                        playerId = Encoding.UTF8.GetString(data, i + 5, length);
                        break;
                }
                i += (length + 5);
            }

            if (playerId != "")
            {
                CCharacterObject player = objectList[playerId] as CCharacterObject;
                if (x != int.MinValue)
                {
                    player.PosX = x;
                }
                if (y != int.MinValue)
                {
                    player.PosY = y;
                }
                objectList[playerId] = player;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Move Sync: x = " + player.PosX.ToString() + ", y = " + player.PosY.ToString());

                MySqlCommand comm = new MySqlCommand();
                comm.Connection = dbConnection;
                comm.CommandText = "update game_character set character_posx=" + player.PosX + ", character_posy=" + player.PosY + " where GUID='" + playerId + "'";
                comm.ExecuteNonQuery();

                foreach (CGameObject o in objectList)
                {
                    if (o.client != null && o != player)
                    {
                        CServerPackage package = new CServerPackage();
                        package.success = ACK_CONFIRM;
                        package.controller = NPC_CONTROLLER_MOVE;
                        package.action = ACTION_MOVE;
                        package.param.Add(new object[] { player.ObjectId.Length, player.ObjectId });
                        package.param.Add(new object[] { 4, player.PosX });
                        package.param.Add(new object[] { 4, player.PosY });
                        sendPackage(o.client, package);
                    }
                }
            }
        }

        static public void parseSing(byte[] data, int recv)
        {
            String playerId = "";
            String skillId = "";
            int skillLevel = int.MinValue;
            int direction = int.MinValue;
            String targetId = "";
            int targetX = int.MinValue;
            int targetY = int.MinValue;

            for (int i = 1; i < recv; )
            {
                int length = BitConverter.ToInt32(new byte[] { data[i + 3], data[i + 2], data[i + 1], data[i] }, 0);
                int type = data[i + 4];
                switch (type)
                {
                    case TYPE_STRING:
                        if (playerId == "")
                        {
                            playerId = Encoding.UTF8.GetString(data, i + 5, length);
                        }
                        else if (skillId == "")
                        {
                            skillId = Encoding.UTF8.GetString(data, i + 5, length);
                        }
                        else if (targetId == "")
                        {
                            targetId = Encoding.UTF8.GetString(data, i + 5, length);
                        }
                        break;
                    case TYPE_INT:
                        byte[] intNumber = new byte[] { data[i + 8], data[i + 7], data[i + 6], data[i + 5] };
                        if (skillLevel == int.MinValue)
                        {
                            skillLevel = BitConverter.ToInt32(intNumber, 0);
                        }
                        else if (direction == int.MinValue)
                        {
                            direction = BitConverter.ToInt32(intNumber, 0);
                        }
                        else if (targetX == int.MinValue)
                        {
                            targetX = BitConverter.ToInt32(intNumber, 0);
                        }
                        else if (targetY == int.MinValue)
                        {
                            targetY = BitConverter.ToInt32(intNumber, 0);
                        }
                        break;
                }
                i += (length + 5);
            }
            if (playerId != "")
            {
                foreach (CGameObject o in objectList)
                {
                    if (o.client != null && o.ObjectId != playerId)
                    {
                        CServerPackage package = new CServerPackage();
                        package.success = ACK_CONFIRM;
                        package.controller = NPC_CONTROLLER_BATTLE;
                        package.action = ACTION_SING;
                        package.param.Add(new object[] { playerId.Length, playerId });
                        package.param.Add(new object[] { skillId.Length, skillId });
                        package.param.Add(new object[] { 4, skillLevel });
                        package.param.Add(new object[] { 4, direction });
                        package.param.Add(new object[] { targetId.Length, targetId });
                        package.param.Add(new object[] { 4, targetX });
                        package.param.Add(new object[] { 4, targetY });
                        sendPackage(o.client, package);
                    }
                }
            }
        }

        static public void parseAttack(byte[] data, int recv, Socket socket)
        {
            String playerId = "undefined";
            String targetId = "undefined";
            int targetX = int.MinValue;
            int targetY = int.MinValue;
            String skillId = "undefined";
            for (int i = 1; i < recv; )
            {
                int length = BitConverter.ToInt32(new byte[] { data[i + 3], data[i + 2], data[i + 1], data[i] }, 0);
                int type = data[i + 4];
                switch (type)
                {
                    case TYPE_STRING:
                        if (playerId == "undefined")
                        {
                            playerId = Encoding.UTF8.GetString(data, i + 5, length);
                        }
                        else if (targetId == "undefined")
                        {
                            targetId = Encoding.UTF8.GetString(data, i + 5, length);
                        }
                        else if (skillId == "undefined")
                        {
                            skillId = Encoding.UTF8.GetString(data, i + 5, length);
                        }
                        break;
                    case TYPE_INT:
                        byte[] intNumber = new byte[] { data[i + 8], data[i + 7], data[i + 6], data[i + 5] };
                        if (targetX == int.MinValue)
                        {
                            targetX = BitConverter.ToInt32(intNumber, 0);
                        }
                        else if (targetY == int.MinValue)
                        {
                            targetY = BitConverter.ToInt32(intNumber, 0);
                        }
                        break;
                }
                i += (length + 5);
            }
            if (playerId != "")
            {
                CCharacterObject player = objectList[playerId] as CCharacterObject;
            }
            //反馈
            if (skillId != "undefined")
            {
                //计算攻击力
                CServerPackage package = new CServerPackage();
                package.success = ACK_CONFIRM;
                package.controller = CONTROLLER_BATTLE;
                package.action = ACTION_ATTACK;

                package.param.Add(new object[2] { skillId.Length, skillId });
                package.param.Add(new object[2] { targetId.Length, targetId });
                package.param.Add(new object[2] { 4, targetX });
                package.param.Add(new object[2] { 4, targetY });

                if (targetId == "")
                {
                    Point target = new Point(targetX, targetY);
                    foreach (CGameObject o in objectList)
                    {
                        if (Point.distance(target, new Point(o.PosX, o.PosY)) < 200 && o.ObjectId != playerId)
                        {
                            int power = new Random().Next(50) + 50;
                            package.param.Add(new object[2] { o.ObjectId.Length, o.ObjectId });
                            package.param.Add(new object[2] { 4, power });
                        }
                    }
                }
                else
                {
                    int power = new Random().Next(50) + 50;
                    package.param.Add(new object[2] { targetId.Length, targetId });
                    package.param.Add(new object[2] { 4, power });
                }
                sendPackage(socket, package);
            }
            /*
            CServerPackage package1 = new CServerPackage();
            package1.success = ORDER_CONFIRM;
            package1.controller = NPC_CONTROLLER_BATTLE;
            package1.action = ACTION_PREPARE_ATTACK;

            object[] parameter1 = new object[2];
            parameter1[1] = targetId;
            parameter1[0] = ((String)parameter1[1]).Length;
            package1.param.Add(parameter1);

            object[] parameter2 = new object[2];
            parameter2[0] = 4;
            parameter2[1] = 0;
            package1.param.Add(parameter2);
            sendPackage(socket, package1);
            */
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("Attack confirm, playerId=" + playerId + ", targetId=" + targetId + ", targetX=" + targetX + ", targetY=" + targetY + ", skillId=" + skillId);
        }

        static public void parseNPCAttack(byte[] data, int recv, Socket socket)
        {
            String targetId = "";
            String playerId = "";
            int skillId = int.MaxValue;
            for (int i = 1; i < recv; )
            {
                int length = BitConverter.ToInt32(new byte[] { data[i + 3], data[i + 2], data[i + 1], data[i] }, 0);
                int type = data[i + 4];
                switch (type)
                {
                    case TYPE_INT:
                        byte[] intNumber = new byte[] { data[i + 8], data[i + 7], data[i + 6], data[i + 5] };
                        skillId = BitConverter.ToInt32(intNumber, 0);
                        break;
                    case TYPE_STRING:
                        if (targetId == "")
                        {
                            targetId = Encoding.UTF8.GetString(data, i + 5, length);
                        }
                        else if (playerId == "")
                        {
                            playerId = Encoding.UTF8.GetString(data, i + 5, length);
                        }
                        break;
                }
                i += (length + 5);
            }
            Console.Write("TargetId: " + targetId + ", ");
            Console.WriteLine("PlayerId: " + playerId + ", ");

            //反馈
            if (skillId != int.MaxValue)
            {
                if (skillId == 0)
                {
                    CServerPackage package = new CServerPackage();
                    package.success = ORDER_CONFIRM;
                    package.controller = NPC_CONTROLLER_BATTLE;
                    package.action = ACTION_ATTACK;

                    object[] parameter = new object[2];
                    parameter[1] = targetId;
                    parameter[0] = targetId.Length;
                    package.param.Add(parameter);

                    object[] parameter1 = new object[2];
                    parameter1[1] = playerId;
                    parameter1[0] = playerId.Length;
                    package.param.Add(parameter1);

                    object[] parameter2 = new object[2];
                    parameter2[0] = 4;
                    int power = new Random().Next(180) + 50;
                    parameter2[1] = power;
                    package.param.Add(parameter2);
                    sendPackage(socket, package);

                    Console.WriteLine("Attack power: 普通攻击" + power.ToString() + "点");
                }
            }
        }

        static void sendPackage(Socket socket, CServerPackage package)
        {
            int dataLength = 0;

            byte[] result = new byte[1024];
            result[2] = (byte)package.success;
            result[3] = (byte)package.controller;
            result[4] = (byte)package.action;
            dataLength += 3;

            int resultOffset = 5;
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
                else if (parameter[1].GetType() == typeof(Boolean))
                {
                    result[resultOffset] = TYPE_BOOL;
                    dataLength += 1;
                    resultOffset += 1;
                    byte[] bytes = BitConverter.GetBytes((Boolean)parameter[1]);
                    bytes.CopyTo(result, resultOffset);
                    dataLength += 1;
                    resultOffset += 1;
                }
            }
            byte[] packageLength = BitConverter.GetBytes((short)dataLength);
            packageLength.CopyTo(result, 0);
            dataLength += 2;
            socket.Send(result, dataLength, SocketFlags.None);
        }

        static void requestObjectList(byte[] data, int recv, Socket socket)
        {
            String objectId = "";
            int startX = int.MaxValue;
            int startY = int.MaxValue;
            int width = int.MaxValue;
            int height = int.MaxValue;

            for (int i = 1; i < recv; )
            {
                int length = BitConverter.ToInt32(new byte[] { data[i + 3], data[i + 2], data[i + 1], data[i] }, 0);
                int type = data[i + 4];
                switch (type)
                {
                    case TYPE_INT:
                        byte[] intNumber = new byte[] { data[i + 8], data[i + 7], data[i + 6], data[i + 5] };
                        if (startX == int.MaxValue)
                        {
                            startX = BitConverter.ToInt32(intNumber, 0);
                        }
                        else if (startY == int.MaxValue)
                        {
                            startY = BitConverter.ToInt32(intNumber, 0);
                        }
                        else if (width == int.MaxValue)
                        {
                            width = BitConverter.ToInt32(intNumber, 0);
                        }
                        else if (height == int.MaxValue)
                        {
                            height = BitConverter.ToInt32(intNumber, 0);
                        }
                        break;
                    case TYPE_STRING:
                        objectId = Encoding.UTF8.GetString(data, i + 5, length);
                        break;
                }
                i += (length + 5);
            }

            foreach(CGameObject o in objectList)
            {
                if (o.PosX > startX && o.PosX < startX + width && o.PosY > startY && o.PosY < startY + height && o.ObjectId != objectId)
                {
                    CServerPackage package = new CServerPackage();
                    package.success = ACK_CONFIRM;
                    package.controller = CONTROLLER_INFO;
                    package.action = ACTION_CAMERAVIEW_OBJECT_LIST;

                    if (o.ObjectType == CObjectType.PLAYER && o is CCharacterObject)
                    {
                        CCharacterObject player = o as CCharacterObject;
                        package.param.Add(new object[] { player.ObjectId.Length, player.ObjectId });
                        package.param.Add(new object[] { 4, player.Level });
                        package.param.Add(new object[] { Encoding.UTF8.GetBytes(player.CharacterName).Length, player.CharacterName });
                        package.param.Add(new object[] { player.ResourceId.Length, player.ResourceId });
                        package.param.Add(new object[] { 4, player.Direction });
                        package.param.Add(new object[] { 4, player.PosX });
                        package.param.Add(new object[] { 4, player.PosY });
                        package.param.Add(new object[] { 4, player.Speed });
                        package.param.Add(new object[] { 4, player.HealthMax });
                        package.param.Add(new object[] { 4, player.Health });
                        package.param.Add(new object[] { 4, player.ManaMax });
                        package.param.Add(new object[] { 4, player.Mana });
                        package.param.Add(new object[] { 4, player.EnergyMax });
                        package.param.Add(new object[] { 4, player.Energy });
                        package.param.Add(new object[] { 4, player.AttackRange });
                        package.param.Add(new object[] { 4, player.AttackSpeed });

                        package.param.Add(new object[] { 4, player.Action });
                        package.param.Add(new object[] { 4, player.TargetX });
                        package.param.Add(new object[] { 4, player.TargetY });
                    }
                    else if (o.ObjectType == CObjectType.MONSTER && o is CMonsterObject)
                    {
                        CMonsterObject monster = o as CMonsterObject;
                        package.param.Add(new object[] { monster.ObjectId.Length, monster.ObjectId });
                        package.param.Add(new object[] { 4, monster.Level });
                        package.param.Add(new object[] { Encoding.UTF8.GetBytes(monster.CharacterName).Length, monster.CharacterName });
                        package.param.Add(new object[] { monster.ResourceId.Length, monster.ResourceId });
                        package.param.Add(new object[] { 4, monster.Direction });
                        package.param.Add(new object[] { 4, monster.PosX });
                        package.param.Add(new object[] { 4, monster.PosY });
                        package.param.Add(new object[] { 4, monster.Speed });
                        package.param.Add(new object[] { 4, monster.HealthMax });
                        package.param.Add(new object[] { 4, monster.Health });
                        package.param.Add(new object[] { 4, monster.ManaMax });
                        package.param.Add(new object[] { 4, monster.Mana });
                        package.param.Add(new object[] { 4, 0 });
                        package.param.Add(new object[] { 4, 0 });
                        package.param.Add(new object[] { 4, monster.AttackRange });
                        package.param.Add(new object[] { 4, monster.AttackSpeed });
                        package.param.Add(new object[] { 4, monster.PassitiveMonster });

                        package.param.Add(new object[] { 4, monster.Action });
                        package.param.Add(new object[] { 4, monster.TargetX });
                        package.param.Add(new object[] { 4, monster.TargetY });
                    }

                    sendPackage(socket, package);
                }
            }
            /*
            if (other_posX > startX && other_posX < startX + width && other_posY > startY && other_posY < startY + height)
            {
                CServerPackage package = new CServerPackage();
                package.success = ACK_CONFIRM;
                package.controller = CONTROLLER_INFO;
                package.action = ACTION_CAMERAVIEW_OBJECT_LIST;

                package.param.Add(new object[] { otherObjectId.Length, otherObjectId });
                package.param.Add(new object[] { otherResourceId.Length, otherResourceId });
                package.param.Add(new object[] { Encoding.UTF8.GetBytes(otherPlayerName).Length, otherPlayerName });
                package.param.Add(new object[] { 4, other_posX });
                package.param.Add(new object[] { 4, other_posY });
                package.param.Add(new object[] { 4, other_speed });
                package.param.Add(new object[] { 4, action1 });
                package.param.Add(new object[] { 4, other_targetX });
                package.param.Add(new object[] { 4, other_targetY });

                sendPackage(socket, package);
                //Console.WriteLine("Found a player!");

                CServerPackage package1 = new CServerPackage();
                package1.success = ACK_CONFIRM;
                package1.controller = CONTROLLER_INFO;
                package1.action = ACTION_CAMERAVIEW_OBJECT_LIST;

                package1.param.Add(new object[] { other1ObjectId.Length, other1ObjectId });
                package1.param.Add(new object[] { other1ResourceId.Length, other1ResourceId });
                package1.param.Add(new object[] { Encoding.UTF8.GetBytes(other1PlayerName).Length, other1PlayerName });
                package1.param.Add(new object[] { 4, other1_posX });
                package1.param.Add(new object[] { 4, other1_posY });
                package1.param.Add(new object[] { 4, other1_speed });
                package1.param.Add(new object[] { 4, action2 });
                package1.param.Add(new object[] { 4, other1_targetX });
                package1.param.Add(new object[] { 4, other1_targetY });

                sendPackage(socket, package1);
                //Console.WriteLine("Found a player!");
            }
            */
            //Console.WriteLine("Request Rectangle: x = " + startX.ToString() + ", y = " + startY.ToString() + ", width = " + width.ToString() + ", height = " + height.ToString());
        }

        static public void changeAction(byte[] data, int recv)
        {
            String _objectId = "";
            int _action = 0;
            for (int i = 1; i < recv; )
            {
                int length = BitConverter.ToInt32(new byte[] { data[i + 3], data[i + 2], data[i + 1], data[i] }, 0);
                int type = data[i + 4];
                switch (type)
                {
                    case TYPE_STRING:
                        _objectId = Encoding.UTF8.GetString(data, i + 5, length);
                        break;
                    case TYPE_INT:
                        byte[] intNumber = new byte[] { data[i + 8], data[i + 7], data[i + 6], data[i + 5] };
                        _action = BitConverter.ToInt32(intNumber, 0);
                        break;
                }
                i += (length + 5);
            }
            if (_objectId != "")
            {
                CCharacterObject player = objectList[_objectId] as CCharacterObject;
                player.Action = _action;
            }
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("Action objectId: " + _objectId + ", action: " + _action.ToString());
        }

        static public void changeDirection(byte[] data, int recv)
        {
            String _objectId = "";
            int _direction = 0;
            for (int i = 1; i < recv; )
            {
                int length = BitConverter.ToInt32(new byte[] { data[i + 3], data[i + 2], data[i + 1], data[i] }, 0);
                int type = data[i + 4];
                switch (type)
                {
                    case TYPE_STRING:
                        _objectId = Encoding.UTF8.GetString(data, i + 5, length);
                        break;
                    case TYPE_INT:
                        byte[] intNumber = new byte[] { data[i + 8], data[i + 7], data[i + 6], data[i + 5] };
                        _direction = BitConverter.ToInt32(intNumber, 0);
                        break;
                }
                i += (length + 5);
            }
            if (_objectId != "")
            {
                CCharacterObject player = objectList[_objectId] as CCharacterObject;
                player.Direction = _direction;

                MySqlCommand comm = new MySqlCommand();
                comm.Connection = dbConnection;
                comm.CommandText = "update game_character set character_direction=" + player.Direction;
                comm.ExecuteNonQuery();
            }
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Direction objectId: " + _objectId + ", direction: " + _direction.ToString());
        }
    }
}

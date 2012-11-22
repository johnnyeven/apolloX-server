using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wooha.Utils
{
    class Vector2D
    {
        private double _x;
        private double _y;

        public Vector2D(double x, double y)
        {
            _x = x;
            _y = y;
        }

        public Vector2D()
            : this(0, 0)
        {
        }

        public Vector2D clone()
        {
            return new Vector2D(_x, _y);
        }

        public Vector2D zero()
        {
            _x = 0;
            _y = 0;
            return this;
        }

        public Boolean isZero()
        {
            return _x == 0 && _y == 0;
        }

        public double length
        {
            set
            {
                double a = angle;
                _x = Math.Cos(a) * value;
                _y = Math.Sin(a) * value;
            }
            get
            {
                return Math.Sqrt(lengthSQ);
            }
        }

        public double lengthSQ
        {
            get
            {
                return _x * _x + _y * _y;
            }
        }

        public double angle
        {
            set
            {
                double len = length;
                _x = Math.Cos(value) * len;
                _y = Math.Sin(value) * len;
            }
            get
            {
                return Math.Atan2(_y, _x);
            }
        }

        //将当前向量转化为单位向量
        public Vector2D normalize()
        {
            if (length == 0)
            {
                _x = 1;
                return this;
            }
            double len = length;
            _x /= len;
            _y /= len;
            return this;
        }

        //截取向量
        public Vector2D truncate(double max)
        {
            length = Math.Min(max, length);
            return this;
        }

        //反转向量
        public Vector2D reverse()
        {
            _x = -_x;
            _y = -_y;
            return this;
        }

        public Boolean isNormalized()
        {
            return length == 1.0;
        }

        //向量积
        public double dotProd(Vector2D v2)
        {
            return _x * v2.x + _y * v2.y;
        }

        //判断两向量是否垂直
        public double crossProd(Vector2D v2)
        {
            return _x * v2.y - _y * v2.x;
        }

        //返回两向量夹角的弧度值
        public static double angleBetween(Vector2D v1, Vector2D v2)
        {
            if (!v1.isNormalized())
            {
                v1 = v1.clone().normalize();
            }
            if (!v2.isNormalized())
            {
                v2 = v2.clone().normalize();
            }
            return Math.Acos(v1.dotProd(v2));
        }

        public int sign(Vector2D v2)
        {
            return prep.dotProd(v2) < 0 ? -1 : 1;
        }

        //返回当前向量与V2的距离
        public double distance(Vector2D v2)
        {
            return Math.Sqrt(distanceSQ(v2));
        }

        //返回当前向量与V2的距离的平方
        public double distanceSQ(Vector2D v2)
        {
            double dx = v2.x - _x;
            double dy = v2.y - _y;
            return dx * dx + dy * dy;
        }

        //两向量相加
        public Vector2D add(Vector2D v2)
        {
            return new Vector2D(_x + v2.x, _y + v2.y);
        }

        //两向量相减
        public Vector2D subtract(Vector2D v2)
        {
            return new Vector2D(_x - v2.x, _y - v2.y);
        }

        //数与向量的乘积
        public Vector2D multiply(double value)
        {
            return new Vector2D(_x * value, _y * value);
        }

        //数与向量的商
        public Vector2D divide(double value)
        {
            return new Vector2D(_x / value, _y / value);
        }

        public Boolean equals(Vector2D v2)
        {
            return _x == v2.x && _y == v2.y;
        }

        public Vector2D prep
        {
            get
            {
                return new Vector2D(-y, x);
            }
        }

        public double x
        {
            set
            {
                _x = value;
            }
            get
            {
                return _x;
            }
        }

        public double y
        {
            set
            {
                _y = value;
            }
            get
            {
                return _y;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wooha.Utils
{
    class Point
    {
        private int _x;
        private int _y;

        public Point(int x, int y)
        {
            this._x = x;
            this._y = y;
        }

        public Point(): this(0, 0)
        {
        }

        public int x
        {
            get
            {
                return this._x;
            }
            set
            {
                this._x = value;
            }
        }

        public int y
        {
            get
            {
                return this._y;
            }
            set
            {
                this._x = value;
            }
        }

        override public String ToString()
        {
            return "x=" + this._x + ", y=" + this._y;
        }

        public static int distance(Point p1, Point p2)
        {
            int width = Math.Abs(p1.x - p2.x);
            int height = Math.Abs(p1.y - p2.y);
            if (p1 != null && p1 != null)
            {
                return (int)Math.Round(Math.Sqrt(width * width + height * height));
            }
            return 0;
        }
    }
}

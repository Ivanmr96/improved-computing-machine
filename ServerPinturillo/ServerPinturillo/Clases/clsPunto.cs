﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerPinturillo
{
    public class clsPunto
    {
        private double x;
        private double y;
        private object color;
        private object size;

        public clsPunto(double x, double y,object color, object size)
        {
            this.x = x;
            this.y = y;
            this.Color = color;
            this.Size = size;
        }

        public double X { get => x; set => x = value; }
        public double Y { get => y; set => y = value; }
        public object Color { get => color; set => color = value; }
        public object Size { get => size; set => size = value; }
    }
}

using System;
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
        private byte R;
        private byte G;
        private byte A;
        private byte B;
        private object size;

        public clsPunto(double x, double y, byte R, byte G, byte A, byte B, object size)
        {
            this.x = x;
            this.y = y;
            this.B = B;
            this.A = A;
            this.G = G;
            this.R = R;
            this.Size = size;
        }

        public double X { get => x; set => x = value; }
        public double Y { get => y; set => y = value; }
        public byte R1 { get => R; set => R = value; }
        public byte G1 { get => G; set => G = value; }
        public byte A1 { get => A; set => A = value; }
        public byte B1 { get => B; set => B = value; }
        public object Size { get => size; set => size = value; }
    }


}

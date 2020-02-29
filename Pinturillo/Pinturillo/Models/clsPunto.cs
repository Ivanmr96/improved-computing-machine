using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace Pinturillo
{
    public class clsPunto
    {
        private double x;
        private double y;
        private Color color;
        private float pressure; //grosor
        public clsPunto(double x, double y,Color color, float pressure)
        {
            this.x = x;
            this.y = y;
            this.Color = color;
            this.pressure = pressure;
        }

        public double X { get => x; set => x = value; }
        public double Y { get => y; set => y = value; }
        public Color Color { get => color; set => color = value; }
        public float Pressure { get => pressure; set => pressure = value; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;

namespace PinturilloParaPruebas3
{
    public class clsPunto
    {
        private double x;
        private double y;
        private Color color;
        private Size size;//grosor
        public clsPunto(double x, double y,Color color, Size size)
        {
            this.x = x;
            this.y = y;
            this.Color = color;
            this.Size = size;
        }

        public double X { get => x; set => x = value; }
        public double Y { get => y; set => y = value; }
        public Color Color { get => color; set => color = value; }
        public Size Size { get => size; set => size = value; }
    }
}

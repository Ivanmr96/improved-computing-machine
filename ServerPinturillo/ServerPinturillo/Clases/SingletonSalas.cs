using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ServerPinturillo.Clases
{
    public class SingletonSalas
    {
        private List<clsPartida> listadoPartidas;
        private readonly static SingletonSalas instance = new SingletonSalas();


        //Constructor privado porque es un singleton
        private SingletonSalas()
        {
            listadoPartidas = new List<clsPartida>();
        }



        public static SingletonSalas Instance
        {
            get
            {
                return instance;
            }
        }

        public List<clsPartida> ListadoPartidas
        {
            get { return this.listadoPartidas; }
            set { this.listadoPartidas = value; }
        }




    }
}
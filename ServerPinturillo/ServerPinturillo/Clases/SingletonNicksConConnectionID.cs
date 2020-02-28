using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ServerPinturillo.Clases
{
    public class SingletonNicksConConnectionID
    {
     
        private List<Tuple<string, string>> listadoNickConConnectionID;
        private readonly static SingletonNicksConConnectionID instance = new SingletonNicksConConnectionID();


        //Constructor privado porque es un singleton
        private SingletonNicksConConnectionID()
        {
            this.listadoNickConConnectionID = new List<Tuple<string, string>>();
        }



        public static SingletonNicksConConnectionID Instance
        {
            get
            {
                return instance;
            }
        }

        public List<Tuple<string, string>> ListadoNickConConnectionID
        {
            get { return this.listadoNickConConnectionID; }
            set { this.listadoNickConConnectionID = value; }
        }
    }
}
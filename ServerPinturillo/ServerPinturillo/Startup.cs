using System;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Owin;
using ServerPinturillo.Clases;

[assembly: OwinStartup(typeof(ServerPinturillo.Startup))]

namespace ServerPinturillo
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Para obtener más información sobre cómo configurar la aplicación, visite https://go.microsoft.com/fwlink/?LinkID=316888

            GlobalHost.DependencyResolver.Register(typeof(PictionaryHub), () => new PictionaryHub(SingletonSalas.Instance));

            app.MapSignalR();

            //Para que se desconecte rapido
            
            GlobalHost.Configuration.DisconnectTimeout = TimeSpan.FromSeconds(60);
        }
    }
}

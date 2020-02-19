using GalaSoft.MvvmLight.Views;
using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Pinturillo.ViewModels
{
    public class MainPageVM : clsVMBase
    {
        private String _nick;
        private DelegateCommand _entrarAlJuegoCommand;
        private HubConnection conn;
        private IHubProxy proxy;
        Frame navigationFrame = Window.Current.Content as Frame;

        public MainPageVM() {
            SignalR();
        }

        public async void SignalR()
        {
            conn = new HubConnection("https://pictionary-di.azurewebsites.net");
            //conn = new HubConnection("http://localhost:11111/");
            proxy = conn.CreateHubProxy("PictionaryHub");
            Connection.Connection.conn = conn;
            Connection.Connection.proxy = proxy;

            await conn.Start();



        }

        public String nick {
            get { return _nick; }
            set {
                if (_nick != value)
                {
                    _nick = value;
                    _entrarAlJuegoCommand.RaiseCanExecuteChanged();
                    NotifyPropertyChanged("nick");
                }
            }
        }

        public DelegateCommand EntrarCommand
        {
            get
            {
                _entrarAlJuegoCommand = new DelegateCommand(EntrarAlJuego_Executed, EntrarAlJuego_CanExecuted);
                return _entrarAlJuegoCommand;
            }
        }

        private void EntrarAlJuego_Executed()
        {
            navigationFrame.Navigate(typeof(ListadoSalas),nick);
        }

        private bool EntrarAlJuego_CanExecuted()
        {
            bool puedeEntrar = false;

            if (!String.IsNullOrEmpty(_nick)) {
                puedeEntrar = true;
            }

            return puedeEntrar;
        }
    }
}

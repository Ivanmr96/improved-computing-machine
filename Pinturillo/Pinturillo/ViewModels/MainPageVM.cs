using GalaSoft.MvvmLight.Views;
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
        Frame navigationFrame = Window.Current.Content as Frame;

        public MainPageVM() {
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

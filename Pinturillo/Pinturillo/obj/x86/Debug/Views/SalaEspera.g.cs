﻿#pragma checksum "C:\Users\vmperez\Documents\GitHub\improved-computing-machine\Pinturillo\Pinturillo\Views\SalaEspera.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "94C557AC3F0F3689F1A30B426B94D217"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Pinturillo
{
    partial class SalaEspera : 
        global::Windows.UI.Xaml.Controls.Page, 
        global::Windows.UI.Xaml.Markup.IComponentConnector,
        global::Windows.UI.Xaml.Markup.IComponentConnector2
    {
        /// <summary>
        /// Connect()
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 10.0.17.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 2: // Views\SalaEspera.xaml line 182
                {
                    this.txtTituloSala = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                }
                break;
            case 3: // Views\SalaEspera.xaml line 24
                {
                    this.listaJugadores = (global::Windows.UI.Xaml.Controls.ListView)(target);
                }
                break;
            case 4: // Views\SalaEspera.xaml line 48
                {
                    this.stkBotones = (global::Windows.UI.Xaml.Controls.StackPanel)(target);
                }
                break;
            case 5: // Views\SalaEspera.xaml line 141
                {
                    this.txtMensaje = (global::Windows.UI.Xaml.Controls.RelativePanel)(target);
                }
                break;
            case 6: // Views\SalaEspera.xaml line 150
                {
                    this.txtBoxMensaje = (global::Windows.UI.Xaml.Controls.TextBox)(target);
                }
                break;
            case 7: // Views\SalaEspera.xaml line 163
                {
                    this.btnEnviarMensaje = (global::Windows.UI.Xaml.Controls.Button)(target);
                }
                break;
            case 9: // Views\SalaEspera.xaml line 84
                {
                    global::Windows.UI.Xaml.Controls.Button element9 = (global::Windows.UI.Xaml.Controls.Button)(target);
                    ((global::Windows.UI.Xaml.Controls.Button)element9).Click += this.Button_Click;
                }
                break;
            default:
                break;
            }
            this._contentLoaded = true;
        }

        /// <summary>
        /// GetBindingConnector(int connectionId, object target)
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 10.0.17.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::Windows.UI.Xaml.Markup.IComponentConnector GetBindingConnector(int connectionId, object target)
        {
            global::Windows.UI.Xaml.Markup.IComponentConnector returnValue = null;
            return returnValue;
        }
    }
}


﻿#pragma checksum "D:\Documents\GIT\improved-computing-machine\Pinturillo\Pinturillo\Views\PantallaJuego.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "5568D8C99EAAD63EC2360CC8CA27C55F"
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
    partial class PantallaJuego : 
        global::Windows.UI.Xaml.Controls.Page, 
        global::Windows.UI.Xaml.Markup.IComponentConnector,
        global::Windows.UI.Xaml.Markup.IComponentConnector2
    {
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 10.0.17.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        private static class XamlBindingSetters
        {
            public static void Set_Windows_UI_Xaml_Controls_InkToolbar_TargetInkCanvas(global::Windows.UI.Xaml.Controls.InkToolbar obj, global::Windows.UI.Xaml.Controls.InkCanvas value, string targetNullValue)
            {
                if (value == null && targetNullValue != null)
                {
                    value = (global::Windows.UI.Xaml.Controls.InkCanvas) global::Windows.UI.Xaml.Markup.XamlBindingHelper.ConvertValue(typeof(global::Windows.UI.Xaml.Controls.InkCanvas), targetNullValue);
                }
                obj.TargetInkCanvas = value;
            }
        };

        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 10.0.17.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        private class PantallaJuego_obj1_Bindings :
            global::Windows.UI.Xaml.Markup.IDataTemplateComponent,
            global::Windows.UI.Xaml.Markup.IXamlBindScopeDiagnostics,
            global::Windows.UI.Xaml.Markup.IComponentConnector,
            IPantallaJuego_Bindings
        {
            private global::Pinturillo.PantallaJuego dataRoot;
            private bool initialized = false;
            private const int NOT_PHASED = (1 << 31);
            private const int DATA_CHANGED = (1 << 30);

            // Fields for each control that has bindings.
            private global::Windows.UI.Xaml.Controls.TextBox obj5;
            private global::Windows.UI.Xaml.Controls.InkToolbar obj11;

            // Fields for each event bindings event handler.
            private global::Windows.UI.Xaml.Controls.TextChangedEventHandler obj5TextChanged;

            // Static fields for each binding's enabled/disabled state
            private static bool isobj11TargetInkCanvasDisabled = false;

            public PantallaJuego_obj1_Bindings()
            {
            }

            public void Disable(int lineNumber, int columnNumber)
            {
                if (lineNumber == 269 && columnNumber == 13)
                {
                    this.obj5.TextChanged -= obj5TextChanged;
                }
                else if (lineNumber == 221 && columnNumber == 11)
                {
                    isobj11TargetInkCanvasDisabled = true;
                }
            }

            // IComponentConnector

            public void Connect(int connectionId, global::System.Object target)
            {
                switch(connectionId)
                {
                    case 5: // Views\PantallaJuego.xaml line 258
                        this.obj5 = (global::Windows.UI.Xaml.Controls.TextBox)target;
                        this.obj5TextChanged = (global::System.Object p0, global::Windows.UI.Xaml.Controls.TextChangedEventArgs p1) =>
                        {
                            this.dataRoot.viewModel.textoCambiado();
                        };
                        ((global::Windows.UI.Xaml.Controls.TextBox)target).TextChanged += obj5TextChanged;
                        break;
                    case 11: // Views\PantallaJuego.xaml line 213
                        this.obj11 = (global::Windows.UI.Xaml.Controls.InkToolbar)target;
                        break;
                    default:
                        break;
                }
            }

            // IDataTemplateComponent

            public void ProcessBindings(global::System.Object item, int itemIndex, int phase, out int nextPhase)
            {
                throw new global::System.NotImplementedException();
            }

            public void Recycle()
            {
                throw new global::System.NotImplementedException();
            }

            // IPantallaJuego_Bindings

            public void Initialize()
            {
                if (!this.initialized)
                {
                    this.Update();
                }
            }
            
            public void Update()
            {
                this.Update_(this.dataRoot, NOT_PHASED);
                this.initialized = true;
            }

            public void StopTracking()
            {
            }

            public void DisconnectUnloadedObject(int connectionId)
            {
                throw new global::System.ArgumentException("No unloadable elements to disconnect.");
            }

            public bool SetDataRoot(global::System.Object newDataRoot)
            {
                if (newDataRoot != null)
                {
                    this.dataRoot = (global::Pinturillo.PantallaJuego)newDataRoot;
                    return true;
                }
                return false;
            }

            public void Loading(global::Windows.UI.Xaml.FrameworkElement src, object data)
            {
                this.Initialize();
            }

            // Update methods for each path node used in binding steps.
            private void Update_(global::Pinturillo.PantallaJuego obj, int phase)
            {
                if (obj != null)
                {
                    if ((phase & (NOT_PHASED | (1 << 0))) != 0)
                    {
                        this.Update_inkCanvas(obj.inkCanvas, phase);
                    }
                }
            }
            private void Update_inkCanvas(global::Windows.UI.Xaml.Controls.InkCanvas obj, int phase)
            {
                if ((phase & ((1 << 0) | NOT_PHASED )) != 0)
                {
                    // Views\PantallaJuego.xaml line 213
                    if (!isobj11TargetInkCanvasDisabled)
                    {
                        XamlBindingSetters.Set_Windows_UI_Xaml_Controls_InkToolbar_TargetInkCanvas(this.obj11, obj, null);
                    }
                }
            }
        }
        /// <summary>
        /// Connect()
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 10.0.17.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 2: // Views\PantallaJuego.xaml line 25
                {
                    this.borderStackPanel = (global::Windows.UI.Xaml.Controls.Border)(target);
                }
                break;
            case 3: // Views\PantallaJuego.xaml line 101
                {
                    this.borderCanvas = (global::Windows.UI.Xaml.Controls.Border)(target);
                }
                break;
            case 4: // Views\PantallaJuego.xaml line 234
                {
                    this.chatMensajes = (global::Windows.UI.Xaml.Controls.ListView)(target);
                }
                break;
            case 5: // Views\PantallaJuego.xaml line 258
                {
                    this.inputMensajes = (global::Windows.UI.Xaml.Controls.TextBox)(target);
                }
                break;
            case 6: // Views\PantallaJuego.xaml line 274
                {
                    this.btnSend = (global::Windows.UI.Xaml.Controls.Button)(target);
                }
                break;
            case 7: // Views\PantallaJuego.xaml line 286
                {
                    this.backArrow = (global::Windows.UI.Xaml.Controls.Button)(target);
                }
                break;
            case 9: // Views\PantallaJuego.xaml line 148
                {
                    this.palabra = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                }
                break;
            case 10: // Views\PantallaJuego.xaml line 194
                {
                    this.canvasBorder = (global::Windows.UI.Xaml.Controls.Border)(target);
                }
                break;
            case 11: // Views\PantallaJuego.xaml line 213
                {
                    this.inkToolbar = (global::Windows.UI.Xaml.Controls.InkToolbar)(target);
                    ((global::Windows.UI.Xaml.Controls.InkToolbar)this.inkToolbar).EraseAllClicked += this.InkToolbar_EraseAllClicked;
                }
                break;
            case 12: // Views\PantallaJuego.xaml line 222
                {
                    this.ballpointpen = (global::Windows.UI.Xaml.Controls.InkToolbarBallpointPenButton)(target);
                }
                break;
            case 13: // Views\PantallaJuego.xaml line 203
                {
                    this.inkCanvas = (global::Windows.UI.Xaml.Controls.InkCanvas)(target);
                }
                break;
            case 14: // Views\PantallaJuego.xaml line 36
                {
                    this.stkpanelListados = (global::Windows.UI.Xaml.Controls.StackPanel)(target);
                }
                break;
            case 15: // Views\PantallaJuego.xaml line 43
                {
                    this.listadoSalas = (global::Windows.UI.Xaml.Controls.ListView)(target);
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
            switch(connectionId)
            {
            case 1: // Views\PantallaJuego.xaml line 1
                {                    
                    global::Windows.UI.Xaml.Controls.Page element1 = (global::Windows.UI.Xaml.Controls.Page)target;
                    PantallaJuego_obj1_Bindings bindings = new PantallaJuego_obj1_Bindings();
                    returnValue = bindings;
                    bindings.SetDataRoot(this);
                    this.Bindings = bindings;
                    element1.Loading += bindings.Loading;
                    global::Windows.UI.Xaml.Markup.XamlBindingHelper.SetDataTemplateComponent(element1, bindings);
                }
                break;
            }
            return returnValue;
        }
    }
}


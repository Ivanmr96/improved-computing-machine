﻿#pragma checksum "D:\Documents\GIT\improved-computing-machine\Pinturillo\Pinturillo\Views\CrearSalaPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "7162FFCD288B836082AB737263D9B3C5"
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
    partial class CrearSalaPage : 
        global::Windows.UI.Xaml.Controls.Page, 
        global::Windows.UI.Xaml.Markup.IComponentConnector,
        global::Windows.UI.Xaml.Markup.IComponentConnector2
    {

        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 10.0.17.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        private class CrearSalaPage_obj1_Bindings :
            global::Windows.UI.Xaml.Markup.IDataTemplateComponent,
            global::Windows.UI.Xaml.Markup.IXamlBindScopeDiagnostics,
            global::Windows.UI.Xaml.Markup.IComponentConnector,
            ICrearSalaPage_Bindings
        {
            private global::Pinturillo.CrearSalaPage dataRoot;
            private bool initialized = false;
            private const int NOT_PHASED = (1 << 31);
            private const int DATA_CHANGED = (1 << 30);

            // Fields for each control that has bindings.
            private global::Windows.UI.Xaml.Controls.CheckBox obj6;

            // Fields for each event bindings event handler.
            private global::Windows.UI.Xaml.RoutedEventHandler obj6Checked;
            private global::Windows.UI.Xaml.RoutedEventHandler obj6Unchecked;

            // Static fields for each binding's enabled/disabled state

            public CrearSalaPage_obj1_Bindings()
            {
            }

            public void Disable(int lineNumber, int columnNumber)
            {
                if (lineNumber == 46 && columnNumber == 67)
                {
                    this.obj6.Checked -= obj6Checked;
                }
                else if (lineNumber == 46 && columnNumber == 106)
                {
                    this.obj6.Unchecked -= obj6Unchecked;
                }
            }

            // IComponentConnector

            public void Connect(int connectionId, global::System.Object target)
            {
                switch(connectionId)
                {
                    case 6: // Views\CrearSalaPage.xaml line 46
                        this.obj6 = (global::Windows.UI.Xaml.Controls.CheckBox)target;
                        this.obj6Checked = (global::System.Object p0, global::Windows.UI.Xaml.RoutedEventArgs p1) =>
                        {
                            this.dataRoot.vm.CheckBox_Changed(p0, p1);
                        };
                        ((global::Windows.UI.Xaml.Controls.CheckBox)target).Checked += obj6Checked;
                        this.obj6Unchecked = (global::System.Object p0, global::Windows.UI.Xaml.RoutedEventArgs p1) =>
                        {
                            this.dataRoot.vm.CheckBox_Changed(p0, p1);
                        };
                        ((global::Windows.UI.Xaml.Controls.CheckBox)target).Unchecked += obj6Unchecked;
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

            // ICrearSalaPage_Bindings

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
                    this.dataRoot = (global::Pinturillo.CrearSalaPage)newDataRoot;
                    return true;
                }
                return false;
            }

            public void Loading(global::Windows.UI.Xaml.FrameworkElement src, object data)
            {
                this.Initialize();
            }

            // Update methods for each path node used in binding steps.
            private void Update_(global::Pinturillo.CrearSalaPage obj, int phase)
            {
                if (obj != null)
                {
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
            case 2: // Views\CrearSalaPage.xaml line 25
                {
                    this.btnVolver = (global::Windows.UI.Xaml.Controls.Button)(target);
                    ((global::Windows.UI.Xaml.Controls.Button)this.btnVolver).Click += this.Button_Click;
                }
                break;
            case 3: // Views\CrearSalaPage.xaml line 34
                {
                    this.border = (global::Windows.UI.Xaml.Controls.Border)(target);
                }
                break;
            case 4: // Views\CrearSalaPage.xaml line 36
                {
                    this.txtboxNombreSala = (global::Windows.UI.Xaml.Controls.TextBox)(target);
                }
                break;
            case 5: // Views\CrearSalaPage.xaml line 40
                {
                    this.txtErrorNombreSala = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                }
                break;
            case 6: // Views\CrearSalaPage.xaml line 46
                {
                    this.checkbox = (global::Windows.UI.Xaml.Controls.CheckBox)(target);
                }
                break;
            case 7: // Views\CrearSalaPage.xaml line 47
                {
                    this.passwordbox = (global::Windows.UI.Xaml.Controls.PasswordBox)(target);
                }
                break;
            case 8: // Views\CrearSalaPage.xaml line 50
                {
                    this.combobox = (global::Windows.UI.Xaml.Controls.ComboBox)(target);
                }
                break;
            case 9: // Views\CrearSalaPage.xaml line 53
                {
                    this.txtErrorContrasena = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                }
                break;
            case 10: // Views\CrearSalaPage.xaml line 58
                {
                    this.txtErrorNumJugadores = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
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
            case 1: // Views\CrearSalaPage.xaml line 1
                {                    
                    global::Windows.UI.Xaml.Controls.Page element1 = (global::Windows.UI.Xaml.Controls.Page)target;
                    CrearSalaPage_obj1_Bindings bindings = new CrearSalaPage_obj1_Bindings();
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


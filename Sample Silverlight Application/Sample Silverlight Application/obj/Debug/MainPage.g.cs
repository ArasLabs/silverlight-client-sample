﻿#pragma checksum "C:\Documents and Settings\Administrator\Desktop\Sample Silverlight Application\Sample Silverlight Application\MainPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "703975894DC3EF8089A6AE42C5E296CA"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace Sample_Silverlight_Application {
    
    
    public partial class MainPage : System.Windows.Controls.UserControl {
        
        internal System.Windows.Controls.Grid HeaderGrid;
        
        internal System.Windows.Controls.Button exitButton;
        
        internal System.Windows.Controls.Grid LogonPanel;
        
        internal System.Windows.Controls.TextBox Login_Name;
        
        internal System.Windows.Controls.PasswordBox Password;
        
        internal System.Windows.Controls.TextBox Server;
        
        internal System.Windows.Controls.TextBox Database;
        
        internal System.Windows.Controls.Button LoginButton;
        
        internal System.Windows.Controls.Grid bigUserGrid;
        
        internal System.Windows.Controls.DataGrid UserGrid;
        
        internal System.Windows.Controls.Canvas FEEDBACK;
        
        internal System.Windows.Controls.TextBlock FEEDBACKMSG;
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Windows.Application.LoadComponent(this, new System.Uri("/Sample%20Silverlight%20Application;component/MainPage.xaml", System.UriKind.Relative));
            this.HeaderGrid = ((System.Windows.Controls.Grid)(this.FindName("HeaderGrid")));
            this.exitButton = ((System.Windows.Controls.Button)(this.FindName("exitButton")));
            this.LogonPanel = ((System.Windows.Controls.Grid)(this.FindName("LogonPanel")));
            this.Login_Name = ((System.Windows.Controls.TextBox)(this.FindName("Login_Name")));
            this.Password = ((System.Windows.Controls.PasswordBox)(this.FindName("Password")));
            this.Server = ((System.Windows.Controls.TextBox)(this.FindName("Server")));
            this.Database = ((System.Windows.Controls.TextBox)(this.FindName("Database")));
            this.LoginButton = ((System.Windows.Controls.Button)(this.FindName("LoginButton")));
            this.bigUserGrid = ((System.Windows.Controls.Grid)(this.FindName("bigUserGrid")));
            this.UserGrid = ((System.Windows.Controls.DataGrid)(this.FindName("UserGrid")));
            this.FEEDBACK = ((System.Windows.Controls.Canvas)(this.FindName("FEEDBACK")));
            this.FEEDBACKMSG = ((System.Windows.Controls.TextBlock)(this.FindName("FEEDBACKMSG")));
        }
    }
}

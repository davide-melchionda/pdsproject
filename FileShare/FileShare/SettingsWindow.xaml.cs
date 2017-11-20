using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace FileShare
{
   
    public partial class SettingsWindow : Window
    {
        //private const int GWL_STYLE = -16;
        //private const int WS_SYSMENU = 0x80000;
        //[DllImport("user32.dll", SetLastError = true)]
        //private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        //[DllImport("user32.dll")]
        //private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        public SettingsWindow()
        {
            InitializeComponent();
            SettingsPage page = new SettingsPage();
            page.OnClosed += Page_OnClosed;
            this.SettingsFrame.Navigate(page);
            //this.Loaded += new RoutedEventHandler(Window_Loaded);
        }

        private void Page_OnClosed()
        {
            this.Close();        }

        //private void Window_Loaded(object sender, RoutedEventArgs e)
        //{
        //    var hwnd = new WindowInteropHelper(this).Handle;
        //    SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);
        //}

    }
}

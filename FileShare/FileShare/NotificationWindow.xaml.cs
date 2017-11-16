using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace FileShare {
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class NotificationWindow : Window
    {
        private const int GWL_STYLE = -16;
        private const int WS_SYSMENU = 0x80000;
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        private Page1 page;
        public NotificationWindow()
        {
            InitializeComponent();
            Page = new Page1();
            NotificationFrame.Navigate(page);
            this.Loaded += new RoutedEventHandler(Window_Loaded);
            //var location = this.PointToScreen(new Point(0, 0));
            //this.Left = location.X;
            //this.Top = location.Y - this.Height;

        }

        //protected override void OnClosed(EventArgs e) {
        //    //base.OnClosed(e);

        //    //Application.Current.Shutdown();
        //}
        
        public Page1 Page { get => page; set => page = value; }

        protected override void OnDeactivated(EventArgs e)
        {

            this.Hide();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var hwnd = new WindowInteropHelper(this).Handle;
            SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);
            var desktopWorkingArea = System.Windows.SystemParameters.WorkArea;
            this.Left = desktopWorkingArea.Right - this.Width;
            this.Top = desktopWorkingArea.Bottom - this.Height;
        }

    }

    }


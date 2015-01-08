using System;
using System.Windows;

namespace RKT_WPF_GETFlights
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindow_OnClosed(object sender, EventArgs e)
        {
            ExitApp();
        }

        private void ExitApp()
        {
            Application.Current.Shutdown();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace wpf
{
    /// <summary>
    /// Interaction logic for ShutdownConfrim.xaml
    /// </summary>
    public partial class ShutdownConfrim : Window
    {
        public ShutdownConfrim()    
        {
            InitializeComponent();
        }
        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            var addUserWindow = App.Current.Windows.OfType<ShutdownConfrim>().FirstOrDefault();
            addUserWindow?.Close();
        }
    }
}

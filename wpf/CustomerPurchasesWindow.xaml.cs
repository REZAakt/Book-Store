using Data_Access.Models;
using Data_Access;
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
    /// Interaction logic for CustomerPurchasesWindow.xaml
    /// </summary>
    public partial class CustomerPurchasesWindow : Window
    {
        private readonly Customer _customer;
        private readonly ProductDataAccess _productDataAccess;

        public CustomerPurchasesWindow(Customer customer, ProductDataAccess productDataAccess)
        {
            InitializeComponent();

            _customer = customer;
            _productDataAccess = productDataAccess;

            Title = $"Books of {_customer.FirstName} {_customer.LastName}";

            PurchasesGrid.ItemsSource = _productDataAccess.GetPurchasesByCustomer(_customer.Id);
        }

        private void btnCansel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}

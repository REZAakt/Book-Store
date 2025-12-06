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
    /// Interaction logic for BuyProductWindow.xaml
    /// </summary>
    public partial class BuyProductWindow : Window
    {
        private readonly Customer _customer;
        private readonly ProductDataAccess _productDataAccess;

        public BuyProductWindow(Customer customer, ProductDataAccess productDataAccess)
        {
            InitializeComponent();

            _customer = customer;
            _productDataAccess = productDataAccess;

            // لیست کتاب‌ها رو از دیتااکسس می‌گیریم
            ProductsGrid.ItemsSource = _productDataAccess.Products;
        }

        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            txtError.Text = string.Empty;

            if (ProductsGrid.SelectedItem is not Product selectedProduct)
            {
                txtError.Text = "Please select a book.";
                return;
            }

            if (!int.TryParse(txtQuantity.Text, out int quantity) || quantity <= 0)
            {
                txtError.Text = "Invalid quantity entered.";
                return;
            }

            // اینجا customerId رو هم پاس می‌دیم
            if (!_productDataAccess.TryPurchaseProduct(selectedProduct.Id, quantity, _customer.Id, out string errorMessage))
            {
                txtError.Text = errorMessage;
                return;
            }

            MessageBox.Show("Purchase completed successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

            DialogResult = true;
            Close();
        }


        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }

}

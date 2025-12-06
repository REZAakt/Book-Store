using Data_Access;
using Data_Access.Models;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace wpf
{
    public partial class MainWindow : Window
    {
        EmployeeDataAccess employeeDataAccess = new EmployeeDataAccess();
        CustomerDataAccess customerDataAccess = new CustomerDataAccess();
        ProductDataAccess productDataAccess = new ProductDataAccess();

        ObservableCollection<Employee> employees = new ObservableCollection<Employee>();
        ObservableCollection<Customer> customers = new ObservableCollection<Customer>();
        ObservableCollection<Product> products = new ObservableCollection<Product>();

        public Employee currentEmployee { get; set; } = new Employee();
        public Customer currentCustomer { get; set; } = new Customer();
        public Product currentProduct { get; set; } = new Product();

        public MainWindow()
        {
            InitializeComponent();
            var filePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "resources", "Book.json");
            if (File.Exists(filePath))
            {
                LottieView.FileName = filePath;
                LottieView.PlayAnimation();
            }
            else
            {
                return;
            }
            FillData();
            EmployeeListView.ItemsSource = employees;
            CustomerListView.ItemsSource = customers;
            ProductListView.ItemsSource = products;

        }

        private void FillData()
        {
            employees = employeeDataAccess.Employees;
            customers = customerDataAccess.Customers;
            products = productDataAccess.Products;
        }

        private void btnHome_Click(object sender, RoutedEventArgs e)
        {
            HomePanel.Visibility = Visibility.Visible;
            EmployeesPanel.Visibility = Visibility.Collapsed;
            CustomersPanel.Visibility = Visibility.Collapsed;
            ProductsPanel.Visibility = Visibility.Collapsed;
        }

        private void btnEmployees_Click(object sender, RoutedEventArgs e)
        {
            HomePanel.Visibility = Visibility.Collapsed;
            EmployeesPanel.Visibility = Visibility.Visible;
            CustomersPanel.Visibility = Visibility.Collapsed;
            ProductsPanel.Visibility = Visibility.Collapsed;

        }

        private void btnCustomers_Click(object sender, RoutedEventArgs e)
        {
            HomePanel.Visibility = Visibility.Collapsed;
            EmployeesPanel.Visibility = Visibility.Collapsed;
            CustomersPanel.Visibility = Visibility.Visible;
            ProductsPanel.Visibility = Visibility.Collapsed;
        }

        private void btnProducts_Click(object sender, RoutedEventArgs e)
        {
            HomePanel.Visibility = Visibility.Collapsed;
            EmployeesPanel.Visibility = Visibility.Collapsed;
            CustomersPanel.Visibility = Visibility.Collapsed;
            ProductsPanel.Visibility = Visibility.Visible;
        }

        private void EmployeeListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (EmployeeListView.SelectedIndex >= 0)
            {
                currentEmployee = EmployeeListView.SelectedItem as Employee;
                EmployeeLabel.Content = currentEmployee.GetBasicInfo();
            }
        }

        private void btnAddemployee_Click(object sender, RoutedEventArgs e)
        {
            AddEditEmployee addWindow = new AddEditEmployee(employeeDataAccess);
            addWindow.ShowDialog();
            RefreshEmployees();
        }

        private void btnDeletemployee_Click(object sender, RoutedEventArgs e)
        {

            //Employee currentEmployee = EmployeeListView.SelectedItem as Employee;
            var result = System.Windows.MessageBox.Show(
             $"Do you want to delete this user ?",
             "Confirm",
             System.Windows.MessageBoxButton.YesNo,
             MessageBoxImage.Warning);

            if (result == System.Windows.MessageBoxResult.Yes)
            {

                Employee currentEmployee = ((sender as Button).CommandParameter as Employee);

                if (currentEmployee != null)
                {
                    employeeDataAccess.RemoveEmployee(currentEmployee.Id);
                    employees.Remove(currentEmployee);
                    EmployeeLabel.Content = "---";
                    RefreshEmployees();
                }
            }
        }

        private void btnEditemployee_Click(object sender, RoutedEventArgs e)
        {
            if (EmployeeListView.SelectedIndex >= 0)
            {
                currentEmployee = EmployeeListView.SelectedItem as Employee;
                AddEditEmployee addWindow = new AddEditEmployee(employeeDataAccess, currentEmployee);
                addWindow.ShowDialog();
                RefreshEmployees();
            }
        }



        private void btnAddCustomer_Click(object sender, RoutedEventArgs e)
        {
            AddEditCustomer addWindow = new AddEditCustomer(customerDataAccess);
            addWindow.ShowDialog();
            RefreshCustomers();
        }

        private void btnDeleteCustomer_Click(object sender, RoutedEventArgs e)
        {

            //Customer currentCustomer = CustomerListView.SelectedItem as Customer;
            var result = System.Windows.MessageBox.Show(
              $"Do you want to delete this user ?",
              "Confirm",
              System.Windows.MessageBoxButton.YesNo,
              MessageBoxImage.Warning);

            if (result == System.Windows.MessageBoxResult.Yes)
            {
                Customer currentCustomer = ((sender as Button).CommandParameter as Customer);

                if (currentCustomer != null)
                {
                    customerDataAccess.RemoveCustomer(currentCustomer.Id);
                    customers.Remove(currentCustomer);
                    CustomerLabel.Content = "---";
                    RefreshCustomers();
                }
            }
        }


        private void btnEditCustomers_Click(object sender, RoutedEventArgs e)
        {
            if (CustomerListView.SelectedIndex >= 0)
            {
                currentCustomer = CustomerListView.SelectedItem as Customer;
                AddEditCustomer addWindow = new AddEditCustomer(customerDataAccess, currentCustomer);
                addWindow.ShowDialog();
                RefreshCustomers();
            }
        }


        private void btnAddProduct_Click(object sender, RoutedEventArgs e)
        {
            AddEditProduct addWindow = new AddEditProduct(productDataAccess);
            addWindow.ShowDialog();
            RefreshProducts();
        }

        private void btnDeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            var result = System.Windows.MessageBox.Show(
                  $"Do you want to delete this Book ?",
                  "Confirm",
                  System.Windows.MessageBoxButton.YesNo,
                  MessageBoxImage.Warning);

            if (result == System.Windows.MessageBoxResult.Yes)
            {
                //Product currentProduct = ProductListView.SelectedItem as Product;
                Product currentProduct = ((sender as Button).CommandParameter as Product);

                if (currentProduct != null)
                {
                    productDataAccess.RemoveProduct(currentProduct.Id);
                    products.Remove(currentProduct);
                    ProductLabel.Content = "---";
                    RefreshProducts();
                }
            }
        }

        private void btnEditProduct_Click(object sender, RoutedEventArgs e)
        {
            if (ProductListView.SelectedIndex >= 0)
            {
                currentProduct = ProductListView.SelectedItem as Product;
                AddEditProduct addWindow = new AddEditProduct(productDataAccess, currentProduct);
                addWindow.ShowDialog();
                RefreshProducts();
            }
        }

        private void RefreshEmployees()
        {
            EmployeeListView.ItemsSource = null;
            EmployeeListView.ItemsSource = employees;
            //SetBackgrounds(EmployeeListView);
        }

        private void RefreshCustomers()
        {
            CustomerListView.ItemsSource = null;
            CustomerListView.ItemsSource = customers;
            //SetBackgrounds(CustomeListView);
        }

        private void RefreshProducts()
        {
            ProductListView.ItemsSource = null;
            ProductListView.ItemsSource = products;
            //SetBackgrounds(ProductListView);
        }


        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            var shutdownConfrim = new ShutdownConfrim();
            shutdownConfrim.ShowDialog();
        }

        private void customer_edit(object sender, RoutedEventArgs e)
        {
            var selectedCustomer = ((FrameworkElement)sender).DataContext as Customer;
            if (selectedCustomer != null)
            {
                AddEditCustomer addEditWindow = new AddEditCustomer(customerDataAccess, selectedCustomer);
                addEditWindow.ShowDialog();
                RefreshCustomers();
            }
        }



        private void product_edit(object sender, RoutedEventArgs e)
        {
            var selectedProduct = ((FrameworkElement)sender).DataContext as Product;
            if (selectedProduct != null)
            {
                AddEditProduct addEditWindow = new AddEditProduct(productDataAccess, selectedProduct);
                addEditWindow.ShowDialog();
                RefreshProducts();
            }
        }



        private void Employee_edit(object sender, RoutedEventArgs e)
        {

            var selectedEmployee = ((FrameworkElement)sender).DataContext as Employee;
            if (selectedEmployee != null)
            {
                AddEditEmployee addEditWindow = new AddEditEmployee(employeeDataAccess, selectedEmployee);
                addEditWindow.ShowDialog();
                RefreshProducts();
            }
        }

        private void EmployeeListViewSelectionChange(object sender, SelectionChangedEventArgs e)
        {
            if (EmployeeListView.SelectedIndex >= 0)
            {
                currentEmployee = EmployeeListView.SelectedItem as Employee;
                EmployeeLabel.Content = currentEmployee.GetBasicInfo();
            }
        }

        private void CustomerListViewSelectionChange(object sender, SelectionChangedEventArgs e)
        {
            if (CustomerListView.SelectedIndex >= 0)
            {
                currentCustomer = CustomerListView.SelectedItem as Customer;
                CustomerLabel.Content = currentCustomer.GetBasicInfo();
            }
        }

        private void ProductListViewSelectionChange(object sender, SelectionChangedEventArgs e)
        {
            if (ProductListView.SelectedIndex >= 0)
            {
                currentProduct = ProductListView.SelectedItem as Product;
                ProductLabel.Content = currentProduct.GetBasicInfo();
            }
        }


        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
            e.Handled = true;
        }

        private void btnBuyBook_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.CommandParameter is Customer customer)
            {
                var buyWindow = new BuyProductWindow(customer, productDataAccess)
                {
                    Owner = this
                };

                bool? result = buyWindow.ShowDialog();

                if (result == true)
                {
                    RefreshProducts();

                    RefreshCustomers();
                }
            }
        }

        private void btnShowCustomerBooks_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.CommandParameter is Customer customer)
            {
                var win = new CustomerPurchasesWindow(customer, productDataAccess)
                {
                    Owner = this
                };

                win.ShowDialog();
            }
        }

    }
}





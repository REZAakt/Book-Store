using Data_Access;
using System;
using System.Windows;
using Data_Access.Models;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using System.Windows.Controls;

namespace wpf
{
    public partial class AddEditProduct : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private ProductDataAccess productDataAccess;
        private Product editingProduct;
        private bool isEdit = false;

        private string _btnContent = "OK";
        public string MyProperty
        {
            get 
            {
                return _btnContent;
            } 
            set
            {
                if (_btnContent == value) return;
                _btnContent = value;
                OnPropertyChanged();
            }
        }


        public AddEditProduct(ProductDataAccess proDataAccess)
        {
            InitializeComponent();
            productDataAccess = proDataAccess;
            Loaded += AddEditProduct_Loaded;
        }

        private void AddEditProduct_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = this;
        }

        public AddEditProduct(ProductDataAccess proDataAccess, Product pro)
        {
            InitializeComponent();
            productDataAccess = proDataAccess;
            editingProduct = pro;
            isEdit = true;
            Loaded += AddEditProduct_Loaded;
            tbName.Text = editingProduct.Name;
            tbAuthor.Text = editingProduct.Author;
            tbPrice.Text = editingProduct.Price.ToString();
            tbAvailable.Text = editingProduct.AvailableCount.ToString();
        }

        private void btnCancelp_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnOKp_Click(object sender, RoutedEventArgs e)
        {
            bool isValid = CheckEmployeeValidity();
            if (isValid)
            {
                try
                {
                    Product pro = new Product()
                    {
                        Name = tbName.Text,
                        Author = tbAuthor.Text,
                        Price = Convert.ToUInt64(tbPrice.Text),
                        AvailableCount = int.Parse(tbAvailable.Text),
                    };

                    if (isEdit)
                    {
                        pro.Id = editingProduct.Id;
                        productDataAccess.EditProduct(pro);
                    }
                    else
                    {
                        pro.Id = productDataAccess.GetNextId();
                        productDataAccess.AddProduct(pro);
                    }

                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        private bool CheckEmployeeValidity()
        {
            bool isValid = true;
            string Name = tbName.Text.Trim();
            string Author = tbAuthor.Text.Trim();
            string Available = tbAvailable.Text.Trim();
            string Price = tbPrice.Text.Trim();

            if (string.IsNullOrEmpty(Name))
            {
                isValid = false;
                MessageBox.Show("Name is invalid!");
                tbName.BorderBrush = Brushes.Red;
            }
            else if (string.IsNullOrEmpty(Author))
            {
                isValid = false;
                MessageBox.Show("Author is invalid!");
                tbAuthor.BorderBrush = Brushes.Red;
            }
            else if (!decimal.TryParse(Price, out decimal p))
            {
                isValid = false;
                MessageBox.Show("Price is invalid!");
                tbPrice.BorderBrush = Brushes.Red;
            }
            else if (!UInt64.TryParse(Available, out ulong a))
            {
                isValid = false;
                MessageBox.Show("Available number is invalid!");
                tbAvailable.BorderBrush = Brushes.Red;
            }

            return isValid;
        }

     

        private void BBBB(object sender, RoutedEventArgs e)
        {
            MyProperty = "HJ";
        }

        private void ValidateTextInput(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null)
            {
                string text = textBox.Text;
                if (!IsValidText(text))
                {
                    textBox.Text = new string(text.Where(char.IsLetterOrDigit).ToArray());
                    textBox.CaretIndex = textBox.Text.Length; 
                    lblError.Content = "Only English letters and numbers are allowed.";
                }
                else
                {
                    lblError.Content = "";
                }
            }
        }

        private bool IsValidText(string text)
        {
            Regex regex = new Regex("^[a-zA-Z0-9 ]*$");
            return regex.IsMatch(text);
        }

    }
}

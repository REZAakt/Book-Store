using Data_Access;
using System;
using System.Windows;
using Data_Access.Models;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Net;
using System.Windows.Media;
using System.Windows.Controls;
using System.Text.RegularExpressions;

namespace wpf
{
    public partial class AddEditCustomer : Window, INotifyPropertyChanged
    {
        #region PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));  
            }
        }
        #endregion

        private CustomerDataAccess customerDataAccess;
        private Customer editingCustomer;
        private bool isEdit = false;


        public AddEditCustomer(CustomerDataAccess cusDataAccess)
        {
            InitializeComponent();
            customerDataAccess = cusDataAccess;
        }


        public AddEditCustomer(CustomerDataAccess cusDataAccess, Customer cus)
        {
            InitializeComponent();
            customerDataAccess = cusDataAccess;
            editingCustomer = cus;
            isEdit = true;
            tbFirstNamec.Text = editingCustomer.FirstName;
            tbLastNamec.Text = editingCustomer.LastName;
            tbPhoneNumberc.Text = editingCustomer.PhoneNumber.ToString();
            tbAddressc.Text = editingCustomer.Address;
        }

        private void btnCancelc_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnOKc_Click(object sender, RoutedEventArgs e)
        {
            bool isValid = true;
            isValid = CheckEmployeeValidity();

            if (isValid) 
            {
                try
                {
                    Customer cus = new Customer()
                    {
                        FirstName = tbFirstNamec.Text,
                        LastName = tbLastNamec.Text,
                        PhoneNumber = Convert.ToUInt64(tbPhoneNumberc.Text),
                        Address = tbAddressc.Text,
                    };

                    if (isEdit)
                    {
                        cus.Id = editingCustomer.Id;
                        customerDataAccess.EditCustomer(cus);
                    }
                    else
                    {
                        cus.Id = customerDataAccess.GetNextId();
                        customerDataAccess.AddCustomer(cus);
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
            string FirstName = tbFirstNamec.Text.Trim().ToLower();
            string Lastname = tbLastNamec.Text.Trim().ToLower();
            string PhoneNumber = tbPhoneNumberc.Text.Trim().ToLower();
            string Address = tbAddressc.Text.Trim().ToLower();
            
            if (string.IsNullOrEmpty(FirstName))
            {
                isValid = false;
                //MessageBox.Show("   First name is Invalid !   ");
                lblError.Content = "* First name is Invalid !   ";
                tbFirstNamec.BorderBrush = Brushes.Red;
            }

            else if (string.IsNullOrEmpty(Lastname))
            {
                isValid = false;
                //MessageBox.Show("   Last name is Invalid !   ");
                lblError.Content = "* Last name is Invalid ! ";
                tbLastNamec.BorderBrush = Brushes.Red;
            }

            else if (!UInt64.TryParse(PhoneNumber, out ulong p))
            {

                isValid = false;
                lblError.Content = "* Phone number is Invalid ! ";
                //MessageBox.Show("   Phone number is Invalid !   ");
                 tbPhoneNumberc.BorderBrush = Brushes.Red;
               
            }
                        
            else if (Address.Contains("usa"))
            {
                isValid = false;
                //MessageBox.Show("  no , nooo , god nooo ");
                lblError.Content = "  no , nooo , god nooo";
                tbAddressc.BorderBrush = Brushes.Red;
            }

            return isValid;
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
                    textBox.CaretIndex = textBox.Text.Length; // Move caret to the end
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

using Data_Access;
using System;
using System.Windows;
using Data_Access.Models;
using System.Net;
using System.Windows.Media;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Text.RegularExpressions;

namespace wpf
{
    public partial class AddEditEmployee : Window, INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private EmployeeDataAccess employeeDataAccess;
        private Employee editingEmployee;
        private bool isEdit = false;



        public AddEditEmployee(EmployeeDataAccess empDataAccess)
        {
            InitializeComponent();
            employeeDataAccess = empDataAccess;
        }

        private string _btnContent1 = "OK";
        public string Count1
        {
            get
            {
                return _btnContent1;
            }
            set
            {

                _btnContent1 = value;
                OnPropertyChanged();
            }
        }

        private string _btnContent0 = "Cancel";
        public string Count0
        {
            get
            {
                return _btnContent0;
            }
            set
            {

                _btnContent0 = value;
                OnPropertyChanged();
            }
        }


        private void AddEditEmployee_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = this;
        }

        public AddEditEmployee(EmployeeDataAccess empDataAccess, Employee emp)
        {
            InitializeComponent();
            employeeDataAccess = empDataAccess;
            editingEmployee = emp;
            Loaded += AddEditEmployee_Loaded;
            isEdit = true;
            tbFirstName.Text = editingEmployee.FirstName;
            tbLastName.Text = editingEmployee.LastName;
            tbPhoneNumber.Text = editingEmployee.PhoneNumber.ToString();
            tbAddress.Text = editingEmployee.Address;
            tbSalary.Text = editingEmployee.BaseSalary.ToString();
            comboDeparyment.SelectedIndex = (int)editingEmployee.Department;
        }

        private void btnCansel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            bool isValid = true;
            isValid = CheckEmployeeValidity();

            if (isValid)

            {
                try
                {
                    Employee emp = new Employee()
                    {
                        FirstName = tbFirstName.Text,
                        LastName = tbLastName.Text,
                        PhoneNumber = Convert.ToUInt64(tbPhoneNumber.Text),
                        Address = tbAddress.Text,
                        BaseSalary = Convert.ToDecimal(tbSalary.Text),
                        Department = (Department)comboDeparyment.SelectedIndex
                    };

                    if (isEdit)
                    {
                        emp.Id = editingEmployee.Id;
                        employeeDataAccess.EditEmployees(emp);
                    }
                    else
                    {
                        emp.Id = employeeDataAccess.GetNextId();
                        employeeDataAccess.AddEmployee(emp);
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
            string FirstName = tbFirstName.Text.Trim().ToLower();
            string Lastname = tbLastName.Text.Trim().ToLower();
            string PhoneNumber = tbPhoneNumber.Text.Trim().ToLower();
            string Address = tbAddress.Text.Trim().ToLower();
            int Department = comboDeparyment.SelectedIndex;
            string BaseSalary = tbSalary.Text.Trim().ToLower();

            if (string.IsNullOrEmpty(FirstName))
            {
                isValid = false;
                //MessageBox.Show("   First name is Invalid !   ");
                tbFirstName.BorderBrush = Brushes.Red;
                lblerorr.Content = "* First name can not be empty  ! ";
            }

            else if (string.IsNullOrEmpty(Lastname))
            {
                isValid = false;
                lblerorr.Content = "* Last name can not be empty  ! ";
                //MessageBox.Show("   Last name is Invalid !   ");
                tbLastName.BorderBrush = Brushes.Red;
            }

            else if (!UInt64.TryParse(PhoneNumber, out ulong p))
            {

                isValid = false;
                //  MessageBox.Show("   Phone number is Invalid !   ");
                //  tbPhoneNumber.BorderBrush = Brushes.Red;
                lblerorr.Content = "* Phone number is incerrect  ! ";
            }

            else if (Address.Contains("usa"))
            {
                isValid = false;
                lblerorr.Content = "*  NO NO NO usa  , ";
                //MessageBox.Show("  NO NO NO usa  , you cant use this app if your Amrican , nigg  ");
                tbAddress.BorderBrush = Brushes.Red;
            }

            else if (Department < 0)
            {
                isValid = false;
                //MessageBox.Show("   Please select a Department !  ");
                lblerorr.Content = "* Please select a Department !  ";

                comboDeparyment.BorderBrush = Brushes.Red;
            }

            else if (!decimal.TryParse(BaseSalary, out decimal b) || b > 100000000000000)
            {
                isValid = false;
                //MessageBox.Show("   Salary is incerrect   ! ");
                tbSalary.BorderBrush = Brushes.Red;
                lblerorr.Content = "  ** Salary is incerrect  ! ";

            }

            else
            {

                lblerorr.Content = "";

            }

            return isValid;
        }

        private void tbPhoneNumber_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)

        {
            string PhoneNumber = tbPhoneNumber.Text.Trim().ToLower();
            if (!UInt64.TryParse(PhoneNumber, out ulong p))
            {

                lblerorr.Content = "  ** Phone number is incerrect  ! ";

            }
            else
            {
                lblerorr.Content = "";
            }

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
                    lblerorr.Content = "Only English letters and numbers are allowed.";
                }
                else
                {
                    lblerorr.Content = "";
                }

            }
        }
        private bool IsValidText(string text)
        {
            Regex regex = new Regex("^[a-zA-Z0-9]*$");
            return regex.IsMatch(text);
        }
    }

}
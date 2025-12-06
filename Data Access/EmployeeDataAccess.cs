using System;
using System.Collections.ObjectModel;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using Data_Access.Models;
namespace Data_Access
{
    public class EmployeeDataAccess
    {
        private string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Test11DB.accdb");
        private string connectionString;

        public ObservableCollection<Employee> Employees { get; set; } = new ObservableCollection<Employee>();

        public EmployeeDataAccess()
        {
            connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=Test11DB.accdb;Persist Security Info=False;";

            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"Could not find file at {path}");
            }

            ReadEmployee();
        }

        private void ReadEmployee()
        {
            Employees.Clear();

            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM Employee"; // Assuming the table name is Employee
                using (OleDbCommand command = new OleDbCommand(query, connection))
                {
                    using (OleDbDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Enum.TryParse(reader["Department"].ToString(), out Department dept);

                            Employee emp = new Employee()
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                FirstName = reader["FirstName"].ToString(),
                                LastName = reader["LastName"].ToString(),
                                PhoneNumber = reader["PhoneNumber"].ToString(),
                                Address = reader["Address"].ToString(),
                                Department = dept,
                                BaseSalary = Convert.ToDecimal(reader["BaseSalary"]),
                            };
                            Employees.Add(emp);
                        }
                    }
                }
            }
        }

        private void SaveEmployees()
        {
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                connection.Open();
                string deleteQuery = "DELETE FROM Employee";
                using (OleDbCommand deleteCommand = new OleDbCommand(deleteQuery, connection))
                {
                    deleteCommand.ExecuteNonQuery();
                }

                foreach (Employee emp in Employees)
                {
                    string insertQuery = "INSERT INTO Employee (Id, FirstName, LastName, PhoneNumber, Address, Department, BaseSalary) VALUES (@Id, @FirstName, @LastName, @PhoneNumber, @Address, @Department, @BaseSalary)";
                    using (OleDbCommand insertCommand = new OleDbCommand(insertQuery, connection))
                    {
                        insertCommand.Parameters.AddWithValue("@Id", emp.Id);
                        insertCommand.Parameters.AddWithValue("@FirstName", emp.FirstName);
                        insertCommand.Parameters.AddWithValue("@LastName", emp.LastName);
                        insertCommand.Parameters.AddWithValue("@PhoneNumber", emp.PhoneNumber);
                        insertCommand.Parameters.AddWithValue("@Address", emp.Address);
                        insertCommand.Parameters.AddWithValue("@Department", emp.Department.ToString());
                        insertCommand.Parameters.AddWithValue("@BaseSalary", emp.BaseSalary);

                        insertCommand.ExecuteNonQuery();
                    }
                }
            }
        }

        public void AddEmployee(Employee emp)
        {
            emp.Id = GetNextId();
            Employees.Add(emp);
            SaveEmployees();
        }

        public void RemoveEmployee(int Id)
        {
            Employee temp = Employees.FirstOrDefault(x => x.Id == Id);
            if (temp != null)
            {
                Employees.Remove(temp);
                SaveEmployees();
            }
        }

        public void EditEmployees(Employee emp)
        {
            Employee temp = Employees.FirstOrDefault(x => x.Id == emp.Id);
            if (temp != null)
            {
                int Index = Employees.IndexOf(temp);
                Employees[Index] = emp;
                SaveEmployees();
            }
        }

        public int GetNextId()
        {
            return Employees.Any() ? Employees.Max(x => x.Id) + 1 : 1;
        }
    }
}

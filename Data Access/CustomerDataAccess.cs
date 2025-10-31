using System;
using System.Collections.ObjectModel;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using Data_Access.Models;
namespace Data_Access
{
    public class CustomerDataAccess
    {
        private string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Test11DB.accdb");
        private string connectionString;

        public ObservableCollection<Customer> Customers { get; set; } = new ObservableCollection<Customer>();

        public CustomerDataAccess()
        {
            connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=Test11DB.accdb;Persist Security Info=False;";
            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"Could not find file at {path}");
            } 

            ReadCustomer();
        }

        private void ReadCustomer()
        {
            Customers.Clear();

            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM Customer"; // Assuming the table name is Customer
                using (OleDbCommand command = new OleDbCommand(query, connection))
                {
                    using (OleDbDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Customer cus = new Customer
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                FirstName = reader["FirstName"].ToString(),
                                LastName = reader["LastName"].ToString(),
                                PhoneNumber = Convert.ToUInt64(reader["PhoneNumber"]),
                                Address = reader["Address"].ToString(),
                            };
                            Customers.Add(cus);
                        }
                    }
                }
            }
        }
        private void SaveCustomer(Customer cus)
        {
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                connection.Open();
                string updateQuery = "UPDATE Customer SET FirstName = ?, LastName = ?, PhoneNumber = ?, Address = ? WHERE Id = ?";
                using (OleDbCommand command = new OleDbCommand(updateQuery, connection))
                {
                    command.Parameters.AddWithValue("?", cus.FirstName);
                    command.Parameters.AddWithValue("?", cus.LastName);
                    command.Parameters.AddWithValue("?", cus.PhoneNumber);
                    command.Parameters.AddWithValue("?", cus.Address);
                    command.Parameters.AddWithValue("?", cus.Id);

                    command.ExecuteNonQuery();
                }
            }
        }

        private void AddCustomerToDb(Customer cus)
        {
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                connection.Open();
                string insertQuery = $"INSERT INTO Customer (FirstName, LastName, PhoneNumber, Address) VALUES (?, ?, ?, ?)";
                using (OleDbCommand command = new OleDbCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("?", cus.FirstName);
                    command.Parameters.AddWithValue("?", cus.LastName);
                    command.Parameters.AddWithValue("?", cus.PhoneNumber);
                    command.Parameters.AddWithValue("?", cus.Address);

                    command.ExecuteNonQuery();
                }
            }
        }

        private void DeleteCustomerFromDb(int id)
        {
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                connection.Open();
                string deleteQuery = "DELETE FROM Customer WHERE Id = ?";
                using (OleDbCommand command = new OleDbCommand(deleteQuery, connection))
                {
                    command.Parameters.AddWithValue("?", id);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void AddCustomer(Customer cus)
        {
            cus.Id = GetNextId();
            Customers.Add(cus);
            AddCustomerToDb(cus);
        }

        public void RemoveCustomer(int Id)
        {
            Customer temp = Customers.FirstOrDefault(x => x.Id == Id);
            if (temp != null)
            {
                Customers.Remove(temp);
                DeleteCustomerFromDb(Id);
            }
        }

        public void EditCustomer(Customer cus)
        {
            Customer temp = Customers.FirstOrDefault(x => x.Id == cus.Id);
            if (temp != null)
            {
                int index = Customers.IndexOf(temp);
                Customers[index] = cus;
                SaveCustomer(cus);
            }
        }

        public int GetNextId()
        {
            return Customers.Any() ? Customers.Max(x => x.Id) + 1 : 1;
        }
    }
}

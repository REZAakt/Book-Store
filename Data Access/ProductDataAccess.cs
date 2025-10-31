using System;
using System.Collections.ObjectModel;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using Data_Access.Models;
namespace Data_Access
{
    public class ProductDataAccess
    {
        private string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Test11DB.accdb");
        private string connectionString;

        public ObservableCollection<Product> Products { get; set; } = new ObservableCollection<Product>();

        public ProductDataAccess()
        {
            connectionString = $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=Test11DB.accdb;Persist Security Info=False;";

            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"Could not find file at {path}");
            }

            ReadProducts();
        }

        private void ReadProducts()
        {
            Products.Clear();

            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM Product"; // Assuming the table name is Product
                using (OleDbCommand command = new OleDbCommand(query, connection))
                {
                    using (OleDbDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Product pro = new Product
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Name = reader["Name"].ToString(),
                                Author = reader["Author"].ToString(),
                                Price = Convert.ToUInt64(reader["Price"]),
                                AvailableCount = Convert.ToInt32(reader["AvailableCount"]),
                            };
                            Products.Add(pro);
                        }
                    }
                }
            }
        }

        private void SaveProduct(Product pro)
        {
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                connection.Open();
                string updateQuery = "UPDATE Product SET Name = ?, Author = ?, Price = ?, AvailableCount = ? WHERE Id = ?";
                using (OleDbCommand command = new OleDbCommand(updateQuery, connection))
                {
                    command.Parameters.AddWithValue("?", pro.Name);
                    command.Parameters.AddWithValue("?", pro.Author);
                    command.Parameters.AddWithValue("?", pro.Price);
                    command.Parameters.AddWithValue("?", pro.AvailableCount);
                    command.Parameters.AddWithValue("?", pro.Id);

                    command.ExecuteNonQuery();
                }
            }
        }

        private void AddProductToDb(Product pro)
        {
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                connection.Open();
                string insertQuery = "INSERT INTO Product (Name, Author, Price, AvailableCount) VALUES (?, ?, ?, ?)";
                using (OleDbCommand command = new OleDbCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("?", pro.Name);
                    command.Parameters.AddWithValue("?", pro.Author);
                    command.Parameters.AddWithValue("?", pro.Price);
                    command.Parameters.AddWithValue("?", pro.AvailableCount);

                    command.ExecuteNonQuery();
                }
            }
        }

        private void DeleteProductFromDb(int id)
        {
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                connection.Open();
                string deleteQuery = "DELETE FROM Product WHERE Id = ?";
                using (OleDbCommand command = new OleDbCommand(deleteQuery, connection))
                {
                    command.Parameters.AddWithValue("?", id);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void AddProduct(Product pro)
        {
            pro.Id = GetNextId();
            Products.Add(pro);
            AddProductToDb(pro);
        }

        public void RemoveProduct(int Id)
        {
            Product temp = Products.FirstOrDefault(x => x.Id == Id);
            if (temp != null)
            {
                Products.Remove(temp);
                DeleteProductFromDb(Id);
            }
        }

        public void EditProduct(Product pro)
        {
            Product temp = Products.FirstOrDefault(x => x.Id == pro.Id);
            if (temp != null)
            {
                int index = Products.IndexOf(temp);
                Products[index] = pro;
                SaveProduct(pro);
            }
        }

        public int GetNextId()
        {
            return Products.Any() ? Products.Max(x => x.Id) + 1 : 1;
        }
    }
}

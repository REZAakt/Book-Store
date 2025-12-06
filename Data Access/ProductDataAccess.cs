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

        public ObservableCollection<CustomerPurchaseViewModel> GetPurchasesByCustomer(int customerId)
        {
            var result = new ObservableCollection<CustomerPurchaseViewModel>();

            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                connection.Open();

                string query = @"
            SELECT cp.Id, cp.Quantity, cp.PurchaseDate,
                   p.Name, p.Author, p.Price
            FROM CustomerPurchase cp
            INNER JOIN Product p ON cp.ProductId = p.Id
            WHERE cp.CustomerId = ?
            ORDER BY cp.PurchaseDate DESC";

                using (OleDbCommand command = new OleDbCommand(query, connection))
                {
                    command.Parameters.AddWithValue("?", customerId);

                    using (OleDbDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var item = new CustomerPurchaseViewModel
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                BookName = reader["Name"].ToString(),
                                Author = reader["Author"].ToString(),
                                Price = Convert.ToDecimal(reader["Price"]),
                                Quantity = Convert.ToInt32(reader["Quantity"]),
                                PurchaseDate = Convert.ToDateTime(reader["PurchaseDate"])
                            };

                            result.Add(item);
                        }
                    }
                }
            }

            return result;
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
                                Price = Convert.ToDecimal(reader["Price"]),
                                AvailableCount = Convert.ToInt32(reader["AvailableCount"]),
                            };
                            Products.Add(pro);
                        }
                    }
                }
            }
        }

        //public ObservableCollection<CustomerPurchaseViewModel> GetPurchasesByCustomer(int customerId)
        //{
        //    var result = new ObservableCollection<CustomerPurchaseViewModel>();

        //    using (OleDbConnection connection = new OleDbConnection(connectionString))
        //    {
        //        connection.Open();

        //        string query = @"
        //    SELECT cp.Id, cp.Quantity, cp.PurchaseDate,
        //           p.Name, p.Author, p.Price
        //    FROM CustomerPurchase cp
        //    INNER JOIN Product p ON cp.ProductId = p.Id
        //    WHERE cp.CustomerId = ?
        //    ORDER BY cp.PurchaseDate DESC";

        //        using (OleDbCommand command = new OleDbCommand(query, connection))
        //        {
        //            command.Parameters.AddWithValue("?", customerId);

        //            using (OleDbDataReader reader = command.ExecuteReader())
        //            {
        //                while (reader.Read())
        //                {
        //                    var item = new CustomerPurchaseViewModel
        //                    {
        //                        Id = Convert.ToInt32(reader["Id"]),
        //                        BookName = reader["Name"].ToString(),
        //                        Author = reader["Author"].ToString(),
        //                        Price = Convert.ToDecimal(reader["Price"]),
        //                        Quantity = Convert.ToInt32(reader["Quantity"]),
        //                        PurchaseDate = Convert.ToDateTime(reader["PurchaseDate"])
        //                    };

        //                    result.Add(item);
        //                }
        //            }
        //        }
        //    }

        //    return result;
        //}


        public bool TryPurchaseProduct(int productId, int quantity, int customerId, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (quantity <= 0)
            {
                errorMessage = "Quantity must be greater than zero.";
                return false;
            }

            Product product = Products.FirstOrDefault(p => p.Id == productId);
            if (product == null)
            {
                errorMessage = "Selected book was not found.";
                return false;
            }

            if (product.AvailableCount < quantity)
            {
                errorMessage = "Requested quantity is more than available stock.";
                return false;
            }

            product.AvailableCount -= quantity;
            SaveProduct(product);

            SavePurchase(customerId, productId, quantity);

            return true;
        }

        private void SavePurchase(int customerId, int productId, int quantity)
        {
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                connection.Open();

                string insertQuery = @"
            INSERT INTO CustomerPurchase
                (CustomerId, ProductId, Quantity, PurchaseDate)
            VALUES (?, ?, ?, ?)";

                using (OleDbCommand command = new OleDbCommand(insertQuery, connection))
                {
                    // حتماً نوع پارامتر رو مشخص کن
                    command.Parameters.Add("?", OleDbType.Integer).Value = customerId;
                    command.Parameters.Add("?", OleDbType.Integer).Value = productId;
                    command.Parameters.Add("?", OleDbType.Integer).Value = quantity;
                    command.Parameters.Add("?", OleDbType.Date).Value = DateTime.Now;

                    command.ExecuteNonQuery();
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

                // اول اینسرت
                string insertQuery = "INSERT INTO Product (Name, Author, Price, AvailableCount) VALUES (?, ?, ?, ?)";
                using (OleDbCommand command = new OleDbCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("?", pro.Name);
                    command.Parameters.AddWithValue("?", pro.Author);
                    command.Parameters.AddWithValue("?", pro.Price);
                    command.Parameters.AddWithValue("?", pro.AvailableCount);

                    command.ExecuteNonQuery();
                }

                // بعد گرفتن Id واقعی که Access تولید کرده
                using (OleDbCommand idCommand = new OleDbCommand("SELECT @@IDENTITY", connection))
                {
                    object result = idCommand.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        // @@IDENTITY تو Access معمولاً به صورت decimal برمی‌گرده
                        pro.Id = Convert.ToInt32(result);
                    }
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
            // دیگه Id دستی نمی‌دیم
            AddProductToDb(pro);   // این خودش pro.Id رو ست می‌کنه
            Products.Add(pro);
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

        //public int GetNextId()
        //{
        //    return Products.Any() ? Products.Max(x => x.Id) + 1 : 1;
        //}
    }
}

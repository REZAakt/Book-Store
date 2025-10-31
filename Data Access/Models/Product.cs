using System;

namespace Data_Access.Models
{
    public class Product : IProduct
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Author { get; set; }
        public decimal Price { get; set; }
        public int AvailableCount { get; set; }

        public string GetBasicInfo()
        {
            string finalStr = $"Name: {Name}\nAuthor: {Author}\nPrice: {Price:C}\nAvailable Count: {AvailableCount}";
            return finalStr;
        }
    }
}
public interface IProduct
{
    int Id { get; set; }
    string Name { get; set; }
    string Author { get; set; }
    decimal Price { get; set; }
    int AvailableCount { get; set; }
    string GetBasicInfo();
}

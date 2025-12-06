using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Access.Models
{
    public class CustomerPurchaseViewModel
    {
        public int Id { get; set; }
        public string BookName { get; set; }
        public string Author { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public DateTime PurchaseDate { get; set; }
    }
}

using System;

namespace Data_Access.Models
{
    public class Employee : IPerson
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }  
        public Department Department { get; set; }
        public decimal BaseSalary { get; set; }

        public string GetBasicInfo()
        {
            string finalStr = FirstName + " " + LastName +
                "\n TELL : " + PhoneNumber +
                "\n Address : " + Address +
                "\n Department : " + Department +
                "\n Salary : " + BaseSalary;
            return finalStr;
        }
    }

    public enum Department
    {
        Production,
        Sales,
        Advertisement,
        Management
    }
   
}

using System;

namespace Data_Access.Models
{
    public class Customer : IPerson
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }


        public string GetBasicInfo()
        {

            string finalStr = "Name :" + FirstName + 
                " " + LastName +
               "\n Tell : " + PhoneNumber +
               "\n Address : " + Address;
            return finalStr;


        }
    }

  
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MeghanC_ShoppingCart.Models
{
    public class CustomerAddress
    {
        public string ShipToFirstName { get; set; }
        public string ShipToLastName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Country { get; set; }
        public string Phone { get; set; }
    }
}
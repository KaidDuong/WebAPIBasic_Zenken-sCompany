using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SampleAPI.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Nullable<decimal> Price { get; set; }
        public Nullable<decimal> quantity { get; set; }
        public Nullable<int> CategoryId { get; set; }

        public virtual Category Category { get; set; }
    }
}
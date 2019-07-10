﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SampleAPI.ViewModels
{
    public class ProductViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal? Price { get; set; }
        public decimal? Quantity { get; set; }
        public int? CategoryId { get; set; }
    }
}
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.Models
{
    public class Order
    {
        public int ID { get; set; }
        public string BuyerName { get; set; }
        public string Address { get; set; }
        public IEnumerable<OrderItem> Items { get; set; }
    }
}

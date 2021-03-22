using MySqlORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookStore.Models
{
    public class Book
    {
        public int ID { get; set; }

        public string Title { get; set; }

        public int CategoryID { get; set; }

        public float Price { get; set; }

        public List<Author> Authors { get; set; }
    }
}
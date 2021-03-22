using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.Models
{
    public class AuthorBookRelation
    {
        public int ID { get; set; }
        public int AuthorID { get; set; }
        public int BookID { get; set; }
    }
}

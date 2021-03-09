using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySqlORM
{
    class Program
    {
        static void Main(string[] args)
        {
            Type bookType = typeof(BookStore.Models.Book);
            var props = bookType.GetProperties();
            foreach (var prop in props)
            {
                Console.WriteLine(prop.Name);
            }


            Console.ReadKey();
        }
    }
}

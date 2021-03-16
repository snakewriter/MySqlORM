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


            var authorConnector = new MySqlCrudConnector<Authors>()
            {
                TableName = "authors_books",
                ConnectionString = "server=localhost;user id=root;database=book_store"
            };

            var connector = new MySqlCrudConnector<BookStore.Models.Book>()
            {
                TableName = "books",
                ConnectionString = "server=localhost;user id=root;database=book_store"
            };
            connector.Relations.Add("Authors", authorConnector);

            connector.OnErrorRaise += Connector_OnErrorRaise;
            var book = new BookStore.Models.Book()
            {
                Title = "Война и мир",
                Price = 300,
                Authors = new Authors() { AuthorID = 1 }
            };

            connector.CreateItem(book);


            Console.ReadKey();
        }

        private static void Connector_OnErrorRaise(string errorText)
        {
            Console.WriteLine(errorText);
        }
    }
}

using BookStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySqlORM
{
    class Program
    {
        static MySqlRelation abConnector = new MySqlRelation(typeof(AuthorBookRelation))
        {
            TableName = "authors_books"
        };
        static MySqlInsertConnector aConnector = new MySqlInsertConnector(typeof(Author))
        {
            TableName = "authors"
        };
        static MySqlInsertConnector bookConnector = new MySqlInsertConnector(typeof(Book))
        {
            TableName = "books"
        };


        static void Main(string[] args)
        {
            MySqlConnectorBase.ConnectionString = 
                "server=localhost;" +
                "user id=root;" +
                "database=book_store";
            MySqlConnectorBase.OnErrorRaise += Connector_OnErrorRaise;

            abConnector.RelationProperty = (b) => { return ((Book)b).Authors; };
            abConnector.TargetSourcePropsMap.Add("AuthorID", "FOREIGN.ID");
            abConnector.TargetSourcePropsMap.Add("BookID", "PARENT.ID");
            bookConnector.Relations.Add(abConnector);

            var book = new Book()
            {
                Title = "Обитаемый остров",
                CategoryID = 1,
                Price = 300,
                Authors = new List<Author>()
                {
                    new Author() { ID = 1 },
                    new Author() { ID = 2 }
                }
            };

            bookConnector.CreateItem(book);


            Console.ReadKey();
        }

        private static void Connector_OnErrorRaise(string errorText)
        {
            Console.WriteLine(errorText);
        }
    }
}

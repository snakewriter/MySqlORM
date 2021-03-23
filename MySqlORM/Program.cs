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


        static MySqlRelation oiConnector = new MySqlRelation(typeof(OrderItem))
        {
            TableName = "order_items"
        };
        static MySqlInsertConnector orderConnector = new MySqlInsertConnector(typeof(Order))
        {
            TableName = "orders"
        };


        static void Main(string[] args)
        {
            MySqlConnectorBase.ConnectionString = 
                "server=localhost;" +
                "user id=root;" +
                "database=book_store";
            MySqlConnectorBase.OnErrorRaise += Connector_OnErrorRaise;

            abConnector.RelationProperty = (b) => { return ((Book)b).Authors; };
            abConnector.TargetSourcePropsMap.Add("BookID", "ID");
            bookConnector.Relations.Add(abConnector);

            oiConnector.RelationProperty = (o) => { return ((Order)o).Items; };
            oiConnector.TargetSourcePropsMap.Add("OrderID", "ID");
            orderConnector.Relations.Add(oiConnector);

            //var book = new Book()
            //{
            //    Title = "Обитаемый остров",
            //    CategoryID = 1,
            //    Price = 300,
            //    Authors = new List<Author>()
            //    {
            //        new Author() { ID = 1 },
            //        new Author() { ID = 2 }
            //    }
            //};

            //bookConnector.CreateItem(book);

            var order = new Order()
            {
                BuyerName = "John Smith",
                Address = "NY City,  Wall Street",
                Items = new List<OrderItem>()
                {
                    new OrderItem() { BookID = 9, Quantity = 1 },
                    new OrderItem() { BookID = 10, Quantity = 1 },
                    new OrderItem() { BookID = 11, Quantity = 1 },
                }
            };
            orderConnector.CreateItem(order);

            Console.ReadKey();
        }

        private static void Connector_OnErrorRaise(string errorText)
        {
            Console.WriteLine(errorText);
        }
    }
}

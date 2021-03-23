using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySqlConnector;

namespace MySqlORM
{
    public class MySqlInsertConnector : MySqlConnectorBase
    {
        protected string getLastIdQuery = "SELECT LAST_INSERT_ID()";

        object item;
        int itemID;


        public MySqlInsertConnector(Type type) : base(type)
        {
            queryTemplate = "INSERT INTO {0} ({1}) VALUES ({2})";
        }


        public void CreateItem(object item)
        {
            this.item = item;
            itemID = 0;
            var queryText = $"{GetQueryText()}; {getLastIdQuery}";
            var command = SetupCommand(queryText, item);

            Execute(command);
            foreach (var relation in Relations)
            {
                relation.CreateItemsFor(item);
            }
        }

        public void CreateItems(IEnumerable<object> items)
        {

        }

        protected override void OnConnect(MySqlCommand command)
        {
            itemID = int.Parse(command.ExecuteScalar().ToString());
            entityType.GetProperty("ID").SetValue(item, itemID);
        }
    }
}

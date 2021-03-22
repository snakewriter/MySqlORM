using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySqlConnector;

namespace MySqlORM
{
    public class MySqlRelation : MySqlInsertConnector
    {
        List<object> itemsToInsert = new List<object>();
        DataTable itemsTable;

        public Func<object, IEnumerable<object>> RelationProperty { get; set; }
        /// <summary>
        /// Сопоставляет названия свойств родительской и внешней сущностей 
        /// в формате TYPE.PROPERTY, где TYPE  должно иметь значение
        /// PARENT если родительская сущность исходная, либо FOREIGN
        /// если родительская сущность является навигационным свойством
        /// </summary>
        public Dictionary<string, string> TargetSourcePropsMap
        {
            get;
        } = new Dictionary<string, string>();


        public MySqlRelation(Type type) : base(type)
        {
        }


        public void CreateItemsFor(object item)
        {
            var relatedItems = RelationProperty(item);            
            foreach (var relItem in relatedItems)
            {
                var itemToStore =
                    entityType != item.GetType() ?
                    Activator.CreateInstance(entityType) :
                    relItem;
                var itemsWithValues = new object[] { item, relItem };
                SetValuesForEntity(itemToStore, itemsWithValues);
                itemsToInsert.Add(itemToStore);
            }
            itemsTable = MySqlDataTableCreator.Create(itemsToInsert, 
                entityType);

            foreach (DataRow r in itemsTable.Rows)
            {
                Console.WriteLine($" {r["ID"]} {r["AuthorID"]} {r["BookID"]}");
            }

            Execute();
        }

        void SetValuesForEntity(object itemToStore, object[] itemsWithValues)
        {
            foreach (var propName in entityPropsNames)
            {
                var valueSource = TargetSourcePropsMap[propName];
                var valSrcSegments = valueSource.Split('.');
                var itemWithValues =
                    valSrcSegments[0] == "PARENT" ?
                    itemsWithValues[0] : itemsWithValues[1];
                var valueProp = 
                    itemWithValues.GetType().GetProperty(valSrcSegments[1]);
                var value = valueProp.GetValue(itemWithValues);
                entityType.GetProperty(propName).SetValue(itemToStore, value);
            }
        }

        protected void Execute()
        {
            var command = SetupCommand(GetQueryText());
            Execute(command);
        }

        protected override void OnConnect(MySqlCommand command)
        {
            var dataAdapter = new MySqlDataAdapter();
            dataAdapter.InsertCommand = command; 
            dataAdapter.Update(itemsTable);
        }
    }
}

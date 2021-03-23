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
                CreateItemFor(item, relItem, itemsToInsert);
             itemsTable = MySqlDataTableCreator
                .Create(itemsToInsert,entityType);
            Execute();
        }

        void CreateItemFor(object item, object relItem, List<object> items)
        {
            var itemToStore =
                entityType != item.GetType() ?
                Activator.CreateInstance(entityType) :
                relItem;
            var itemsWithValues = new object[] { item, relItem };
            SetValuesForEntity(itemToStore, itemsWithValues);
            items.Add(itemToStore);
        }

        void SetValuesForEntity(object itemToStore, object[] itemsWithValues)
        {
            foreach (var propName in entityPropsNames) SetValueForEntity(
                itemToStore,
                itemsWithValues,
                propName);
        }

        void SetValueForEntity(object target, object[] source, string propName)
        {
            object value =
                TargetSourcePropsMap.ContainsKey(propName) ?
                GetValueFromParent(source[0], propName) :
                source[1].GetType().GetProperty(propName).GetValue(source[1]);
            entityType.GetProperty(propName).SetValue(target, value);
        }

        object GetValueFromParent(object parent, string propName)
        {
            var valueSourcePropName = TargetSourcePropsMap[propName];
            var valueProp = parent.GetType().GetProperty(valueSourcePropName);
            return valueProp.GetValue(parent);
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
            //dataAdapter.SelectCommand = new MySqlCommand()
            //{ CommandText = $"SELECT ID FROM {TableName}" };
            //dataAdapter.SelectCommand.Parameters.Add(new MySqlParameter()
            //{                
            //    MySqlDbType = MySqlDbType.Int32,
            //    ParameterName = "ID",
            //    SourceColumn = "ID"
            //});
            dataAdapter.Update(itemsTable);
        }
    }
}

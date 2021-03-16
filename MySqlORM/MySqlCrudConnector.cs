using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySqlORM
{
    public class MySqlCrudConnector<T> : IMySqlConnector
    {
        string insertQueryTemplate = "INSERT INTO {0} ({1}) VALUES ({2}); SELECT LAST_INSERT_ID()";
        Type entityType;
        IEnumerable<string> entityPropsNames;
        IEnumerable<string> entQueryParams;
        MySqlConnection connection;


        public string TableName { get; set; }

        public string ConnectionString { get; set; }

        public Dictionary<string, IMySqlConnector> Relations { get; set; }
        = new Dictionary<string, IMySqlConnector>();

        public event Action<string> OnErrorRaise = msg => { };


        public MySqlCrudConnector()
        {
            entityType = typeof(T);
            entityPropsNames = entityType.GetProperties()
                .Where(pi => pi.Name != "ID" && pi.PropertyType.Namespace.Contains("System"))
                .Select(pi => pi.Name);
            entQueryParams = entityPropsNames.Select(pn => '@' + pn);
        }


        public void CreateItem(T item)
        {
            var queryText = string.Format(insertQueryTemplate,
                TableName,
                string.Join(", ", entityPropsNames),
                string.Join(", ", entQueryParams));
            SetupConnection();
            int itemID = 0;

            Connect(command =>
            {
                command.CommandText = queryText;
                SetCommandParams(command.Parameters, item, entityPropsNames);
                itemID = int.Parse(command.ExecuteScalar().ToString());
                entityType.GetProperty("ID").SetValue(item, itemID);
            });

            foreach (var relation in Relations)
            {
                var propName = relation.Key;
                var navPropValue = entityType.GetProperty(propName).GetValue(item);

                var navPropType = navPropValue.GetType();


                var navThisID = navPropType.GetProperty(entityType.Name + "ID");
                if (navThisID != null) navThisID.SetValue(navPropValue, itemID);


                var connector = relation.Value;
                connector.CreateItem(navPropValue);
            }
        }

        void IMySqlConnector.CreateItem(object item)
        {
            CreateItem((T)item);
        }

        void SetupConnection()
        {
            connection = new MySqlConnection(ConnectionString);
        }

        void SetCommandParams(MySqlParameterCollection parameters, T item, IEnumerable<string> props)
        {
            foreach (var propName in props) SetCommandParam(parameters, item, propName);
        }

        private void SetCommandParam(MySqlParameterCollection parameters, T item, string propName)
        {
            // Получаем значение из объекта item из свойства по имени propName
            var value = entityType.GetProperty(propName).GetValue(item);
            var parameter = new MySqlParameter('@' + propName, value);
            parameters.Add(parameter);
        }

        void Connect(Action<MySqlCommand> callback)
        {
            try
            {
                connection.Open();
                var command = connection.CreateCommand();
                callback(command);
            }
            catch (Exception e)
            {
                OnErrorRaise(e.Message);
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }
    }
}

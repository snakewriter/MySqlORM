using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MSPC = MySqlConnector.MySqlParameterCollection;
using IES = System.Collections.Generic.IEnumerable<string>;

namespace MySqlORM
{
    public abstract class MySqlConnectorBase
    {
        Dictionary<Type, MySqlDbType> netDbTypesMap = new Dictionary<Type, MySqlDbType>()
        {
            { typeof(int), MySqlDbType.Int32 },
            { typeof(float), MySqlDbType.Float },
            { typeof(string), MySqlDbType.VarChar }
        };

        protected string queryTemplate;
        protected Type entityType;
        protected IES entityPropsNames;
        protected IES entQueryParams;
        protected MySqlConnection connection;


        public static string ConnectionString { get; set; }
        public string TableName { get; set; }
        public List<MySqlRelation> Relations
        {
            get;
        } = new List<MySqlRelation>();

        public static event Action<string> OnErrorRaise;


        public MySqlConnectorBase(Type type)
        {
            entityType = type;
            entityPropsNames = entityType.GetProperties()
                .Where(pi => pi.Name != "ID" && pi.PropertyType.Namespace == "System")
                .Select(pi => pi.Name);
            entQueryParams = entityPropsNames.Select(pn => '@' + pn);
        }


        protected abstract void OnConnect(MySqlCommand command);


        protected void Execute(MySqlCommand command)
        {
            Connect(command);
        }               

        protected virtual string GetQueryText()
        {
            var queryText = string.Format(queryTemplate,
                TableName,
                string.Join(", ", entityPropsNames),
                string.Join(", ", entQueryParams));
            return queryText;
        }

        protected MySqlCommand SetupCommand(string queryText, object item)
        {
            var command = CreateCommand(queryText);
            SetCommandParams(command.Parameters, entityPropsNames, item);
            return command;
        }   
        
        protected MySqlCommand SetupCommand(string queryText)
        {
            var command = CreateCommand(queryText);
            SetCommandParams(command.Parameters, entityPropsNames);
            return command;
        }

        MySqlCommand CreateCommand(string queryText)
        {
            connection = new MySqlConnection(ConnectionString);
            var command = connection.CreateCommand();
            command.CommandText = queryText;
            return command;
        }

        void SetCommandParams(MSPC parameters, IES props, object item = null)
        {
            foreach (var propName in props)
            {
                SetCommandParam(parameters, propName, item);
            }
        }

        void SetCommandParam(MSPC parameters, string propName, object item = null)
        {
            var propType = entityType.GetProperty(propName)
                    .PropertyType;
            var parameter = new MySqlParameter()
            {
                ParameterName = '?' + propName,
                MySqlDbType = netDbTypesMap[propType]
            };
            if (item != null)
                // Получаем значение из объекта item из свойства по имени propName
                parameter.Value = entityType.GetProperty(propName).GetValue(item);
            parameters.Add(parameter);
        }

        private void Connect(MySqlCommand command)
        {
            try
            {
                connection.Open();
                OnConnect(command);
            }
            catch (Exception e)
            {
                OnErrorRaise?.Invoke(e.Message);
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }
    }
}

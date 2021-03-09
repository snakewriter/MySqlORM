using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySqlORM
{
    public class MySqlCrudConnector<T>
    {
        string insertQueryTemplate = "INSERT INTO {0} ({1}) VALUES ({2})";
        IEnumerable<string> entityPropsNames;
        MySqlConnection connection;


        public string TableName { get; set; }

        public string ConnectionString { get; set; }

        public event Action<string> OnErrorRaise = msg => { };


        public MySqlCrudConnector()
        {

        }


        public void CreateItem(T item)
        {
            SetupConnection();
            Connect(command =>
            {

            });
        }

        void SetupConnection()
        {
            connection = new MySqlConnection(ConnectionString);
        }

        void SetCommandParams(MySqlParameterCollection parameters)
        {

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

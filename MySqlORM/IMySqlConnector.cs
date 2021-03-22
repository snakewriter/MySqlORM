using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySqlORM
{
    public interface IMySqlConnector
    {
        string TableName { get; set; }
        string ConnectionString { get; set; }
        Dictionary<string, IMySqlConnector> Relations { get; set; }
        event Action<string> OnErrorRaise;

        void CreateItem(object item);
    }
}

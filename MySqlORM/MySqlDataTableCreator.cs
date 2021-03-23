using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MySqlORM
{
    public static class MySqlDataTableCreator
    {
        public static DataTable Create(IEnumerable<object> data, Type dataType)
        {
            var dataProps = dataType.GetProperties();
            var dataTable = new DataTable();

            foreach (var propInfo in dataProps) SetTableColumn(dataTable, propInfo);
            foreach (var item in data) FillTable(dataTable, dataProps, item);
            return dataTable;
        }

        private static void FillTable(DataTable dataTable, PropertyInfo[] dataProps, object item)
        {
            var dataRow = dataTable.NewRow();
            foreach (var propInfo in dataProps)
                dataRow[propInfo.Name] = propInfo.GetValue(item);
            dataTable.Rows.Add(dataRow);
        }

        static void SetTableColumn(DataTable table, PropertyInfo dataProp)
        {
            var column = new DataColumn(dataProp.Name, dataProp.PropertyType);
            table.Columns.Add(column);
        }
    }
}

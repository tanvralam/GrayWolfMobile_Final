using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;

namespace GrayWolf.Extensions
{
    public static class DataTableExtensions
    {
        public static DataTable FillColumns(this DataTable dataTable, List<string> columns)
        {
            var array = columns
                .Select(x => new DataColumn(x))
                .ToArray();

            dataTable.Columns
                .Add("DateTime");
            dataTable.Columns
                .AddRange(array);
            return dataTable;
        }

        public static DataTable FillRows(this DataTable dataTable, List<string> lines)
        {
            foreach(var line in lines)
            {
                var items = line
                    .Split(',')
                    .Select(x => x.Trim())
                    .ToArray();
                if(DateTime.TryParse(items.FirstOrDefault(), out var dateTime))
                {
                    items[0] = $"{dateTime.ToLocalTime().ToString(CultureInfo.CurrentUICulture)}";
                }

                dataTable.Rows
                    .Add(items);
            }

            return dataTable;
        }
    }
}

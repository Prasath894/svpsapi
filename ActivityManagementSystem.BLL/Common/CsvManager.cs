using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace Erp.Application.Common
{
    /// <summary>
    /// The Csv file manager class
    /// </summary>
    public class CsvManager
    {
        /// <summary>
        /// To Save Datatable to CSV file for given row numbers
        /// </summary>
        /// <param name="dataTable">The datatable which holds the the original data</param>
        /// <param name="filePath">The new Csv file to be stored</param>
        /// <param name="rowNumbers">The list of rows to be included in the new Csv file</param>
        public static void SaveDataTableToCsvByRowNumbers(DataTable dataTable, string filePath, string rowNumbers)
        {
            StringBuilder csvContent = new();
            foreach (DataColumn column in dataTable.Columns)
            {
                csvContent.Append($"\"{column.ColumnName}\",");
            }
            csvContent.AppendLine();

            int[] rowsToIncludeInCsv = Array.ConvertAll(rowNumbers.Split(','), int.Parse);

            int rowNumber = 1;
            // Write the selected rows to the CSV content
            foreach (DataRow row in dataTable.Rows)
            {
                if (rowsToIncludeInCsv.Contains(rowNumber))
                {
                    foreach (object value in row.ItemArray)
                    {
                        csvContent.Append($"\"{value}\",");
                    }
                    csvContent.AppendLine();
                }
                rowNumber++;
            }

            File.WriteAllText(filePath, csvContent.ToString());
        }
    }
}

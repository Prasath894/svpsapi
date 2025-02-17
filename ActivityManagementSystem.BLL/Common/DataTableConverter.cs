using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace Erp.Application.Common
{
    public class DataTableConverter
    {
        //Converting CSV formatted file to data table
        public static DataTable ConvertCsvToDataTable(string filePath)
        {
            DataTable dataTable = new();

            using (TextFieldParser parser = new(filePath))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");

                // Read column names from the first line of the CSV
                string[] fields = parser.ReadFields() ?? Array.Empty<string>();
                foreach (string field in fields)
                {
                    if (field == "S.No")
                    {
                        dataTable.Columns.Add(new DataColumn("SNo"));

                    }
                    else
                    {
                        dataTable.Columns.Add(new DataColumn(field));

                    }
                }

                // Read data rows from the CSV
                while (!parser.EndOfData)
                {
                    string[]? rows = parser.ReadFields();
                    if (rows != null)
                    {
                        dataTable.Rows.Add(rows);
                    }
                }
            }

            return dataTable;
        }

        // Method to convert List<Clsobj> to DataTable
        public static DataTable ConvertListToDataTable<T>(List<T> list)
        {
            DataTable dataTable = new();
            PropertyInfo[] props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            // Create columns for DataTable
            foreach (var prop in props)
            {
                dataTable.Columns.Add(prop.Name, prop.PropertyType);

            }

            // Add rows to DataTable
            foreach (var item in list)
            {
                DataRow dataRow = dataTable.NewRow();
                foreach (var prop in props)
                {
                    dataRow[prop.Name] = prop.GetValue(item);
                }
                dataTable.Rows.Add(dataRow);
            }

            return dataTable;
        }

        /// <summary>
        /// Adds an identity column to the provided DataTable if it doesn't already exist.
        /// The identity column contains incrementing integer values starting from 1.
        /// </summary>
        /// <param name="identityColumnName">The name of the identity column to add.</param>
        /// <param name="table">The DataTable to which the identity column will be added.</param>
        /// <returns>
        /// The DataTable with the identity column added or the original DataTable if the identity column already exists.
        /// </returns>
        public static DataTable AddIdentityColumn(string identityColumnName, DataTable table)
        {
            // Check if the identity column already exists
            if (!table.Columns.Contains(identityColumnName))
            {
                // Add the new column to the DataTable
                table.Columns.Add(identityColumnName, typeof(int));

                int identityRowNumber = 1;
                
                // Set values for the new column
                foreach (DataRow row in table.Rows)
                {
                    row[identityColumnName] = identityRowNumber;
                    identityRowNumber++;
                }
            }
            
            return table;
        }
    }
}

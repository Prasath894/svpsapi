using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.Application.Common
{
    public class ConvertDatasetAsJSON
    {
        public static string GetDataSetAsJson(DataSet dataSet, string[] tableNames)
        {
            for (int i = 0; i < tableNames.Length; i++)
            {
                dataSet.Tables[i].TableName = tableNames[i];
            }

            var settings = new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy
                    {
                        ProcessDictionaryKeys = true
                    }
                }
            };

            return JsonConvert.SerializeObject(dataSet, Formatting.Indented, settings);
        }
    }
}

using MDP.Data.MSSql;
using MDP.Registration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MDP.AspNetCore.Authentication.MSSql
{
    [Service<IDataProtectionKeyRepository>()]
    public class SqlDataProtectionKeyRepository : IDataProtectionKeyRepository
    {
        // Fields
        private readonly SqlClientFactory _sqlClientFactory = null;

        private readonly string _tableName = "__DataProtectionKeys";


        // Constructors
        public SqlDataProtectionKeyRepository(SqlClientFactory sqlClientFactory)
        {
            #region Contracts

            if (sqlClientFactory == null) throw new ArgumentException(nameof(sqlClientFactory));

            #endregion

            // Default
            _sqlClientFactory = sqlClientFactory;
        }


        // Methods
        public void StoreElement(XElement xmlElement, string friendlyName)
        {
            #region Contracts

            ArgumentNullException.ThrowIfNull(xmlElement);
            ArgumentNullException.ThrowIfNullOrEmpty(friendlyName);

            #endregion

            // SqlClient
            using (var sqlClient = _sqlClientFactory.CreateClient("DefaultDatabase"))
            {
                // CommandText
                sqlClient.CommandText = @$"
                    INSERT INTO [dbo].[{_tableName}] 
                    ( 
                        [FriendlyName],
                        [XmlData],
                        [CreateTime],
                        [UpdateTime]
                    )
                    VALUES 
                    (                         
                        @FriendlyName,
                        @XmlData,
                        @CreateTime,
                        @UpdateTime
                    )
                ";

                // Parameters
                sqlClient.AddParameter("@FriendlyName", friendlyName);
                sqlClient.AddParameter("@XmlData", xmlElement.ToString());
                sqlClient.AddParameter("@CreateTime", DateTime.Now);
                sqlClient.AddParameter("@UpdateTime", DateTime.Now);

                // Execute
                sqlClient.ExecuteNonQuery();
            }
        }

        public IReadOnlyCollection<XElement> GetAllElements()
        {
            // Result
            var elementList = new List<XElement>();

            // SqlClient
            using (var sqlClient = _sqlClientFactory.CreateClient("DefaultDatabase"))
            {
                // CommandText
                sqlClient.CommandText = @$"
                    SELECT [XmlData]
                      FROM [dbo].[{_tableName}]
                ";

                // Execute
                using (var reader = sqlClient.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // XmlData 
                        var xmlData = reader.GetString(0);
                        if (string.IsNullOrEmpty(xmlData) == true) continue;

                        // Add
                        elementList.Add(XElement.Parse(xmlData));
                    }
                }
            }

            // Return
            return elementList;
        }
    }
}

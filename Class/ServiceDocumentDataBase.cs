using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace Profisys_Zadanie.Class
{
    public class ServiceDocumentDataBase
    {
        private static readonly string _connectionString = ConfigurationManager.ConnectionStrings["DataBaseConnectionString"].ConnectionString;

        public static async Task InsertDocumentsAsync(List<Document> documents)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        foreach (var document in documents)
                        {
                            var command = new SqlCommand("INSERT INTO Documents (Id, Type, Date, FirstName, LastName, City) VALUES (@Id, @Type, @Date, @FirstName, @LastName, @City)", connection, transaction);
                            command.Parameters.AddWithValue("@Id", document.Id);
                            command.Parameters.AddWithValue("@Type", document.Type ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@Date", document.Date);
                            command.Parameters.AddWithValue("@FirstName", document.FirstName ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@LastName", document.LastName ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@City", document.City ?? (object)DBNull.Value);

                            await command.ExecuteNonQueryAsync();
                        }
                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        
                        throw;
                    }
                }
            }
        }
    }
}

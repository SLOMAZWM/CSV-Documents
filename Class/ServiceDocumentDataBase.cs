using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Configuration;
using System.Collections.ObjectModel;

namespace Profisys_Zadanie.Class
{ 
    public static class ServiceDocumentDataBase
    {
        private static readonly string _connectionString = ConfigurationManager.ConnectionStrings["DataBaseConnectionString"].ConnectionString;

        public static async Task<List<int>> InsertDocumentsAsync(List<Document> documents)
        {
            var newIds = new List<int>();
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        foreach (var document in documents)
                        {
                            string insertQuery = @"
                                INSERT INTO Documents (Type, Date, FirstName, LastName, City)
                                VALUES (@Type, @Date, @FirstName, @LastName, @City);
                                SELECT SCOPE_IDENTITY();";

                            using (var command = new SqlCommand(insertQuery, connection, transaction))
                            {
                                command.Parameters.AddWithValue("@Type", document.Type);
                                command.Parameters.AddWithValue("@Date", document.Date);
                                command.Parameters.AddWithValue("@FirstName", document.FirstName);
                                command.Parameters.AddWithValue("@LastName", document.LastName);
                                command.Parameters.AddWithValue("@City", document.City);

                                object newId = await command.ExecuteScalarAsync();
                                newIds.Add(Convert.ToInt32(newId));
                            }
                        }

                        transaction.Commit(); 
                    }
                    catch
                    {
                        transaction.Rollback(); 
                        throw; 
                    }
                }
            }
            return newIds;
        }


        public static async Task InsertDocumentItemsAsync(List<DocumentItems> items)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var transaction = connection.BeginTransaction()) 
                {
                    try
                    {
                        foreach (var item in items)
                        {
                            string insertQuery = @"
                                INSERT INTO DocumentItems (DocumentID, Ordinal, Product, Quantity, Price, TaxRate)
                                VALUES (@DocumentID, @Ordinal, @Product, @Quantity, @Price, @TaxRate);";

                            using (var command = new SqlCommand(insertQuery, connection, transaction))
                            {
                                command.Parameters.AddWithValue("@DocumentID", item.DocumentId);
                                command.Parameters.AddWithValue("@Ordinal", item.Ordinal);
                                command.Parameters.AddWithValue("@Product", item.Product);
                                command.Parameters.AddWithValue("@Quantity", item.Quantity);
                                command.Parameters.AddWithValue("@Price", item.Price);
                                command.Parameters.AddWithValue("@TaxRate", item.TaxRate);

                                
                                await command.ExecuteNonQueryAsync();
                            }
                        }

                        transaction.Commit(); 
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw; 
                    }
                }
            }
        }

        public static ObservableCollection<Document> GetAllInformationFromDocuments()
        {
            ObservableCollection<Document> documents = new ObservableCollection<Document>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "SELECT Id, Type, Date, FirstName, LastName, City FROM dbo.Documents";

                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Document document = new Document()
                            {
                                Id = reader.GetInt32(0),
                                Type = reader.IsDBNull(1) ? null : reader.GetString(1),
                                Date = reader.IsDBNull(2) ? null : reader.GetString(2),
                                FirstName = reader.IsDBNull(3) ? null : reader.GetString(3),
                                LastName = reader.IsDBNull(4) ? null : reader.GetString(4),
                                City = reader.IsDBNull(5) ? null : reader.GetString(5),
                                Items = new List<DocumentItems>()
                            };
                            documents.Add(document);
                        }
                    }
                }
            }
            return documents;
        }

    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Configuration;

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
                using (var transaction = connection.BeginTransaction()) // Rozpoczęcie transakcji
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

                                // Wykonaj polecenie INSERT i pobierz nowo wstawione ID.
                                object newId = await command.ExecuteScalarAsync();
                                newIds.Add(Convert.ToInt32(newId));
                            }
                        }

                        transaction.Commit(); // Zatwierdzenie transakcji
                    }
                    catch
                    {
                        transaction.Rollback(); // Cofnięcie transakcji w przypadku błędu
                        throw; // Ponowne zgłoszenie wyjątku do obsługi na wyższym poziomie
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
                using (var transaction = connection.BeginTransaction()) // Rozpoczęcie transakcji
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

                                // Wykonaj polecenie INSERT.
                                await command.ExecuteNonQueryAsync();
                            }
                        }

                        transaction.Commit(); // Zatwierdzenie transakcji
                    }
                    catch
                    {
                        transaction.Rollback(); // Cofnięcie transakcji w przypadku błędu
                        throw; // Ponowne zgłoszenie wyjątku do obsługi na wyższym poziomie
                    }
                }
            }
        }

    }
}

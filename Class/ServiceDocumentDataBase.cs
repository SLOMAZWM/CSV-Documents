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

        public static async Task<int> InsertDocumentAndItemsAsync(Document addDocument)
        {
            int newId = 0;
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        string insertQuery = @"
                        INSERT INTO Documents (Type, Date, FirstName, LastName, City)
                        VALUES (@Type, @Date, @FirstName, @LastName, @City);
                        SELECT SCOPE_IDENTITY();";

                        using (var command = new SqlCommand(insertQuery, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@Type", addDocument.Type);
                            command.Parameters.AddWithValue("@Date", DateTime.Parse(addDocument.Date).ToString("yyyy-MM-dd")); // Formatowanie daty
                            command.Parameters.AddWithValue("@FirstName", addDocument.FirstName);
                            command.Parameters.AddWithValue("@LastName", addDocument.LastName);
                            command.Parameters.AddWithValue("@City", addDocument.City);

                            object newIdObj = await command.ExecuteScalarAsync();
                            newId = Convert.ToInt32(newIdObj);


                            if (addDocument.Items != null && addDocument.Items.Count > 0)
                            {
                                foreach (var item in addDocument.Items)
                                {
                                    item.DocumentId = newId;
                                    string insertItemQuery = @"
                                    INSERT INTO DocumentItems (DocumentID, Ordinal, Product, Quantity, Price, TaxRate)
                                    VALUES (@DocumentID, @Ordinal, @Product, @Quantity, @Price, @TaxRate);";

                                    using (var itemCommand = new SqlCommand(insertItemQuery, connection, transaction))
                                    {
                                        itemCommand.Parameters.AddWithValue("@DocumentID", item.DocumentId);
                                        itemCommand.Parameters.AddWithValue("@Ordinal", item.Ordinal);
                                        itemCommand.Parameters.AddWithValue("@Product", item.Product);
                                        itemCommand.Parameters.AddWithValue("@Quantity", item.Quantity);
                                        itemCommand.Parameters.AddWithValue("@Price", item.Price);
                                        itemCommand.Parameters.AddWithValue("@TaxRate", item.TaxRate);

                                        await itemCommand.ExecuteNonQueryAsync();
                                    }
                                }
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

            return newId;
        }

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

        public static async Task<List<DocumentItems>> GetDocumentItemsByDocumentIdAsync(int documentId)
        {
            List<DocumentItems> items = new List<DocumentItems>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = @"
            SELECT DocumentID, Ordinal, Product, Quantity, Price, TaxRate
            FROM DocumentItems
            WHERE DocumentID = @DocumentID";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@DocumentID", documentId);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            items.Add(new DocumentItems
                            {
                                DocumentId = reader.GetInt32(0),
                                Ordinal = reader.GetInt16(1),
                                Product = reader.GetString(2),
                                Quantity = reader.GetInt16(3),
                                Price = reader.GetDecimal(4),
                                TaxRate = reader.GetByte(5)
                            });
                        }
                    }
                }
            }
            return items;
        }

        public static async Task UpdateDocumentAndItemsAsync(Document updatedDocument)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        string updateDocumentQuery = @"
                        UPDATE Documents
                        SET Type = @Type, Date = @Date, FirstName = @FirstName, LastName = @LastName, City = @City
                        WHERE Id = @Id";

                        using (var updateDocumentCommand = new SqlCommand(updateDocumentQuery, connection, transaction))
                        {
                            updateDocumentCommand.Parameters.AddWithValue("@Id", updatedDocument.Id);
                            updateDocumentCommand.Parameters.AddWithValue("@Type", updatedDocument.Type ?? (object)DBNull.Value);
                            updateDocumentCommand.Parameters.AddWithValue("@Date", DateTime.Parse(updatedDocument.Date).ToString("yyyy-MM-dd")); // Formatowanie daty
                            updateDocumentCommand.Parameters.AddWithValue("@FirstName", updatedDocument.FirstName ?? (object)DBNull.Value);
                            updateDocumentCommand.Parameters.AddWithValue("@LastName", updatedDocument.LastName ?? (object)DBNull.Value);
                            updateDocumentCommand.Parameters.AddWithValue("@City", updatedDocument.City ?? (object)DBNull.Value);

                            await updateDocumentCommand.ExecuteNonQueryAsync();
                        }

                        string deleteItemsQuery = "DELETE FROM DocumentItems WHERE DocumentID = @DocumentID";

                        using (var deleteItemsCommand = new SqlCommand(deleteItemsQuery, connection, transaction))
                        {
                            deleteItemsCommand.Parameters.AddWithValue("@DocumentID", updatedDocument.Id);
                            await deleteItemsCommand.ExecuteNonQueryAsync();
                        }

                        foreach (var item in updatedDocument.Items)
                        {
                            string insertItemQuery = @"
                        INSERT INTO DocumentItems (DocumentID, Ordinal, Product, Quantity, Price, TaxRate)
                        VALUES (@DocumentID, @Ordinal, @Product, @Quantity, @Price, @TaxRate);";

                            using (var insertItemCommand = new SqlCommand(insertItemQuery, connection, transaction))
                            {
                                insertItemCommand.Parameters.AddWithValue("@DocumentID", updatedDocument.Id);
                                insertItemCommand.Parameters.AddWithValue("@Ordinal", item.Ordinal);
                                insertItemCommand.Parameters.AddWithValue("@Product", item.Product ?? (object)DBNull.Value);
                                insertItemCommand.Parameters.AddWithValue("@Quantity", item.Quantity);
                                insertItemCommand.Parameters.AddWithValue("@Price", item.Price);
                                insertItemCommand.Parameters.AddWithValue("@TaxRate", item.TaxRate);

                                await insertItemCommand.ExecuteNonQueryAsync();
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

        public static async Task DeleteDocumentAndDocumentItemsAsync(int documentId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        string deleteItemsQuery = "DELETE FROM DocumentItems WHERE DocumentID = @DocumentID";
                        using (var deleteItemsCommand = new SqlCommand(deleteItemsQuery, connection, transaction))
                        {
                            deleteItemsCommand.Parameters.AddWithValue("@DocumentID", documentId);
                            await deleteItemsCommand.ExecuteNonQueryAsync();
                        }

                        string deleteDocumentQuery = "DELETE FROM Documents WHERE Id = @Id";
                        using (var deleteDocumentCommand = new SqlCommand(deleteDocumentQuery, connection, transaction))
                        {
                            deleteDocumentCommand.Parameters.AddWithValue("@Id", documentId);
                            await deleteDocumentCommand.ExecuteNonQueryAsync();
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

    }
}
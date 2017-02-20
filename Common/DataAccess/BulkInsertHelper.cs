using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Common.DataAccess
{
    public static class BulkInsertHelper
    {
        public static void BulkSave<T>(IEnumerable<T> records, string connectionString, string tableName)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();

                using (var bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, transaction))
                {
                    bulkCopy.BatchSize = 10000;
                    bulkCopy.DestinationTableName = tableName;
                    try
                    {
                        var dataTable = records.AsDataTable();
                        bulkCopy.WriteToServer(dataTable);
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        transaction.Rollback();
                        connection.Close();
                    }
                }

                
            }
        }
    }
}
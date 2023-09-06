using AuthoritySTS.Services.Shared;
using System;
using System.Data;
using System.Data.SqlClient;

namespace AuthoritySTS.Services.BLL
{
    public static class SqlHelper
    {
        /// <summary>
        /// return list of azure ad details for all company if they have azure ad
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="providerName"></param>
        /// <returns></returns>
        public static DataTable GetCompanyWiseAzureDetails(string connectionString, string providerName = null)
        {
            providerName = providerName ?? "ALL";
            DataTable dtAzureAdDetails = new DataTable();
            SqlConnection sqlConnection = SqlConnection(connectionString);
            try
            {
                using (var sqlCommand = new SqlCommand())
                {
                    sqlCommand.Connection = sqlConnection;
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.CommandText = Messages.GetCompanyWiseAzureAdDetails;
                    sqlCommand.Parameters.AddWithValue("@providerName", providerName);
                    SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                    sqlDataAdapter.Fill(dtAzureAdDetails);
                    SqlConnectionClose(sqlConnection);
                    sqlDataAdapter.Dispose();
                    sqlCommand.Parameters.Clear();
                }
            }
            catch (Exception ex)
            {}
            finally
            {
                SqlConnectionClose(sqlConnection);
            }
            return dtAzureAdDetails;
        }


        private static SqlConnection SqlConnection(string catalogConnectionString)
        {
            SqlConnection connection = new SqlConnection(catalogConnectionString);
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            return connection;
        }

        private static SqlConnection SqlConnectionClose(SqlConnection sqlConnection)
        {
            sqlConnection.Close();
            return sqlConnection;
        }
    }
}

namespace AppLock.SampleConsole
{
    using System;
    using System.Configuration;
    using System.Data.SqlClient;

    class Program
    {
        static void Main(string[] args)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["Test"].ConnectionString;
            var connection = new SqlConnection(connectionString);
            var appLock = SqlLock.Create(connection, "myResource");

            try
            {
                Console.WriteLine("Opening connection...");
                connection.Open();

                Console.WriteLine("Acquiring lock... {0}", appLock.ResourceName);

                appLock.Acquire(10000);

                Console.WriteLine("Lock {0}. Acquire result: ({1}) - {2}", 
                    appLock.IsAcquired ? "Granted" : "Denied", 
                    appLock.AcquireResultCode, 
                    appLock.AcquireResultMessage);

                Console.WriteLine("Press Enter to exit...");
                Console.Read();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: {0}. Press Enter to exit...", ex.Message);
                Console.Read();
            }
            finally
            {
                if (appLock.IsAcquired)
                {
                    Console.WriteLine("Releasing lock... {0}", appLock.ResourceName);
                    appLock.Release();
                }

                connection.Close();
            }
        }
    }
}

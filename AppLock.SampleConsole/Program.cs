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

            using (var connection = new SqlConnection(connectionString))
            {
                Console.WriteLine("Opening connection...");
                connection.Open();

                var lockService = new SqlLockService(connection);
                // var res = lockService.GetAppLockMode("myResource", "Session", "public");
                // var res = lockService.GetAppLock("myResource", "Exclusive", "Transaction", 1000, "public");
                // var res = lockService.ReleaseAppLock("myResource", "Session", "public");

                using (var txn = connection.BeginTransaction())
                {
                    var res = lockService.CanGetAppLock("myResource", "Exclusive", "Transaction", "public");
                    Console.WriteLine("App lock Mode result {0}", res);
                    Console.WriteLine("Press Enter to release lock and exit...");
                    Console.Read();

                    txn.Rollback();
                }

                connection.Close();
            }
        }
    }
}

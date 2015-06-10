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

                using (var txn = connection.BeginTransaction())
                {
                    var appLock = SqlLock.Create(txn, "myResource");
                    var res = appLock.Acquire(1000);

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

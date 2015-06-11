# app-lock
SQL Server based application manageable lock mechanism.

There is the set of the tasks when you need to serialize access to some code. Assume you have multiple instances of the data processing applications running simultaneously and does not want them to load the same data for the processing.

SQL Server provides an application manageable lock mechanism through the sp\_getapplock / sp\_releaseapplock pair of system stored procedures. They provide a way for application code to use SQL's underlying locking mechanism, without having to lock database rows. The lock can be tied to a transaction or session ensuring lock release when the transaction COMMITs or ROLLSBACK or when the session exits and the connection is closed.

AppLock is a lightweight .NET library that makes it easy to set up and use fully distributed locks.

Sample:

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
                connection.Close();
            }


More info: [http://msdn.microsoft.com/en-us/ms189823](http://msdn.microsoft.com/en-us/ms189823)

AppLock is available for download as a NuGet package.
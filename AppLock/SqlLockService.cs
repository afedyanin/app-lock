namespace AppLock
{
    using System.Data;
    using System.Data.SqlClient;

    public class SqlLockService
    {
        private readonly SqlConnection connection;

        public SqlLockService(SqlConnection connection)
        {
            this.connection = connection;
        }

        public int GetAppLock(string resourceName, string lockMode, string lockOwner, int lockTimeoutMilliSec, string dbPrincipal)
        {
            using (var command = this.connection.CreateCommand())
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "dbo.sp_getapplock";

                command.Parameters.AddWithValue("Resource", resourceName);
                command.Parameters.AddWithValue("LockMode", lockMode);
                command.Parameters.AddWithValue("LockOwner", lockOwner);
                command.Parameters.AddWithValue("LockTimeout", lockTimeoutMilliSec);
                command.Parameters.AddWithValue("DbPrincipal", dbPrincipal);

                var returnValue = new SqlParameter { Direction = ParameterDirection.ReturnValue };
                command.Parameters.Add(returnValue);

                command.ExecuteNonQuery();

                return (int) returnValue.Value;
            }
        }
        public int ReleaseAppLock(string resourceName, string lockOwner, string dbPrincipal)
        {
            using (var command = this.connection.CreateCommand())
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "dbo.sp_releaseapplock";

                command.Parameters.AddWithValue("Resource", resourceName);
                command.Parameters.AddWithValue("LockOwner", lockOwner);
                command.Parameters.AddWithValue("DbPrincipal", dbPrincipal);

                var returnValue = new SqlParameter { Direction = ParameterDirection.ReturnValue };
                command.Parameters.Add(returnValue);

                command.ExecuteNonQuery();

                return (int)returnValue.Value;
            }
        }
        public bool CanGetAppLock(string resourceName, string lockMode, string lockOwner, string dbPrincipal)
        {
            using (var command = this.connection.CreateCommand())
            {
                command.CommandType = CommandType.Text;
                command.CommandText = "SELECT APPLOCK_TEST(@DbPrincipal, @Resource, @LockMode, @LockOwner)";

                command.Parameters.AddWithValue("Resource", resourceName);
                command.Parameters.AddWithValue("LockMode", lockMode);
                command.Parameters.AddWithValue("LockOwner", lockOwner);
                command.Parameters.AddWithValue("DbPrincipal", dbPrincipal);

                return (int)command.ExecuteScalar() > 0;
            }
        }
        public string GetAppLockMode(string resourceName, string lockOwner, string dbPrincipal)
        {
            using (var command = this.connection.CreateCommand())
            {
                command.CommandType = CommandType.Text;
                command.CommandText = "SELECT APPLOCK_MODE(@DbPrincipal, @Resource, @LockOwner)";

                command.Parameters.AddWithValue("Resource", resourceName);
                command.Parameters.AddWithValue("LockOwner", lockOwner);
                command.Parameters.AddWithValue("DbPrincipal", dbPrincipal);

                return (string) command.ExecuteScalar();
            } 
        }
    }
}

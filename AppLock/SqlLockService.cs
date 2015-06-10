namespace AppLock
{
    using System;
    using System.Data;
    using System.Data.SqlClient;

    public class SqlLockService
    {
        public SqlConnection Connection { get; set; }
        public SqlTransaction Transaction { get; set; }

        public SqlLockService(SqlTransaction transaction)
        {
            this.Transaction = transaction;
            this.Connection = transaction.Connection;
        }

        public SqlLockService(SqlConnection connection)
        {
            this.Connection = connection;
        }

        public virtual int GetAppLock(string resourceName, int timeout, string mode, string owner, string dbPrincipal)
        {
            this.ThrowIfNotValidConnection();

            using (var command = this.Connection.CreateCommand())
            {
                this.JoinTransaction(command);

                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "dbo.sp_getapplock";

                // TODO: Check command timeout
                // command.CommandTimeout = timeout > 0 ? 

                command.Parameters.AddWithValue("Resource", resourceName);
                command.Parameters.AddWithValue("LockMode", mode);
                command.Parameters.AddWithValue("LockOwner", owner);
                command.Parameters.AddWithValue("LockTimeout", timeout);
                command.Parameters.AddWithValue("DbPrincipal", dbPrincipal);

                var returnValue = new SqlParameter { Direction = ParameterDirection.ReturnValue };
                command.Parameters.Add(returnValue);

                command.ExecuteNonQuery();

                return (int)returnValue.Value;
            }
        }

        public virtual int ReleaseAppLock(string resourceName, string owner, string dbPrincipal)
        {
            this.ThrowIfNotValidConnection();

            using (var command = this.Connection.CreateCommand())
            {
                this.JoinTransaction(command);

                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "dbo.sp_releaseapplock";

                command.Parameters.AddWithValue("Resource", resourceName);
                command.Parameters.AddWithValue("LockOwner", owner);
                command.Parameters.AddWithValue("DbPrincipal", dbPrincipal);

                var returnValue = new SqlParameter { Direction = ParameterDirection.ReturnValue };
                command.Parameters.Add(returnValue);

                command.ExecuteNonQuery();

                return (int)returnValue.Value;
            }
        }

        public virtual string GetAppLockMode(string resourceName, string owner, string dbPrincipal)
        {
            this.ThrowIfNotValidConnection();

            using (var command = this.Connection.CreateCommand())
            {
                this.JoinTransaction(command);

                command.CommandType = CommandType.Text;
                command.CommandText = "SELECT APPLOCK_MODE(@DbPrincipal, @Resource, @LockOwner)";

                command.Parameters.AddWithValue("Resource", resourceName);
                command.Parameters.AddWithValue("LockOwner", owner);
                command.Parameters.AddWithValue("DbPrincipal", dbPrincipal);

                return (string)command.ExecuteScalar();
            } 
        }

        public virtual int AppLockTest(string resourceName, string mode, string owner, string dbPrincipal)
        {
            this.ThrowIfNotValidConnection();

            using (var command = this.Connection.CreateCommand())
            {
                this.JoinTransaction(command);

                command.CommandType = CommandType.Text;
                command.CommandText = "SELECT APPLOCK_TEST(@DbPrincipal, @Resource, @LockMode, @LockOwner)";

                command.Parameters.AddWithValue("Resource", resourceName);
                command.Parameters.AddWithValue("LockMode", mode);
                command.Parameters.AddWithValue("LockOwner", owner);
                command.Parameters.AddWithValue("DbPrincipal", dbPrincipal);

                return (int)command.ExecuteScalar();
            }
        }

        protected void JoinTransaction(SqlCommand command)
        {
            if (this.Transaction != null)
            {
                command.Transaction = this.Transaction;
            }
        }

        protected void ThrowIfNotValidConnection()
        {
            if (this.Connection == null)
            {
                throw new InvalidOperationException("Connection is null.");
            }
        }
    }
}
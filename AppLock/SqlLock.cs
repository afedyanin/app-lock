namespace AppLock
{
    using System.Data.SqlClient;

    public class SqlLock
    {
        private const string CONST_DefaultPrincipal = "public";
        private const LockMode CONST_DefaultLockMode = LockMode.Exclusive;
        private const int CONST_AcquireResultUndefined = -9999;

        protected SqlLockService LockService { get; set; }

        public string ResourceName { get; set; }
        public LockMode Mode { get; set; }
        public LockOwner Owner { get; set; }
        public string DbPrincipal { get; set; }

        public int AcquireResultCode { get; private set; }
        public string AcquireResultMessage
        {
            get
            {
                return GetAcquireResultMessage(this.AcquireResultCode);
            }
        }
        public bool IsAcquired
        {
            get
            {
                return this.AcquireResultCode >= 0;
            }
        }

        protected SqlLock(SqlLockService service, string resourceName, LockOwner owner)
        {
            this.LockService = service;
            this.ResourceName = resourceName;
            this.Owner = owner;

            this.Mode = CONST_DefaultLockMode;
            this.DbPrincipal = CONST_DefaultPrincipal;
            this.AcquireResultCode = CONST_AcquireResultUndefined;
        }

        public static SqlLock Create(SqlConnection connection, string lockName)
        {
            var service = new SqlLockService(connection);
            var res = new SqlLock(service, lockName, LockOwner.Session);
            return res;
        }
        public static SqlLock Create(SqlTransaction transaction, string lockName, LockOwner owner)
        {
            var service = new SqlLockService(transaction);
            var res = new SqlLock(service, lockName, owner);
            return res;
        }

        public bool Acquire(int lockTimeoutMilliSec)
        {
            this.AcquireResultCode = this.LockService.GetAppLock(
                this.ResourceName, 
                lockTimeoutMilliSec, 
                this.Mode.GetString(),
                this.Owner.GetString(), 
                this.DbPrincipal);

            return this.IsAcquired;
        }
        public bool Release(bool throwIfNotHeld = false)
        {
            try
            {
                var res = this.LockService.ReleaseAppLock(
                    this.ResourceName,
                    this.Owner.GetString(),
                    this.DbPrincipal);

                return res >= 0;
            }
            catch (SqlException)
            {
                if (throwIfNotHeld)
                {
                    throw;
                }

                return false;
            }
        }
        public bool CanAcquire()
        {
            var res = this.LockService.AppLockTest(
                this.ResourceName, 
                this.Mode.GetString(), 
                this.Owner.GetString(),
                this.DbPrincipal);

            return res > 0;
        }
        public LockMode GetActiveLockMode()
        {
            var res = this.LockService.GetAppLockMode(
                this.ResourceName, 
                this.Owner.GetString(), 
                this.DbPrincipal);
            
            return EnumHelper.GetLockMode(res);
        }

        private static string GetAcquireResultMessage(int resultCode)
        {
            switch (resultCode)
            {
                case 0:
                    return "The lock was successfully granted synchronously.";
                case 1:
                    return "The lock was granted successfully after waiting for other incompatible locks to be released.";
                case -1:
                    return "The lock request timed out.";
                case -2:
                    return "The lock request was canceled.";
                case -3:
                    return "The lock request was chosen as a deadlock victim.";
                case -999:
                    return "Parameter validation or other call error.";
                case CONST_AcquireResultUndefined:
                    return "Undefined";
            }

            return resultCode > 0 ? "Successfully granted." : "Unknown error.";
        }
    }
}

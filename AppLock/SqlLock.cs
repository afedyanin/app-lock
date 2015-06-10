namespace AppLock
{
    using System.Data.SqlClient;

    public class SqlLock
    {
        protected const string DefaultPrincipal = "public";
        protected const LockOwner DefaultLockOwner = LockOwner.Session;
        protected const LockMode DefaultLockMode = LockMode.Exclusive;

        protected SqlLockService LockService { get; set; }

        public string ResourceName { get; set; }
        public LockMode Mode { get; set; }
        public LockOwner Owner { get; set; }
        public string DbPrincipal { get; set; }

        protected SqlLock(SqlLockService service, string resourceName)
        {
            this.LockService = service;
            this.ResourceName = resourceName;
            
            this.DbPrincipal = DefaultPrincipal;
            this.Mode = DefaultLockMode;
            this.Owner = DefaultLockOwner;
        }

        public static SqlLock Create(SqlConnection connection, string lockName)
        {
            var service = new SqlLockService(connection);
            var res = new SqlLock(service, lockName);
            return res;
        }

        public static SqlLock Create(SqlTransaction transaction, string lockName)
        {
            var service = new SqlLockService(transaction);
            var res = new SqlLock(service, lockName);
            return res;
        }

/*
        public AcquireResult AcquireResult { get; private set; }

        public bool IsAscuired
        {
            get
            {
                return this.AcquireResult == AcquireResult.SuccessGrantedAfterWaiting ||
                       this.AcquireResult == AcquireResult.SuccessGrantedSynchronously;
            }
        }
*/

        public bool Acquire(int lockTimeoutMilliSec)
        {
            var res = this.LockService.GetAppLock(
                this.ResourceName, 
                lockTimeoutMilliSec, 
                this.Mode.GetString(),
                this.Owner.GetString(), 
                this.DbPrincipal);

            // TODO: Set Acquire result 

            return res >= 0;
        }

        public bool Release()
        {
            var res = this.LockService.ReleaseAppLock(
                this.ResourceName, 
                this.Owner.GetString(), 
                this.DbPrincipal);
            
            // TODO: Catch exception if lock does not exist 
            return res >= 0;
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
    }
}

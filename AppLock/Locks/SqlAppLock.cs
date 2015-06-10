namespace AppLock
{
    using System;
    using System.Data;
    using System.Data.SqlClient;

    public abstract class SqlAppLock
    {
        protected const string DefaultPrincipal = "public";

        private readonly SqlAppLockService lockService;

        public string ResourceName { get; set; }
        public LockMode Mode { get; set; }
        public LockOwner Owner { get; set; }
        public string DbPrincipal { get; set; }

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

        protected SqlAppLock(SqlAppLockService lockService, string resourceName, LockMode mode, LockOwner owner, string dbPrincipal = DefaultPrincipal)
        {
            this.DbPrincipal = dbPrincipal;
            this.lockService = lockService;
            this.ResourceName = resourceName;
            this.Owner = owner;
            this.Mode = mode;
        }

        public bool Acquire(int lockTimeoutMilliSec)
        {
            this.ThrowIfNotValidLockModeInput(this.Mode);
            
            var res = this.lockService.GetAppLock(
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
            var res = this.lockService.ReleaseAppLock(
                this.ResourceName, 
                this.Owner.GetString(), 
                this.DbPrincipal);
            
            return res >= 0;
        }
        
        public bool CanAcquire()
        {
            this.ThrowIfNotValidLockModeInput(this.Mode);

            var res = this.lockService.AppLockTest(
                this.ResourceName, 
                this.Mode.GetString(), 
                this.Owner.GetString(),
                this.DbPrincipal);

            return res > 0;
        }

        public LockMode GetActiveLockMode()
        {
            var res = this.lockService.GetAppLockMode(
                this.ResourceName, 
                this.Owner.GetString(), 
                this.DbPrincipal);
            
            return EnumHelper.GetLockMode(res);
        }

        private void ThrowIfNotValidLockModeInput(LockMode lockMode)
        {
            if (lockMode.IsValidAsInputParameter())
            {
                return;
            }

            throw new InvalidOperationException("Parameter lockMode has invalid value.");
        }
    }
}

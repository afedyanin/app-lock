namespace AppLock.Locks
{
    public class ExclusiveTransactionLock : SqlAppLock
    {
        public ExclusiveTransactionLock(SqlAppLockService lockService, string resourceName) : 
            base(lockService, resourceName, LockMode.Exclusive, LockOwner.Transaction)
        {
        }
    }
}

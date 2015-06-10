namespace AppLock
{
    public enum LockResult
    {
        SuccessGrantedSynchronously = 0,
        SuccessGrantedAfterWaiting = 1,
        ErrorLockRequestTimedOut = -1,
        ErrorLockRequestCanceled = -2,
        ErrorLockRequestDeadlockVictim = -3,
        ErrorCall = -999
    }
}
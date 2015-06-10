namespace AppLock
{
    public enum AcquireResult
    {
        SuccessGrantedSynchronously = 0,
        SuccessGrantedAfterWaiting = 1,
        SuccessUnknown = 999,
        ErrorLockRequestTimedOut = -1,
        ErrorLockRequestCanceled = -2,
        ErrorLockRequestDeadlockVictim = -3,
        ErrorCall = -999,
        ErrorUnknown = -9999,
    }
}
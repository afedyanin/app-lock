namespace AppLock
{
    public enum LockMode
    {
        Undefined,
        NoLock,
        Shared,
        Update,
        IntentShared,
        IntentExclusive,
        Exclusive,
        SharedIntentExclusive,
        UpdateIntentExclusive
    }
}
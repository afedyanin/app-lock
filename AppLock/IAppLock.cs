namespace AppLock
{
    using System;

    public interface IAppLock : IDisposable
    {
        string Name { get; }
        string DbPrincipal { get; }
        LockOwner Owner { get; }
    }
}
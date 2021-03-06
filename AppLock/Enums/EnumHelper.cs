﻿namespace AppLock
{
    using System;

    public static class EnumHelper
    {
        public static string GetString(this LockMode mode)
        {
            switch (mode)
            {
                case LockMode.NoLock:
                    return "NoLock";
                case LockMode.Shared:
                    return "Shared";
                case LockMode.Update:
                    return "Update";
                case LockMode.IntentExclusive:
                    return "IntentExclusive";
                case LockMode.IntentShared:
                    return "IntentShared";
                case LockMode.Exclusive:
                    return "Exclusive";
                case LockMode.SharedIntentExclusive:
                    return "SharedIntentExclusive";
                case LockMode.UpdateIntentExclusive:
                    return "UpdateIntentExclusive";

                default :
                    return mode.ToString();
            }
        }

        public static string GetString(this LockOwner owner)
        {
            switch (owner)
            {
                case LockOwner.Session:
                    return "Session";
                case LockOwner.Transaction:
                    return "Transaction";

                default:
                    return owner.ToString();
            }
        }

        public static LockMode GetLockMode(string lockModeString)
        {
            LockMode res;
            return Enum.TryParse(lockModeString, true, out res) ? res : LockMode.Undefined;
        }
    }
}
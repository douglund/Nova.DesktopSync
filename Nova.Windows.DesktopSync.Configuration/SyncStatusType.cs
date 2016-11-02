using System;

namespace Nova.Windows.DesktopSync.Configuration
{
    [Serializable]
    public enum SyncStatusType
    {
        None,
        Copied,
        Failed,
        AlreadyExist,
        ExceedSizeLimit,
        BelowSizeLimit
    }
}
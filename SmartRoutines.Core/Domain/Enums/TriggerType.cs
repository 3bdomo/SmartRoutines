namespace SmartRoutines.Core.Domain.Enums
{
    public enum TriggerType : byte
    {
        Unknown = 0,
        Time = 1,
        Battery = 2,
        WiFi = 3,
        Startup = 4,
        Shutdown = 5,
        Idle = 6,
        AppOpen = 7,
        ProcessClose = 8
    }
}

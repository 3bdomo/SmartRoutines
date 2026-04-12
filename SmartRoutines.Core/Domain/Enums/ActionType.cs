namespace SmartRoutines.Core.Domain.Enums
{
    public enum ActionType : byte
    {
        Unknown = 0,
        LaunchApp = 1,
        OpenUrl = 2,
        SetVolume = 3,
        Mute = 4,
        KillProcess = 5,
        Notification = 6,
    }
}

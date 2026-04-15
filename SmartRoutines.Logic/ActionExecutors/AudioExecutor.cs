using SmartRoutines.Core.Domain.Enums;
using SmartRoutines.Core.Domain.Models;
using SmartRoutines.Core.Exceptions;
using SmartRoutines.Core.Interfaces.Logic;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text.Json;

namespace SmartRoutines.Logic.ActionExecutors;

/// <summary>
/// Executes system master-audio actions (mute and volume level).
/// This executor is CPU-bound and synchronous; the <see cref="ActionRunner"/>
/// is responsible for dispatching it on a thread-pool thread via <c>Task.Run</c>.
/// </summary>
[SupportedOSPlatform("windows")]
public sealed class AudioExecutor : IAction
{
    /// <inheritdoc />
    public IReadOnlyCollection<ActionType> SupportedActionTypes { get; } =
    [
        ActionType.SetVolume,
        ActionType.Mute
    ];

    /// <inheritdoc />
    /// <remarks>
    /// Returns <see cref="Task.CompletedTask"/> — all COM calls are synchronous.
    /// Threading is delegated to <c>ActionRunner</c> per the layered async contract.
    /// </remarks>
    public Task ExecuteAsync(RuntimeAction action, ActionContext context, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentNullException.ThrowIfNull(action);
        ArgumentNullException.ThrowIfNull(context);

        if (!OperatingSystem.IsWindows())
            throw new PlatformNotSupportedException("Audio actions are only supported on Windows.");

        try
        {
            switch (action.Type)
            {
                case ActionType.SetVolume:
                    WindowsAudioApi.SetMasterVolume(ParseVolumeLevel(action.Arguments));
                    break;

                case ActionType.Mute:
                    WindowsAudioApi.SetMute(ParseMuteValue(action.Arguments));
                    break;

                default:
                    throw new ArgumentException(
                        $"Unsupported action type for audio executor: {action.Type}.",
                        nameof(action));
            }
        }
        catch (Exception ex)
        {
            throw new ActionFailedException(action.Type, $"Audio action failed: {ex.Message}", ex);
        }

        return Task.CompletedTask;
    }

    // ── Argument parsers ────────────────────────────────────────────────────

    private static int ParseVolumeLevel(string arguments)
    {
        if (string.IsNullOrWhiteSpace(arguments))
            throw new ArgumentException(
                "SetVolume action requires an argument value between 0 and 100.",
                nameof(arguments));

        var payload = arguments.Trim();
        if (!payload.StartsWith("{", StringComparison.Ordinal))
            return ValidateVolumeLevel(int.Parse(payload));

        var parsed = JsonSerializer.Deserialize<VolumePayload>(payload)
                     ?? throw new ArgumentException("SetVolume JSON payload is invalid.", nameof(arguments));

        return ValidateVolumeLevel(parsed.Level);
    }

    private static bool ParseMuteValue(string arguments)
    {
        if (string.IsNullOrWhiteSpace(arguments))
            return true;

        var payload = arguments.Trim();

        if (payload.StartsWith("{", StringComparison.Ordinal))
        {
            var parsed = JsonSerializer.Deserialize<MutePayload>(payload);
            if (parsed is not null)
                return parsed.IsMuted;
        }

        if (bool.TryParse(payload, out var asBool))
            return asBool;

        return payload.ToLowerInvariant() switch
        {
            "mute" => true,
            "unmute" => false,
            "on" => true,
            "off" => false,
            "1" => true,
            "0" => false,
            _ => throw new ArgumentException(
                    "Mute argument must be one of: true, false, mute, unmute, 1, 0.",
                    nameof(arguments))
        };
    }

    private static int ValidateVolumeLevel(int level)
    {
        if (level is < 0 or > 100)
            throw new ArgumentOutOfRangeException(nameof(level), "Volume level must be between 0 and 100.");

        return level;
    }

    // ── Private payload records ─────────────────────────────────────────────

    private sealed record VolumePayload(int Level);
    private sealed record MutePayload(bool IsMuted);

    // ═══════════════════════════════════════════════════════════════════════
    //  Windows Audio COM Infrastructure
    //  All COM declarations and low-level API calls are encapsulated here so
    //  that ExecuteAsync remains clean and readable.
    // ═══════════════════════════════════════════════════════════════════════
    [SupportedOSPlatform("windows")]
    private static class WindowsAudioApi
    {
        internal static void SetMasterVolume(int level)
        {
            using var endpoint = AcquireEndpointVolume();
            endpoint.SetMasterVolumeLevelScalar(level / 100f, Guid.Empty);
        }

        internal static void SetMute(bool isMuted)
        {
            using var endpoint = AcquireEndpointVolume();
            endpoint.SetMute(isMuted, Guid.Empty);
        }

        private static AudioEndpointVolumeHandle AcquireEndpointVolume()
        {
            var enumerator = (IMMDeviceEnumerator)new MMDeviceEnumeratorComObject();
            enumerator.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia, out var device);

            var endpointVolumeGuid = typeof(IAudioEndpointVolume).GUID;
            device.Activate(ref endpointVolumeGuid, CLSCTX.CLSCTX_ALL, IntPtr.Zero, out var endpointObj);

            Marshal.ReleaseComObject(device);
            Marshal.ReleaseComObject(enumerator);

            return new AudioEndpointVolumeHandle((IAudioEndpointVolume)endpointObj);
        }

        // ── Safe handle wrapper ────────────────────────────────────────────

        private sealed class AudioEndpointVolumeHandle : IDisposable
        {
            private readonly IAudioEndpointVolume _inner;

            internal AudioEndpointVolumeHandle(IAudioEndpointVolume inner) => _inner = inner;

            internal void SetMasterVolumeLevelScalar(float level, Guid eventContext)
                => _inner.SetMasterVolumeLevelScalar(level, eventContext);

            internal void SetMute(bool isMuted, Guid eventContext)
                => _inner.SetMute(isMuted, eventContext);

            public void Dispose() => Marshal.ReleaseComObject(_inner);
        }

        // ── COM enums ──────────────────────────────────────────────────────

        private enum EDataFlow { eRender, eCapture, eAll }
        private enum ERole { eConsole, eMultimedia, eCommunications }

        [Flags]
        private enum CLSCTX : uint { CLSCTX_ALL = 23 }

        // ── COM co-class & interfaces ──────────────────────────────────────

        [ComImport, Guid("BCDE0395-E52F-467C-8E3D-C4579291692E")]
        private class MMDeviceEnumeratorComObject { }

        [ComImport]
        [Guid("A95664D2-9614-4F35-A746-DE8DB63617E6")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IMMDeviceEnumerator
        {
            int NotImpl1();

            [PreserveSig]
            int GetDefaultAudioEndpoint(EDataFlow dataFlow, ERole role, out IMMDevice endpoint);
        }

        [ComImport]
        [Guid("D666063F-1587-4E43-81F1-B948E807363F")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IMMDevice
        {
            [PreserveSig]
            int Activate(ref Guid iid, CLSCTX clsCtx, IntPtr activationParams,
                [MarshalAs(UnmanagedType.IUnknown)] out object interfacePointer);
        }

        [ComImport]
        [Guid("5CDF2C82-841E-4546-9722-0CF74078229A")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IAudioEndpointVolume
        {
            int RegisterControlChangeNotify(IntPtr notify);
            int UnregisterControlChangeNotify(IntPtr notify);
            int GetChannelCount(out uint channelCount);
            int SetMasterVolumeLevel(float levelDb, Guid eventContext);
            int SetMasterVolumeLevelScalar(float level, Guid eventContext);
            int GetMasterVolumeLevel(out float levelDb);
            int GetMasterVolumeLevelScalar(out float level);
            int SetChannelVolumeLevel(uint channelNumber, float levelDb, Guid eventContext);
            int SetChannelVolumeLevelScalar(uint channelNumber, float level, Guid eventContext);
            int GetChannelVolumeLevel(uint channelNumber, out float levelDb);
            int GetChannelVolumeLevelScalar(uint channelNumber, out float level);
            int SetMute([MarshalAs(UnmanagedType.Bool)] bool isMuted, Guid eventContext);
            int GetMute(out bool isMuted);
            int GetVolumeStepInfo(out uint step, out uint stepCount);
            int VolumeStepUp(Guid eventContext);
            int VolumeStepDown(Guid eventContext);
            int QueryHardwareSupport(out uint hardwareSupportMask);
            int GetVolumeRange(out float volumeMinDb, out float volumeMaxDb, out float volumeIncrementDb);
        }
    }
}

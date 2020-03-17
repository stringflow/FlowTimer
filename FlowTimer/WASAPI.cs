using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Collections;
using static FlowTimer.WASAPI;

namespace FlowTimer {

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct WAVEFORMATEX {

        public ushort wFormatTag;
        public short nChannels;
        public int nSamplesPerSec;
        public int nAvgBytesPerSec;
        public short nBlockAlign;
        public short wBitsPerSample;
        public short cbSize;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct WAVEFORMATEXTENSIBLE {

        [FieldOffset(0x00)] public WAVEFORMATEX Format;
        [FieldOffset(0x12)] public short wValidBitsPerSample;
        [FieldOffset(0x12)] public short wSamplesPerBlock;
        [FieldOffset(0x12)] public short wValidBitsPewReservedrSample;
        [FieldOffset(0x14)] public uint dwChannelMask;
        [FieldOffset(0x18)] public Guid SubFormat;
    }

    [Guid("D666063F-1587-4E43-81F1-B948E807363F"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMMDevice {

        int Activate(ref Guid iid, uint dwClsCtx, IntPtr pActivationParams, [MarshalAs(UnmanagedType.IUnknown)] out object ppInterface);
    }

    [Guid("0BD7A1BE-7A1A-44DB-8397-CC5392387B5E"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMMDeviceCollection {

        int GetCount(out int pcDevices);
        int Item(int nDevice, out IMMDevice ppDevice);
    }

    [Guid("A95664D2-9614-4F35-A746-DE8DB63617E6"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMMDeviceEnumerator {

        int EnumAudioEndpoints(DataFlow dataFlow, uint dwStateMask, out IMMDeviceCollection ppDevices);
        int GetDefaultAudioEndpoint(DataFlow dataFlow, Role role, out IMMDevice ppEndPoint);
    }

    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("7ED4EE07-8E67-4CD4-8C1A-2B7A5987AD42")]
    public interface IAudioClient3 {

        int Initialize(uint shareMode, uint streamFlags, long hnsBufferDuration, long hnsPeriodicity, WAVEFORMATEX pFormat, IntPtr audioSessionGuid);
        int GetBufferSize(out uint bufferSize);
        [return: MarshalAs(UnmanagedType.I8)] long GetStreamLatency();
        int GetCurrentPadding(out int currentPadding);
        int IsFormatSupported(uint shareMode, WAVEFORMATEX pFormat, out IntPtr closestMatchFormat);
        int GetMixFormat(out IntPtr deviceFormatPointer);
        int GetDevicePeriod(out long defaultDevicePeriod, out long minimumDevicePeriod);
        int Start();
        int Stop();
        int Reset();
        int SetEventHandle(IntPtr eventHandle);
        int GetService([MarshalAs(UnmanagedType.LPStruct)] Guid interfaceId, [MarshalAs(UnmanagedType.IUnknown)] out object interfacePointer);
        void IsOffloadCapable(int category, out bool pbOffloadCapable);
        void SetClientProperties(IntPtr pProperties);
        void GetBufferSizeLimits(IntPtr pFormat, bool bEventDriven, out long phnsMinBufferDuration, out long phnsMaxBufferDuration);
        int GetSharedModeEnginePeriod(IntPtr pFormat, out int pDefaultPeriodInFrames, out int pFundementalPeriodInFrames, out int pMinPeriodInFrames, out int pMaxPeriodInFrames);
        int GetCurrentSharedModeEnginePeriod(IntPtr ppFormat, out uint pCurrentPeriodInFrames);
    }

    public class MMDevice {

        private IMMDevice Interface;

        public MMDevice(IMMDevice interfaceIn) => Interface = interfaceIn;

        public AudioClient CreateAudioCilent() {
            Interface.Activate(ref IAudioClient3ID, CLSCTX_ALL, IntPtr.Zero, out object audioClientInterface);
            return new AudioClient(audioClientInterface as IAudioClient3);
        }
    }

    public class MMDeviceCollection : IEnumerable<MMDevice> {

        private IMMDeviceCollection Interface;

        public MMDeviceCollection(IMMDeviceCollection interfaceIn) => Interface = interfaceIn;

        public int Count {
            get {
                Interface.GetCount(out int result);
                return result;
            }
        }

        public MMDevice this[int index] {
            get {
                Interface.Item(index, out IMMDevice result);
                return new MMDevice(result);
            }
        }

        public IEnumerator<MMDevice> GetEnumerator() {
            for(int i = 0; i < Count; i++) {
                yield return this[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }

    public class MMDeviceEnumerator {

        private IMMDeviceEnumerator Interface;

        public MMDeviceEnumerator(IMMDeviceEnumerator interfaceIn) => Interface = interfaceIn;

        public MMDeviceCollection EnumerateAudioEndPoints(DataFlow dataFlow, uint dwStateMask) {
            Interface.EnumAudioEndpoints(dataFlow, dwStateMask, out IMMDeviceCollection result);
            return new MMDeviceCollection(result);
        }

        public MMDevice GetDefaultAudioEndpoint(DataFlow dataFlow, Role role) {
            Interface.GetDefaultAudioEndpoint(dataFlow, role, out IMMDevice result);
            return new MMDevice(result);
        }
    }

    public class AudioClient {

        private IAudioClient3 Interface;

        public AudioClient(IAudioClient3 interfaceIn) => Interface = interfaceIn;

        public IntPtr MixFormat {
            get {
                Interface.GetMixFormat(out IntPtr result);
                return result;
            }
        }

        public void GetSharedModeEnginePeriod(out int defaultPeriod, out int fundementalPeriod, out int minPeriod, out int maxPeriod) {
            Interface.GetSharedModeEnginePeriod(MixFormat, out defaultPeriod, out fundementalPeriod, out minPeriod, out maxPeriod);
        }
    }

    public enum DataFlow : uint {

        Render,
        Capture,
        All,
    }

    public enum Role : uint {

        Console,
        Multimedia,
        Communications,
    }

    public static class WASAPI {

        public static Guid MMDeviceEnumeratorID = new Guid("BCDE0395-E52F-467C-8E3D-C4579291692E");
        public static Guid IAudioClient3ID = new Guid("7ED4EE07-8E67-4CD4-8C1A-2B7A5987AD42");

        public const uint DEVICE_STATE_ACTIVATE   = 0x1;
        public const uint DEVICE_STATE_DISABLED   = 0x2;
        public const uint DEVICE_STATE_NOTPRESENT = 0x4;
        public const uint DEVICE_STATE_UNPLUGGED  = 0x8;

        public const uint CLSCTX_INPROC_SERVER          = 0x1;
        public const uint CLSCTX_INPROC_HANDLER         = 0x2;
        public const uint CLSCTX_LOCAL_SERVER           = 0x4;
        public const uint CLSCTX_INPROC_SERVER16        = 0x8;
        public const uint CLSCTX_REMOTE_SERVER          = 0x10;
        public const uint CLSCTX_INPROC_HANDLER16       = 0x20;
        public const uint CLSCTX_RESERVED1              = 0x40;
        public const uint CLSCTX_RESERVED2              = 0x80;
        public const uint CLSCTX_RESERVED3              = 0x100;
        public const uint CLSCTX_RESERVED4              = 0x200;
        public const uint CLSCTX_NO_CODE_DOWNLOAD       = 0x400;
        public const uint CLSCTX_RESERVED5              = 0x800;
        public const uint CLSCTX_NO_CUSTOM_MARSHAL      = 0x1000;
        public const uint CLSCTX_ENABLE_CODE_DOWNLOAD   = 0x2000;
        public const uint CLSCTX_NO_FAILURE_LOG         = 0x4000;
        public const uint CLSCTX_DISABLE_AAA            = 0x8000;
        public const uint CLSCTX_ENABLE_AAA             = 0x10000;
        public const uint CLSCTX_FROM_DEFAULT_CONTEXT   = 0x20000;
        public const uint CLSCTX_ACTIVATE_32_BIT_SERVER = 0x40000;
        public const uint CLSCTX_ACTIVATE_64_BIT_SERVER = 0x80000;
        public const uint CLSCTX_ENABLE_CLOAKING        = 0x100000;
        public const uint CLSCTX_PS_DLL                 = 0x80000000;
        public const uint CLSCTX_INPROC                 = CLSCTX_INPROC_SERVER | CLSCTX_INPROC_HANDLER;
        public const uint CLSCTX_SERVER                 = CLSCTX_INPROC_SERVER | CLSCTX_LOCAL_SERVER | CLSCTX_REMOTE_SERVER;
        public const uint CLSCTX_ALL                    = CLSCTX_SERVER | CLSCTX_INPROC_HANDLER;

        public static MMDeviceEnumerator CreateMMDeviceEnumerator() {
            return new MMDeviceEnumerator(CreateInternal<IMMDeviceEnumerator>(MMDeviceEnumeratorID));
        }

        private static T CreateInternal<T>(Guid guid) {
            Type type = Type.GetTypeFromCLSID(guid);
            return (T) Activator.CreateInstance(type);
        }
    }
}

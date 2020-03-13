using System;
using System.Text;
using System.Runtime.InteropServices;

namespace FlowTimer {

    public static unsafe class SDL {

        public const string DLL = "SDL2.dll";

        public const uint SDL_INIT_TIMER          = 0x00000001;
        public const uint SDL_INIT_AUDIO          = 0x00000010;
        public const uint SDL_INIT_VIDEO          = 0x00000020;
        public const uint SDL_INIT_JOYSTICK       = 0x00000200;
        public const uint SDL_INIT_HAPTIC         = 0x00001000;
        public const uint SDL_INIT_GAMECONTROLLER = 0x00002000;
        public const uint SDL_INIT_EVENTS         = 0x00004000;
        public const uint SDL_INIT_NOPARACHUTE    = 0x00100000;
        public const uint SDL_INIT_EVERYTHING     = SDL_INIT_TIMER | SDL_INIT_AUDIO | SDL_INIT_VIDEO | SDL_INIT_EVENTS | SDL_INIT_JOYSTICK | SDL_INIT_HAPTIC | SDL_INIT_GAMECONTROLLER;

        public const ushort AUDIO_U8     = 0x0008;
        public const ushort AUDIO_S8     = 0x8008;
        public const ushort AUDIO_U16LSB = 0x0010;
        public const ushort AUDIO_S16LSB = 0x8010;
        public const ushort AUDIO_U16MSB = 0x1010;
        public const ushort AUDIO_S16MSB = 0x9010;
        public const ushort AUDIO_U16    = AUDIO_U16LSB;
        public const ushort AUDIO_S16    = AUDIO_S16LSB;
        public const ushort AUDIO_S32LSB = 0x8020;
        public const ushort AUDIO_S32MSB = 0x9020;
        public const ushort AUDIO_S32    = AUDIO_S32LSB;
        public const ushort AUDIO_F32LSB = 0x8120;
        public const ushort AUDIO_F32MSB = 0x9120;
        public const ushort AUDIO_F32    = AUDIO_F32LSB;

        public static readonly ushort AUDIO_U16SYS = BitConverter.IsLittleEndian ? AUDIO_U16LSB : AUDIO_U16MSB;
        public static readonly ushort AUDIO_S16SYS = BitConverter.IsLittleEndian ? AUDIO_S16LSB : AUDIO_S16MSB;
        public static readonly ushort AUDIO_S32SYS = BitConverter.IsLittleEndian ? AUDIO_S32LSB : AUDIO_S32MSB;
        public static readonly ushort AUDIO_F32SYS = BitConverter.IsLittleEndian ? AUDIO_F32LSB : AUDIO_F32MSB;

        public const int SDL_AUDIO_ALLOW_FREQUENCY_CHANGE = 0x00000001;
        public const int SDL_AUDIO_ALLOW_FORMAT_CHANGE    = 0x00000002;
        public const int SDL_AUDIO_ALLOW_CHANNELS_CHANGE  = 0x00000004;
        public const int SDL_AUDIO_ALLOW_SAMPLES_CHANGE   = 0x00000008;
        public const int SDL_AUDIO_ALLOW_ANY_CHANGE       = SDL_AUDIO_ALLOW_FREQUENCY_CHANGE | SDL_AUDIO_ALLOW_FORMAT_CHANGE | SDL_AUDIO_ALLOW_CHANNELS_CHANGE | SDL_AUDIO_ALLOW_SAMPLES_CHANGE;

        public delegate void SDL_AudioCallback(IntPtr userdata, IntPtr stream, int len);

        public struct SDL_AudioSpec {

            public int freq;
            public ushort format;
            public byte channels;
            public byte silence;
            public ushort samples;
            public uint size;
            public SDL_AudioCallback callback;
            public IntPtr userdata;
        }

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SDL_Init(uint flags);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SDL_Quit();

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SDL_Delay(uint ms);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr SDL_malloc(IntPtr size);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SDL_free(IntPtr ptr);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SDL_memset(IntPtr dst, int c, uint len);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(SDLMarshaler), MarshalCookie = SDLMarshaler.LeaveAllocated)]
        public static extern string SDL_GetError();

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint SDL_OpenAudioDevice([In()] [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(SDLMarshaler))] string device, int iscapture, ref SDL_AudioSpec desired, out SDL_AudioSpec obtained, int allowed_change);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SDL_PauseAudioDevice(uint dev, int pause_on);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SDL_QueueAudio(uint dev, byte* data, int len);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SDL_ClearQueuedAudio(uint dev);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SDL_CloseAudioDevice(uint dev);
    }

    internal unsafe class SDLMarshaler : ICustomMarshaler {

        public const string LeaveAllocated = "LeaveAllocated";

        private bool ShouldLeaveAllocated;

        private static ICustomMarshaler LeaveAllocatedInstance = new SDLMarshaler(true);
        private static ICustomMarshaler DefaultInstance = new SDLMarshaler(false);

        public static ICustomMarshaler GetInstance(string cookie) {
            switch(cookie) {
                case "LeaveAllocated": return LeaveAllocatedInstance;
                default: return DefaultInstance;
            }
        }

        public SDLMarshaler(bool leaveAllocated) {
            ShouldLeaveAllocated = leaveAllocated;
        }

        public object MarshalNativeToManaged(IntPtr pNativeData) {
            if(pNativeData == IntPtr.Zero) {
                return null;
            }

            var ptr = (byte*) pNativeData;
            while(*ptr != 0) {
                ptr++;
            }

            var bytes = new byte[ptr - (byte*) pNativeData];
            Marshal.Copy(pNativeData, bytes, 0, bytes.Length);
            return Encoding.UTF8.GetString(bytes);
        }

        public IntPtr MarshalManagedToNative(object ManagedObj) {
            if(ManagedObj == null) {
                return IntPtr.Zero;
            }

            var str = ManagedObj as string;
            if(str == null) {
                throw new ArgumentException("ManagedObj must be a string.", "ManagedObj");
            }

            var bytes = Encoding.UTF8.GetBytes(str);
            var mem = SDL.SDL_malloc((IntPtr) (bytes.Length + 1));
            Marshal.Copy(bytes, 0, mem, bytes.Length);
            ((byte*) mem)[bytes.Length] = 0;
            return mem;
        }

        public void CleanUpManagedData(object ManagedObj) {
        }

        public void CleanUpNativeData(IntPtr pNativeData) {
            if(!ShouldLeaveAllocated) {
                SDL.SDL_free(pNativeData);
            }
        }

        public int GetNativeDataSize() {
            return -1;
        }
    }
}
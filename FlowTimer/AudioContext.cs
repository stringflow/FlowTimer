using System;
using static FlowTimer.SDL;

namespace FlowTimer {

    public unsafe class AudioContext {

        public SDL_AudioSpec AudioSpec;
        public uint DeviceId;

        public int SampleRate {
            get { return AudioSpec.freq; }
        }

        public int Format {
            get { return AudioSpec.format; }
        }

        public int NumChannels {
            get { return AudioSpec.channels; }
        }

        public int Samples {
            get { return AudioSpec.samples; }
        }

        public static void GlobalInit() {
            if(SDL_Init(SDL_INIT_AUDIO) < 0) {
                throw new Exception("Unable to load SDL!");
            }
        }

        public static void GlobalDestroy() {
            SDL_Quit();
        }

        public static void LoadWAV(string filePath, out byte[] pcm, out SDL_AudioSpec spec) {
            spec = new SDL_AudioSpec();
            IntPtr dataStart;
            uint dataLength;
            SDL_LoadWAV(filePath, ref spec, out dataStart, out dataLength);

            pcm = new byte[dataLength];
            fixed(byte* ptr = pcm) {
                SDL_memcpy((IntPtr) ptr, dataStart, dataLength);
            }

            SDL_FreeWAV(dataStart);
        }

        public AudioContext(int freq, ushort format, byte channels, ushort samples) {
            AudioSpec = new SDL_AudioSpec() {
                freq = freq,
                format = format,
                channels = channels,
                samples = samples,
            };

            DeviceId = SDL_OpenAudioDevice(null, 0, ref AudioSpec, out AudioSpec, 0);
            if(DeviceId == 0) {
                throw new Exception(SDL_GetError());
            }

            Console.WriteLine("Opened Audio Device with the following parameters: freq={0}, format={1}, channels={2}, samples={3}", AudioSpec.freq, AudioSpec.format, AudioSpec.channels, AudioSpec.samples);

            SDL_PauseAudioDevice(DeviceId, 0);
        }

        public void Destroy() {
            SDL_CloseAudioDevice(DeviceId);
        }

        public void QueueAudio(byte[] pcm) {
            fixed(byte* ptr = pcm) SDL_QueueAudio(DeviceId, ptr, (uint) pcm.Length);
        }

        public void ClearQueuedAudio() {
            SDL_ClearQueuedAudio(DeviceId);
        }
    }
}
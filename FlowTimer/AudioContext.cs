using System;
using System.IO;
using System.Runtime.InteropServices;
using static FlowTimer.SDL;

namespace FlowTimer {

    public unsafe class AudioContext {

        public static int MinBufferSize;

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

        public int BytesPerSample;

        public static void GlobalInit() {
            if(SDL_Init(SDL_INIT_AUDIO) < 0) {
                throw new Exception("Unable to load SDL!");
            }

            MMDevice audioDevice = MMDeviceAPI.CreateMMDeviceEnumerator().GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
            AudioClient audioClient = audioDevice.CreateAudioCilent();
            audioClient.GetSharedModeEnginePeriod(out _, out _, out MinBufferSize, out _);
        }

        public static void GlobalDestroy() {
            SDL_Quit();
        }

        public AudioContext(int freq, ushort format, byte channels) {
            AudioSpec = new SDL_AudioSpec() {
                freq = freq,
                format = format,
                channels = channels,
                samples = (ushort) MinBufferSize,
            };

            switch(format) {
                case AUDIO_U8: {
                    BytesPerSample = 1;
                } break;

                case AUDIO_S16LSB: {
                    BytesPerSample = 2;
                } break;
            };

            DeviceId = SDL_OpenAudioDevice(null, 0, ref AudioSpec, out AudioSpec, 0);
            if(DeviceId == 0) {
                throw new Exception(SDL_GetError());
            }

            SDL_PauseAudioDevice(DeviceId, 0);
        }

        public void Destroy() {
            SDL_CloseAudioDevice(DeviceId);
        }

        public void QueueAudio(byte[] pcm) {
            fixed(byte* ptr = pcm) SDL_QueueAudio(DeviceId, ptr, pcm.Length);
        }

        public void ClearQueuedAudio() {
            SDL_ClearQueuedAudio(DeviceId);
        }
    }

    public static class Wave {

        private static readonly uint WaveId_RIFF = MakeRiff("RIFF");
        private static readonly uint WaveId_WAVE = MakeRiff("WAVE");
        private static readonly uint WaveId_data = MakeRiff("data");
        private static readonly uint WaveId_fmt  = MakeRiff("fmt ");

        private static uint MakeRiff(string str) {
            return (uint) ((str[3] << 24) | (str[2] << 16) | (str[1] << 8) | str[0]);
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct WaveHeader {

            public uint RiffId;
            public uint Size;
            public uint WaveId;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct WaveChunkHeader {

            public uint Id;
            public uint Size;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct WaveFmt {

            public ushort FormatTag;
            public ushort Channels;
            public uint SampleRate;
            public uint AvgBytesPerSec;
            public ushort BlockAlign;
            public ushort BitsPerSample;
        }

        public static bool LoadWAV(string fileName, out byte[] pcm, out SDL_AudioSpec spec) {
            pcm = new byte[0];
            spec = new SDL_AudioSpec();

            byte[] bytes = File.ReadAllBytes(fileName);
            int pointer = 0;

            WaveHeader header = bytes.Consume<WaveHeader>(ref pointer);
            if(header.RiffId != WaveId_RIFF) return false;
            if(header.WaveId != WaveId_WAVE) return false;

            while(pointer < bytes.Length) {
                WaveChunkHeader chunkHeader = bytes.Consume<WaveChunkHeader>(ref pointer);

                if(chunkHeader.Id == WaveId_fmt) {
                    WaveFmt fmt = bytes.ReadStruct<WaveFmt>(pointer);
                    spec = new SDL_AudioSpec() {
                        freq = (int) fmt.SampleRate,
                        channels = (byte) fmt.Channels,
                    };

                    if(fmt.FormatTag != 1) return false;

                    switch(fmt.BitsPerSample) {
                        case 8: {
                            spec.format = AUDIO_U8;
                        } break;

                        case 16: {
                            spec.format = AUDIO_S16LSB;
                        } break;
                    };
                } else if(chunkHeader.Id == WaveId_data) {
                    pcm = bytes.Subarray(pointer, (int) chunkHeader.Size);
                }

                pointer += (int) chunkHeader.Size;
            }

            if(spec.format == AUDIO_U8) {
                byte[] newPCM = new byte[pcm.Length * 2];
                for(int i = 0; i < pcm.Length; i++) {
                    short sample16 = (short) ((pcm[i] - 0x80) << 8);
                    newPCM[i * 2 + 0] = (byte) (sample16 & 0xFF);
                    newPCM[i * 2 + 1] = (byte) (sample16 >> 8);
                }
                pcm = newPCM;
                spec.format = AUDIO_S16LSB;
            }

            return true;
        }
    }
}
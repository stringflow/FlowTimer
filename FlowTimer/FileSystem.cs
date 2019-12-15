using System;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;

namespace FlowTimer {

    public static class FileSystem {

        private static Assembly Asm;
        private static MD5 MD5;

        public static void Init() {
            Asm = Assembly.GetExecutingAssembly();
            MD5 = MD5.Create();

            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) => Assembly.Load(ReadPackedResource("FlowTimer.Dependencies.Newtonsoft.Json.dll"));
        }

        public static void UnpackAllFileExtensions(string extension, string destFolder) {
            foreach(string file in Asm.GetManifestResourceNames()) {
                if(file.StartsWith("FlowTimer.Resources")) {
                    string[] splitarray = file.Split('.');
                    string filename = splitarray[2];
                    string fileextension = splitarray[3];

                    if(fileextension.ToLower() == extension.ToLower()) {
                        Unpack(filename + "." + fileextension, destFolder + filename + "." + fileextension);
                    }
                }
            }
        }

        public static void Unpack(string src, string dest) {
            string directory = Path.GetDirectoryName(dest);
            if(!Directory.Exists(directory)) {
                Directory.CreateDirectory(directory);
            }

            byte[] data = ReadPackedResource("FlowTimer.Resources." + src);
            if(ShouldUnpack(data, dest)) {
                Console.WriteLine("Unpacking " + src + " to " + dest);
                File.WriteAllBytes(dest, data);
            }
        }

        private static bool ShouldUnpack(byte[] data, string dest) {
            if(!File.Exists(dest)) {
                return true;
            } else {
                return !Enumerable.SequenceEqual(MD5.ComputeHash(data), MD5.ComputeHash(File.ReadAllBytes(dest)));
            }
        }

        public static Stream ReadPackedResourceStream(string src) {
            return Asm.GetManifestResourceStream(src);
        }

        public static byte[] ReadPackedResource(string src) {
            using(Stream stream = ReadPackedResourceStream(src)) {
                byte[] binary = new byte[stream.Length];
                stream.Read(binary, 0, binary.Length);
                return binary;
            }
        }
    }
}

using System;
using System.Windows.Forms;
using System.Threading;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;

namespace FlowTimer {

    public static class Extensions {

        public static void AbortIfAlive(this Thread thread) {
            if(thread.IsAlive) {
                thread.Abort();
                thread.Join();
            }
        }

        public static double ToDouble(this string val) {
            return double.Parse(val, CultureInfo.InvariantCulture);
        }

        public static string ToFormattedString(this double val) {
            return val.ToString("F3", CultureInfo.InvariantCulture);
        }

        public static string ToFormattedString(this Keys val) {
            return val == Keys.None ? "Unset" : val.ToString();
        }

        public static string ToFormattedString(this KeyMethod val) {
            switch(val) {
                case KeyMethod.OnPress:   return "On Press";
                case KeyMethod.OnRelease: return "On Release";
            }

            throw new Exception("Unknown KeyMethod: " + Enum.GetName(typeof(KeyMethod), val));
        }

        public static bool IsActivatedByEvent(this KeyMethod val, int wParam) {
            switch(val) {
                case KeyMethod.OnPress:   return wParam == Win32.WM_KEYDOWN || wParam == Win32.WM_SYSKEYDOWN;
                case KeyMethod.OnRelease: return wParam == Win32.WM_KEYUP   || wParam == Win32.WM_SYSKEYUP;
            }

            throw new Exception("Unknown KeyMethod: " + Enum.GetName(typeof(KeyMethod), val));
        }

        public static void SetDrawing(this Control control, bool enabled) {
            Win32.SendMessage(control.Handle, Win32.WM_SETREDRAW, enabled, 0);
        }

        // credit to https://stackoverflow.com/a/32859334/7281499
        private static readonly Action<Control, ControlStyles, bool> SetStyle = (Action<Control, ControlStyles, bool>) Delegate.CreateDelegate(typeof(Action<Control, ControlStyles, bool>), typeof(Control).GetMethod("SetStyle", BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { typeof(ControlStyles), typeof(bool) }, null));
        public static void DisableSelect(this Control control) {
            SetStyle(control, ControlStyles.Selectable, false);
        }

        public static T[] Subarray<T>(this T[] source, int index, int length) {
            T[] subarray = new T[length];
            Array.Copy(source, index, subarray, 0, length);
            return subarray;
        }

        public static T Consume<T>(this byte[] array, ref int pointer) where T : unmanaged {
            T ret = ReadStruct<T>(array, pointer);
            pointer += Marshal.SizeOf<T>();
            return ret;
        }

        public static T ReadStruct<T>(this byte[] array, int pointer) where T : unmanaged {
            int structSize = Marshal.SizeOf<T>();
            IntPtr ptr = Marshal.AllocHGlobal(structSize);
            Marshal.Copy(array, pointer, ptr, structSize);
            T str = Marshal.PtrToStructure<T>(ptr);
            Marshal.FreeHGlobal(ptr);
            return str;
        }
    }
}

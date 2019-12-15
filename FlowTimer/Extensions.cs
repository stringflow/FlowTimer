using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Globalization;

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
    }
}

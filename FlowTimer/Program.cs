using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FlowTimer {
    static class Program {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {
            FileSystem.Init();
            FileSystem.UnpackAllFileExtensions("wav", FlowTimer.Beeps);
            FileSystem.Unpack("SDL2.dll", FlowTimer.Folder + "SDL2.dll");
            Win32.SetDllDirectory(FlowTimer.Folder);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}

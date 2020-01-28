using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Newtonsoft.Json;
using System.Media;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Reflection;
using static FlowTimer.Win32;
using static FlowTimer.SDL;

namespace FlowTimer {

    public static class FlowTimer {

        public const string TimerFileFilter = "Json files (*.json)|*.json";
        public const string BeepFileFilter = "WAV files (*.wav)|*.wav";

        public static readonly string Folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/flowtimer/";
        public static readonly string Beeps = Folder + "beeps/";
        public static readonly string SettingsFile = Folder + "settings.json";

        public static MainForm MainForm;
        public static int MainFormBaseHeight;

        public static List<BaseTimer> TimerTabs;
        public static FixedOffsetTimer FixedOffset;
        public static VariableOffsetTimer VariableOffset;
        public static BaseTimer CurrentTab {
            get { return TimerTabs[MainForm.TabControl.SelectedIndex]; }
        }

        public static TabPage LockedTab;

        public static SettingsForm SettingsForm;
        public static Settings Settings;

        public static SpriteSheet PinSheet;

        public static AudioContext AudioContext;
        public static byte[] BeepSound;
        public static byte[] PCM;
        public static double MaxOffset;

        public static bool IsTimerRunning;
        public static long TimerStart;
        public static Thread TimerUpdateThread;

        public static Proc KeyboardCallback;
        public static IntPtr KeyboardHook;

        public static void Init() {
            Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.InvariantCulture;

            Settings = File.Exists(SettingsFile) ? JsonConvert.DeserializeObject<Settings>(File.ReadAllText(SettingsFile)) : new Settings();

            if(Settings.AutoUpdate) {
                CheckForUpdates(false);
            }

            PinSheet = new SpriteSheet(new Bitmap(FileSystem.ReadPackedResourceStream("FlowTimer.Resources.pin.png")), 16, 16);
            Pin(Settings.Pinned);

            AudioContext.GlobalInit();
            ChangeBeepSound(Settings.Beep, false);

            TimerUpdateThread = new Thread(TimerUpdateCallback);

            MainForm.TabControl.Selected += TabControl_Selected;
            MainForm.TabControl.Deselecting += TabControl_Deselecting;

            KeyboardCallback = Keycallback;
            KeyboardHook = SetHook(WH_KEYBOARD_LL, KeyboardCallback);

            foreach(BaseTimer timer in TimerTabs) {
                timer.OnInit();
            }
        }

        public static void Destroy() {
            if(FixedOffset.HaveTimersChanged()) {
                if(MessageBox.Show("You've changed your timers without saving. Would you like to save your timers?", "Save timers?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes) {
                    FixedOffset.SaveTimers(Settings.LastLoadedTimers, true);
                }
            }

            TimerUpdateThread.AbortIfAlive();
            AudioContext.ClearQueuedAudio();
            AudioContext.Destroy();
            AudioContext.GlobalDestroy();
            File.WriteAllText(SettingsFile, JsonConvert.SerializeObject(Settings));
        }

        public static int GetCurrentBuild() {
            return Assembly.GetExecutingAssembly().GetName().Version.Major;
        }

        public static void SetMainForm(MainForm mainForm) {
            MainForm = mainForm;
            MainFormBaseHeight = mainForm.Height;
            mainForm.RemoveKeyControls();
        }

        public static void RemoveKeyControls(this Control control) {
            foreach(Control ctrl in control.Controls) {
                ctrl.RemoveKeyControls();
                ctrl.PreviewKeyDown += MainForm_PreviewKeyDown;
                ctrl.KeyDown += MainForm_KeyDownEvent;
            }
        }

        private static void MainForm_PreviewKeyDown(object sender, PreviewKeyDownEventArgs args) {
            switch(args.KeyCode) {
                case Keys.Up:
                case Keys.Down:
                case Keys.Left:
                case Keys.Right:
                case Keys.Tab:
                    args.IsInputKey = true;
                    break;
            }
        }

        private static void MainForm_KeyDownEvent(object sender, KeyEventArgs args) {
            switch(args.KeyCode) {
                case Keys.Escape:
                    MainForm.ActiveControl = null;
                    args.Handled = true;
                    args.SuppressKeyPress = true;
                    break;
            }
        }

        private static IntPtr Keycallback(int nCode, int wParam, IntPtr lParam) {
            if(SettingsForm == null || !SettingsForm.Visible) {
                if(nCode >= 0 && Settings.KeyMethod.IsActivatedByEvent(wParam)) {
                    Keys key = (Keys) Marshal.ReadInt32(lParam);

                    if(Settings.Start.IsPressed(key)) {
                        StartTimer();
                    } else if(Settings.Stop.IsPressed(key)) {
                        StopTimer(false);
                    }

                    CurrentTab.OnKeyEvent(key);
                }
            }

            return CallNextHookEx(KeyboardHook, nCode, wParam, lParam);
        }

        public static void RegisterTabs(TabPage fixedOffset, TabPage variableOffset) {
            Control[] copyControls = { MainForm.ButtonStart, MainForm.ButtonStop, MainForm.ButtonSettings, MainForm.LabelTimer, MainForm.PictureBoxPin };
            FixedOffset = new FixedOffsetTimer(fixedOffset, copyControls);
            VariableOffset = new VariableOffsetTimer(variableOffset, copyControls);
            TimerTabs = new List<BaseTimer>() { FixedOffset, VariableOffset };
        }

        public static void TabControl_Selected(object sender, TabControlEventArgs e) {
            foreach(TabPage tab in MainForm.TabControl.TabPages) {
                tab.SetDrawing(false);
            }

            TimerTabs[e.TabPageIndex].OnLoad();

            e.TabPage.SetDrawing(true);
            e.TabPage.Refresh();
        }

        public static void TabControl_Deselecting(object sender, TabControlCancelEventArgs e) {
            if(e.TabPage == LockedTab) {
                SystemSounds.Beep.Play();
                e.Cancel = true;
                return;
            }
        }

        public static void ResizeForm(int width, int height) {
            Size size = new Size() {
                Width = width,
                Height = height,
            };

            MainForm.MinimumSize = size;
            MainForm.MaximumSize = size;
            MainForm.Size = size;
            MainForm.TabControl.Height = size.Height;
        }

        public static void EnableControls(bool enabled) {
            MainForm.ButtonSettings.Enabled = enabled;
        }

        public static void TogglePin() {
            Pin(!Settings.Pinned);
        }

        public static void Pin(bool pin) {
            MainForm.TopMost = pin;
            if(SettingsForm != null) SettingsForm.TopMost = pin;
            Settings.Pinned = pin;
            MainForm.PictureBoxPin.Image = PinSheet[pin ? 1 : 0];
        }

        public static void UpdatePCM(double[] offsets, uint interval, uint numBeeps, bool garbageCollect = true) {
            // try to force garbage collection on the old pcm data
            if(garbageCollect) GC.Collect();
            MaxOffset = offsets.Max();

            PCM = new byte[((int) Math.Ceiling(MaxOffset / 1000.0 * AudioContext.SampleRate)) * AudioContext.NumChannels * 2 + BeepSound.Length * 2];

            foreach(double offset in offsets) {
                for(int i = 0; i < numBeeps; i++) {
                    int destOffset = (int) ((offset - i * interval) / 1000.0 * AudioContext.SampleRate) * AudioContext.NumChannels * 2;
                    Array.Copy(BeepSound, 0, PCM, destOffset, BeepSound.Length);
                }
            }
        }

        public static void StartTimer() {
            AudioContext.ClearQueuedAudio();

            IsTimerRunning = true;
            TimerStart = DateTime.Now.Ticks;
            CurrentTab.OnTimerStart();

            if(!MainForm.ButtonStart.Enabled) {
                AudioContext.ClearQueuedAudio();
                return;
            }

            VariableOffset.OnDataChange();

            EnableControls(false);
            LockedTab = MainForm.TabControl.SelectedTab;

            TimerUpdateThread.AbortIfAlive();
            TimerUpdateThread = new Thread(TimerUpdateCallback);
            TimerUpdateThread.Start();
        }

        public static void StopTimer(bool timerExpired) {
            if(!timerExpired) {
                AudioContext.ClearQueuedAudio();
                TimerUpdateThread.AbortIfAlive();
            }

            CurrentTab.OnTimerStop();
            EnableControls(true);
            LockedTab = null;
            IsTimerRunning = false;
            VariableOffset.OnDataChange();
        }

        private static void TimerUpdateCallback() {
            Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.InvariantCulture;
            const uint resolution = 15;

            MethodInvoker inv;
            double currentTime = 0.0f;

            do {
                inv = delegate {
                    currentTime = CurrentTab.TimerCallback(TimerStart);
                    MainForm.LabelTimer.Text = currentTime.ToFormattedString();
                };
                MainForm.Invoke(inv);

                SDL_Delay(resolution);
            } while(currentTime > 0.0);

            inv = delegate {
                StopTimer(true);
            };
            MainForm.Invoke(inv);
        }

        public static void OpenSettingsForm() {
            SettingsForm = new SettingsForm();
            SettingsForm.TopMost = Settings.Pinned;
            SettingsForm.RemoveKeyControls();
            SettingsForm.ShowDialog();
        }

        public static void OpenImportBeepSoundDialog() {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = BeepFileFilter;

            if(dialog.ShowDialog() == DialogResult.OK) {
                ImportBeepSound(dialog.FileName, true);
            }
        }

        public static bool ImportBeepSound(string filePath, bool displayMessages = true) {
            string fileName = Path.GetFileName(filePath);
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);

            if(File.Exists(Beeps + fileName)) {
                if(displayMessages) {
                    MessageBox.Show("Beep sound '" + fileNameWithoutExtension + "' already exists.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return false;
            }

            SDL_AudioSpec audioSpec;
            byte[] pcm;
            AudioContext.LoadWAV(filePath, out pcm, out audioSpec);

            if(audioSpec.format != AudioContext.Format && audioSpec.channels == AudioContext.NumChannels) {
                if(displayMessages) {
                    MessageBox.Show("FlowTimer does not support this audio file (yet).", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return false;
            }

            File.Copy(filePath, Beeps + fileName);
            if(SettingsForm != null) SettingsForm.ComboBoxBeep.Items.Add(fileNameWithoutExtension);
            MessageBox.Show("Beep sucessfully imported from '" + filePath + "'.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return true;
        }

        public static void ChangeBeepSound(string beepName, bool playSound = true) {
            if(AudioContext != null) AudioContext.Destroy();

            SDL_AudioSpec audioSpec;
            AudioContext.LoadWAV(Beeps + beepName + ".wav", out BeepSound, out audioSpec);
            AudioContext = new AudioContext(audioSpec.freq, audioSpec.format, audioSpec.channels, 256);

            CurrentTab.OnBeepSoundChange();
            Settings.Beep = beepName;

            if(playSound) {
                AudioContext.QueueAudio(BeepSound);
            }
        }

        public static void ChangeKeyMethod(KeyMethod newMethod) {
            Settings.KeyMethod = newMethod;
        }

        public static void CheckForUpdates(bool displayNoUpdateMessage = true) {
            int currentBuild = GetCurrentBuild();
            int latestBuild = GetLatestBuild();

            if(currentBuild >= latestBuild) {
                if(displayNoUpdateMessage) {
                    MessageBox.Show("No new update found.", "No Update Found", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                return;
            }

            if(MessageBox.Show("An update has been found!\nDo you wish to update to build " + latestBuild + " from build " + currentBuild + "?", "Update?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes) {
                Update(latestBuild);
            }
        }

        public static bool UpdateAvailable() {
            return GetLatestBuild() > GetCurrentBuild();
        }

        public static int GetLatestBuild() {
            return RunUpdateCommand("-latest");
        }

        public static void Update(int buildNumber) {
            RunUpdateCommand(string.Format("-update {0} \"{1}\" {2}", buildNumber, Assembly.GetExecutingAssembly().Location, Process.GetCurrentProcess().Id));
        }

        public static int RunUpdateCommand(string args) {
            Process process = new Process();
            process.StartInfo = new ProcessStartInfo() {
                FileName = Folder + "Update.exe",
                Arguments = args,
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                CreateNoWindow = true,
            };
            process.Start();
            process.WaitForExit();
            return process.ExitCode;
        }
    }
}

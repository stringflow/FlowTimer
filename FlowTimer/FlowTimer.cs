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
        public static byte[] BeepSoundUnadjusted;
        public static byte[] PCM;
        public static double MaxOffset;

        public static bool IsTimerRunning;
        public static double TimerStart;
        public static Thread TimerUpdateThread;

        public static Proc KeyboardCallback;
        public static IntPtr KeyboardHook;
        private static int[] LastKeyEvent = new int[256];

        public static void Init() {
            Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.InvariantCulture;
            Win32.InitTiming();

            if(File.Exists(SettingsFile)) {
                try {
                    Settings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(SettingsFile));
                } catch(Exception e) {
                    MessageBox.Show("The settings could not be loaded and have been reset to their default values.\n" + e.Source + ": " + e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            if(Settings == null) {
                Settings = new Settings();
            }

            PinSheet = new SpriteSheet(new Bitmap(FileSystem.ReadPackedResourceStream("FlowTimer.Resources.pin.png")), 16, 16);
            Pin(Settings.Pinned);

            AudioContext.GlobalInit();
            ChangeBeepSound(Settings.Beep, false);

            TimerUpdateThread = new Thread(TimerUpdateCallback);

            MainForm.TabControl.Selected += TabControl_Selected;
            MainForm.TabControl.Deselecting += TabControl_Deselecting;

            MainForm.ButtonAdd.DisableSelect();
            MainForm.ButtonStart.DisableSelect();
            MainForm.ButtonStop.DisableSelect();
            MainForm.ButtonSettings.DisableSelect();
            MainForm.ButtonLoadTimers.DisableSelect();
            MainForm.ButtonSaveTimers.DisableSelect();

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

            UnhookWindowsHookEx(KeyboardHook);
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
            MainForm.RemoveKeyControls();

            int buildVersion = Assembly.GetExecutingAssembly().GetName().Version.Major;
            MainForm.Text += " (Build " + buildVersion + ")";
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
            if((SettingsForm == null || !SettingsForm.Visible) && nCode >= 0) {
                Keys key = (Keys) Marshal.ReadInt32(lParam);

                if(Settings.KeyMethod.IsActivatedByEvent(wParam) && wParam != LastKeyEvent[(int) key]) {
                    if(Settings.Start.IsPressed(key)) {
                        StartTimer();
                    } else if(Settings.Stop.IsPressed(key)) {
                        StopTimer(false);
                    }

                    CurrentTab.OnKeyEvent(key);
                }
                LastKeyEvent[(int) key] = wParam;
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
            double maxOffset = offsets.Max();

            PCM = new byte[((int) Math.Ceiling(maxOffset / 1000.0 * AudioContext.SampleRate)) * AudioContext.NumChannels * AudioContext.BytesPerSample + BeepSound.Length];

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
            TimerStart = Win32.GetTime();
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
            CurrentTab.OnVisualTimerStart();
        }

        public static void StopTimer(bool timerExpired) {
            if(IsTimerRunning) {
                if(!timerExpired) {
                    AudioContext.ClearQueuedAudio();
                    TimerUpdateThread.AbortIfAlive();
                }

                IsTimerRunning = false;
                CurrentTab.OnTimerStop();
                EnableControls(true);
                LockedTab = null;
                VariableOffset.OnDataChange();
            }
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
            SettingsForm.StartPosition = FormStartPosition.CenterParent;
            SettingsForm.RemoveKeyControls();
            SettingsForm.ShowDialog(MainForm);
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
            Wave.LoadWAV(filePath, out _, out audioSpec);

            if(audioSpec.format != AUDIO_S16LSB) {
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
            Wave.LoadWAV(Beeps + beepName + ".wav", out BeepSoundUnadjusted, out audioSpec);
            AudioContext = new AudioContext(audioSpec.freq, audioSpec.format, audioSpec.channels);
            AdjustBeepSoundVolume(Settings.Volume);
            CurrentTab.OnBeepSoundChange();
            Settings.Beep = beepName;

            if(playSound) {
                AudioContext.QueueAudio(BeepSound);
            }
        }

        public static void AdjustBeepSoundVolume(int newVolume) {
            float vol = newVolume / 100.0f;
            BeepSound = new byte[BeepSoundUnadjusted.Length];
            for(int i = 0; i < BeepSound.Length; i += 2) {
                short sample = (short) (BeepSoundUnadjusted[i] | (BeepSoundUnadjusted[i + 1] << 8));
                float floatSample = sample;

                floatSample *= vol;

                if(floatSample < short.MinValue) floatSample = short.MinValue;
                if(floatSample > short.MaxValue) floatSample = short.MaxValue;

                sample = (short) floatSample;
                BeepSound[i] = (byte) (sample & 0xFF);
                BeepSound[i + 1] = (byte) (sample >> 8);
            }

            CurrentTab.OnBeepVolumeChange();
        }

        public static void ChangeKeyMethod(KeyMethod newMethod) {
            Settings.KeyMethod = newMethod;
        }
    }
}

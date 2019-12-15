using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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

        public static SettingsForm SettingsForm;
        public static Settings Settings;

        public static SpriteSheet PinSheet;

        public static AudioContext AudioContext;
        public static byte[] BeepSound;
        public static byte[] PCM;

        public static List<Timer> Timers;
        public static Timer SelectedTimer;

        public static Thread TimerUpdateThread;

        public static Proc KeyboardCallback;
        public static IntPtr KeyboardHook;

        public static void Init() {
            Settings = File.Exists(SettingsFile) ? JsonConvert.DeserializeObject<Settings>(File.ReadAllText(SettingsFile)) : new Settings();

            PinSheet = new SpriteSheet(new Bitmap(FileSystem.ReadPackedResourceStream("FlowTimer.Resources.pin.png")), 16, 16);
            Pin(Settings.Pinned);

            AudioContext.GlobalInit();
            ChangeBeepSound(Settings.Beep, false);

            Timers = new List<Timer>();
            AddTimer();

            TimerUpdateThread = new Thread(TimerUpdateCallback);

            KeyboardCallback = Keycallback;
            KeyboardHook = SetHook(WH_KEYBOARD_LL, KeyboardCallback);
        }

        public static void Destroy() {
            TimerUpdateThread.AbortIfAlive();
            AudioContext.DequeueAudio();
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

            foreach(Control ctrl in MainForm.Controls) {
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
                    } else if(Settings.Up.IsPressed(key)) {
                        MoveSelectedTimerIndex(-1);
                    } else if(Settings.Down.IsPressed(key)) {
                        MoveSelectedTimerIndex(+1);
                    }
                }
            }

            return CallNextHookEx(KeyboardHook, nCode, wParam, lParam);
        }

        public static void RepositionAddButton() {
            MainForm.ButtonAdd.SetBounds(Timer.X, Timer.Y + Timer.Size * Timers.Count - 2, MainForm.ButtonAdd.Bounds.Width, MainForm.ButtonAdd.Bounds.Height);

            Size size = new Size() {
                Width = MainForm.Width,
                Height = MainFormBaseHeight + Math.Max(Timers.Count - 5, 0) * Timer.Size,
            };

            MainForm.MinimumSize = size;
            MainForm.MaximumSize = size;
            MainForm.Size = size;
        }

        public static void EnableControls(bool enabled) {
            foreach(Timer timer in Timers) {
                Control[] excluded = { timer.RadioButton, };
                timer.Controls.Except(excluded).ToList().ForEach(control => control.Enabled = enabled);
            }

            MainForm.ButtonAdd.Enabled = enabled;
            MainForm.ButtonSettings.Enabled = enabled;
            MainForm.ButtonLoadTimers.Enabled = enabled;
            MainForm.ButtonSaveTimers.Enabled = enabled;
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

        public static void AddTimer() {
            AddTimer(new Timer(Timers.Count));
        }

        public static void AddTimer(Timer timer) {
            Timers.Add(timer);

            foreach(Control control in timer.Controls) {
                MainForm.Controls.Add(control);
                control.KeyDown += MainForm_KeyDownEvent;
            }

            RepositionAddButton();
            SelectTimer(timer);
        }

        public static void RemoveTimer(Timer timer) {
            int timerIndex = Timers.IndexOf(timer);
            timer.Controls.ForEach(MainForm.Controls.Remove);
            Timers.Remove(timer);
            for(int i = timerIndex; i < Timers.Count; i++) {
                Timers[i].Index = Timers[i].Index - 1;
            }
            RepositionAddButton();
            SelectTimer(Timers[0]);
        }

        public static void ClearAllTimers() {
            for(int i = 0; i < Timers.Count; i++) {
                Timers[i].Controls.ForEach(MainForm.Controls.Remove);
            }
            Timers.Clear();
            RepositionAddButton();
        }

        public static void SelectTimer(Timer timer) {
            SelectedTimer = timer;
            timer.RadioButton.Checked = true;

            TimerInfo timerInfo = UpdatePCM();

            List<Control> controls = new List<Control>() { MainForm.ButtonStart, MainForm.ButtonStop, };

            if(timerInfo != null) {
                MainForm.LabelTimer.Text = (timerInfo.MaxOffset / 1000.0).ToFormattedString();
                controls.ForEach(control => control.Enabled = true);
            } else {
                MainForm.LabelTimer.Text = "Error";
                controls.ForEach(control => control.Enabled = false);
            }
        }

        public static TimerInfo UpdatePCM() {
            TimerInfo timerInfo;
            TimerError error = SelectedTimer.GetTimerInfo(out timerInfo);

            if(error == TimerError.NoError) {
                // try to force garbage collection on the old pcm data
                GC.Collect();

                PCM = new byte[((int) Math.Ceiling(timerInfo.MaxOffset / 1000.0 * AudioContext.SampleRate)) * AudioContext.NumChannels * 2 + BeepSound.Length];

                foreach(uint offset in timerInfo.Offsets) {
                    for(int i = 0; i < timerInfo.NumBeeps; i++) {
                        int destOffset = (int) ((offset - i * timerInfo.Interval) / 1000.0 * AudioContext.SampleRate) * AudioContext.NumChannels * 2;
                        Array.Copy(BeepSound, 0, PCM, destOffset, BeepSound.Length);
                    }
                }

                return timerInfo;
            }

            return null;
        }

        public static void StartTimer() {
            AudioContext.DequeueAudio();
            AudioContext.QueueAudio(PCM);

            if(!MainForm.ButtonStart.Enabled) {
                AudioContext.DequeueAudio();
                return;
            }

            EnableControls(false);

            TimerUpdateThread.AbortIfAlive();
            TimerUpdateThread = new Thread(TimerUpdateCallback);
            TimerUpdateThread.Start();
        }

        public static void StopTimer(bool timerExpired) {
            if(!timerExpired) {
                AudioContext.DequeueAudio();
                TimerUpdateThread.AbortIfAlive();
            }

            EnableControls(true);
            SelectTimer(SelectedTimer);
        }

        private static void TimerUpdateCallback() {
            const uint resolution = 15;

            TimerInfo timerInfo;
            SelectedTimer.GetTimerInfo(out timerInfo);

            MethodInvoker inv;
            uint startTime = SDL_GetTicks();
            double currentTime;

            do {
                currentTime = Math.Max((timerInfo.MaxOffset - ((long) (SDL_GetTicks() - startTime))) / 1000.0, 0.0);

                inv = delegate {
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

            if(SelectedTimer != null) UpdatePCM();

            Settings.Beep = beepName;

            if(playSound) {
                AudioContext.QueueAudio(BeepSound);
            }
        }

        public static void ChangeKeyMethod(KeyMethod newMethod) {
            Settings.KeyMethod = newMethod;
        }

        public static void MoveSelectedTimerIndex(int amount) {
            if(SelectedTimer == null || Timers.Count == 0) {
                return;
            }

            int selectedTimerIndex = Timers.IndexOf(SelectedTimer);
            selectedTimerIndex = (((selectedTimerIndex + amount) % Timers.Count) + Timers.Count) % Timers.Count;
            SelectTimer(Timers[selectedTimerIndex]);
        }

        public static void OpenLoadTimersDialog() {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = TimerFileFilter;

            if(dialog.ShowDialog() == DialogResult.OK) {
                LoadTimers(dialog.FileName, true);
            }
        }

        public static bool LoadTimers(string filePath, bool displayMessages = true) {
            var json = JsonConvert.DeserializeObject<dynamic>(File.ReadAllText(filePath));

            JsonTimerFile file = json.GetType() == typeof(JArray) ? LoadJsonTimerLegacy((JArray) json) : LoadJsonTimerModern((JObject) json);

            if(file.Timers.Count == 0) {
                if(displayMessages) {
                    MessageBox.Show("Timers could not be loaded.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return false;
            }

            ClearAllTimers();
            for(int i = 0; i < file.Timers.Count; i++) {
                JsonTimer timer = file.Timers[i];
                AddTimer(new Timer(i, timer.Name, timer.Offsets, timer.Interval, timer.NumBeeps));
            }

            if(displayMessages) {
                MessageBox.Show("Timers successfully loaded from '" + filePath + "'.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            return true;
        }

        private static JsonTimerFile LoadJsonTimerLegacy(JArray json) {
            return new JsonTimerFile(new JsonTimersHeader(), json.ToObject<List<JsonTimer>>());
        }

        private static JsonTimerFile LoadJsonTimerModern(JObject json) {
            return json.ToObject<JsonTimerFile>();
        }

        public static void OpenSaveTimersDialog() {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = TimerFileFilter;

            if(dialog.ShowDialog() == DialogResult.OK) {
                SaveTimers(dialog.FileName, true);
            }
        }

        public static bool SaveTimers(string filePath, bool displayMessages = true) {
            JsonTimerFile timerFile = new JsonTimerFile(new JsonTimersHeader(), Timers.ConvertAll(timer => new JsonTimer() {
                Name = timer.TextBoxName.Text,
                Offsets = timer.TextBoxOffset.Text,
                Interval = timer.TextBoxInterval.Text,
                NumBeeps = timer.TextBoxNumBeeps.Text,
            }));

            try {
                File.WriteAllText(filePath, JsonConvert.SerializeObject(timerFile));
                if(displayMessages) {
                    MessageBox.Show("Timers successfully saved to '" + filePath + "'.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                return true;
            } catch(Exception e) {
                if(displayMessages) {
                    MessageBox.Show("Timers could not be saved. Exception: " + e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return false;
            }
        }

        public static void CheckForUpdates() {
            int currentBuild = GetCurrentBuild();
            int latestBuild = GetLatestBuild();

            if(currentBuild >= latestBuild) {
                MessageBox.Show("No new update found.", "No Update Found", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            } 

            if(MessageBox.Show("An update has been found!\nDo you wish to update to build " + latestBuild + " from build " + currentBuild + "?", "Update?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes) {
                Update(latestBuild);
            }
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

using System;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FlowTimer {

    public class IGTTracking : BaseTimer {

        public ComboBox ComboBoxGame;
        public ComboBox ComboBoxFPS;
        public Button ButtonAdd;

        public bool Playing;
        public double CurrentOffset;
        public double Adjusted;

        public List<IGTDelayer> Delayers;
        public static readonly string DelayersFile = FlowTimer.Folder + "igtdelayers.json";
        JsonIGTDelayersSettings DelayersSettings;

        public List<IGTTimer> Timers;
        public IGTTimer SelectedTimer;

        public IGTTracking(TabPage tab, params Control[] copyControls) : base(tab, null, copyControls) {
            ComboBoxGame = FlowTimer.MainForm.ComboBoxGame;
            ComboBoxFPS = FlowTimer.MainForm.ComboBoxFPS3;
            ButtonAdd = FlowTimer.MainForm.ButtonAdd3;

            TimerCallback = TimerCallbackFn;
            ComboBoxGame.SelectedIndexChanged += (sender, e) => { FlowTimer.Settings.IGTGame = ComboBoxGame.SelectedItem as string; UpdateGame(); };
            ComboBoxFPS.SelectedIndexChanged += (sender, e) => FlowTimer.Settings.IGTFPS = ComboBoxFPS.SelectedItem as string;

            Timers = new List<IGTTimer>();
            Delayers = new List<IGTDelayer>();
        }

        public override void OnInit() {
            LoadDelayersFile();
            if(FlowTimer.Settings.IGTGame != null) ComboBoxGame.SelectedItem = FlowTimer.Settings.IGTGame; else ComboBoxGame.SelectedItem = ComboBoxGame.Items[0];
            ComboBoxFPS.SelectedItem = FlowTimer.Settings.IGTFPS;

            if(FlowTimer.Settings.LastLoadedIGTTimers != null && File.Exists(FlowTimer.Settings.LastLoadedIGTTimers)) {
                LoadTimers(FlowTimer.Settings.LastLoadedIGTTimers, false);
            } else {
                AddTimer();
            }
        }

        public override void OnLoad() {
            base.OnLoad();
            RepositionAddButton();
            SelectTimer(SelectedTimer);
            OnTimerStop();
        }

        public override void OnTimerStart() {
            CurrentOffset = double.MaxValue;
            Playing = false;
            foreach(IGTDelayer delayer in Delayers)
                delayer.Count.Text = "0";
            Adjusted = 0;
        }

        public override void OnVisualTimerStart() {
        }

        public override void OnTimerStop() {
            Playing = false;
            EnableControls(true);
            FlowTimer.MainForm.LabelTimer.Text = 0.0.ToFormattedString();
            FlowTimer.MainForm.LabelTimer.Focus();
            foreach(IGTDelayer delayer in Delayers)
                delayer.Count.Text = "0";
            Adjusted = 0;
        }

        public override void OnKeyEvent(Keys key) {
            if(FlowTimer.Settings.AddFrame.IsPressed(key)) {
                if(Delayers.Count > 0) Delayers[0].Plus.PerformClick();
            } else if(FlowTimer.Settings.SubFrame.IsPressed(key)) {
                if(Delayers.Count > 0) Delayers[0].Minus.PerformClick();
            } else if(FlowTimer.Settings.Add2.IsPressed(key)) {
                if(Delayers.Count > 1) Delayers[1].Plus.PerformClick();
            } else if(FlowTimer.Settings.Sub2.IsPressed(key)) {
                if(Delayers.Count > 1) Delayers[1].Minus.PerformClick();
            } else if(FlowTimer.Settings.Add3.IsPressed(key)) {
                if(Delayers.Count > 2) Delayers[2].Plus.PerformClick();
            } else if(FlowTimer.Settings.Sub3.IsPressed(key)) {
                if(Delayers.Count > 2) Delayers[2].Minus.PerformClick();
            } else if(FlowTimer.Settings.Add4.IsPressed(key)) {
                if(Delayers.Count > 3) Delayers[3].Plus.PerformClick();
            } else if(FlowTimer.Settings.Sub4.IsPressed(key)) {
                if(Delayers.Count > 3) Delayers[3].Minus.PerformClick();
            } else if(FlowTimer.Settings.Add5.IsPressed(key)) {
                if(Delayers.Count > 4) Delayers[4].Plus.PerformClick();
            } else if(FlowTimer.Settings.Sub5.IsPressed(key)) {
                if(Delayers.Count > 4) Delayers[4].Minus.PerformClick();
            } else if(FlowTimer.Settings.Undo.IsPressed(key) && FlowTimer.MainForm.ButtonUndoPlay.Enabled) {
                Undo();
            } else if(FlowTimer.Settings.Play.IsPressed(key) && FlowTimer.MainForm.ButtonPlay.Enabled) {
                Play();
            } else if(FlowTimer.Settings.Up.IsPressed(key)) {
                MoveSelectedTimerIndex(-1);
            } else if(FlowTimer.Settings.Down.IsPressed(key)) {
                MoveSelectedTimerIndex(+1);
            }
        }

        public override void OnBeepSoundChange() {
        }

        public override void OnBeepVolumeChange() {
        }

        public void UpdateGame() {
            foreach(IGTDelayer delayer in Delayers) {
                Tab.Controls.Remove(delayer.Name);
                Tab.Controls.Remove(delayer.Count);
                Tab.Controls.Remove(delayer.Minus);
                Tab.Controls.Remove(delayer.Plus);
            }
            Delayers.Clear();

            foreach(JsonIGTDelayersGame g in DelayersSettings.Games) {
                if(ComboBoxGame.Text == g.Game) {
                    foreach(JsonIGTDelayer d in g.Delayers) {
                        AddDelayer(d.Name, d.Delay);
                    }
                    break;
                }
            }

            UpdateTimersBaseY();
        }

        public void LoadDelayersFile() {
            if(File.Exists(DelayersFile)) {
                try {
                    DelayersSettings = JsonConvert.DeserializeObject<JsonIGTDelayersSettings>(File.ReadAllText(DelayersFile));
                    if(DelayersSettings.Header.Version < FlowTimer.GetCurrentBuild()) DelayersSettings = null;
                } catch(Exception e) {
                    MessageBox.Show("Error reading delayers settings.\n" + e.Source + ": " + e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            if(DelayersSettings == null) {
                DelayersSettings = new JsonIGTDelayersSettings {
                    Header = new JsonTimersHeader(),
                    Games = new List<JsonIGTDelayersGame>
                    {
                        new JsonIGTDelayersGame
                        {
                            Game = "Red/Blue",
                            Delayers = new List<JsonIGTDelayer>
                            {
                                new JsonIGTDelayer { Name = "Encounter", Delay = 9.10 },
                                new JsonIGTDelayer { Name = "Encounter (forest)", Delay = 6.35 },
                                new JsonIGTDelayer { Name = "Heal/Swap (outdoors)", Delay = 6.60 },
                                new JsonIGTDelayer { Name = "Heal/Swap (indoors)", Delay = 3.90 },
                                new JsonIGTDelayer { Name = "Heal (in battle)", Delay = 1.05 },
                            }
                        },
                        new JsonIGTDelayersGame
                        {
                            Game = "Yellow",
                            Delayers = new List<JsonIGTDelayer>
                            {
                                new JsonIGTDelayer { Name = "Color lag",  Delay = 2.00 },
                                new JsonIGTDelayer { Name = "Pika death",  Delay = 83.00 },
                                new JsonIGTDelayer { Name = "Encounter",  Delay = 51.35 },
                                new JsonIGTDelayer { Name = "Encounter (forest)",  Delay = 15.85 },
                                new JsonIGTDelayer { Name = "Heal (indoors)",  Delay = 12.00 },
                                new JsonIGTDelayer { Name = "Heal (in battle)",  Delay = 5.00 },
                            }
                        },
                    }
                };
            }

            foreach(JsonIGTDelayersGame g in DelayersSettings.Games)
                ComboBoxGame.Items.Add(g.Game);
        }

        public void SaveDelayersFile() {
            File.WriteAllText(DelayersFile, JsonConvert.SerializeObject(DelayersSettings));
        }

        public void UpdateTimersBaseY() {
            IGTTimer.Y = 30 + Math.Max(5, Delayers.Count) * IGTDelayer.Size;

            FlowTimer.MainForm.LabelName3.Top = IGTTimer.Y - 16;
            FlowTimer.MainForm.LabelFrame3.Top = IGTTimer.Y - 16;
            FlowTimer.MainForm.LabelOffset3.Top = IGTTimer.Y - 16;
            FlowTimer.MainForm.LabelInterval3.Top = IGTTimer.Y - 16;
            FlowTimer.MainForm.LabelBeeps3.Top = IGTTimer.Y - 16;
            for(int i = 0; i < Timers.Count; ++i)
                Timers[i].Index = i;

            RepositionAddButton();
        }

        public void AddDelayer(string name, double delay) {
            Delayers.Add(new IGTDelayer(Delayers.Count, name, delay));
        }

        public double TimerCallbackFn(double start) {
            OnDataChange();
            return Math.Max((Win32.GetTime() - start) / 1000.0, 0.001);
        }

        public void OnDataChange() {
            double currentTime = double.Parse(FlowTimer.MainForm.LabelTimer.Text);
            if(Playing && CurrentOffset < currentTime)
                Playing = false;
        }

        public void Play() {
            if(!FlowTimer.IsTimerRunning) return;
            IGTTimerInfo info;
            TimerError error = SelectedTimer.GetTimerInfo(out info);
            if(error != TimerError.NoError) return;
            double now = Win32.GetTime();
            double offset = (info.Frame / info.FPS * 1000.0f) - (now - FlowTimer.TimerStart) + info.Offset + Adjusted;
            while(offset - info.Interval * (info.NumBeeps - 1) < 0.0f)
                offset += 60.0f / info.FPS * 1000.0f;
            double[] offsets = new double[10];
            for(int i = 0; i < offsets.Length; ++i)
                offsets[i] = offset + 60.0f / info.FPS * 1000.0f * i;
            FlowTimer.UpdatePCM(offsets, info.Interval, info.NumBeeps, false);
            FlowTimer.AudioContext.QueueAudio(FlowTimer.PCM);
            CurrentOffset = (offsets[9] + now - FlowTimer.TimerStart) / 1000.0f;
            Playing = true;
            EnableControls(false);
        }

        public void Undo() {
            Playing = false;
            EnableControls(true);
            CurrentOffset = double.MaxValue;
            FlowTimer.AudioContext.ClearQueuedAudio();
        }

        public void ChangeAudio(double numFrames) {
            FlowTimer.AudioContext.ClearQueuedAudio();
            IGTTimerInfo info;
            SelectedTimer.GetTimerInfo(out info);
            double amount = numFrames * 1000.0 / info.FPS;
            Adjusted += amount;
            if(Playing) Play();
        }

        public void EnableControls(bool enabled) {
        }

        public void RepositionAddButton() {
            ButtonAdd.SetBounds(IGTTimer.X, IGTTimer.Y + IGTTimer.Size * Timers.Count - 2, ButtonAdd.Bounds.Width, ButtonAdd.Bounds.Height);
            if(Selected) {
                FlowTimer.ResizeForm(FlowTimer.MainForm.Width, FlowTimer.MainFormBaseHeight + 10
                    + Math.Max(Delayers.Count - 5, 0) * IGTDelayer.Size
                    + Math.Max(Timers.Count - 1, 0) * IGTTimer.Size);
            }
        }

        public void AddTimer() {
            AddTimer(new IGTTimer(Timers.Count));
        }

        public void AddTimer(IGTTimer timer) {
            Timers.Add(timer);

            foreach(Control control in timer.Controls) {
                Tab.Controls.Add(control);
                control.RemoveKeyControls();
            }

            RepositionAddButton();
            SelectTimer(timer);
        }

        public void RemoveTimer(IGTTimer timer) {
            int timerIndex = Timers.IndexOf(timer);
            timer.Controls.ForEach(Tab.Controls.Remove);
            Timers.Remove(timer);
            for(int i = timerIndex; i < Timers.Count; i++) {
                Timers[i].Index = Timers[i].Index - 1;
            }
            RepositionAddButton();
            SelectTimer(Timers[0]);
        }

        public void ClearAllTimers() {
            for(int i = 0; i < Timers.Count; i++) {
                Timers[i].Controls.ForEach(Tab.Controls.Remove);
            }
            Timers.Clear();
            RepositionAddButton();
        }

        public void SelectTimer(IGTTimer timer) {
            if(timer == null) return;

            SelectedTimer = timer;
            timer.RadioButton.Checked = true;
        }

        public void MoveSelectedTimerIndex(int amount) {
            if(SelectedTimer == null || Timers.Count == 0 || !Selected) {
                return;
            }

            int selectedTimerIndex = Timers.IndexOf(SelectedTimer);
            selectedTimerIndex = (((selectedTimerIndex + amount) % Timers.Count) + Timers.Count) % Timers.Count;
            SelectTimer(Timers[selectedTimerIndex]);
        }

        public void OpenLoadTimersDialog() {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = FlowTimer.TimerFileFilter;

            if(dialog.ShowDialog() == DialogResult.OK) {
                LoadTimers(dialog.FileName, true);
            }
        }

        public JsonIGTTimersFile ReadTimers(string filePath) {
            var json = JsonConvert.DeserializeObject<dynamic>(File.ReadAllText(filePath));
            return json.GetType() == typeof(JArray) ? ReadJsonIGTTimersLegacy((JArray) json) : ReadJsonIGTTimersModern((JObject) json);
        }

        private JsonIGTTimersFile ReadJsonIGTTimersLegacy(JArray json) {
            return new JsonIGTTimersFile(new JsonTimersHeader(), json.ToObject<List<JsonIGTTimer>>());
        }

        private JsonIGTTimersFile ReadJsonIGTTimersModern(JObject json) {
            return json.ToObject<JsonIGTTimersFile>();
        }

        public bool LoadTimers(string filePath, bool displayMessages = true) {
            JsonIGTTimersFile file = ReadTimers(filePath);

            if(file.Timers.Count == 0) {
                if(displayMessages) {
                    MessageBox.Show("Timers could not be loaded.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return false;
            }

            ClearAllTimers();
            for(int i = 0; i < file.Timers.Count; i++) {
                JsonIGTTimer timer = file[i];
                AddTimer(new IGTTimer(i, timer.Name, timer.Frame, timer.Offsets, timer.Interval, timer.NumBeeps));
            }

            if(displayMessages) {
                MessageBox.Show("Timers successfully loaded from '" + filePath + "'.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            FlowTimer.Settings.LastLoadedIGTTimers = filePath;
            return true;
        }

        public void OpenSaveTimersDialog() {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = FlowTimer.TimerFileFilter;

            if(dialog.ShowDialog() == DialogResult.OK) {
                SaveTimers(dialog.FileName, true);
            }
        }

        public JsonIGTTimersFile BuildJsonIGTTimerFile() {
            return new JsonIGTTimersFile(new JsonTimersHeader(), Timers.ConvertAll(timer => new JsonIGTTimer() {
                Name = timer.TextBoxName.Text,
                Frame = timer.TextBoxFrame.Text,
                Offsets = timer.TextBoxOffset.Text,
                Interval = timer.TextBoxInterval.Text,
                NumBeeps = timer.TextBoxNumBeeps.Text,
            }));
        }

        public bool SaveTimers(string filePath, bool displayMessages = true) {
            JsonIGTTimersFile timerFile = BuildJsonIGTTimerFile();

            try {
                File.WriteAllText(filePath, JsonConvert.SerializeObject(timerFile));
                if(displayMessages) {
                    MessageBox.Show("Timers successfully saved to '" + filePath + "'.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                FlowTimer.Settings.LastLoadedIGTTimers = filePath;
                return true;
            } catch(Exception e) {
                if(displayMessages) {
                    MessageBox.Show("Timers could not be saved. Exception: " + e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return false;
            }
        }

        public bool HaveTimersChanged() {
            if(FlowTimer.Settings.LastLoadedIGTTimers == null) {
                return false;
            }

            if(!File.Exists(FlowTimer.Settings.LastLoadedIGTTimers)) {
                return false;
            }

            JsonIGTTimersFile oldTimers = ReadTimers(FlowTimer.Settings.LastLoadedIGTTimers);
            JsonIGTTimersFile newTimers = BuildJsonIGTTimerFile();

            if(oldTimers.Timers.Count != newTimers.Timers.Count) {
                return true;
            }

            for(int i = 0; i < oldTimers.Timers.Count; i++) {
                JsonIGTTimer timer1 = oldTimers[i];
                JsonIGTTimer timer2 = newTimers[i];
                if(timer1.Name != timer2.Name ||
                   timer1.Frame != timer2.Frame ||
                   timer1.Offsets != timer2.Offsets ||
                   timer1.Interval != timer2.Interval ||
                   timer1.NumBeeps != timer2.NumBeeps) {
                    return true;
                }
            }

            return false;
        }
    }

    public class IGTDelayer {

        public Label Name;
        public TextBox Count;
        public Button Minus;
        public Button Plus;

        public const int Size = 26;

        public IGTDelayer(int index, string name, double delay) {
            Control.ControlCollection parent = FlowTimer.IGTTracking.Tab.Controls;
            int y = index * Size;

            Name = new Label();
            Name.Text = name + ":";
            Name.SetBounds(161, 14 + y, 115, 13);
            parent.Add(Name);

            Count = new TextBox();
            Count.Text = "0";
            Count.SetBounds(277, 10 + y, 23, 20);
            Count.TextAlign = HorizontalAlignment.Center;
            Count.Enabled = false;
            parent.Add(Count);

            Minus = new Button();
            Minus.Text = "-";
            Minus.SetBounds(303, 9 + y, 22, 22);
            Minus.Click += (s, e) => { Count.Text = (int.Parse(Count.Text) - 1).ToString(); FlowTimer.IGTTracking.ChangeAudio(-delay); };
            parent.Add(Minus);

            Plus = new Button();
            Plus.Text = "+";
            Plus.SetBounds(327, 9 + y, 22, 22);
            Plus.Click += (s, e) => { Count.Text = (int.Parse(Count.Text) + 1).ToString(); FlowTimer.IGTTracking.ChangeAudio(+delay); };
            parent.Add(Plus);
        }
    }

    public struct IGTTimerInfo {
        public uint Frame;
        public int Offset;
        public uint Interval;
        public uint NumBeeps;
        public double FPS;
    }

    public class IGTTimer {
        public const int X = 165;
        public static int Y = 160;
        public const int Size = 28;

        public TextBox TextBoxName;
        public TextBox TextBoxFrame;
        public TextBox TextBoxOffset;
        public TextBox TextBoxInterval;
        public TextBox TextBoxNumBeeps;
        public RadioButton RadioButton;
        public Button RemoveButton;
        public List<Control> Controls;

        public List<TextBox> TextBoxes {
            get { return Controls.FindAll(control => control.GetType() == typeof(TextBox)).ConvertAll(control => (TextBox) control); }
        }

        public int Index {
            get {
                return (TextBoxes[0].Location.Y - Y) / Size;
            }
            set {
                int yPosition = Y + value * Size;

                TextBoxName.SetBounds(X, yPosition, 65, 21);
                TextBoxFrame.SetBounds(X + 70, yPosition, 40, 21);
                TextBoxOffset.SetBounds(X + 115, yPosition, 65, 21);
                TextBoxInterval.SetBounds(X + 185, yPosition, 50, 21);
                TextBoxNumBeeps.SetBounds(X + 240, yPosition, 40, 21);

                RadioButton.SetBounds(X - 21, yPosition + 4, 14, 13);

                if(value > 0) {
                    Rectangle lastbox = TextBoxes.Last().Bounds;
                    RemoveButton.SetBounds(lastbox.X + lastbox.Width + 5, lastbox.Y, 38, 21);
                }
            }
        }

        public IGTTimer(int index, string name = "Timer", string frame = "0", string offset = "0", string interval = "250", string numBeeps = "3") {
            Controls = new List<Control>();

            TextBoxName = new TextBox();
            TextBoxName.Text = name;
            Controls.Add(TextBoxName);

            TextBoxFrame = new TextBox();
            TextBoxFrame.Text = frame;
            Controls.Add(TextBoxFrame);

            TextBoxOffset = new TextBox();
            TextBoxOffset.Text = offset;
            Controls.Add(TextBoxOffset);

            TextBoxInterval = new TextBox();
            TextBoxInterval.Text = interval;
            Controls.Add(TextBoxInterval);

            TextBoxNumBeeps = new TextBox();
            TextBoxNumBeeps.Text = numBeeps;
            Controls.Add(TextBoxNumBeeps);

            foreach(TextBox textbox in TextBoxes) {
                textbox.Font = new Font(textbox.Font.FontFamily, 9.0f);
                textbox.TextChanged += DataChanged;
                textbox.TabStop = false;
            }

            RadioButton = new RadioButton();
            RadioButton.Click += RadioButton_Click;
            Controls.Add(RadioButton);

            if(index > 0) {
                RemoveButton = new Button();
                RemoveButton.Text = "-";
                RemoveButton.Click += RemoveButton_Click;
                RemoveButton.TabStop = false;
                RemoveButton.DisableSelect();
                Controls.Add(RemoveButton);
            }

            Index = index;
        }

        public TimerError GetTimerInfo(out IGTTimerInfo info) {
            info = new IGTTimerInfo();

            if(!uint.TryParse(TextBoxFrame.Text, out info.Frame)) {
                return TimerError.InvalidFrame;
            }

            if(!int.TryParse(TextBoxOffset.Text, out info.Offset)) {
                return TimerError.InvalidOffset;
            }

            if(!uint.TryParse(TextBoxInterval.Text, out info.Interval)) {
                return TimerError.InvalidInterval;
            }

            if(!uint.TryParse(TextBoxNumBeeps.Text, out info.NumBeeps)) {
                return TimerError.InvalidNumBeeps;
            }

            if(!double.TryParse(FlowTimer.IGTTracking.ComboBoxFPS.SelectedItem as string, out info.FPS)) {
                return TimerError.InvalidFPS;
            }

            if(info.Frame >= ushort.MaxValue << 8) {
                return TimerError.InvalidFrame;
            }

            if(info.Offset >= ushort.MaxValue << 9) {
                return TimerError.InvalidOffset;
            }

            if(info.Interval >= ushort.MaxValue << 9) {
                return TimerError.InvalidInterval;
            }

            if(info.NumBeeps >= ushort.MaxValue << 9) {
                return TimerError.InvalidNumBeeps;
            }

            return TimerError.NoError;
        }

        private void RadioButton_Click(object sender, EventArgs e) {
            FlowTimer.IGTTracking.SelectTimer(this);
        }

        private void DataChanged(object sender, EventArgs e) {
            FlowTimer.IGTTracking.SelectTimer(this);
        }

        private void RemoveButton_Click(object sender, EventArgs e) {
            FlowTimer.IGTTracking.RemoveTimer(this);
        }
    }
}

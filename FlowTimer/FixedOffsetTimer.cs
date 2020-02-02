using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Windows.Forms;
using System.Drawing;

namespace FlowTimer {
    
    public class FixedOffsetTimer : BaseTimer {

        public Button ButtonAdd;
        public Button ButtonLoadTimers;
        public Button ButtonSaveTimers;

        public List<Timer> Timers;
        public Timer SelectedTimer;

        public FixedOffsetTimer(TabPage tab, params Control[] copyControls) : base(tab, (start) => Math.Max((FlowTimer.MaxOffset - ((DateTime.Now.Ticks - start) / 10000.0)) / 1000.0, 0.0), copyControls) {
            ButtonAdd = FlowTimer.MainForm.ButtonAdd;
            ButtonLoadTimers = FlowTimer.MainForm.ButtonLoadTimers;
            ButtonSaveTimers = FlowTimer.MainForm.ButtonSaveTimers;

            Timers = new List<Timer>();
        }

        public override void OnInit() {
            if(FlowTimer.Settings.LastLoadedTimers != null && File.Exists(FlowTimer.Settings.LastLoadedTimers)) {
                LoadTimers(FlowTimer.Settings.LastLoadedTimers, false);
            } else {
                AddTimer();
            }
        }

        public override void OnLoad() {
            base.OnLoad();
            RepositionAddButton();
            SelectTimer(SelectedTimer);
        }

        public override void OnTimerStart() {
            FlowTimer.AudioContext.QueueAudio(FlowTimer.PCM);
            EnableControls(false);
        }

        public override void OnTimerStop() {
            EnableControls(true);
            SelectTimer(SelectedTimer);
        }

        public override void OnKeyEvent(Keys key) {
            if(FlowTimer.Settings.Up.IsPressed(key)) {
                MoveSelectedTimerIndex(-1);
            } else if(FlowTimer.Settings.Down.IsPressed(key)) {
                MoveSelectedTimerIndex(+1);
            }
        }

        public override void OnBeepSoundChange() {
            SelectTimer(SelectedTimer);
        }

        public void RepositionAddButton() {
            ButtonAdd.SetBounds(Timer.X, Timer.Y + Timer.Size * Timers.Count - 2, ButtonAdd.Bounds.Width, ButtonAdd.Bounds.Height);
            FlowTimer.ResizeForm(FlowTimer.MainForm.Width, FlowTimer.MainFormBaseHeight + Math.Max(Timers.Count - 5, 0) * Timer.Size);
        }

        public void EnableControls(bool enabled) {
            foreach(Timer timer in Timers) {
                Control[] excluded = { timer.RadioButton, };
                timer.Controls.Except(excluded).ToList().ForEach(control => control.Enabled = enabled);
            }

            ButtonAdd.Enabled = enabled;
            ButtonLoadTimers.Enabled = enabled;
            ButtonSaveTimers.Enabled = enabled;
        }

        public void AddTimer() {
            AddTimer(new Timer(Timers.Count));
        }

        public void AddTimer(Timer timer) {
            Timers.Add(timer);

            foreach(Control control in timer.Controls) {
                Tab.Controls.Add(control);
                control.RemoveKeyControls();
            }

            RepositionAddButton();
            SelectTimer(timer);
        }

        public void RemoveTimer(Timer timer) {
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

        public void SelectTimer(Timer timer) {
            if(!Selected || timer == null) return;

            SelectedTimer = timer;
            timer.RadioButton.Checked = true;

            TimerInfo timerInfo;
            TimerError error = SelectedTimer.GetTimerInfo(out timerInfo);

            List<Control> controls = new List<Control>() { FlowTimer.MainForm.ButtonStart, FlowTimer.MainForm.ButtonStop, };

            if(error == TimerError.NoError) {
                FlowTimer.UpdatePCM(Array.ConvertAll(timerInfo.Offsets, x => (double) x), timerInfo.Interval, timerInfo.NumBeeps);
                FlowTimer.MainForm.LabelTimer.Text = (timerInfo.MaxOffset / 1000.0).ToFormattedString();
                controls.ForEach(control => control.Enabled = true);
            } else {
                FlowTimer.MainForm.LabelTimer.Text = "Error";
                controls.ForEach(control => control.Enabled = false);
            }
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

        public JsonTimersFile ReadTimers(string filePath) {
            var json = JsonConvert.DeserializeObject<dynamic>(File.ReadAllText(filePath));
            return json.GetType() == typeof(JArray) ? ReadJsonTimersLegacy((JArray) json) : ReadJsonTimersModern((JObject) json);
        }

        private JsonTimersFile ReadJsonTimersLegacy(JArray json) {
            return new JsonTimersFile(new JsonTimersHeader(), json.ToObject<List<JsonTimer>>());
        }

        private JsonTimersFile ReadJsonTimersModern(JObject json) {
            return json.ToObject<JsonTimersFile>();
        }

        public bool LoadTimers(string filePath, bool displayMessages = true) {
            JsonTimersFile file = ReadTimers(filePath);

            if(file.Timers.Count == 0) {
                if(displayMessages) {
                    MessageBox.Show("Timers could not be loaded.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return false;
            }

            ClearAllTimers();
            for(int i = 0; i < file.Timers.Count; i++) {
                JsonTimer timer = file[i];
                AddTimer(new Timer(i, timer.Name, timer.Offsets, timer.Interval, timer.NumBeeps));
            }

            if(displayMessages) {
                MessageBox.Show("Timers successfully loaded from '" + filePath + "'.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            FlowTimer.Settings.LastLoadedTimers = filePath;
            return true;
        }

        public void OpenSaveTimersDialog() {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = FlowTimer.TimerFileFilter;

            if(dialog.ShowDialog() == DialogResult.OK) {
                SaveTimers(dialog.FileName, true);
            }
        }

        public JsonTimersFile BuildJsonTimerFile() {
            return new JsonTimersFile(new JsonTimersHeader(), Timers.ConvertAll(timer => new JsonTimer() {
                Name = timer.TextBoxName.Text,
                Offsets = timer.TextBoxOffset.Text,
                Interval = timer.TextBoxInterval.Text,
                NumBeeps = timer.TextBoxNumBeeps.Text,
            }));
        }

        public bool SaveTimers(string filePath, bool displayMessages = true) {
            JsonTimersFile timerFile = BuildJsonTimerFile();

            try {
                File.WriteAllText(filePath, JsonConvert.SerializeObject(timerFile));
                if(displayMessages) {
                    MessageBox.Show("Timers successfully saved to '" + filePath + "'.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                FlowTimer.Settings.LastLoadedTimers = filePath;
                return true;
            } catch(Exception e) {
                if(displayMessages) {
                    MessageBox.Show("Timers could not be saved. Exception: " + e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return false;
            }
        }

        public bool HaveTimersChanged() {
            if(FlowTimer.Settings.LastLoadedTimers == null) {
                return false;
            }

            if(!File.Exists(FlowTimer.Settings.LastLoadedTimers)) {
                return false;
            }

            JsonTimersFile oldTimers = ReadTimers(FlowTimer.Settings.LastLoadedTimers);
            JsonTimersFile newTimers = BuildJsonTimerFile();

            if(oldTimers.Timers.Count != newTimers.Timers.Count) {
                return true;
            }

            for(int i = 0; i < oldTimers.Timers.Count; i++) {
                JsonTimer timer1 = oldTimers[i];
                JsonTimer timer2 = newTimers[i];
                if(timer1.Name != timer2.Name ||
                   timer1.Offsets != timer2.Offsets ||
                   timer1.Interval != timer2.Interval ||
                   timer1.NumBeeps != timer2.NumBeeps) {
                    return true;
                }
            }

            return false;
        }
    }

    public class TimerInfo {

        public uint[] Offsets;
        public uint MaxOffset;
        public uint Interval;
        public uint NumBeeps;
    }

    public class Timer {

        public const int X = 165;
        public const int Y = 31;
        public const int Size = 28;

        public TextBox TextBoxName;
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
                int xOffset = 0;

                for(int i = 0; i < TextBoxes.Count; i++) {
                    TextBoxes[i].SetBounds(X + xOffset, yPosition, 65, 21);
                    xOffset += TextBoxes[i].Width + 5;
                }

                RadioButton.SetBounds(X - 21, yPosition + 4, 14, 13);

                //only reposition remove button if not index = 0
                if(value > 0) {
                    Rectangle lastbox = TextBoxes.Last().Bounds;
                    RemoveButton.SetBounds(lastbox.X + lastbox.Width + 5, lastbox.Y, 38, 21);
                }
            }
        }

        public Timer(int index, string name = "Timer", string offset = "5000", string interval = "500", string numBeeps = "5") {
            Controls = new List<Control>();

            TextBoxName = new TextBox();
            TextBoxName.Text = name;
            Controls.Add(TextBoxName);

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

        public TimerError GetTimerInfo(out TimerInfo info) {
            info = new TimerInfo();

            if(!uint.TryParse(TextBoxInterval.Text, out info.Interval)) {
                return TimerError.InvalidInterval;
            }

            if(!uint.TryParse(TextBoxNumBeeps.Text, out info.NumBeeps)) {
                return TimerError.InvalidNumBeeps;
            }

            string[] offsetsStr = TextBoxOffset.Text.Split('/');
            uint[] offsets = new uint[offsetsStr.Length];

            for(int i = 0; i < offsetsStr.Length; i++) {
                if(!uint.TryParse(offsetsStr[i], out offsets[i])) {
                    return TimerError.InvalidOffset;
                }
            }

            Array.Sort(offsets);
            info.Offsets = offsets;
            info.MaxOffset = offsets.Last();

            // << 9 on all of these to avoid going over the 32-bit integer limit when rebuilding the pcm
            // makes the maximum offset be 33553919
            if(info.Interval >= ushort.MaxValue << 9) {
                return TimerError.InvalidInterval;
            }

            if(info.NumBeeps >= ushort.MaxValue << 9) {
                return TimerError.InvalidNumBeeps;
            }

            foreach(uint offset in info.Offsets) {
                if(offset >= ushort.MaxValue << 9 || offset < info.Interval * (info.NumBeeps - 1)) {
                    return TimerError.InvalidOffset;
                }
            }

            return TimerError.NoError;
        }

        private void RadioButton_Click(object sender, EventArgs e) {
            FlowTimer.FixedOffset.SelectTimer(this);
        }

        private void DataChanged(object sender, EventArgs e) {
            FlowTimer.FixedOffset.SelectTimer(this);
        }

        private void RemoveButton_Click(object sender, EventArgs e) {
            FlowTimer.FixedOffset.RemoveTimer(this);
        }
    }
}

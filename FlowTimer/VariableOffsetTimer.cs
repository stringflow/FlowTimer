using System;
using System.Windows.Forms;

namespace FlowTimer {

    public struct VariableInfo {

        public uint Frame;
        public double FPS;
        public int Offset;
        public uint Interval;
        public uint NumBeeps;
    }

    public class VariableOffsetTimer : BaseTimer {

        public TextBox TextBoxFrame;
        public ComboBox ComboBoxFPS;
        public TextBox TextBoxOffset;
        public TextBox TextBoxInterval;
        public TextBox TextBoxBeeps;
        public Button ButtonSubmit;

        public VariableInfo Info;
        public bool Submitted;
        public double CurrentOffset;

        public VariableOffsetTimer(TabPage tab, params Control[] copyControls) : base(tab, null, copyControls) {
            TextBoxFrame = FlowTimer.MainForm.TextBoxFrame;
            ComboBoxFPS = FlowTimer.MainForm.ComboBoxFPS;
            TextBoxOffset = FlowTimer.MainForm.TextBoxOffset;
            TextBoxInterval = FlowTimer.MainForm.TextBoxInterval;
            TextBoxBeeps = FlowTimer.MainForm.TextBoxBeeps;
            ButtonSubmit = FlowTimer.MainForm.ButtonSubmit;

            TimerCallback = TimerCallbackFn; // c# is silly!!
            TextBoxFrame.KeyDown += (sender, e) => { if(e.KeyCode == Keys.Enter && FlowTimer.MainForm.ButtonSubmit.Enabled) { Submit(); e.SuppressKeyPress = true; } };
            ComboBoxFPS.SelectedIndexChanged += (sender, e) => FlowTimer.Settings.VariableFPS = FlowTimer.MainForm.ComboBoxFPS.SelectedItem as string;
            TextBoxOffset.TextChanged += (sender, e) => FlowTimer.Settings.VariableOffset = FlowTimer.MainForm.TextBoxOffset.Text;
            TextBoxInterval.TextChanged += (sender, e) => FlowTimer.Settings.VariableInterval = FlowTimer.MainForm.TextBoxInterval.Text;
            TextBoxBeeps.TextChanged += (sender, e) => FlowTimer.Settings.VariableNumBeeps = FlowTimer.MainForm.TextBoxBeeps.Text;
        }

        public override void OnInit() {
            ComboBoxFPS.SelectedItem = FlowTimer.Settings.VariableFPS;
            TextBoxOffset.Text = FlowTimer.Settings.VariableOffset;
            TextBoxInterval.Text = FlowTimer.Settings.VariableInterval;
            TextBoxBeeps.Text = FlowTimer.Settings.VariableNumBeeps;
        }

        public override void OnLoad() {
            base.OnLoad();
            FlowTimer.ResizeForm(FlowTimer.MainForm.Width, 211);
            FlowTimer.MainForm.ButtonStart.Enabled = true;
            FlowTimer.MainForm.ButtonStop.Enabled = true;
            FlowTimer.MainForm.LabelTimer.Text = 0.0.ToFormattedString();
            OnTimerStop();
        }

        public override void OnTimerStart() {
            Submitted = false;
            TextBoxFrame.Enabled = true;
            TextBoxFrame.Focus();
        }

        public override void OnTimerStop() {
            Submitted = false;
            FlowTimer.MainForm.LabelTimer.Text = 0.0.ToFormattedString();
            TextBoxFrame.Text = "";
            TextBoxFrame.Enabled = false;
            EnableControls(true);
            CurrentOffset = double.MaxValue;
        }

        public override void OnKeyEvent(Keys key) {
        }

        public override void OnBeepSoundChange() {
        }

        public double TimerCallbackFn(long start) {
            OnDataChange();
            double ret = Math.Min(Math.Max((DateTime.Now.Ticks - start) / 10000000.0, 0.001), CurrentOffset);
            if(ret == CurrentOffset) ret = 0.0;
            return ret;
        }

        public void OnDataChange() {
            TimerError error = GetVariableInfo(out Info);
            double currentTime = double.Parse(FlowTimer.MainForm.LabelTimer.Text);
            FlowTimer.MainForm.ButtonSubmit.Enabled = error == TimerError.NoError && !Submitted && FlowTimer.IsTimerRunning && Info.Frame / Info.FPS + Info.Offset / 1000.0f >= currentTime + (Info.Interval * (Info.NumBeeps - 1) / 1000.0f);
        }

        public void Submit() {
            GetVariableInfo(out Info);
            long now = DateTime.Now.Ticks;
            double offset = (Info.Frame / Info.FPS * 1000.0f) - ((now - FlowTimer.TimerStart) / 10000.0) + Info.Offset;
            FlowTimer.UpdatePCM(new double[] { offset }, Info.Interval, Info.NumBeeps, false);
            FlowTimer.AudioContext.QueueAudio(FlowTimer.PCM);
            ButtonSubmit.Enabled = false;
            CurrentOffset = Info.Frame / Info.FPS + Info.Offset / 1000.0f;
            Submitted = true;
            EnableControls(false);
            FlowTimer.MainForm.TextBoxFrame.Enabled = false;
        }

        public void EnableControls(bool enabled) {
            ComboBoxFPS.Enabled = enabled;
            TextBoxOffset.Enabled = enabled;
            TextBoxInterval.Enabled = enabled;
            TextBoxBeeps.Enabled = enabled;
        }

        public TimerError GetVariableInfo(out VariableInfo info) {
            info = new VariableInfo();

            if(!uint.TryParse(FlowTimer.MainForm.TextBoxFrame.Text, out info.Frame)) {
                return TimerError.InvalidFrame;
            }

            if(!double.TryParse(FlowTimer.MainForm.ComboBoxFPS.SelectedItem as string, out info.FPS)) {
                return TimerError.InvalidFPS;
            }

            if(!int.TryParse(FlowTimer.MainForm.TextBoxOffset.Text, out info.Offset)) {
                return TimerError.InvalidOffset;
            }

            if(!uint.TryParse(FlowTimer.MainForm.TextBoxInterval.Text, out info.Interval)) {
                return TimerError.InvalidInterval;
            }

            if(!uint.TryParse(FlowTimer.MainForm.TextBoxBeeps.Text, out info.NumBeeps)) {
                return TimerError.InvalidNumBeeps;
            }

            if(info.Interval >= ushort.MaxValue << 9) {
                return TimerError.InvalidInterval;
            }

            if(info.NumBeeps >= ushort.MaxValue << 9) {
                return TimerError.InvalidNumBeeps;
            }

            if(info.Frame >= ushort.MaxValue << 8) {
                return TimerError.InvalidFrame;
            }

            return TimerError.NoError;
        }
    }
}

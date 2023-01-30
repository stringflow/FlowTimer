using System;
using System.Windows.Forms;
using System.Threading;

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

        public double Adjusted;

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
            CurrentOffset = double.MaxValue;
            Submitted = false;
            TextBoxFrame.Enabled = true;
            TextBoxFrame.Focus();
            Adjusted = 0;
        }

        public override void OnVisualTimerStart() {
        }

        public override void OnTimerStop() {
            Submitted = false;
            TextBoxFrame.Enabled = false;
            TextBoxFrame.Text = "";
            EnableControls(true);
            FlowTimer.MainForm.LabelTimer.Text = 0.0.ToFormattedString();
            FlowTimer.MainForm.LabelTimer.Focus();
        }

        public override void OnKeyEvent(Keys key) {
            if(FlowTimer.Settings.AddFrame.IsPressed(key) && FlowTimer.MainForm.ButtonPlus.Enabled) {
                ChangeAudio(1);
            } else if(FlowTimer.Settings.SubFrame.IsPressed(key) && FlowTimer.MainForm.ButtonMinus.Enabled) {
                ChangeAudio(-1);
            } else if(FlowTimer.Settings.Undo.IsPressed(key) && FlowTimer.MainForm.ButtonUndo.Enabled) {
                Undo();
            }
        }

        public override void OnBeepSoundChange() {
        }

        public override void OnBeepVolumeChange() {
        }

        public double TimerCallbackFn(double start) {
            OnDataChange();
            double ret = Math.Min(Math.Max((Win32.GetTime() - start) / 1000.0, 0.001), CurrentOffset);
            if(ret == CurrentOffset) ret = 0.0;
            return ret;
        }

        public void OnDataChange() {
            TimerError error = GetVariableInfo(out Info);
            double currentTime = error == TimerError.NoError ? double.Parse(FlowTimer.MainForm.LabelTimer.Text) : 0;
            FlowTimer.MainForm.ButtonSubmit.Enabled = error == TimerError.NoError && !Submitted && FlowTimer.IsTimerRunning && Info.Frame / Info.FPS + Info.Offset / 1000.0f >= currentTime + (Info.Interval * (Info.NumBeeps - 1) / 1000.0f);
            FlowTimer.MainForm.ButtonUndo.Enabled = Submitted && FlowTimer.IsTimerRunning;

            bool canAdjust = Submitted && currentTime < CurrentOffset - Info.Interval * (Info.NumBeeps - 1) / 1000.0f - 0.05;
            FlowTimer.MainForm.ButtonPlus.Enabled = canAdjust;
            FlowTimer.MainForm.ButtonMinus.Enabled = canAdjust;
        }

        public void Submit() {
            GetVariableInfo(out Info);
            double now = Win32.GetTime();
            double offset = (Info.Frame / Info.FPS * 1000.0f) - (now - FlowTimer.TimerStart) + Info.Offset + Adjusted;
            FlowTimer.UpdatePCM(new double[] { offset }, Info.Interval, Info.NumBeeps, false);
            FlowTimer.AudioContext.QueueAudio(FlowTimer.PCM);
            ButtonSubmit.Enabled = false;
            CurrentOffset = Info.Frame / Info.FPS + (Info.Offset + Adjusted) / 1000.0f;
            Submitted = true;
            EnableControls(false);
            FlowTimer.MainForm.TextBoxFrame.Enabled = false;
        }

        public void Undo() {
            Submitted = false;
            EnableControls(true);
            CurrentOffset = double.MaxValue;
            Adjusted = 0;
            FlowTimer.AudioContext.ClearQueuedAudio();

            new Thread(() => {
                Thread.Sleep(50);
                MethodInvoker inv = delegate {
                    FlowTimer.MainForm.TextBoxFrame.Text = "";
                    FlowTimer.MainForm.TextBoxFrame.Enabled = true;
                    FlowTimer.MainForm.TextBoxFrame.Focus();
                };
                FlowTimer.MainForm.Invoke(inv);
            }).Start();
        }

        public void ChangeAudio(int numFrames) {
            FlowTimer.AudioContext.ClearQueuedAudio();
            double amount = numFrames * 1000.0 / Info.FPS;
            Adjusted += amount;
            Submit();

            int numFramesAdjusted = (int) Math.Round(Adjusted / 1000.0 * Info.FPS);
            GetVariableInfo(out VariableInfo info);
            TextBoxFrame.Text = info.Frame.ToString();
            if(numFramesAdjusted != 0) {
                TextBoxFrame.Text += numFramesAdjusted.ToString("+#;-#");
            }
        }

        public void EnableControls(bool enabled) {
            ComboBoxFPS.Enabled = enabled;
            TextBoxOffset.Enabled = enabled;
            TextBoxInterval.Enabled = enabled;
            TextBoxBeeps.Enabled = enabled;
        }

        public TimerError GetVariableInfo(out VariableInfo info) {
            info = new VariableInfo();

            string frameText = FlowTimer.MainForm.TextBoxFrame.Text;
            if(frameText.Contains("+") || frameText.Contains("-")) frameText = frameText.Substring(0, Math.Max(frameText.IndexOf("+"), frameText.IndexOf("-")));

            if(!uint.TryParse(frameText, out info.Frame)) {
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

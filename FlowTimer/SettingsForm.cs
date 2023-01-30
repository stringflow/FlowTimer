using System;
using System.Windows.Forms;
using System.IO;

namespace FlowTimer {

    public partial class SettingsForm : Form {

        public static bool UpdateFound;

        public SettingsForm() {
            InitializeComponent();

            FlowTimer.Settings.Start.SetControls(ButtonStartPrimary, ButtonStartSecondary, ButtonStartClear, CheckBoxStartGlobal);
            FlowTimer.Settings.Stop.SetControls(ButtonStopPrimary, ButtonStopSecondary, ButtonStopClear, CheckBoxStopGlobal);
            FlowTimer.Settings.Play.SetControls(ButtonPlayPrimary, ButtonPlaySecondary, ButtonPlayClear, CheckBoxPlayGlobal);
            FlowTimer.Settings.Undo.SetControls(ButtonUndoPrimary, ButtonUndoSecondary, ButtonUndoClear, CheckBoxUndoGlobal);
            FlowTimer.Settings.Up.SetControls(ButtonUpPrimary, ButtonUpSecondary, ButtonUpClear, CheckBoxUpGlobal);
            FlowTimer.Settings.Down.SetControls(ButtonDownPrimary, ButtonDownSecondary, ButtonDownClear, CheckBoxDownGlobal);
            FlowTimer.Settings.AddFrame.SetControls(ButtonAddFramePrimary, ButtonAddFrameSecondary, ButtonAddFrameClear, CheckBoxAddFrameGlobal);
            FlowTimer.Settings.SubFrame.SetControls(ButtonSubFramePrimary, ButtonSubFrameSecondary, ButtonSubFrameClear, CheckBoxSubFrameGlobal);
            FlowTimer.Settings.Add2.SetControls(ButtonAdd2Primary, ButtonAdd2Secondary, ButtonAdd2Clear, CheckBoxAdd2Global);
            FlowTimer.Settings.Sub2.SetControls(ButtonSub2Primary, ButtonSub2Secondary, ButtonSub2Clear, CheckBoxSub2Global);
            FlowTimer.Settings.Add3.SetControls(ButtonAdd3Primary, ButtonAdd3Secondary, ButtonAdd3Clear, CheckBoxAdd3Global);
            FlowTimer.Settings.Sub3.SetControls(ButtonSub3Primary, ButtonSub3Secondary, ButtonSub3Clear, CheckBoxSub3Global);
            FlowTimer.Settings.Add4.SetControls(ButtonAdd4Primary, ButtonAdd4Secondary, ButtonAdd4Clear, CheckBoxAdd4Global);
            FlowTimer.Settings.Sub4.SetControls(ButtonSub4Primary, ButtonSub4Secondary, ButtonSub4Clear, CheckBoxSub4Global);
            FlowTimer.Settings.Add5.SetControls(ButtonAdd5Primary, ButtonAdd5Secondary, ButtonAdd5Clear, CheckBoxAdd5Global);
            FlowTimer.Settings.Sub5.SetControls(ButtonSub5Primary, ButtonSub5Secondary, ButtonSub5Clear, CheckBoxSub5Global);

            foreach(string file in Directory.GetFiles(FlowTimer.Beeps, "*.wav")) {
                ComboBoxBeep.Items.Add(Path.GetFileNameWithoutExtension(file));
            }
            ComboBoxBeep.SelectedItem = FlowTimer.Settings.Beep;

            foreach(KeyMethod keymethod in Enum.GetValues(typeof(KeyMethod))) {
                ComboBoxKey.Items.Add(keymethod.ToFormattedString());
            }
            ComboBoxKey.SelectedIndex = (int) FlowTimer.Settings.KeyMethod;

            TrackBarVolume.Value = FlowTimer.Settings.Volume;
            TextBoxVolume.Text = FlowTimer.Settings.Volume.ToString();

            ComboBoxBeep.SelectedIndexChanged += ComboBoxBeep_SelectedIndexChanged;
            TrackBarVolume.ValueChanged += TrackBarVolume_ValueChanged;
            TrackBarVolume.MouseUp += TrackBarVolume_MouseUp;
            TextBoxVolume.KeyPress += TextBoxVolume_KeyPress;
            TextBoxVolume.TextChanged += TextBoxVolume_TextChanged;
            ComboBoxKey.SelectedIndexChanged += ComboBoxKey_SelectedIndexChanged;
        }

        private void ComboBoxBeep_SelectedIndexChanged(object sender, EventArgs e) {
            FlowTimer.ChangeBeepSound(ComboBoxBeep.SelectedItem as string);
        }

        private void ComboBoxKey_SelectedIndexChanged(object sender, EventArgs e) {
            FlowTimer.ChangeKeyMethod((KeyMethod) ComboBoxKey.SelectedIndex);
        }

        private void ButtonImportBeep_Click(object sender, EventArgs e) {
            FlowTimer.OpenImportBeepSoundDialog();
        }

        private void TrackBarVolume_ValueChanged(object sender, EventArgs e) {
            if((Win32.GetKeyState(Keys.LButton) & 0x80) == 0) {
                if(TextBoxVolume.Text != "") TextBoxVolume.Text = TrackBarVolume.Value.ToString();
                FlowTimer.AdjustBeepSoundVolume(TrackBarVolume.Value);
                FlowTimer.AudioContext.QueueAudio(FlowTimer.BeepSound);
                FlowTimer.Settings.Volume = TrackBarVolume.Value;
            }
        }

        private void TrackBarVolume_MouseUp(object sender, MouseEventArgs e) {
            TrackBarVolume_ValueChanged(sender, e);
        }

        private void TextBoxVolume_KeyPress(object sender, KeyPressEventArgs e) {
            if(!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar)) {
                e.Handled = true;
            }

            if(e.KeyChar != 0x8 && TextBoxVolume.Text.Length >= 3) {
                e.Handled = true;
            }
        }

        private void TextBoxVolume_TextChanged(object sender, EventArgs e) {
            int newValue;
            if(TextBoxVolume.Text == "") {
                newValue = 0;
            } else {
                newValue = Convert.ToInt32(TextBoxVolume.Text);
                newValue = Math.Min(newValue, 100);
                newValue = Math.Max(newValue, 0);
                int cursorPosition = TextBoxVolume.SelectionStart;
                TextBoxVolume.Text = newValue.ToString();
                TextBoxVolume.SelectionStart = cursorPosition;
            }

            TrackBarVolume.Value = newValue;
        }
    }
}

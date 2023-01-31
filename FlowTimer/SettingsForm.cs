using System;
using System.Windows.Forms;
using System.IO;

namespace FlowTimer {

    public partial class SettingsForm : Form {

        public static bool UpdateFound;
        private int SizeKeybinds = 0;

        public SettingsForm() {
            InitializeComponent();

            int tab = FlowTimer.MainForm.TabControl.SelectedIndex + 1;

            AddKeybindSetting("Start", FlowTimer.Settings.Start);
            AddKeybindSetting("Stop", FlowTimer.Settings.Stop);
            if(tab == 3) AddKeybindSetting("Play", FlowTimer.Settings.Play);
            if(tab == 2 || tab == 3) AddKeybindSetting("Undo", FlowTimer.Settings.Undo);
            if(tab == 1 || tab == 3) AddKeybindSetting("Up", FlowTimer.Settings.Up);
            if(tab == 1 || tab == 3) AddKeybindSetting("Down", FlowTimer.Settings.Down);
            if(tab == 2) AddKeybindSetting("+Frame", FlowTimer.Settings.AddFrame);
            if(tab == 2) AddKeybindSetting("-Frame", FlowTimer.Settings.SubFrame);
            if(tab == 3) AddKeybindSetting("+  (1)", FlowTimer.Settings.AddFrame);
            if(tab == 3) AddKeybindSetting("-   (1)", FlowTimer.Settings.SubFrame);
            if(tab == 3) AddKeybindSetting("+  (2)", FlowTimer.Settings.Add2);
            if(tab == 3) AddKeybindSetting("-   (2)", FlowTimer.Settings.Sub2);
            if(tab == 3) AddKeybindSetting("+  (3)", FlowTimer.Settings.Add3);
            if(tab == 3) AddKeybindSetting("-   (3)", FlowTimer.Settings.Sub3);
            if(tab == 3) AddKeybindSetting("+  (4)", FlowTimer.Settings.Add4);
            if(tab == 3) AddKeybindSetting("-   (4)", FlowTimer.Settings.Sub4);
            if(tab == 3) AddKeybindSetting("+  (5)", FlowTimer.Settings.Add5);
            if(tab == 3) AddKeybindSetting("-   (5)", FlowTimer.Settings.Sub5);
            if(tab == 3) AddKeybindSetting("+  (6)", FlowTimer.Settings.Add6);
            if(tab == 3) AddKeybindSetting("-   (6)", FlowTimer.Settings.Sub6);

            LabelBeep.Top = SizeKeybinds + 14;
            ComboBoxBeep.Top = SizeKeybinds + 10;
            ButtonImportBeep.Top = SizeKeybinds + 9;
            LabelVolume.Top = SizeKeybinds + 38;
            TrackBarVolume.Top = SizeKeybinds + 37;
            TextBoxVolume.Top = SizeKeybinds + 36;
            LabelKey.Top = SizeKeybinds + 65;
            ComboBoxKey.Top = SizeKeybinds + 62;

            Size = new System.Drawing.Size {
                Width = Size.Width,
                Height = SizeKeybinds + 128
            };

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

        private void AddKeybindSetting(string name, Hotkey hotkey) {

            Label Label = new Label {
                AutoSize = true,
                Location = new System.Drawing.Point(5, SizeKeybinds + 12),
                Size = new System.Drawing.Size(32, 13),
                TabIndex = 0,
                Text = name + ":"
            };
            Controls.Add(Label);

            Button ButtonPrimary = new Button {
                Location = new System.Drawing.Point(51, SizeKeybinds + 7),
                Size = new System.Drawing.Size(75, 23),
                TabIndex = 1,
                Text = "Unset",
                UseVisualStyleBackColor = true
            };
            Controls.Add(ButtonPrimary);

            Button ButtonSecondary = new Button {
                Location = new System.Drawing.Point(129, SizeKeybinds + 7),
                Size = new System.Drawing.Size(75, 23),
                TabIndex = 2,
                Text = "Unset",
                UseVisualStyleBackColor = true
            };
            Controls.Add(ButtonSecondary);

            Button ButtonClear = new Button {
                Location = new System.Drawing.Point(207, SizeKeybinds + 7),
                Size = new System.Drawing.Size(75, 23),
                TabIndex = 3,
                Text = "Clear",
                UseVisualStyleBackColor = true
            };
            Controls.Add(ButtonClear);

            CheckBox CheckBoxGlobal = new CheckBox {
                AutoSize = true,
                Location = new System.Drawing.Point(289, SizeKeybinds + 11),
                Size = new System.Drawing.Size(56, 17),
                TabIndex = 4,
                Text = "Global",
                UseVisualStyleBackColor = true
            };
            Controls.Add(CheckBoxGlobal);
            SizeKeybinds += 26;

            hotkey.SetControls(ButtonPrimary, ButtonSecondary, ButtonClear, CheckBoxGlobal);
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

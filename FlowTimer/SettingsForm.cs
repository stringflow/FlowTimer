using System;
using System.Windows.Forms;
using System.IO;

namespace FlowTimer {

    public partial class SettingsForm : Form {

        public SettingsForm() {
            InitializeComponent();

            FlowTimer.Settings.Start.SetControls(ButtonStartPrimary, ButtonStartSecondary, ButtonStartClear, CheckBoxStartGlobal);
            FlowTimer.Settings.Stop.SetControls(ButtonStopPrimary, ButtonStopSecondary, ButtonStopClear, CheckBoxStopGlobal);
            FlowTimer.Settings.Up.SetControls(ButtonUpPrimary, ButtonUpSecondary, ButtonUpClear, CheckBoxUpGlobal);
            FlowTimer.Settings.Down.SetControls(ButtonDownPrimary, ButtonDownSecondary, ButtonDownClear, CheckBoxDownGlobal);

            foreach(string file in Directory.GetFiles(FlowTimer.Beeps, "*.wav")) {
                ComboBoxBeep.Items.Add(Path.GetFileNameWithoutExtension(file));
            }
            ComboBoxBeep.SelectedItem = FlowTimer.Settings.Beep;
            ComboBoxBeep.SelectedIndexChanged += ComboBoxBeep_SelectedIndexChanged;

            foreach(KeyMethod keymethod in Enum.GetValues(typeof(KeyMethod))) {
                ComboBoxKey.Items.Add(keymethod.ToFormattedString());
            }
            ComboBoxKey.SelectedIndex = (int) FlowTimer.Settings.KeyMethod;
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

        private void ButtonCheckForUpdates_Click(object sender, EventArgs e) {
            FlowTimer.CheckForUpdates();
        }
    }        
}

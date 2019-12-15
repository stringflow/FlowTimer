using System;
using System.Windows.Forms;

namespace FlowTimer {

    public unsafe partial class MainForm : Form {

        public MainForm() {
            InitializeComponent();
            FlowTimer.SetMainForm(this);
            FlowTimer.Init();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e) {
            FlowTimer.Destroy();
        }

        private void ButtonAdd_Click(object sender, EventArgs e) {
            FlowTimer.AddTimer();
        }

        private void ButtonStart_Click(object sender, EventArgs e) {
            FlowTimer.StartTimer();
        }

        private void ButtonStop_Click(object sender, EventArgs e) {
            FlowTimer.StopTimer(false);
        }

        private void ButtonSettings_Click(object sender, EventArgs e) {
            FlowTimer.OpenSettingsForm();
        }

        private void ButtonLoadTimers_Click(object sender, EventArgs e) {
            FlowTimer.OpenLoadTimersDialog();
        }

        private void ButtonSaveTimers_Click(object sender, EventArgs e) {
            FlowTimer.OpenSaveTimersDialog();
        }

        private void PictureBoxPin_Click(object sender, EventArgs e) {
            FlowTimer.TogglePin();
        }
    }
}

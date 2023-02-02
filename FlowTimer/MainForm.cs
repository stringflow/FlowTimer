using System;
using System.Windows.Forms;

namespace FlowTimer {

    public unsafe partial class MainForm : Form {

        public MainForm() {
            InitializeComponent();
            FlowTimer.SetMainForm(this);
            FlowTimer.RegisterTabs(TabPageFixedOffset, TabPageVariableOffset, TabPageIGTTracking);
            FlowTimer.Init();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e) {
            FlowTimer.Destroy();
        }

        private void ButtonAdd_Click(object sender, EventArgs e) {
            FlowTimer.FixedOffset.AddTimer();
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
            FlowTimer.FixedOffset.OpenLoadTimersDialog();
        }

        private void ButtonSaveTimers_Click(object sender, EventArgs e) {
            FlowTimer.FixedOffset.OpenSaveTimersDialog();
        }

        private void PictureBoxPin_Click(object sender, EventArgs e) {
            FlowTimer.TogglePin();
        }

        private void ButtonSubmit_Click(object sender, EventArgs e) {
            FlowTimer.VariableOffset.Submit();
        }

        private void VariableTimer_DataChange(object sender, EventArgs e) {
            FlowTimer.VariableOffset.OnDataChange();
        }

        private void ButtonPlus_Click(object sender, EventArgs e) {
            FlowTimer.VariableOffset.ChangeAudio(1);
        }

        private void ButtonMinus_Click(object sender, EventArgs e) {
            FlowTimer.VariableOffset.ChangeAudio(-1);
        }

        private void ButtonUndo_Click(object sender, EventArgs e) {
            FlowTimer.VariableOffset.Undo();
        }

        private void ButtonPlay_Click(object sender, EventArgs e) {
            FlowTimer.IGTTracking.Play();
        }

        private void ComboBoxFPS3_DataChange(object sender, EventArgs e) {
            FlowTimer.IGTTracking.OnDataChange();
        }

        private void ButtonUndoPlay_Click(object sender, EventArgs e) {
            FlowTimer.IGTTracking.Undo();
        }

        private void ButtonAdd3_Click(object sender, EventArgs e) {
            FlowTimer.IGTTracking.AddTimer();
        }

        private void ButtonLoadTimers3_Click(object sender, EventArgs e) {
            FlowTimer.IGTTracking.OpenLoadTimersDialog();
        }

        private void ButtonSaveTimers3_Click(object sender, EventArgs e) {
            FlowTimer.IGTTracking.OpenSaveTimersDialog();
        }
    }
}

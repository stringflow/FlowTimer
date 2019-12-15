using System.Windows.Forms;
using static FlowTimer.Win32;

namespace FlowTimer {

    public partial class HotkeySelection : Form {

        public Keys Key;

        public HotkeySelection() {
            InitializeComponent();
            TopMost = FlowTimer.Settings.Pinned;
            KeyDown += Form_KeyDown;
        }

        private void Form_KeyDown(object sender, KeyEventArgs e) {
            Key = (Keys) e.KeyValue;

            if(Key == Keys.Shift || Key == Keys.ShiftKey) {
                Key = GetAsyncKey(Keys.LShiftKey, Keys.RShiftKey);
            } else if(Key == Keys.Control || Key == Keys.ControlKey) {
                Key = GetAsyncKey(Keys.LControlKey, Keys.RControlKey);
            } else if(Key == Keys.Menu) {
                Key = GetAsyncKey(Keys.LMenu, Keys.RMenu);
            }

            DialogResult = DialogResult.OK;
        }
    }
}
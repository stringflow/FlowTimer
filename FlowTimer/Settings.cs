using System;
using Newtonsoft.Json;
using System.Windows.Forms;

namespace FlowTimer {

    public class Settings {

        public Hotkey Start = new Hotkey(Keys.None, Keys.None, false);
        public Hotkey Stop  = new Hotkey(Keys.None, Keys.None, false);
        public Hotkey Up    = new Hotkey(Keys.None, Keys.None, false);
        public Hotkey Down  = new Hotkey(Keys.None, Keys.None, false);
        public KeyMethod KeyMethod = KeyMethod.OnPress;
        public string Beep = "ping1";
        public bool Pinned = false;
        public string LastLoadedTimers = null;
    }

    public class Hotkey {

        public Input Primary;
        public Input Secondary;

        [JsonIgnore]
        private Button _ClearButton;
        [JsonIgnore]
        public Button ClearButton {
            get { return _ClearButton; }
            set {
                _ClearButton = value;
                _ClearButton.Click += ClearButton_Click;
            }
        }

        [JsonIgnore]
        private CheckBox _GlobalCheckBox;
        [JsonIgnore]
        public CheckBox GlobalCheckBox {
            get { return _GlobalCheckBox; }
            set {
                _GlobalCheckBox = value;
                _GlobalCheckBox.Checked = Global;
                _GlobalCheckBox.CheckedChanged += GlobalCheckBox_CheckChanged;
            }
        }

        public bool Global;

        // Json Constructor
        public Hotkey() { }

        public Hotkey(Keys primary, Keys secondary, bool global) {
            Primary = new Input() {
                Key = primary,
            };
            Secondary = new Input() {
                Key = secondary,
            };
            Global = global;
        }

        public void SetControls(Button primaryButton, Button secondaryButton, Button clearButton, CheckBox globalCheckBox) {
            Primary.Button = primaryButton;
            Secondary.Button = secondaryButton;
            ClearButton = clearButton;
            GlobalCheckBox = globalCheckBox;
        }

        public bool IsPressed(Keys key) {
            return (Primary.Key == key || Secondary.Key == key) && (Form.ActiveForm == FlowTimer.MainForm || Global);
        }

        private void GlobalCheckBox_CheckChanged(object sender, EventArgs args) {
            Global = _GlobalCheckBox.Checked;
        }

        private void ClearButton_Click(object sender, EventArgs args) {
            (Secondary.Key != Keys.None ? Secondary : Primary).Key = Keys.None;
        }
    }

    public class Input {

        [JsonIgnore]
        private Button _Button;
        [JsonIgnore]
        public Button Button {
            get { return _Button; }
            set {
                _Button = value;
                _Button.Click += Button_Click;
                UpdateButtonText();
            }
        }

        [JsonIgnore]
        private Keys _Key;

        public Keys Key {
            get { return _Key; }
            set {
                _Key = value;
                UpdateButtonText();
            }
        }

        private void UpdateButtonText() {
            if(_Button != null) {
                _Button.Text = _Key.ToFormattedString();
            }
        }

        private void Button_Click(object sender, EventArgs args) {
            HotkeySelection selection = new HotkeySelection();
            if(selection.ShowDialog() == DialogResult.OK) {
                Key = selection.Key;
            }
        }
    }

    public enum KeyMethod {

        OnPress,
        OnRelease,
    }
}

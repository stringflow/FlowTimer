using System;
using System.Windows.Forms;
using System.Drawing;
using Newtonsoft.Json;

namespace FlowTimer {

    public class Settings {

        public Hotkey Start = new Hotkey(Keys.None, Keys.None, false);
        public Hotkey Stop  = new Hotkey(Keys.None, Keys.None, false);
        public Hotkey Play  = new Hotkey(Keys.None, Keys.None, false);
        public Hotkey Undo  = new Hotkey(Keys.None, Keys.None, false);
        public Hotkey Up    = new Hotkey(Keys.None, Keys.None, false);
        public Hotkey Down  = new Hotkey(Keys.None, Keys.None, false);
        public Hotkey AddFrame = new Hotkey(Keys.None, Keys.None, false);
        public Hotkey SubFrame = new Hotkey(Keys.None, Keys.None, false);
        public Hotkey Add2 = new Hotkey(Keys.None, Keys.None, false);
        public Hotkey Sub2 = new Hotkey(Keys.None, Keys.None, false);
        public Hotkey Add3 = new Hotkey(Keys.None, Keys.None, false);
        public Hotkey Sub3 = new Hotkey(Keys.None, Keys.None, false);
        public Hotkey Add4 = new Hotkey(Keys.None, Keys.None, false);
        public Hotkey Sub4 = new Hotkey(Keys.None, Keys.None, false);
        public Hotkey Add5 = new Hotkey(Keys.None, Keys.None, false);
        public Hotkey Sub5 = new Hotkey(Keys.None, Keys.None, false);
        public Hotkey Add6 = new Hotkey(Keys.None, Keys.None, false);
        public Hotkey Sub6 = new Hotkey(Keys.None, Keys.None, false);
        public KeyMethod KeyMethod = KeyMethod.OnPress;
        public string Beep = "ping1";
        public bool Pinned = false;
        public string LastLoadedTimers = null;
        public string LastLoadedIGTTimers = null;
        public bool AutoUpdate = false;
        public int Volume = 100;

        [JsonIgnore]
        private CheckBox _CheckBoxAutoUpdate;
        [JsonIgnore]
        public CheckBox CheckBoxAutoUpdate {
            get { return _CheckBoxAutoUpdate; }
            set {
                _CheckBoxAutoUpdate = value;
                _CheckBoxAutoUpdate.Checked = AutoUpdate;
                _CheckBoxAutoUpdate.Click += CheckBoxAutoUpdate_CheckChanged;
            }
        }

        private void CheckBoxAutoUpdate_CheckChanged(object sender, EventArgs args) {
            AutoUpdate = _CheckBoxAutoUpdate.Checked;
        }

        public string VariableFPS = "59.7275";
        public string VariableOffset = "0";
        public string VariableInterval = "500";
        public string VariableNumBeeps = "5";
        public string IGTFPS = "59.7275";
        public string IGTGame = null;
    }

    public class Hotkey {

        public Input Primary;
        public Input Secondary;

        [JsonIgnore]
        private Button _ButtonClear;
        [JsonIgnore]
        public Button ButtonClear {
            get { return _ButtonClear; }
            set {
                _ButtonClear = value;
                _ButtonClear.Click += ButtonClear_Click;
            }
        }

        [JsonIgnore]
        private CheckBox _CheckBoxGlobal;
        [JsonIgnore]
        public CheckBox CheckBoxGlobal {
            get { return _CheckBoxGlobal; }
            set {
                _CheckBoxGlobal = value;
                _CheckBoxGlobal.Checked = Global;
                _CheckBoxGlobal.CheckedChanged += CheckBoxGlobal_CheckChanged;
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
            ButtonClear = clearButton;
            CheckBoxGlobal = globalCheckBox;
        }

        public bool IsPressed(Keys key) {
            return (Primary.Key == key || Secondary.Key == key) && (Form.ActiveForm == FlowTimer.MainForm || Global);
        }

        private void CheckBoxGlobal_CheckChanged(object sender, EventArgs args) {
            Global = _CheckBoxGlobal.Checked;
        }

        private void ButtonClear_Click(object sender, EventArgs args) {
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
                _Button.ForeColor = _Key == Keys.None ? Color.Gray : SystemColors.ControlText;
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

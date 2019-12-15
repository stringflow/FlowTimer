using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace FlowTimer {

    public enum TimerError {

        NoError,
        InvalidOffset,
        InvalidInterval,
        InvalidNumBeeps,
    }

    public class TimerInfo {

        public uint[] Offsets;
        public uint MaxOffset;
        public uint Interval;
        public uint NumBeeps;
    }

    public class Timer {

        public const int X = 165;
        public const int Y = 31;
        public const int Size = 28;

        public TextBox TextBoxName;
        public TextBox TextBoxOffset;
        public TextBox TextBoxInterval;
        public TextBox TextBoxNumBeeps;
        public RadioButton RadioButton;
        public Button RemoveButton;
        public List<Control> Controls;

        public List<TextBox> TextBoxes {
            get { return Controls.FindAll(control => control.GetType() == typeof(TextBox)).ConvertAll(control => (TextBox) control); }
        }

        public int Index {
            get {
                return (TextBoxes[0].Location.Y - Y) / Size;
            }
            set {
                int yPosition = Y + value * Size;
                int xOffset = 0;

                for(int i = 0; i < TextBoxes.Count; i++) {
                    TextBoxes[i].SetBounds(X + xOffset, yPosition, 65, 21);
                    xOffset += TextBoxes[i].Width + 5;
                }

                RadioButton.SetBounds(X - 21, yPosition + 4, 14, 13);

                //only reposition remove button if not index = 0
                if(value > 0) {
                    Rectangle lastbox = TextBoxes.Last().Bounds;
                    RemoveButton.SetBounds(lastbox.X + lastbox.Width + 5, lastbox.Y, 38, 21);
                }
            }
        }

        public Timer(int index, string name = "Timer", string offset = "5000", string interval = "500", string numBeeps = "5") {
            Controls = new List<Control>();

            TextBoxName = new TextBox();
            TextBoxName.Text = name;
            Controls.Add(TextBoxName);

            TextBoxOffset = new TextBox();
            TextBoxOffset.Text = offset;
            Controls.Add(TextBoxOffset);

            TextBoxInterval = new TextBox();
            TextBoxInterval.Text = interval;
            Controls.Add(TextBoxInterval);

            TextBoxNumBeeps = new TextBox();
            TextBoxNumBeeps.Text = numBeeps;
            Controls.Add(TextBoxNumBeeps);

            foreach(TextBox textbox in TextBoxes) {
                textbox.Font = new Font(textbox.Font.FontFamily, 9.0f);
                textbox.TextChanged += DataChanged;
                textbox.TabStop = false;
            }

            RadioButton = new RadioButton();
            RadioButton.Click += RadioButton_Click;
            Controls.Add(RadioButton);

            if(index > 0) {
                RemoveButton = new Button();
                RemoveButton.Text = "-";
                RemoveButton.Click += RemoveButton_Click;
                RemoveButton.TabStop = false;
                Controls.Add(RemoveButton);
            }

            Index = index;
        }

        public TimerError GetTimerInfo(out TimerInfo info) {
            info = new TimerInfo();

            if(!uint.TryParse(TextBoxInterval.Text, out info.Interval)) {
                return TimerError.InvalidInterval;
            }
            
            if(!uint.TryParse(TextBoxNumBeeps.Text, out info.NumBeeps)) {
                return TimerError.InvalidNumBeeps;
            }

            string[] offsetsStr = TextBoxOffset.Text.Split('/');
            uint[] offsets = new uint[offsetsStr.Length];

            for(int i = 0; i < offsetsStr.Length; i++) {
                if(!uint.TryParse(offsetsStr[i], out offsets[i])) {
                    return TimerError.InvalidOffset;
                }
            }

            Array.Sort(offsets);
            info.Offsets = offsets;
            info.MaxOffset = offsets.Last();

            // << 9 on all of these to avoid going over the 32-bit integer limit when rebuilding the pcm
            // makes the maximum offset be 33553919
            if(info.Interval >= ushort.MaxValue << 9) {
                return TimerError.InvalidInterval;
            }

            if(info.NumBeeps >= ushort.MaxValue << 9) {
                return TimerError.InvalidNumBeeps;
            }

            foreach(uint offset in info.Offsets) {
                if(offset >= ushort.MaxValue << 9 || offset < info.Interval * (info.NumBeeps - 1)) {
                    return TimerError.InvalidOffset;
                }
            }

            return TimerError.NoError;
        }

        private void RadioButton_Click(object sender, EventArgs e) {
            FlowTimer.SelectTimer(this);
        }

        private void DataChanged(object sender, EventArgs e) {
            FlowTimer.SelectTimer(this);
        }

        private void RemoveButton_Click(object sender, EventArgs e) {
            FlowTimer.RemoveTimer(this);
        }
    }
}
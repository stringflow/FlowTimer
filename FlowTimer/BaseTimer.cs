using System.Windows.Forms;

namespace FlowTimer {
    
    public abstract class BaseTimer {

        public delegate double TimerUpdateCallback(double startTime);

        public TabPage Tab;
        public TimerUpdateCallback TimerCallback;
        public Control[] ControlsToCopy;

        public bool Selected {
            get { return FlowTimer.MainForm.TabControl.SelectedTab == Tab; }
        }
        
        public BaseTimer(TabPage tab, TimerUpdateCallback timerCallback, params Control[] controlsToCopy) {
            Tab = tab;
            Tab.SetDrawing(false);
            Tab.RemoveKeyControls();
            TimerCallback = timerCallback;
            ControlsToCopy = controlsToCopy;
        }

        public virtual void OnLoad() {
            foreach(Control control in ControlsToCopy) {
                Tab.Controls.Add(control);
            }
        }

        public abstract void OnInit();
        public abstract void OnTimerStart();
        public abstract void OnTimerStop();
        public abstract void OnKeyEvent(Keys key);
        public abstract void OnBeepSoundChange();
        public abstract void OnBeepVolumeChange();
    }
}

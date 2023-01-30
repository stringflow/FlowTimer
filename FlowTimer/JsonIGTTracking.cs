using System.Collections.Generic;

namespace FlowTimer {
    public class JsonIGTDelayer {
        public string Name;
        public double Delay;
    }

    public class JsonIGTDelayersGame {
        public string Game;
        public List<JsonIGTDelayer> Delayers;
    }

    public class JsonIGTDelayersSettings {
        public JsonTimersHeader Header;
        public List<JsonIGTDelayersGame> Games;
    }

    public class JsonIGTTimer {

        public string Name;
        public string Frame;
        public string Offsets;
        public string Interval;
        public string NumBeeps;
    }

    public class JsonIGTTimersFile {

        public JsonTimersHeader Header;
        public List<JsonIGTTimer> Timers;

        public JsonIGTTimersFile() { }

        public JsonIGTTimersFile(JsonTimersHeader header, List<JsonIGTTimer> timers) => (Header, Timers) = (header, timers);

        public JsonIGTTimer this[int i] {
            get { return Timers[i]; }
            set { Timers[i] = value; }
        }
    }
}

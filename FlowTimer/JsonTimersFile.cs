using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection;

namespace FlowTimer {

    public class JsonTimersHeader {

        public int Version = FlowTimer.GetCurrentBuild();
        // future use (maybe)
    }

    public class JsonTimer {

        public string Name;
        public string Offsets;
        public string Interval;
        public string NumBeeps;
    }

    public class JsonTimersFile {

        public JsonTimersHeader Header;
        public List<JsonTimer> Timers;

        // Json Constructor
        public JsonTimersFile() { }

        public JsonTimersFile(JsonTimersHeader header, List<JsonTimer> timers) => (Header, Timers) = (header, timers);

        public JsonTimer this[int i] {
            get { return Timers[i]; }
            set { Timers[i] = value; }
        }
    }
}

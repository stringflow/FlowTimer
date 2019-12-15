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
    }

    public class JsonTimer {

        public string Name;
        public string Offsets;
        public string Interval;
        public string NumBeeps;
    }

    public class JsonTimerFile {

        public JsonTimersHeader Header;
        public List<JsonTimer> Timers;

        // Json Constructor
        public JsonTimerFile() { }

        public JsonTimerFile(JsonTimersHeader header, List<JsonTimer> timers) => (Header, Timers) = (header, timers);
    }
}

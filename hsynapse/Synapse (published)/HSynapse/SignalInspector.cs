using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace HAKGERSoft.Synapse {

    public class SignalInspector {
        public bool Debug;

        public List<string> Log {
            get;
            private set;
        }

        // 0 - general info
        // 1 - nodes 
        // 2 - weights,delta
        public int TraceLevel {
            get;
            set;
        }

        public SignalInspector() {
            Log = new List<string>(10000);
        }

        public void Trace(NetworkElement sender,string message,params object[] args) {
            if(args!=null)
                for(int i=0;i<args.Length;i++)
                    if(args[i] is double)
                        args[i] = ((double)args[i]).ToString("0.000");
            Trace(sender,string.Format(message,args));
        }

        public void Trace(NetworkElement sender,string message) {
            Trace(string.Format("[{0}]: {1}",sender.GetDescription(),message));
        }
 
        public void Trace(string format, params object[] args) {
            Trace(string.Format(format,args));
        }

        //overload
        public void Trace(string message) {
            Log.Add(message);
            if(Debug)
                System.Diagnostics.Debug.WriteLine(message);
        }

        public void Clear() {
            Log.Clear();
        }

        public void SaveToFile(string path) {
            File.WriteAllLines(path, Log.ToArray());
        }


    }
}
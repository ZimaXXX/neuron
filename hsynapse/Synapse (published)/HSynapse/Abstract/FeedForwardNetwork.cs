using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace HAKGERSoft.Synapse {

    public abstract class FeedForwardNetwork: Network {
        static readonly Random Rnd=new Random();
        public readonly NetworkStructure Structure;
             
        double _Lf = 1;
        public double LearnFactor {
            get {
                return _Lf;
            }
            set {
                Utility.Verify(value,x => x<=1 && x>=0,"Invalid learn factor value");
                _Lf=value;
            }
        }

        SignalInspector _Inspector;
        public SignalInspector Inspector {
            get {
                return _Inspector;
            }
            set {
                _Inspector = value;
                foreach(Unit node in Structure.Elements)
                    node.Inspector=value;
                foreach(Link link in Structure.Elements) {
                    if(link.Next!=null) {
                        foreach(Connection connection in link.Next)
                            connection.Inspector=value;
                    }
                }
            }
        }

        public FeedForwardNetwork(NetworkStructure structure) {
            Utility.Verify(()=>structure!=null);
            Structure=structure;
        }

        public void Reset(Func<double> weightGenerator) {
            if(Inspector!=null)
                Inspector.Trace("Reset network, initialize weight values");
            foreach(Link link in Structure.Elements)
                link.Reset(weightGenerator);
        }

        public void Reset(double lo,double hi) {
            Reset(()=> (hi - lo)*Rnd.NextDouble() + lo);
        }


    }
}

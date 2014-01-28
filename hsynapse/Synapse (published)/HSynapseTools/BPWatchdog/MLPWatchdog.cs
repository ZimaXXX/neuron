using System;
using System.Collections.Generic;
using System.Linq;
using HAKGERSoft.Synapse;

namespace HAKGERSoft.Synapse.Tools {

    public class MLPWatchdog: MultilayerPerceptron {
        readonly double Lo;
        readonly double Hi;

        public MLPWatchdog(NetworkStructure structure)
            : this(structure,-1,1) {
        }

        public MLPWatchdog(NetworkStructure structure,double lo,double hi)
            : base(structure,lo,hi) {
                Lo=lo;
                Hi=hi;
        }

        public List<BPResponse> BPWatched(BPWDRequest bpr) {
            List<BPResponse> toret=new List<BPResponse>(bpr.MaxBPs);
            bool trace=this.Inspector!=null;
            for(int i=0; i<bpr.MaxBPs; i++) {
                if(trace)
                    Inspector.Trace("BP attempt {0}",i+1);
                BPResponse response=this.BP(bpr);
                toret.Add(response);
                if(response.Estimate!=BPResponseEstimate.Failure) {
                    if(trace)
                        Inspector.Trace("BPWatched process end, no failures found");
                    break;
                }
                if(trace)
                    Inspector.Trace("BP failed to learn network");
                this.Reset(Lo,Hi);
            }
            return toret;
        }

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using HAKGERSoft.Synapse;

namespace HAKGERSoft.Synapse.Tools {

    public class WatchdogGenerator: MLPGenerator {

        public new MLPWatchdog Create(int[] layers) {
            return Create(layers,null,null);
        }

        public new MLPWatchdog Create(int[] layers,IContinuousActivator func) {
            return Create(layers,null,func);
        }

        public new MLPWatchdog Create(int[] layers,double? biasValue,IContinuousActivator func) {
            Bias bias=biasValue.HasValue? new Bias(biasValue.Value):null;
            return new MLPWatchdog(GetLayers(layers,bias,func));
        }






    }
}
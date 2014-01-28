using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HAKGERSoft.Synapse {

    public class SharedWeight: Weight {
        readonly int Share;
        double PendingDelta;
        int DeltaReceived;

        public SharedWeight():this(0){
        }

        public SharedWeight(int share) :base(0) {
            Share=share;
        }

        public override void Correct(double delta) {
            PendingDelta+=delta;
            if(++DeltaReceived<Share)
                return;
            base.Correct(delta);
            PendingDelta=DeltaReceived=0;
        }





    }
}

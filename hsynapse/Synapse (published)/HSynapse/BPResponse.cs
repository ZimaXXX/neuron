using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace HAKGERSoft.Synapse {

    public class BPResponse {
        public readonly BPResponseEstimate Estimate;
        public readonly BPEpochResponse[] Epochs;
        public readonly TimeSpan BPTime;

        internal BPResponse(BPResponseEstimate estimate,BPEpochResponse[] epochs,TimeSpan bpTime) {
            Estimate=estimate;
            Epochs=epochs;
            BPTime=bpTime;
        }
    }

    public class BPEpochResponse{
        public readonly double[] TrainingMSE;
        public readonly double Epsilon;

        internal BPEpochResponse(double[] trainingMSE,double epsilon){
            TrainingMSE=trainingMSE;
            Epsilon=epsilon;
        }
    }

    public enum BPResponseEstimate {
        Unknown, Success, Failure
    }

}
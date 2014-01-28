using System;
using System.Collections.Generic;
using System.Linq;
using HAKGERSoft.Synapse;

namespace HAKGERSoft.Synapse.Tools {

    public class BPWDRequest: BPRequest {
        public readonly int MaxBPs;

        public BPWDRequest(TrainingData[] trainingSet,int maxEpochs,int minEpochs,double maxMSE,int maxBPs) 
            :base(trainingSet,maxEpochs,minEpochs,maxMSE) {
                Utility.Verify(maxBPs,x => x>0,"Invalid maxBPs value, must be greater than 0");
            MaxBPs=maxBPs;
        }


    }
}
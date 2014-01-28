using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HAKGERSoft.Synapse {

    public class TrainingData {
        public readonly double[] Input;
        public readonly double[] Output;
        public readonly double[][] TestingSets;

        public TrainingData(double[] input,double[] output)
            : this(input,output,null) {
        }

        public TrainingData(double[] input,double[] output,double[][] testingSets) {
            Utility.Verify(() => input!=null,"TrainingPair input");
            Utility.Verify(() => output!=null,"TrainingPair output");
            Utility.Verify(testingSets,x => x==null || x.All(i => i.Length==output.Length),"TrainingPair testingSets");

            Input=input;
            Output=output;
            TestingSets=testingSets;
        }
    }

}

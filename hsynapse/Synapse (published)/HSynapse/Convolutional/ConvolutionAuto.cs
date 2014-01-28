using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace HAKGERSoft.Synapse {

    public class ConvolutionAuto:Convolution {

        public ConvolutionAuto(int kernelSize,int featureMapsCount,int priorFeatureMapsCount,int overlap)
            : base(kernelSize,featureMapsCount,priorFeatureMapsCount,overlap,GenerateFullSchema(priorFeatureMapsCount,featureMapsCount)) {
        }

        static bool[][] GenerateFullSchema(int rows,int cols){
            return Utility.Generate<bool[]>(() => {
                return Utility.Generate<bool>(() => true,cols).ToArray();
            },rows).ToArray();
        }

        
    }
}

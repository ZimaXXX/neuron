using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace HAKGERSoft.Synapse {

    public class ConvolutionalTopology {
        public readonly int[] Layers;
        public readonly double? Bias;
        public readonly LayerConnector[] Map;
        public readonly IContinuousActivator Func;

        public ConvolutionalTopology(int[] layers,double? bias,LayerConnector[] map)
            :this(layers,bias,map,new Sigmoid()){
        }

        public ConvolutionalTopology(int[] layers,double? bias,LayerConnector[] map,IContinuousActivator func) {
            Utility.Verify(layers,l => l!=null && l.All(x => x>0),"Invalid layers array");
            Utility.Verify(map,m => m.Length==layers.Length-1,"Invalid Map");
            Utility.Verify(func,f => f!=null,"Invalid func");

            Layers=layers;
            Bias=bias;
            Map=map;
            Func=func;
        }



    }
}

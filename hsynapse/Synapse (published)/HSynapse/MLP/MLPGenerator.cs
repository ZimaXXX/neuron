using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HAKGERSoft.Synapse {

    public class MLPGenerator: NetworkGenerator {

        public MultilayerPerceptron Create(int[] layers) {
            return Create(layers,null,new Sigmoid());
        }

        public MultilayerPerceptron Create(int[] layers,IContinuousActivator func){
            return Create(layers,null,func);
        }

        public MultilayerPerceptron Create(int[] layers,double? biasValue,IContinuousActivator func) {
            Bias bias=biasValue.HasValue? new Bias(biasValue.Value):null;
            return new MultilayerPerceptron(GetLayers(layers,bias,func));
        }

        protected NetworkStructure GetLayers(int[] layers,Bias bias,IContinuousActivator func) {
            var l=layers.Length;
            Link[][] structure=new Link[l][];
            FullLayerConnector connector=new FullLayerConnector();
            for(var i=0;i<l-1;i++) {
                if(i==0)
                    structure[i]=Utility.Generate<Link>(layers[i]).ToArray();
                int j=i+1;
                structure[j]= Utility.Generate<Neuron>(layers[j]).ToArray();
                connector.Connect(structure[i],(NeuronBase[])structure[j],bias);
            }
            SetIdentity(structure);
            foreach(Link[] layer in structure)
                foreach(Link link in layer)
                    if(link is NeuronBase)
                        ((NeuronBase)link).Func=func;
            return new NetworkStructure(structure,bias);
        }
       



    }
}

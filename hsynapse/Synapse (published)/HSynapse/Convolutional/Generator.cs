using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HAKGERSoft.Synapse {

    public class ConvolutionalGenerator: NetworkGenerator {

        public ConvolutionalNetwork Create(ConvolutionalTopology topology) {
            Utility.Verify(topology,t => t!=null,"Invalid network topology");

            var l=topology.Layers.Length;
            Link[][] layers=new Link[l][];
            Bias bias=null;
            if(topology.Bias.HasValue)
                bias=new Bias(topology.Bias.Value);
            for(var i=0; i<l-1;i++){
                if(i==0) 
                    layers[i]=Utility.Generate<Link>(topology.Layers[i]).ToArray();
                int j=i+1;
                int size=topology.Layers[j];
                if(topology.Map[i] is SubSampling)
                    layers[j]= Utility.Generate<SubSamplingNeuron>(size).ToArray();
                else
                    layers[j]= Utility.Generate<Neuron>(size).ToArray();

                topology.Map[i].Connect(layers[i],(NeuronBase[])layers[j],bias);
            }
            SetIdentity(layers);
            foreach(Link[] layer in layers)
                foreach(Link link in layer)
                    if(link is NeuronBase)
                        ((NeuronBase)link).Func=topology.Func;
            return new ConvolutionalNetwork(new NetworkStructure(layers,bias));
        }


    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace HAKGERSoft.Synapse {

    public class FullLayerConnector: LayerConnector {

        public override void Connect(Link[] prior,NeuronBase[] next,Bias bias) {
            Utility.Verify(prior,x => x!=null && prior.Length>0,"Invalid prior layer");
            Utility.Verify(next,x => x!=null && next.Length>0,"Invalid next layer");
            
            foreach(NeuronBase neuron in next)
                neuron.Previous=GetNeuronConnections(prior,neuron,bias).ToArray();
            int idx=0;
            foreach(Link link in prior)
                link.Next=GetLinkConnections(idx++,next).ToArray();
            if(bias!=null){
                var biasConnections=GetBiasConnections(bias,next);
                if(bias.Next==null)
                    bias.Next=new Connection[] { };
                bias.Next=bias.Next.Concat(biasConnections).ToArray();
            }
                
        }

        IEnumerable<Connection> GetNeuronConnections(Link[] prior,NeuronBase neuron, Bias bias) {
            foreach(Link link in prior)
                yield return new Connection(link,neuron);
            if(bias!=null)
                yield return new Connection(bias,neuron);
        }

        IEnumerable<Connection> GetLinkConnections(int index,NeuronBase[] next){
            foreach(NeuronBase neuron in next)
                yield return neuron.Previous[index];
        }

        IEnumerable<Connection> GetBiasConnections(Bias bias, NeuronBase[] next){
            foreach(NeuronBase neuron in next)
                yield return neuron.Previous[neuron.Previous.Length-1];

        }
        

    }
}

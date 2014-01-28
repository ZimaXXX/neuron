using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HAKGERSoft.Synapse {

    public class NetworkStructure {
        public readonly Link[] Elements;
        public readonly Link[][] Layers;
        public readonly Bias Bias;

        NeuronBase[] OutputCache=null;

        public Link[] Input {
            get {
                return Layers[0];
            }
        }

        public NeuronBase[] Output {
            get {
                if(OutputCache==null)
                    OutputCache=Layers[Layers.Length-1].Select(x => (NeuronBase)x).ToArray();
                return OutputCache;
            }
        }

        public NetworkStructure(Link[][] layers,Bias bias){
            Utility.Verify(() => layers!=null && layers.All(x => x!=null&&x.Length>0),"layers");
            Layers=layers;
            Bias=bias;
            Elements=Gather(layers,bias).ToArray();
        }

        IEnumerable<Link> Gather(Link[][] layers,Bias bias) {
            foreach(Link[] layer in layers)
                foreach(Link link in layer)
                    yield return link;
            if(bias!=null)
                yield return bias;
            yield break;
        }


    }
}

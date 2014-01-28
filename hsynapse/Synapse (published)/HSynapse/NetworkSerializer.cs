using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Reflection;

namespace HAKGERSoft.Synapse {

    public  abstract class NetworkSerializer {
        protected static Dictionary<string,string> Aliases;

        static NetworkSerializer(){
            Aliases=new Dictionary<string,string>();
            RegisterAlias<Link>("l");
            RegisterAlias<Neuron>("n");
            RegisterAlias<SubSamplingNeuron>("ssn");
            RegisterAlias<Bias>("b");
            RegisterAlias<Weight>("w");
            RegisterAlias<SharedWeight>("sw");
            RegisterAlias<Sigmoid>("si");
            RegisterAlias<Linear>("lr");
            RegisterAlias<HyperbolicTangent>("hyp");
        }

        protected static void RegisterAlias<T>(string key) {
            string type=typeof(T).ToString();
            Aliases[key]=type;
            Aliases[type]=key;
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace HAKGERSoft.Synapse {

    public class SubSampling: FeatureMapsConnector {
        readonly int FeatureMapsCount;

        public SubSampling(int featureMapsCount) {
            Utility.Verify(featureMapsCount,f => f>0,"invalid featureMapsCount");
            FeatureMapsCount=featureMapsCount;
        }

        public override void Connect(Link[] prior,NeuronBase[] next,Bias bias) {
            Utility.Verify(prior,x => x!=null && prior.Length>0,"Invalid prior layer");
            Utility.Verify(next,x => x!=null && next.Length>0,"Invalid next layer");
            Utility.Verify(() => next.Length % FeatureMapsCount==0,"FeatureMapsCount - next layer size conflict");
            Utility.Verify(() => prior.Length % FeatureMapsCount==0,"FeatureMapsCount - prior layer size conflict");

            Link[][] c=GetFeatureMaps(prior,FeatureMapsCount);
            NeuronBase[][] s=GetFeatureMaps(next,FeatureMapsCount);
            Link[][][] kernelCollection=GetKernelCollection(c,2,0); //always x/2

            Dictionary<Link,List<Connection>> map=new Dictionary<Link,List<Connection>>();

            for(var i=0;i<kernelCollection.Length;i++)
                ConnectFeatureMaps(s[i],kernelCollection[i],bias,map);

            foreach(Link link in prior)
                link.Next= map[link].ToArray();
            foreach(NeuronBase neuron in next)
                neuron.Previous=map[neuron].ToArray();
            if(bias!=null){
                if(bias.Next==null)
                    bias.Next=new Connection[] { };
                bias.Next=bias.Next.Concat(map[bias]).ToArray();
            }
        }

        void ConnectFeatureMaps(NeuronBase[] next,Link[][] kernelCollection,Bias bias,Dictionary<Link,List<Connection>> map) {
            SharedWeight weight=new SharedWeight(next.Length);
            SharedWeight biasWeight=new SharedWeight(next.Length);
            for(var i=0;i<kernelCollection.Length;i++){
                ConnectKernel(next[i],kernelCollection[i],weight,map);
                if(bias!=null){
                    Map(bias,next[i],biasWeight,map);
                }
            }
        }

        void ConnectKernel(NeuronBase neuron,Link[] kernel,SharedWeight weight,Dictionary<Link,List<Connection>> map){
            foreach(Link link in kernel) {
                Map(link,neuron,weight,map);
            }
        }

        void Map(Link a,Link b,Weight weight,Dictionary<Link,List<Connection>> map) {
            Connection connection=new Connection(weight,a,(NeuronBase)b);
            if(!map.ContainsKey(a))
                map[a]=new List<Connection>();
            map[a].Add(connection);
            if(!map.ContainsKey(b))
                map[b]=new List<Connection>();
            map[b].Add(connection);
        }
        



    }
}

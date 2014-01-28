using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace HAKGERSoft.Synapse {

    public class Convolution: FeatureMapsConnector {
        readonly int KernelSize;
        readonly int NextFeatureMapsCount;
        readonly int PriorFeatureMapsCount;
        readonly int Overlap;
        readonly bool[][] Schema;

        public Convolution(int kernelSize,int featureMapsCount,int priorFeatureMapsCount,int overlap,bool[][] schema) {
            Utility.Verify(kernelSize,x => x>0,"Invalid kernelSize value");
            Utility.Verify(featureMapsCount,x => x>0,"Invalid featureMapsCount value");
            Utility.Verify(priorFeatureMapsCount,x => x>0,"Invalid priorFeatureMapsCount value");
            Utility.Verify(overlap,x => x>=0,"Invalid overlap value");
            Utility.Verify(schema,x => x.Length==priorFeatureMapsCount,"Invalid schema");
            foreach(bool[] schemaRow in schema)
                Utility.Verify(schemaRow,x => x.Length==featureMapsCount,"Invalid schema");

            KernelSize=kernelSize;
            NextFeatureMapsCount=featureMapsCount;
            PriorFeatureMapsCount=priorFeatureMapsCount;
            Overlap=overlap;
            Schema=schema;
        }

        public override void Connect(Link[] prior,NeuronBase[] next,Bias bias) {
            Utility.Verify(prior,x => x!=null && prior.Length>0,"Invalid prior layer");
            Utility.Verify(next,x => x!=null && next.Length>0,"Invalid next layer");
            Utility.Verify(() => next.Length % NextFeatureMapsCount==0,"NextFeatureMapsCount - next layer size conflict");
            Utility.Verify(() => prior.Length % PriorFeatureMapsCount==0,"PriorFeatureMapsCount - prior layer size conflict");

            Link[][] fmp=GetFeatureMaps(prior,PriorFeatureMapsCount);
            Link[][] fmn=GetFeatureMaps(next,NextFeatureMapsCount);
            Link[][][] kernelCollection=GetKernelCollection(fmp,KernelSize,Overlap);

            Dictionary<Link,List<Connection>> map=new Dictionary<Link,List<Connection>>();

            for(int f=0;f<fmn.Length;f++) {
                Neuron[] nextFeatureMap=fmn[f].Select(x => (Neuron)x).ToArray();
                for(int p=0;p<fmp.Length;p++) {
                    bool inSchema=Schema[p][f];
                    if(inSchema)
                        ConnectFeatureMaps(nextFeatureMap,kernelCollection[p],map);
                }
                if(bias!=null) {
                    Weight weight=new SharedWeight(nextFeatureMap.Length);
                    foreach(Neuron node in nextFeatureMap)
                        Map(bias,node,weight,map);
                }
            }

            foreach(Link link in prior)
                link.Next= map[link].ToArray();
            foreach(Neuron node in next)
                node.Previous=map[node].ToArray();
            if(bias!=null){
                if(bias.Next==null)
                    bias.Next=new Connection[] { };
                bias.Next=bias.Next.Concat(map[bias]).ToArray();
            }
        }

        void ConnectFeatureMaps(Neuron[] next,Link[][] kernelCollection,Dictionary<Link,List<Connection>> map) {
            Weight[] weights=Utility.Generate(() => new SharedWeight(kernelCollection.Length),KernelSize*KernelSize).ToArray();
            for(int i=0;i<kernelCollection.Length;i++){
                for(var j=0; j<kernelCollection[i].Length; j++){
                    Link link=kernelCollection[i][j];
                    Map(link,next[i],weights[j],map);
                }
            }
        }

        void Map(Link a,Link b,Weight weight,Dictionary<Link,List<Connection>> map){
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

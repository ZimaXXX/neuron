using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace HAKGERSoft.Synapse {

    public class ConvolutionMapAuto: ConnectionMap {
        readonly int KernelSize;
        readonly int FeatureMapsCount;
        readonly int Overlap;
        readonly Func<double> WeightGenerator;
        readonly Bias Bias;

        public ConvolutionMapAuto(int kernelSize, int featureMapsCount, int overlap, Bias bias, Func<double> weightGenerator) {
            Utility.Verify(kernelSize,x => x>0,"Invalid kernelSize value");
            Utility.Verify(featureMapsCount,x => x>0,"Invalid featureMapsCount value");
            Utility.Verify(overlap,x => x>=0,"Invalid overlap value");
            Utility.Verify(weightGenerator,x => x!=null,"Null weightGenerator");

            KernelSize=kernelSize;
            FeatureMapsCount=featureMapsCount;
            Overlap=overlap;
            WeightGenerator=weightGenerator;
            Bias=bias;
        }

    public override void Connect(Link[] prior,Node[] next) {
            Utility.Verify(prior,x => x!=null,"Invalid prior layer");
            Utility.Verify(next,x => x!=null,"Invalid next layer");
            Utility.Verify(() => next.Length % FeatureMapsCount==0,"FeatureMapsCount - next layer size conflict");

            Node[][] fm=GetFeatureMaps(next);
            Link[][] kernelSet=GetKernelSet(prior);
            if(Bias!=null)
                Bias.Next=new Connection[next.Length];

            foreach(Link[] featureMap in fm)
                Utility.Verify(() => featureMap.Length==kernelSet.Length,"featureMap - kernel conflict");

            Dictionary<Link,int> ccd=new Dictionary<Link,int>();
            foreach(Link p in prior)
                ccd[p]=GetConnectionsCount(p,fm.Length,kernelSet);

            for(int f=0;f<fm.Length;f++) {
                Node[] featureMap=fm[f];
                int weightCount=KernelSize*KernelSize;
                if(Bias!=null)
                    weightCount++;
                double[] weightValues=Utility.Generate<double>(WeightGenerator,weightCount).ToArray();
                Weight[] weights=weightValues.Select(x => new Weight(x)).ToArray();
                for(int n=0; n<featureMap.Length;n++){
                    Node node=featureMap[n];
                    Link[] kernel=kernelSet[n];
                    ConnectKernel(kernel,node,weights,Bias,ccd);
                }
            }

        }

        int GetConnectionsCount(Link link, int fmc,Link[][] kernelSet){
            int toret=0;
            foreach(Link[] kernel in kernelSet)
                foreach(Link l in kernel)
                    if(l==link)
                        toret++;
            return toret*fmc;
        }

        Node[][] GetFeatureMaps(Node[] layer) {
            Node[][] fm=new Node[FeatureMapsCount][];
            int idx=0;
            int count=layer.Length / FeatureMapsCount;
            for(var i=0; i<fm.Length; i++){
                fm[i]=layer.Skip(idx).Take(count).ToArray();
                idx+=count;
            }
            return fm;
        }

        Link[][] GetKernelSet(Link[] layer) {
            int size=(int)Math.Sqrt(layer.Length);
            int step=KernelSize-Overlap;
            List<Link[]> kernelSet=new List<Link[]>();
            for(int y=0;y+KernelSize<=size;y+=step) {
                for(int x=0;x+KernelSize<=size;x+=step) {
                    Link[] kernel=GetKernel(layer,x,y);
                    kernelSet.Add(kernel);
                }
            }
            return kernelSet.ToArray();
        }

        Link[] GetKernel(Link[] layer,int offsetX,int offsetY) {
            Utility.Verify(() => offsetX+KernelSize<layer.Length && offsetY+KernelSize<layer.Length,"Invalid receptive field");
            int size=(int)Math.Sqrt(layer.Length);
            List<Link> kernel=new List<Link>(KernelSize*KernelSize);
            for(int y=offsetY;y<offsetY+KernelSize;y++) {
                for(int x=offsetX;x<offsetX+KernelSize;x++) {

                    Link link=layer[y*size + x];
                    kernel.Add(link);
                }
            }
            return kernel.ToArray();
        }

        void ConnectKernel(Link[] kernel,Node node,Weight[] weights, Bias bias,Dictionary<Link,int> ccd) {
            int l=kernel.Length;
            if(bias!=null)
                l++;
            node.Previous=new Connection[l];
            for(var i=0;i<kernel.Length;i++) {
                kernel[i].Next=kernel[i].Next??new Connection[ccd[kernel[i]]];
                Connect(kernel[i],node,weights[i]);
            }
            if(bias!=null)
                Connect(bias,node,weights[kernel.Length]);
        }

        void Connect(Link pre,Node post,Weight weight) {
            Connection i=new Connection(weight);
            pre.Next[Utility.FirstNull(pre.Next)]=i;
            post.Previous[Utility.FirstNull(post.Previous)]=i;
            i.Previous=pre;
            i.Next=post;
            i.Identity=string.Format("{0} --> {1}",i.Previous.Identity,i.Next.Identity);
        }

        


    }
}

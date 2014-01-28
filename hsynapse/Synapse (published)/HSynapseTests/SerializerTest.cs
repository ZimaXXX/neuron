using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

using HAKGERSoft.Synapse;
using System.Xml.Linq;

namespace HAKGERSoft.Synapse.Tests {

    [TestFixture]
    public class SerializerTest {
        
        [Test]
        public void MLPSerialization(){
            MultilayerPerceptron mlp= new MLPGenerator().Create(new int[] { 2,2,1 });
            mlp.Reset(0,1);
            MLPXMLSerializer serializer=new MLPXMLSerializer();
            XDocument xdoc=serializer.Serialize(mlp);
            MultilayerPerceptron mlp2=serializer.Deserialize(xdoc);

            Assert.AreEqual(mlp2.Structure.Elements.Length,mlp.Structure.Elements.Length);
            Assert.AreEqual(mlp2.Structure.Elements[3].GetDescription(),mlp.Structure.Elements[3].GetDescription());
            Assert.AreEqual(mlp2.Structure.Elements[0].Next[0].Weight.Value,mlp.Structure.Elements[0].Next[0].Weight.Value);
            Assert.AreEqual(((NeuronBase)mlp2.Structure.Elements[4]).Previous[0].Weight.Value,((NeuronBase)mlp.Structure.Elements[4]).Previous[0].Weight.Value);
            Assert.AreEqual(((NeuronBase)mlp2.Structure.Elements[3]).Func.GetType(),((NeuronBase)mlp.Structure.Elements[3]).Func.GetType());
        }

        [Test]
        public void ConvolutionSerialization(){
            int[] layers=new int[] {
                1024, //32x32 input image
                4704, //6 @ 28x28 convolution
                1176, //6 @  14x14 subsampling
                1600, //16 @ 10x10 convolution
                400, //16 @ 5x5 subsampling
                120, // 120x1x1 convolution!
                84, //full
                10 //full
            };
            LayerConnector[] map=new LayerConnector[] {
                new ConvolutionAuto(5,6,1,4),
                new SubSampling(6),
                new ConvolutionAuto(5,16,6,4),
                new SubSampling(16),
                new ConvolutionAuto(5,120,16,0),
                new FullLayerConnector(),
                new FullLayerConnector()
            };
            ConvolutionalTopology topology=new ConvolutionalTopology(layers,1,map);
            ConvolutionalGenerator generator=new ConvolutionalGenerator();
            ConvolutionalNetwork network =generator.Create(topology);

            ConvolutionalXMLSerializer serializer=new ConvolutionalXMLSerializer();
            XDocument doc= serializer.Serialize(network);
            ConvolutionalNetwork network2=serializer.Deserialize(doc);

            Assert.AreEqual(network2.Structure.Elements.Length,network.Structure.Elements.Length);
            Assert.AreEqual(network2.Structure.Elements[3000].GetDescription(),network.Structure.Elements[3000].GetDescription());
            Assert.AreEqual(network2.Structure.Elements[0].Next[0].Weight.Value,network.Structure.Elements[0].Next[0].Weight.Value);
            Assert.AreEqual(((NeuronBase)network2.Structure.Elements[4000]).Previous[0].Weight.Value,((NeuronBase)network.Structure.Elements[4000]).Previous[0].Weight.Value);
            Assert.AreEqual(((NeuronBase)network2.Structure.Elements[3004]).Func.GetType(),((NeuronBase)network.Structure.Elements[3004]).Func.GetType());

            Weight w1=network2.Structure.Layers[0][0].Next[0].Weight;
            Weight w2=network2.Structure.Layers[0][1].Next.Select(x => x.Weight).FirstOrDefault(x => x==w1);
            Assert.IsNotNull(w1);
            Assert.IsTrue(w1==w2);

            Link[] l1=network2.Structure.Layers[1];

            int c=GetConnectionsCount(l1,false,true);
            int w=GetWeightsCount(l1,false,true);
            Assert.AreEqual(c,122304);
            Assert.AreEqual(w,156);

            for(int i=0;i<network.Structure.Layers.Length;i++){
                for(int j=0; j<network.Structure.Layers[i].Length; j++){
                    Connection[] n1=network.Structure.Layers[i][j].Next;
                    if(n1!=null) {
                        Connection[] n2=network2.Structure.Layers[i][j].Next;
                        Assert.IsNotNull(n2);
                        for(int z=0; z<n1.Length;z++){
                            Assert.AreEqual(n1[z].Weight.Value,n2[z].Weight.Value);
                            Assert.AreEqual(n1[z].Weight.GetType(),n2[z].Weight.GetType());
                        }
                    }
                    if(network.Structure.Layers[i][j] is NeuronBase) {
                        Connection[] p1=((NeuronBase)network.Structure.Layers[i][j]).Previous;
                        if(p1!=null) {
                            Connection[] p2=((NeuronBase)network2.Structure.Layers[i][j]).Previous;
                            Assert.IsNotNull(p2);
                            for(int z=0;z<p1.Length;z++) {
                                Assert.AreEqual(p1[z].Weight.Value,p2[z].Weight.Value);
                                Assert.AreEqual(p1[z].Weight.GetType(),p2[z].Weight.GetType());
                            }
                        }
                    }
                }
            }
        }

        int GetConnectionsCount(Link[] network,bool next,bool previous) {
            return GetConnections(network,next,previous).Count;
        }

        int GetWeightsCount(Link[] network,bool next,bool previous) {
            HashSet<Connection> hs= GetConnections(network,next,previous);
            HashSet<Weight> whs=new HashSet<Weight>();
            foreach(Connection c in hs)
                whs.Add(c.Weight);
            return whs.Count;
        }

        HashSet<Connection> GetConnections(Link[] network,bool next,bool previous) {
            HashSet<Connection> hs=new HashSet<Connection>();
            foreach(Link link in network) {
                if(next&&link.Next!=null)
                    foreach(Connection c in link.Next)
                        hs.Add(c);
                if(previous&& link is NeuronBase) {
                    NeuronBase nb=(NeuronBase)link;
                    foreach(Connection c in nb.Previous)
                        hs.Add(c);
                }
            }
            return hs;
        }








    }
}
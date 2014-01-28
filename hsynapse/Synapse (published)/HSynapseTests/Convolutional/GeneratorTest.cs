using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

using HAKGERSoft.Synapse;
using HAKGERSoft.Synapse.Tools;

namespace HAKGERSoft.Synapse.Tests {

    [TestFixture]
    public class ConvolutionalGeneratorTest {
        ConvolutionalNetwork Network=null;

        bool[][] GetSchema() {
            bool T=true;
            bool F=false;
            return new[] {
                new[] {T,F,F,F,T,T,T,F,F,T,T,T,T,F,T,T},
                new[] {T,T,F,F,F,T,T,T,F,F,T,T,T,T,F,T},
                new[] {T,T,T,F,F,F,T,T,T,F,F,T,F,T,T,T},
                new[] {F,T,T,T,F,F,T,T,T,T,F,F,T,F,T,T},
                new[] {F,F,T,T,T,F,F,T,T,T,T,F,T,T,F,T},
                new[] {F,F,F,T,T,T,F,F,T,T,T,T,F,T,T,T}
            };
        }

        [TestFixtureSetUp]
        public void TSSetUp() {
            
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
                new Convolution(5,16,6,4,GetSchema()),
                new SubSampling(16),
                new ConvolutionAuto(5,120,16,0),
                new FullLayerConnector(),
                new FullLayerConnector()
            };
            ConvolutionalTopology topology=new ConvolutionalTopology(layers,1,map);
            ConvolutionalGenerator generator=new ConvolutionalGenerator();
            Network=generator.Create(topology);
        }

        [Test]
        public void SharedWeightsCheck(){
            Weight w=Network.Structure.Layers[0][0].Next[0].Weight;
            Weight w2=Network.Structure.Layers[0][1].Next.Select(x => x.Weight).FirstOrDefault(x => x==w);
            Assert.IsNotNull(w);
            Assert.IsTrue(w==w2);
        }

        [Test,Ignore]
        public void CheckOverallConnectionsCount(){
            Link[] network=Network.Structure.Layers.SelectMany(x => x).ToArray();
            int count=GetConnectionsCount(network,true,true);
            Assert.AreEqual(count,340908);
        }

        [Test,Ignore]
        public void CheckOverallWeightsCount() {
            Link[] network=Network.Structure.Layers.SelectMany(x => x).ToArray();
            int count=GetWeightsCount(network,true,true);
            Assert.AreEqual(count,60000);
        }

        [Test]
        public void C1(){
            int cc=GetConnectionsCount(Network.Structure.Layers[1],false,true);
            int wc=GetWeightsCount(Network.Structure.Layers[1],false,true);
            Assert.AreEqual(cc,122304);
            Assert.AreEqual(wc,156);
        }

        [Test]
        public void S2() {
            int cc=GetConnectionsCount(Network.Structure.Layers[2],false,true);
            int wc=GetWeightsCount(Network.Structure.Layers[2],false,true);
            Assert.AreEqual(cc,5880);
            Assert.AreEqual(wc,12);
        }

        [Test]
        public void C3() {
            int cc=GetConnectionsCount(Network.Structure.Layers[3],false,true);
            int wc=GetWeightsCount(Network.Structure.Layers[3],false,true);
            Assert.AreEqual(cc,151600);
            Assert.AreEqual(wc,1516);
        }

        [Test]
        public void S4() {
            int cc=GetConnectionsCount(Network.Structure.Layers[4],false,true);
            int wc=GetWeightsCount(Network.Structure.Layers[4],false,true);
            Assert.AreEqual(cc,2000);
            Assert.AreEqual(wc,32);
        }

        [Test]
        public void C5() {
            int cc=GetConnectionsCount(Network.Structure.Layers[5],false,true);
            int wc=GetWeightsCount(Network.Structure.Layers[5],false,true);
            Assert.AreEqual(cc,48120);
            Assert.AreEqual(wc,cc);
        }

        [Test]
        public void F6() {
            int cc=GetConnectionsCount(Network.Structure.Layers[6],false,true);
            int wc=GetWeightsCount(Network.Structure.Layers[6],false,true);
            Assert.AreEqual(cc,10164);
            Assert.AreEqual(wc,cc);
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
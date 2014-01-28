using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

using HAKGERSoft.Synapse;
using HAKGERSoft.Synapse.Tools;

namespace HAKGERSoft.Synapse.Tests {

    [TestFixture]
    public class ConvolutionMapAutoTest {
        Link[] l1=null;
        Neuron[] l2=null;
        Bias Bias=null;
        ConvolutionAuto Cma=null;
        int Weight=0;

        [TestFixtureSetUp]
        public void TSSetUp() {
            l1= Utility.Generate<Link>(() => new Link(),841).ToArray();
            l2=Utility.Generate<Neuron>(() => new Neuron(new Sigmoid()),1014).ToArray();
            Func<double> wg= () => Weight++;
            Bias=new Bias(1);
            Cma=new ConvolutionAuto(5,6,1,3);
            Cma.Connect(l1,l2,Bias);
        }

        [Test]
        public void NeuronsCount() {
            Assert.AreEqual(l1.Length,841);
            Assert.AreEqual(l2.Length,1014);
        }

        [Test]
        public void KernelCheck() {
            foreach(Neuron node in l2)
                Assert.AreEqual(node.Previous.Length,26);
        }

        [Test]
        public void SharedWeightsCheck() {
            HashSet<Weight> hs=new HashSet<Weight>();
            foreach(Neuron node in l2) {
                foreach(Connection c in node.Previous) {
                    Weight w=c.Weight;
                    if(!hs.Contains(w))
                        hs.Add(w);
                }
            }
            Assert.AreEqual(hs.Count,156);
        }

        [Test]
        public void ConnectionsCheck() {
            int c1=0;
            int c2=0;
            foreach(Link link in l1)
                c1+=link.Next.Length;
            foreach(Neuron node in l2)
                c2+=node.Previous.Length;

            Assert.AreEqual(c2,1014*26);
            Assert.AreEqual(c1,1014*26 - 1014); //substract bias term
        }

        [Test]
        public void BiasCheck() {
            Assert.AreEqual(Bias.Next.Length,1014);
        }

        [Test]
        public void KernelNeighbourhoodTest(){
        }


    }
}
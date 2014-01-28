using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

using HAKGERSoft.Synapse;
using HAKGERSoft.Synapse.Tools;

namespace HAKGERSoft.Synapse.Tests {

    [TestFixture]
    public class ConvolutionMapTest {
        Link[] l1=null;
        Neuron[] l2=null;
        Bias Bias=null;
        Convolution Cma=null;
        int Weight=0;

        bool[][] GetSchema(){
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
            l1= Utility.Generate<Link>(() => new Link(),1176).ToArray();
            l2=Utility.Generate<Neuron>(() => new Neuron(new Sigmoid()),1600).ToArray();
            Func<double> wg= () => Weight++;
            Bias=new Bias(1);
            Cma=new Convolution(5,16,6,4,GetSchema());
            Cma.Connect(l1,l2,Bias);
        }


        [Test]
        public void NeuronsCount() {
            Assert.AreEqual(l1.Length,1176);
            Assert.AreEqual(l2.Length,1600);
        }

        [Test]
        public void ConnectionsCheck() {
            int c2=0;
            foreach(Neuron node in l2)
                c2+=node.Previous.Length;

            Assert.AreEqual(c2,151600);
        }

        [Test] 
        public void WeightsCheck(){
            HashSet<Weight> hs=new HashSet<Weight>();
            foreach(Neuron node in l2) {
                foreach(Connection c in node.Previous) {
                    Weight w=c.Weight;
                    if(!hs.Contains(w))
                        hs.Add(w);
                }
            }
            Assert.AreEqual(hs.Count,1516);
        }









    }
}
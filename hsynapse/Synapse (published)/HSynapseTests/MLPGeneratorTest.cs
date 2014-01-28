using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

using HAKGERSoft.Synapse;

namespace HAKGERSoft.Synapse.Tests {

    [TestFixture]
    public class MLPGeneratorTest {
        
        void CheckConnection(Link pre, NeuronBase post,ref int i){
            var bridge= from a in pre.Next join b in post.Previous on a equals b select a;
            Assert.AreEqual(bridge.Count(),1);
            Connection inp=bridge.First();
            Assert.AreEqual(inp.Previous,pre);
            Assert.AreEqual(inp.Next,post);
            i++;
            if(post.Next!=null){
                foreach(var n in post.Next) {
                    var pp=n.Next;
                    CheckConnection(post,pp,ref i);
                }
            }
        }

        [Test]
        public void FullNetworkScanTest(){
            int[] layers=new int[]{2,3,1};
            MultilayerPerceptron nn=new MLPGenerator().Create(layers,1,new Sigmoid());
            //check input and output layer
            Assert.AreEqual(nn.Structure.Input.Length,2);
            Assert.AreEqual(nn.Structure.Output.Length,1);

            int count=0;
            foreach(var i in nn.Structure.Input) {
                foreach(var j in i.Next)
                    CheckConnection(i,j.Next,ref count);
            }
            Assert.AreEqual(count,12);
            //check bias
            Assert.AreEqual(nn.Structure.Bias.Next.Length,4);
            Assert.IsTrue(nn.Structure.Bias.Next.All(x => x.Next is Neuron));

        }

      












    }
}
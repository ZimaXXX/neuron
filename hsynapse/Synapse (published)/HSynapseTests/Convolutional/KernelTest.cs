using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

using HAKGERSoft.Synapse;
using HAKGERSoft.Synapse.Tools;

namespace HAKGERSoft.Synapse.Tests {

    [TestFixture]
    public class KernelTest {
        Link[] l1=null;
        Neuron[] l2=null;
        Bias Bias=null;
        ConvolutionAuto Cma=null;

        [TestFixtureSetUp]
        public void TSSetUp() {
            l1= Utility.Generate<Link>(() => new Link(),36).ToArray();
            l2=Utility.Generate<Neuron>(() => new Neuron(new Sigmoid()),4).ToArray();
            Bias=new Bias(1);
            Cma=new ConvolutionAuto(4,1,1,2);
            Cma.Connect(l1,l2,Bias);
        }

        [Test]
        public void FullScan(){
            int[][] kernelSet=new int[][] {
                new int[] { 0,1,2,3,6,7,8,9,12,13,14,15,18,19,20,21 },
                new int[] { 2,3,4,5,8,9,10,11,14,15,16,17,20,21,22,23 },
                new int[] { 12,13,14,15,18,19,20,21,24,25,26,27,30,31,32,33 },
                new int[] { 14,15,16,17,20,21,22,23,26,27,28,29,32,33,34,35 }
            };
            for(int i=0;i<l2.Length;i++)
                NeighbourhoodCheck(kernelSet[i],i);
            WeightsCheck(kernelSet);
        }

        void NeighbourhoodCheck(int[] kernelIdxs,int neuronIdx) {
            int pos=0;
            foreach(int idx in kernelIdxs) {
                Link link=l1[idx];
                Assert.AreEqual(l2[neuronIdx].Previous[pos++].Previous,link);
            }
            Assert.AreEqual(l2[neuronIdx].Previous[pos].Previous,Bias);
            Assert.AreEqual(l2[neuronIdx].Previous.Length,kernelIdxs.Length+1);
        }

        void WeightsCheck(int[][] kernelSet) {
            for(int i=0; i<16; i++){
                Assert.AreEqual(l2[0].Previous[i].Weight,l2[1].Previous[i].Weight);
                Assert.AreEqual(l2[1].Previous[i].Weight,l2[2].Previous[i].Weight);
                Assert.AreEqual(l2[2].Previous[i].Weight,l2[3].Previous[i].Weight);
                
                // random sanity check
                for(int x=0; x!=l2[0].Previous.Length;x++){
                    if(x!=i)
                        Assert.AreNotEqual(l2[0].Previous[x].Weight,l2[0].Previous[i].Weight);
                }
            }
        }
             




    }
}
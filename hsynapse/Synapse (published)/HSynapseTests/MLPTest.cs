using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

using HAKGERSoft.Synapse;

namespace HAKGERSoft.Synapse.Tests {

    [TestFixture]
    public class MLPTest {
        Random r = new Random();

        TrainingData[] TestVec=new TrainingData[] {
            new TrainingData(new double[] {0,1,1,0}, new double[] {0}),
            new TrainingData(new double[] {1,0,0,1}, new double[] {1}),
            new TrainingData(new double[] {1,1,0,0}, new double[] {0}),
            new TrainingData(new double[] {0,0,0,0}, new double[] {1}),
            new TrainingData(new double[] {1,1,1,1}, new double[] {0}),
            new TrainingData(new double[] {0,0,1,0}, new double[] {1})
        };

        [Test]
        public void PulseManySameResultsTest() {
            MLPGenerator gen = new MLPGenerator();
            int[] layers = new int[] { 4,3,2 };
            MultilayerPerceptron nn = gen.Create(layers,1,new Sigmoid());
            double[] inp = new double[] { -0.978, 2.34, 0.2, -0.33};
            double[] res = nn.Pulse(inp);
            double[] res2 = nn.Pulse(inp);
            double[] res3 = nn.Pulse(inp);
            Assert.AreEqual(res.Length, res2.Length);
            Assert.AreEqual(res.Length, res3.Length);
            for(int i=0; i<res.Length; i++) {
                Assert.AreEqual(res[i], res2[i]);
                Assert.AreEqual(res[i], res3[i]);
            }
        }
  
        [Test]
        public void BPMaxCyclesTest(){
            int[] layers = new int[] { 4,5,1 };
            MultilayerPerceptron nn = new MLPGenerator().Create(layers,1,new Sigmoid());
            var result=nn.BP(new BPRequest(TestVec,5));
            Assert.AreEqual(result.Epochs.Length,5);
        }

        [Test]
        public void BPMinCyclesTest() {
            int[] layers = new int[] { 4,5,1 };
            MultilayerPerceptron nn = new MLPGenerator().Create(layers,1,new Sigmoid());
            var result=nn.BP(new BPRequest(TestVec,5000,40));
            Assert.IsTrue(result.Epochs.Length>=40);
        }

        [Test]
        public void BPMaxMSETest() {
            int[] layers = new int[] { 4,5,1 };
            MultilayerPerceptron nn = new MLPGenerator().Create(layers,1,new Sigmoid());
            var result=nn.BP(new BPRequest(TestVec,1000,0,0.7));
            Assert.IsTrue(result.Epochs.Length<1000);
        }

        [Test]
        public void ResetAutoTest(){
            int[] layers = new int[] { 4,5,1 };
            MultilayerPerceptron nn = new MLPGenerator().Create(layers,1,new Sigmoid());
            double[] randomSelected=new double[] {
                nn.Structure.Input[1].Next[1].Weight.Value,
                nn.Structure.Input[1].Next[2].Weight.Value,
                nn.Structure.Input[1].Next[2].Next.Next[0].Weight.Value,
                nn.Structure.Output[0].Previous[1].Weight.Value
            };
            double[] inp = new double[] { -0.978, 2.34, 0.2, -0.33};
            double[] pulsed=nn.Pulse(inp);

            nn.Reset(0,1);
            double[] randomSelected2=new double[] {
                nn.Structure.Input[1].Next[1].Weight.Value,
                nn.Structure.Input[1].Next[2].Weight.Value,
                nn.Structure.Input[1].Next[2].Next.Next[0].Weight.Value,
                nn.Structure.Output[0].Previous[1].Weight.Value
            };
            for(int i=0;i<randomSelected.Length;i++) {
                Assert.AreNotEqual(randomSelected[i],randomSelected2[i]);
            }
            double[] pulsed2=nn.Pulse(inp);
            for(int i=0;i<pulsed.Length;i++) {
                Assert.AreNotEqual(pulsed[i],pulsed2[i]);
            }
        }

        [Test]
        public void ResetManualTest(){
            int[] layers = new int[] { 4,5,1 };
            MultilayerPerceptron nn = new MLPGenerator().Create(layers,1,new Sigmoid());
            nn.Reset(() => 0.7);
            Assert.AreEqual(nn.Structure.Input[1].Next[2].Weight.Value,0.7); // random selected
        }

        [Test]
        public void ReasonableBPTimeTest(){
            int[] layers = new int[] { 4,5,1 };
            MultilayerPerceptron nn = new MLPGenerator().Create(layers,1,new Sigmoid());
            var result=nn.BP(new BPRequest(TestVec,10000));
            Assert.IsTrue(result.BPTime.TotalMilliseconds<2000);
        }

        [Test,Explicit]
        public void ExplicitBPTimeTest() {
            int[] layers = new int[] { 4,10,1 };
            MultilayerPerceptron nn = new MLPGenerator().Create(layers,1,new Sigmoid());
            var result=nn.BP(new BPRequest(TestVec,1500*10));
            throw new NotImplementedException("BP Total ms="+result.BPTime.TotalMilliseconds.ToString());
        }





    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

using HAKGERSoft.Synapse;
using HAKGERSoft.Synapse.Tools;

namespace HAKGERSoft.Synapse.Tests {

    [TestFixture]
    public class BPWatchdogTest:TestBase {

        TrainingData[] XOR=new TrainingData[] {
            new TrainingData(new double[] {1,0}, new double[] {1}),
            new TrainingData(new double[] {0,1}, new double[] {1}),
            new TrainingData(new double[] {1,1}, new double[] {0}),
            new TrainingData(new double[] {0,0}, new double[] {0}),
        };

        [Test]
        public void LearnNeverTest() {
            int[] layers = new int[] { 2,1 }; // no linear separity
            MLPWatchdog nn = new WatchdogGenerator().Create(layers,1,new Sigmoid());
            nn.LearnFactor=0.9;
            nn.Momentum=0.5;
            var response= nn.BPWatched(new BPWDRequest(XOR,10000,0,0.1,10));
            Assert.AreEqual(response.Count,10);
            Assert.AreEqual(response[9].Estimate,BPResponseEstimate.Failure);
        }

        [Test]
        public void LearnXORTest() {
            for(var x=0; x<5; x++) {
                int[] layers = new int[] { 2,2,1 }; // linear separity
                MLPWatchdog nn = new WatchdogGenerator().Create(layers,1,new Sigmoid());
                nn.LearnFactor=0.9;
                nn.Momentum=0.5;
                var response= nn.BPWatched(new BPWDRequest(XOR,10000,0,0.001,10));
                Assert.IsTrue(response.Count<10); // should learn faster
                Assert.AreEqual(response.Last().Estimate,BPResponseEstimate.Success);

                //sanity check
                IsBetween(nn.Pulse(new double[] { 0,0 })[0],0,0.05);
                IsBetween(nn.Pulse(new double[] { 0,1 })[0],0.95,1);
                IsBetween(nn.Pulse(new double[] { 1,0 })[0],0.95,1);
                IsBetween(nn.Pulse(new double[] { 1,1 })[0],0,0.05);
            }
        }




    }
}
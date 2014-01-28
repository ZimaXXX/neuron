using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

using HAKGERSoft.Synapse;
using HAKGERSoft.Synapse.Tools;

namespace HAKGERSoft.Synapse.Tests {

    [TestFixture]
    public class SubSamplingTest {
        Link[] l1=null;
        Neuron[] l2=null;
        Bias Bias=null;
        SubSampling SbSg=null;
        int Weight=0;

        [TestFixtureSetUp]
        public void TSSetUp() {
            l1= Utility.Generate<Link>(() => new Link(),4704).ToArray();
            l2=Utility.Generate<Neuron>(() => new Neuron(new Sigmoid()),1176).ToArray();
            Func<double> wg= () => Weight++;
            Bias=new Bias(1);
            SbSg=new SubSampling(6);
            SbSg.Connect(l1,l2,Bias);
        }

        [Test]
        public void NeuronsCount() {
            Assert.AreEqual(l1.Length,4704);
            Assert.AreEqual(l2.Length,1176);
        }

        //todo add more tests




    }
}
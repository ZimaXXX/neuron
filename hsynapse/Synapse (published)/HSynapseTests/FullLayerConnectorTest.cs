using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

using HAKGERSoft.Synapse;
using HAKGERSoft.Synapse.Tools;

namespace HAKGERSoft.Synapse.Tests {

    [TestFixture]
    public class FullLayerConnectorTest {
        Link[] l1=null;
        Neuron[] l2=null;
        Bias Bias=null;
        FullLayerConnector Connector=null;

        [TestFixtureSetUp]
        public void TSSetUp() {
            l1= Utility.Generate<Link>(() => new Link(),2).ToArray();
            l2=Utility.Generate<Neuron>(() => new Neuron(new Sigmoid()),3).ToArray();
            Bias=new Bias(1);
            Connector=new FullLayerConnector();
            Connector.Connect(l1,l2,Bias);
        }

        [Test]
        public void FullScan(){
            Assert.IsTrue(l1[0].Next[0]==l2[0].Previous[0]);
            Assert.IsTrue(l1[0].Next[1]==l2[1].Previous[0]);
            Assert.IsTrue(l1[0].Next[2]==l2[2].Previous[0]);

            Assert.IsTrue(l1[1].Next[0]==l2[0].Previous[1]);
            Assert.IsTrue(l1[1].Next[1]==l2[1].Previous[1]);
            Assert.IsTrue(l1[1].Next[2]==l2[2].Previous[1]);

            Assert.IsTrue(l2[0].Previous[2]==Bias.Next[0]);
            Assert.IsTrue(l2[1].Previous[2]==Bias.Next[1]);
            Assert.IsTrue(l2[2].Previous[2]==Bias.Next[2]);

            Assert.IsTrue(l1.All(x => x.Next.Length==3));

            Assert.IsTrue(l2.All(x => x.Next==null));
            Assert.IsTrue(l2.All(x => x.Previous.Length==3));
        }




    }
}
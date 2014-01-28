using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace HAKGERSoft.Synapse.Tests {

    [TestFixture]
    public class ToolsTest {

        [Test]
        public void FillManyTest() {
            object[][] testarr = new object[3][];
            int inner = 4;
            for(int i=0; i<testarr.Length; i++)
                testarr[i] = new object[inner--];

            object[] manynulls = testarr.SelectMany(x => x).ToArray();
            // sanity check
            Assert.AreEqual(manynulls.Length, 9);
            Assert.IsTrue(manynulls.All(x => x==null));

            Utility.ExecCtor<object>(testarr);
            object[] objct = testarr.SelectMany(x => x).ToArray();
            Assert.AreEqual(objct.Length, 9);
            Assert.IsTrue(objct.All(x => x!=null && x.GetType()==typeof(object)));
 
        }








    }
}
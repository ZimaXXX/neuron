using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

using HAKGERSoft.Synapse;
using HAKGERSoft.Synapse.Tools;

namespace HAKGERSoft.Synapse.Tests {

    public class TestBase {

        public void IsBetween(double value,double lo, double hi) {
            Assert.Greater(value,lo);
            Assert.Greater(hi,value);
        }





    }
}
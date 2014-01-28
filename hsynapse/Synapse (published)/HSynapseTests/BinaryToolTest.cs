using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using HAKGERSoft.Synapse.Tools;

using HAKGERSoft.Synapse;

namespace HAKGERSoft.Synapse.Tests {

    [TestFixture]
    public class BinaryToolTest {
       
        [Test]
        public void IntToBinaryTest(){
            Assert.AreEqual(BinaryTool.IntToBinary(3,2),new int[] { 1,1 });
            Assert.AreEqual(BinaryTool.IntToBinary(3,8),new int[] { 1,1,0,0,0,0,0,0 });
            Assert.AreEqual(BinaryTool.IntToBinary(255,8),new int[] { 1,1,1,1,1,1,1,1 });
        }

        [Test]
        public void BinaryToIntTest(){
            Assert.AreEqual(BinaryTool.BinaryToInt(new int[] { 1,1 }),3);
            Assert.AreEqual(BinaryTool.BinaryToInt(new int[] { 1,1,0,0,0,0,0,0 }),3);
            Assert.AreEqual(BinaryTool.BinaryToInt(new int[] { 1,1,1,1,1,1,1,1 }),255);
        }

        [Test]
        public void GrayIntSwitchTest(){
            Assert.AreEqual(1, BinaryTool.GrayToInt(BinaryTool.IntToGray(1,3)));
            Assert.AreEqual(0,BinaryTool.GrayToInt(BinaryTool.IntToGray(0,1)));
            Assert.AreEqual(0,BinaryTool.GrayToInt(BinaryTool.IntToGray(0,10)));
            Assert.AreEqual(17,BinaryTool.GrayToInt(BinaryTool.IntToGray(17,5)));
            Assert.AreEqual(255,BinaryTool.GrayToInt(BinaryTool.IntToGray(255,8)));
            Assert.AreEqual(156,BinaryTool.GrayToInt(BinaryTool.IntToGray(156,20)));
        }





    }
}
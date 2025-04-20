using Microsoft.VisualStudio.TestTools.UnitTesting;
using ArrayMethods;
using System;
using System.Linq;
using System.Collections;

namespace UnitTests
{
    [TestClass]
    public class ArrayTests
    {
        public int[] array;
        public ArrayClass _arraytest;
        [TestInitialize]
        public void Setup()
        {
            array = new int[] { 2, 1, 3, 4, 6, 5, 7, 8, 9 };
            _arraytest = new ArrayClass();
            _arraytest.array = (int[])array.Clone();
        }
        [TestMethod]
        public void ArraySort_ShouldTestSort()
        {
            int[] expected = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            _arraytest.ArraySort();
            CollectionAssert.AreEqual(expected, _arraytest.array);
        }

        [TestMethod]
        public void ArrayFilter_ShouldFilt()
        {
            int[] expected = new int[] { 3, 6, 9 };
            _arraytest.ArrayFilter();
            CollectionAssert.AreEqual(expected, _arraytest.array);
        }

        [TestMethod]
        public void ArraySum_ShouldReturnCorrectSum()
        {
            int expectedSum = 2 + 1 + 3 + 4 + 6 + 5 + 7 + 8 + 9; // 45
            int actualSum = _arraytest.ArraySum();
            Assert.AreEqual(expectedSum, actualSum);
        }
    }
}

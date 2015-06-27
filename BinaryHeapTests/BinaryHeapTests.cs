using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using InformationRetrieval;

namespace BinaryHeapTests
{
    [TestClass]
    public class BinaryHeapTests
    {
        public TestContext TestContext { get; set; }


        [TestMethod]
        public void BuildHeap_InitialArrayIsNull_ThrowsArgumentNullException()
        {
            // arrange
            int[] array = null;
            var heap = new BinaryHeap<int>();

            // act
            try
            {
                heap.BuildHeap(array);
            }

            // assert
            catch (ArgumentNullException e)
            {
                StringAssert.Contains(e.Message, BinaryHeap<int>.InitializationFromNullArrayError);
                return;
            }
            Assert.Fail("No ArgumentNullException was thrown.");
        }

        [TestMethod]
        public void GetMax_GettingElementAtZeroIndex_MaxElementObtained()
        {
            // arrange
            int[] array = {1, 2, 3, 4, 5};
            int expectedMaxElement = 5;
            int actualMaxElement;
            var heap = new BinaryHeap<int>();
            heap.BuildHeap(array);
            
            // act
            actualMaxElement = heap.GetMax();

            // assert
            Assert.AreEqual(expectedMaxElement, actualMaxElement);
        }


        [TestMethod]
        public void GetMax_MaxElementObtained_NoMaxElementInHeap()
        {
            // arrange
            int[] array = {1, 2, 3, 4, 5};
            int actualMaxElement = 5;
            var heap = new BinaryHeap<int>();
            heap.BuildHeap(array);
            Type heapType = typeof(BinaryHeap<int>);
            FieldInfo field = heapType.GetField("list", BindingFlags.NonPublic | BindingFlags.Instance);
            var list = (List<int>) field.GetValue(heap);

            // act
            heap.GetMax();

            // assert
            for (int i = 0; i < heap.HeapSize; i++)
            {
                Assert.AreNotEqual(actualMaxElement, list[i]);
            }
        }

        [TestMethod]
        public void Add_AddingNewElement_ElementAddedToHeap()
        {
            // arrange
            int[] array = {1, 3, 4, 5};
            int newElement = 2;
            var heap = new BinaryHeap<int>();
            heap.BuildHeap(array);
            Type heapType = typeof(BinaryHeap<int>);
            FieldInfo field = heapType.GetField( "list", BindingFlags.NonPublic | BindingFlags.Instance);
            var list = (List<int>)field.GetValue(heap);

            // act
            heap.Add(newElement);

            // assert
            Assert.IsTrue(list.Contains(2));
        }


        [TestMethod]
        [DataSource("System.Data.OleDb",
            "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=\"..\\..\\TestData.mdb\"",
            "Table1", DataAccessMethod.Sequential)]
        public void BuildHeap_FromDataSourceTest()
        {
            var heap = new BinaryHeap<int>();

            int[] initialArray = TestContext.DataRow["Initial array"].ToString().Split(
                new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries).Select(
                n => Convert.ToInt32(n)).ToArray();

            List<int> expectedResult = TestContext.DataRow["Result array"].ToString().Split(
                new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries).Select(
                n => Convert.ToInt32(n)).ToList(); 

            var actualResult = new List<int>();
            

            heap.BuildHeap(initialArray);
            while (heap.HeapSize > 0)
            {
                actualResult.Add(heap.GetMax());
            }

            CollectionAssert.AreEqual(expectedResult, actualResult);
        }
    }
}

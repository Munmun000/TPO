using NUnit.Framework;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BTreeSelectionAlgorithm.Tests
{
    [TestFixture]
    public class BTreeTests
    {
        private const int Degree = 3;
        private BTree<int> _btree;

        [SetUp]
        public void Setup()
        {
            _btree = new BTree<int>(Degree);
        }

        [Test]
        public void Constructor_ValidDegree_CreatesInstance()
        {
            Assert.DoesNotThrow(() => new BTree<int>(2));
        }

        [Test]
        public void Constructor_InvalidDegree_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() => new BTree<int>(1));
        }

        [Test]
        public void Insert_SingleItem_RootContainsItem()
        {
            _btree.Build(new[] { 42 });
            var result = _btree.Select(x => true).ToList();
            Assert.That(1, Is.EqualTo(result.Count));
            Assert.That(42, Is.EqualTo(result[0]));
        }

        [Test]
        public void Select_WithCondition_ReturnsFilteredResults()
        {
            var data = new[] { 10, 20, 30, 40, 50 };
            _btree.Build(data);

            var result = _btree.Select(x => x > 25).ToList();

            Assert.That(3, Is.EqualTo(result.Count));
            CollectionAssert.Contains(result, 30);
            CollectionAssert.Contains(result, 40);
            CollectionAssert.Contains(result, 50);
        }

        [Test]
        public void Select_EmptyTree_ReturnsEmptyCollection()
        {
            var result = _btree.Select(x => true);
            CollectionAssert.IsEmpty(result);
        }

        [Test]
        public void Build_LargeDataset_DoesNotThrow()
        {
            var largeData = Enumerable.Range(1, 10000);
            Assert.DoesNotThrow(() => _btree.Build(largeData));
        }

        [Test]
        public void Select_WithMockCondition_CallsConditionForEachItem()
        {
            var mockCondition = new Mock<Func<int, bool>>();
            mockCondition.Setup(x => x(It.IsAny<int>())).Returns(true);

            _btree.Build(new[] { 1, 2, 3 });
            _btree.Select(mockCondition.Object).ToList();

            mockCondition.Verify(x => x(1), Times.Once);
            mockCondition.Verify(x => x(2), Times.Once);
            mockCondition.Verify(x => x(3), Times.Once);
        }

        [Test]
        public void Select_WithFailingCondition_ReturnsEmpty()
        {
            _btree.Build(new[] { 1, 2, 3 });
            var result = _btree.Select(x => false).ToList();
            CollectionAssert.IsEmpty(result);
        }

        [Test]
        public void Select_WithPartialCondition_ReturnsMatchingItems()
        {
            var data = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            _btree.Build(data);

            var result = _btree.Select(x => x % 2 == 0).ToList();

            Assert.That(5, Is.EqualTo(result.Count));
            CollectionAssert.AreEquivalent(new[] { 2, 4, 6, 8, 10 }, result);
        }

        [Test]
        public void Select_WithStringData_WorksCorrectly()
        {
            var stringTree = new BTree<string>(Degree);
            stringTree.Build(new[] { "apple", "banana", "cherry" });

            var result = stringTree.Select(x => x.StartsWith("a")).ToList();

            Assert.That(1, Is.EqualTo(result.Count));
            StringAssert.AreEqualIgnoringCase("apple", result[0]);
        }

        [Test]
        public void Select_WithMockEnumerable_OnlyEnumeratesOnce()
        {
            var mockSequence = new Mock<IEnumerable<int>>();
            mockSequence.Setup(x => x.GetEnumerator())
                       .Returns(() => ((IEnumerable<int>)new[] { 1, 2, 3 }).GetEnumerator());

            var tree = new BTree<int>(Degree);
            tree.Build(mockSequence.Object);

            mockSequence.Verify(x => x.GetEnumerator(), Times.Once);
        }
    }

    [TestFixture]
    public class SelectionAlgorithmTests
    {
        [Test]
        public void Select_WithMockSequence_ReturnsCorrectResults()
        {
            var mockSequence = new Mock<IEnumerable<int>>();
            mockSequence.Setup(x => x.GetEnumerator())
                       .Returns(() => ((IEnumerable<int>)new[] { 1, 2, 3, 4, 5 }).GetEnumerator());

            var result = SelectionAlgorithm.Select(mockSequence.Object, x => x > 3).ToList();

            CollectionAssert.AreEquivalent(new[] { 4, 5 }, result);
        }

        [Test]
        public void Select_WithEmptySequence_ReturnsEmpty()
        {
            var result = SelectionAlgorithm.Select(Enumerable.Empty<int>(), x => true);
            CollectionAssert.IsEmpty(result);
        }

        [Test]
        public void Select_WithNullSequence_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                SelectionAlgorithm.Select<int>(null, x => true));
        }

        [Test]
        public void Select_IntegrationTest_ReturnsCorrectSubset()
        {
            var data = Enumerable.Range(1, 100);
            var result = SelectionAlgorithm.Select(data, x => x % 10 == 0).ToList();

            Assert.That(10, Is.EqualTo(result.Count));
            CollectionAssert.Contains(result, 10);
            CollectionAssert.Contains(result, 100);
        }
    }
}
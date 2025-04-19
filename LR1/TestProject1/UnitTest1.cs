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
        private const int Degree = 2;
        private BTree<int> _btree;
        private Mock<BTree<int>.INodeCreator<int>> _mockNodeCreator;
        private Mock<IBTreeNode<int>> _mockRootNode;

        [SetUp]
        public void Setup()
        {
            _mockNodeCreator = new Mock<BTree<int>.INodeCreator<int>>();
            _mockRootNode = new Mock<IBTreeNode<int>>();

            _mockRootNode.SetupGet(x => x.Keys).Returns(new List<int>());
            _mockRootNode.SetupGet(x => x.Children).Returns(new List<IBTreeNode<int>>());
            _mockRootNode.SetupGet(x => x.IsLeaf).Returns(true);

            _mockNodeCreator.Setup(x => x.CreateNode()).Returns(() =>
            {
                var mockNode = new Mock<IBTreeNode<int>>();
                mockNode.SetupGet(n => n.Keys).Returns(new List<int>());
                mockNode.SetupGet(n => n.Children).Returns(new List<IBTreeNode<int>>());
                mockNode.SetupGet(n => n.IsLeaf).Returns(true);
                return mockNode.Object;
            });

            _btree = new BTree<int>(Degree, _mockRootNode.Object, _mockNodeCreator.Object);
        }

        [Test]
        public void Constructor_ValidDegree_CreatesInstance()
        {
            var creator = new DefaultNodeCreator<int>();
            var root = creator.CreateNode();
            Assert.DoesNotThrow(() => new BTree<int>(2, root, creator));
        }

        [Test]
        public void Constructor_InvalidDegree_ThrowsException()
        {
            var creator = new DefaultNodeCreator<int>();
            var root = creator.CreateNode();
            Assert.Throws<ArgumentException>(() => new BTree<int>(1, root, creator));
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
        public void Select_EmptyTree_ReturnsEmptyCollection()
        {
            var result = _btree.Select(x => true);
            CollectionAssert.IsEmpty(result);
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
        public void Build_WithMultipleItems_CreatesValidBTree()
        {
            // Arrange
            var creator = new DefaultNodeCreator<int>();
            var root = creator.CreateNode();
            var btree = new BTree<int>(Degree, root, creator);
            var sequence = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

            // Act
            btree.Build(sequence);
            var result = btree.Select(x => true).ToList();

            // Assert
            Assert.That(result, Has.Count.EqualTo(10));
            CollectionAssert.AreEquivalent(sequence, result);
        }

        [Test]
        public void Select_WithComplexCondition_ReturnsCorrectSubset()
        {
            // Arrange
            var creator = new DefaultNodeCreator<int>();
            var root = creator.CreateNode();
            var btree = new BTree<int>(Degree, root, creator);
            btree.Build(Enumerable.Range(1, 100));

            // Act
            var result = btree.Select(x => x % 3 == 0 && x % 5 == 0).ToList();

            // Assert
            Assert.That(result, Has.Count.EqualTo(6)); // 15, 30, 45, 60, 75, 90
            CollectionAssert.Contains(result, 15);
            CollectionAssert.Contains(result, 90);
            CollectionAssert.DoesNotContain(result, 10);
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
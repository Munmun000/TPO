using System;
using System.Collections.Generic;
using System.Linq;

namespace BTreeSelectionAlgorithm
{
    // Узел B-дерева
    public class BTreeNode<T> where T : IComparable<T>
    {
        public List<T> Keys { get; } = new List<T>();
        public List<BTreeNode<T>> Children { get; } = new List<BTreeNode<T>>();
        public bool IsLeaf => Children.Count == 0;
    }

    // B-дерево 
    public class BTree<T> where T : IComparable<T>
    {
        private readonly int _degree;
        private BTreeNode<T> _root;

        public BTree(int degree)
        {
            if (degree < 2)
                throw new ArgumentException("Degree must be at least 2", nameof(degree));
            _degree = degree;
            _root = new BTreeNode<T>();
        }

        // Сборка
        public void Build(IEnumerable<T> sequence)
        {
            if (sequence == null)
                throw new ArgumentNullException(nameof(sequence));

            foreach (var item in sequence)
            {
                Insert(item);
            }
        }

        // Добавление ключа в дерево
        private void Insert(T key)
        {
            var root = _root;
            if (root.Keys.Count == (2 * _degree) - 1)
            {
                var newRoot = new BTreeNode<T>();
                newRoot.Children.Add(root);
                SplitChild(newRoot, 0);
                _root = newRoot;
                InsertNonFull(newRoot, key);
            }
            else
            {
                InsertNonFull(root, key);
            }
        }

        // Поиск места для вставки
        private void InsertNonFull(BTreeNode<T> node, T key)
        {
            int i = node.Keys.Count - 1;
            if (node.IsLeaf)
            {
                while (i >= 0 && key.CompareTo(node.Keys[i]) < 0)
                {
                    i--;
                }
                node.Keys.Insert(i + 1, key);
            }
            else
            {
                while (i >= 0 && key.CompareTo(node.Keys[i]) < 0)
                {
                    i--;
                }
                i++;
                if (node.Children[i].Keys.Count == (2 * _degree) - 1)
                {
                    SplitChild(node, i);
                    if (key.CompareTo(node.Keys[i]) > 0)
                    {
                        i++;
                    }
                }
                InsertNonFull(node.Children[i], key);
            }
        }

        // Разделение потомка на два узла
        private void SplitChild(BTreeNode<T> parentNode, int childIndex)
        {
            var child = parentNode.Children[childIndex];
            var newNode = new BTreeNode<T>();
            parentNode.Keys.Insert(childIndex, child.Keys[_degree - 1]);
            parentNode.Children.Insert(childIndex + 1, newNode);

            newNode.Keys.AddRange(child.Keys.GetRange(_degree, _degree - 1));
            child.Keys.RemoveRange(_degree - 1, _degree);

            if (!child.IsLeaf)
            {
                newNode.Children.AddRange(child.Children.GetRange(_degree, _degree));
                child.Children.RemoveRange(_degree, _degree);
            }
        }

        // Поиск
        public IEnumerable<T> Select(Func<T, bool> condition)
        {
            return SelectInternal(_root, condition);
        }

        private IEnumerable<T> SelectInternal(BTreeNode<T> node, Func<T, bool> condition)
        {
            if (node == null) yield break;

            for (int i = 0; i < node.Keys.Count; i++)
            {
                if (!node.IsLeaf)
                {
                    foreach (var item in SelectInternal(node.Children[i], condition))
                    {
                        yield return item;
                    }
                }

                if (condition(node.Keys[i]))
                {
                    yield return node.Keys[i];
                }
            }

            if (!node.IsLeaf)
            {
                foreach (var item in SelectInternal(node.Children.Last(), condition))
                {
                    yield return item;
                }
            }
        }
    }

    public static class SelectionAlgorithm
    {
        public static IEnumerable<T> Select<T>(IEnumerable<T> sequence, Func<T, bool> condition) where T : IComparable<T>
        {
            if (sequence == null)
                throw new ArgumentNullException(nameof(sequence));
            if (condition == null)
                throw new ArgumentNullException(nameof(condition));

            const int degree = 100;
            var btree = new BTree<T>(degree);
            btree.Build(sequence);
            return btree.Select(condition);
        }
    }
}
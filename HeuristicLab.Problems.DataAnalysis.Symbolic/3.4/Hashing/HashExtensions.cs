#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
 *
 * This file is part of HeuristicLab.
 *
 * HeuristicLab is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * HeuristicLab is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with HeuristicLab. If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

using System;
using System.Linq;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  public static class SymbolicExpressionHashExtensions {
    public sealed class HashNode<T> : IComparable<HashNode<T>>, IEquatable<HashNode<T>> where T : class {
      public T Data;
      public int Arity;
      public int Size;
      public bool IsCommutative;

      public bool Enabled;
      public ulong HashValue;           // the initial (fixed) hash value for this individual node/data
      public ulong CalculatedHashValue; // the calculated hash value (taking into account the children hash values)

      public delegate void SimplifyAction(ref HashNode<T>[] nodes, int i);
      public SimplifyAction Simplify;

      //public IComparer<T> Comparer;

      public bool IsLeaf => Arity == 0;

      //public HashNode(IComparer<T> comparer) {
      //  Comparer = comparer;
      //}

      //public HashNode() { }

      public int CompareTo(HashNode<T> other) {
        return CalculatedHashValue.CompareTo(other.CalculatedHashValue);
      }

      public override string ToString() {
        return $"{Data} {Arity} {Size} {CalculatedHashValue} {Enabled}";
      }

      public bool Equals(HashNode<T> other) {
        return CalculatedHashValue.Equals(other.CalculatedHashValue);
      }

      public override bool Equals(object obj) {
        var other = obj as HashNode<T>;
        if (other != null)
          return Equals(other);
        return base.Equals(obj);
      }

      public override int GetHashCode() {
        return (int)CalculatedHashValue;
      }

      public static bool operator ==(HashNode<T> a, HashNode<T> b) {
        return a.Equals(b);
      }

      public static bool operator !=(HashNode<T> a, HashNode<T> b) {
        return !a.Equals(b);
      }
    }

    public static ulong ComputeHash<T>(this HashNode<T>[] nodes, int i, Func<byte[], ulong> hashFunction) where T : class {
      var node = nodes[i];
      const int size = sizeof(ulong);
      var hashes = new ulong[node.Arity + 1];
      var bytes = new byte[(node.Arity + 1) * size];

      for (int j = i - 1, k = 0; k < node.Arity; ++k, j -= 1 + nodes[j].Size) {
        hashes[k] = nodes[j].CalculatedHashValue;
      }
      hashes[node.Arity] = node.HashValue;
      Buffer.BlockCopy(hashes, 0, bytes, 0, bytes.Length);
      return hashFunction(bytes);
    }

    // set the enabled state for the whole subtree rooted at this node 
    public static void SetEnabled<T>(this HashNode<T>[] nodes, int i, bool enabled) where T : class {
      nodes[i].Enabled = enabled;
      for (int j = i - nodes[i].Size; j < i; ++j)
        nodes[j].Enabled = enabled;
    }

    public static HashNode<T>[] Simplify<T>(this HashNode<T>[] nodes, Func<byte[], ulong> hashFunction) where T : class {
      bool simplified = false;
      nodes = nodes.UpdateNodeSizes().Reduce().Sort(hashFunction);
      do {
        if (simplified) {
          simplified = false;
          nodes = nodes.Where(x => x.Enabled).ToArray().UpdateNodeSizes().Reduce().Sort(hashFunction);
        }

        for (int i = 0; i < nodes.Length; ++i) {
          var node = nodes[i];
          if (node.IsLeaf) {
            continue;
          }
          node.Simplify?.Invoke(ref nodes, i);
          for (int j = i - node.Size; j < i; ++j) {
            // detect if anything was simplified
            if (!nodes[j].Enabled) {
              simplified = true;
              break;
            }
          }
        }
      } while (simplified);
      return nodes.UpdateNodeSizes().Sort(hashFunction);
    }

    public static HashNode<T>[] Sort<T>(this HashNode<T>[] nodes, Func<byte[], ulong> hashFunction) where T : class {
      int sort(int a, int b) => nodes[a].CompareTo(nodes[b]);

      for (int i = 0; i < nodes.Length; ++i) {
        var node = nodes[i];

        if (node.IsLeaf) {
          continue;
        }

        if (node.IsCommutative) { // only sort when the argument order does not matter
          var arity = node.Arity;
          var size = node.Size;

          if (arity == size) { // all child nodes are terminals
            Array.Sort(nodes, i - size, size);
          } else { // i have some non-terminal children
            var sorted = new HashNode<T>[size];
            var indices = new int[node.Arity];
            for (int j = i - 1, k = 0; k < node.Arity; j -= 1 + nodes[j].Size, ++k) {
              indices[k] = j;
            }
            Array.Sort(indices, sort);

            int idx = 0;
            foreach (var j in indices) {
              var child = nodes[j];
              if (!child.IsLeaf) { // must copy complete subtree
                Array.Copy(nodes, j - child.Size, sorted, idx, child.Size);
                idx += child.Size;
              }
              sorted[idx++] = nodes[j];
            }
            Array.Copy(sorted, 0, nodes, i - size, size);
          }
        }
        node.CalculatedHashValue = nodes.ComputeHash(i, hashFunction);
      }
      return nodes;
    }

    /// <summary>
    /// Get a function node's child indicest
    /// </summary>
    /// <typeparam name="T">The data type encapsulated by a hash node</typeparam>
    /// <param name="nodes">An array of hash nodes with up-to-date node sizes</param>
    /// <param name="i">The index in the array of hash nodes of the node whose children we want to iterate</param>
    /// <returns>An array containing child indices</returns>
    public static int[] IterateChildren<T>(this HashNode<T>[] nodes, int i) where T : class {
      var node = nodes[i];
      var arity = node.Arity;
      var children = new int[arity];
      var idx = i - 1;
      for (int j = 0; j < arity; ++j) {
        children[j] = idx;
        idx -= 1 + nodes[idx].Size;
      }
      return children;
    }

    public static HashNode<T>[] UpdateNodeSizes<T>(this HashNode<T>[] nodes) where T : class {
      for (int i = 0; i < nodes.Length; ++i) {
        var node = nodes[i];
        if (node.IsLeaf) {
          node.Size = 0;
          continue;
        }
        node.Size = node.Arity;

        for (int j = i - 1, k = 0; k < node.Arity; j -= 1 + nodes[j].Size, ++k) {
          node.Size += nodes[j].Size;
        }
      }
      return nodes;
    }

    public static HashNode<T>[] Reduce<T>(this HashNode<T>[] nodes) where T : class {
      int count = 0;
      for (int i = 0; i < nodes.Length; ++i) {
        var node = nodes[i];
        if (node.IsLeaf || !node.IsCommutative) {
          continue;
        }

        var arity = node.Arity;
        for (int j = i - 1, k = 0; k < arity; j -= 1 + nodes[j].Size, ++k) {
          if (node.HashValue == nodes[j].HashValue) {
            nodes[j].Enabled = false;
            node.Arity += nodes[j].Arity - 1;
            ++count;
          }
        }
      }
      if (count == 0)
        return nodes;

      var reduced = new HashNode<T>[nodes.Length - count];
      var idx = 0;
      foreach (var node in nodes) {
        if (node.Enabled) { reduced[idx++] = node; }
      }
      return reduced.UpdateNodeSizes();
    }
  }
}

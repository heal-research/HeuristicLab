#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.LinearLinkageEncoding {
  [Item("LinearLinkage", "Represents an LLE grouping of items.")]
  [StorableClass]
  public sealed class LinearLinkage : IntArray {

    [StorableConstructor]
    private LinearLinkage(bool deserializing) : base(deserializing) { }
    private LinearLinkage(LinearLinkage original, Cloner cloner) : base(original, cloner) { }
    public LinearLinkage() { }
    public LinearLinkage(int length) : base(length) { }
    public LinearLinkage(int[] elements) : base(elements) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new LinearLinkage(this, cloner);
    }

    /// <summary>
    /// This method parses the encoded array and calculates the membership of
    /// each element to the groups. It starts at the lowest element.
    /// </summary>
    /// <remarks>
    /// Runtime complexity of this method is O(n) where n is the length of the
    /// array.
    /// </remarks>
    /// <returns>An enumeration of all groups.</returns>
    public IEnumerable<List<int>> GetGroups() {
      var len = array.Length;
      var remaining = new HashSet<int>(Enumerable.Range(0, len));
      // iterate from lowest to highest index
      for (var i = 0; i < len; i++) {
        if (!remaining.Contains(i)) continue;
        var group = new List<int> { i };
        remaining.Remove(i);
        var next = array[i];
        if (next != i) {
          int prev;
          do {
            group.Add(next);
            if (!remaining.Remove(next))
              throw new ArgumentException("Array is malformed and does not represent a valid LLE forward encoding.");
            prev = next;
            next = array[next];
          } while (next != prev);
        }
        yield return group;
      }
    }

    /// <summary>
    /// This method parses the encoded array and gathers all elements
    /// that belong to the same group as element <paramref name="index"/>.
    /// </summary>
    /// <param name="index">The element whose group should be returned.
    /// </param>
    /// <returns>The element at <paramref name="index"/> and all other
    /// elements in the same group.</returns>
    public IEnumerable<int> GetGroup(int index) {
      foreach (var n in GetGroupForward(index))
        yield return n;
      // the element index has already been yielded
      foreach (var n in GetGroupBackward(index).Skip(1))
        yield return n;
    }

    /// <summary>
    /// This method parses the encoded array and gathers the element
    /// <paramref name="index"/> as well as subsequent elements that
    /// belong to the same group.
    /// </summary>
    /// <param name="index">The element from which subsequent (having a
    /// larger number) elements in the group should be returned.
    /// </param>
    /// <returns>The element <paramref name="index"/> and all subsequent
    /// elements in the same group.</returns>
    public IEnumerable<int> GetGroupForward(int index) {
      yield return index;
      var next = array[index];
      if (next == index) yield break;
      int prev;
      do {
        yield return next;
        prev = next;
        next = array[next];
      } while (next != prev);
    }

    /// <summary>
    /// This method parses the encoded array and gathers the element
    /// given <paramref name="index"/> as well as preceeding elements that
    /// belong to the same group.
    /// </summary>
    /// <remarks>
    /// Warning, this code has performance O(index) as the array has to
    /// be fully traversed backwards from the given index.
    /// </remarks>
    /// <param name="index">The element from which preceeding (having a
    /// smaller number) elements in the group should be returned.
    /// </param>
    /// <returns>The element <paramref name="index"/> and all preceeding
    /// elements in the same group.</returns>
    public IEnumerable<int> GetGroupBackward(int index) {
      yield return index;
      var next = array[index];
      // return preceding elements in group
      for (var prev = index - 1; prev >= 0; prev--) {
        if (array[prev] != next) continue;
        next = prev;
        yield return next;
      }
    }

    /// <summary>
    /// This method translates an enumeration of groups into the underlying
    /// array representation.
    /// </summary>
    /// <remarks>
    /// Throws an ArgumentException when there is an element assigned to
    /// multiple groups or elements that are not assigned to any group.
    /// </remarks>
    /// <param name="grouping">The grouping of the elements, each element must
    /// be part of exactly one group.</param>
    public void SetGroups(IEnumerable<IEnumerable<int>> grouping) {
      var len = array.Length;
      var remaining = new HashSet<int>(Enumerable.Range(0, len));
      foreach (var group in grouping) {
        var prev = -1;
        foreach (var g in group.OrderBy(x => x)) {
          if (prev >= 0) array[prev] = g;
          prev = g;
          if (!remaining.Remove(prev))
            throw new ArgumentException(string.Format("Element {0} is contained at least twice.", prev), "grouping");
        }
        if (prev >= 0) array[prev] = prev;
      }
      if (remaining.Count > 0)
        throw new ArgumentException(string.Format("Elements are not assigned a group: {0}", string.Join(", ", remaining)));
    }

    /// <summary>
    /// Performs a check whether the array represents a valid LLE encoding.
    /// </summary>
    /// <remarks>
    /// The runtime complexity of this method is O(n) where n is the length of
    /// the array.
    /// </remarks>
    /// <returns>True if the encoding is valid.</returns>
    public bool Validate() {
      var len = array.Length;
      var remaining = new HashSet<int>(Enumerable.Range(0, len));
      for (var i = 0; i < len; i++) {
        if (!remaining.Contains(i)) continue;
        remaining.Remove(i);
        var next = array[i];
        if (next == i) continue;
        int prev;
        do {
          if (!remaining.Remove(next)) return false;
          prev = next;
          next = array[next];
        } while (next != prev);
      }
      return remaining.Count == 0;
    }

    /// <summary>
    /// This method flattens tree structures that may be present in groups.
    /// These tree structures may be created by e.g. merging two groups by
    /// linking one end node to the end node of another.
    /// Consider following 1-based index array: 6, 6, 7, 5, 5, 8, 8, 8, 9.
    /// This results in the following tree structure for group 8:
    ///      8
    ///    /   \
    ///   6     7
    ///  / \    |
    /// 1   2   3
    /// After this operation the array will be 2, 3, 6, 5, 5, 7, 8, 8, 9.
    /// Representing a tree with one branch: 1 -> 2 -> 3 -> 6 -> 7 -> 8
    /// </summary>
    /// <remarks>
    /// The method first converts the array to LLE-e format and then
    /// linearizes the links. This requires two passes of the whole array
    /// as well as a dictionary to hold the smallest index of each group.
    /// The runtime complexity is O(n).
    /// 
    /// The method assumes that there are no back links present.
    /// </remarks>
    public void LinearizeTreeStructures() {
      // Step 1: Convert the array into LLE-e
      ToLLEeInplace(array);
      // Step 2: For all groups linearize the links
      FromLLEe(array);
    }

    /// <summary>
    /// Creates a copy of the underlying array and turns it into LLE-e.
    /// </summary>
    /// <remarks>
    /// LLE-e is a special format where each element points to the
    /// ending item of a group.
    /// The LLE representation 2, 3, 5, 6, 5, 7, 8, 8 would become 
    /// 5, 5, 5, 8, 5, 8, 8, 8 in LLE-e.
    /// 
    /// This operation runs in O(n) time.
    /// </remarks>
    /// <returns>An integer array in LLE-e representation</returns>
    public int[] ToLLEe() {
      var result = (int[])array.Clone();
      ToLLEeInplace(result);
      return result;
    }

    private void ToLLEeInplace(int[] a) {
      var length = a.Length;
      for (var i = length - 1; i >= 0; i--) {
        if (array[i] == i) a[i] = i;
        else a[i] = a[a[i]];
      }
    }

    /// <summary>
    /// Parses an LLE-e representation and modifies the underlying array
    /// so that it is in LLE representation.
    /// </summary>
    /// <remarks>
    /// This operation runs in O(n) time, but requires additional memory
    /// in form of a dictionary.
    /// </remarks>
    /// <param name="llee">The LLE-e representation</param>
    public void FromLLEe(int[] llee) {
      var length = array.Length;
      var groups = new Dictionary<int, int>();
      for (var i = length - 1; i >= 0; i--) {
        if (llee[i] == i) {
          array[i] = i;
          groups[i] = i;
        } else {
          var g = llee[i];
          array[i] = groups[g];
          groups[g] = i;
        }
      }
    }
  }
}

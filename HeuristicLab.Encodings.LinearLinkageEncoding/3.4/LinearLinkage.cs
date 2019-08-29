#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Optimization;
using HEAL.Attic;

namespace HeuristicLab.Encodings.LinearLinkageEncoding {
  [Item("LinearLinkage", "Represents an LLE grouping of items.")]
  [StorableType("91492281-3335-4F5A-82BA-BA76142DAD2D")]
  public sealed class LinearLinkage : IntArray, IEncodedSolution {

    [StorableConstructor]
    private LinearLinkage(StorableConstructorFlag _) : base(_) { }
    private LinearLinkage(LinearLinkage original, Cloner cloner) : base(original, cloner) { }
    public LinearLinkage() { }

    private LinearLinkage(int length) : base(length) { }
    private LinearLinkage(int[] elements) : base(elements) { }

    /// <summary>
    /// Create a new LinearLinkage object where every element is in a seperate group.
    /// </summary>
    public static LinearLinkage SingleElementGroups(int length) {
      var elements = new int[length];
      for (var i = 0; i < length; i++) {
        elements[i] = i;
      }
      return new LinearLinkage(elements);
    }

    /// <summary>
    /// Create a new LinearLinkage object from an int[] in LLE
    /// </summary>
    /// <remarks>
    /// This operation checks if the argument is a well formed LLE
    /// and throws an ArgumentException otherwise.
    /// </remarks>
    /// <exception cref="ArgumentException">If <paramref name="lle"/> does not represent a valid LLE array.</exception>
    /// <param name="lle">The LLE representation</param>
    /// <returns>The linear linkage encoding in LLE format (with forward-links).</returns>
    public static LinearLinkage FromForwardLinks(int[] lle) {
      if (!Validate(lle))
        throw new ArgumentException("Array is malformed and does not represent a valid LLE forward encoding.", "elements");
      return new LinearLinkage(lle);
    }

    /// <summary>
    /// Create a new LinearLinkage object by parsing a LLE-b representation
    /// and modifing the underlying array so that it is in LLE representation.
    /// </summary>
    /// <remarks>
    /// This operation runs in O(n) time, the parameter <paramref name="lleb"/> is not modified.
    /// </remarks>
    /// <exception cref="ArgumentException">If <paramref name="lleb"/> does not represent a valid LLE-b array.</exception>
    /// <param name="lleb">The LLE-b representation (LLE with back-links)</param>
    /// <returns>The linear linkage encoding in LLE format (with forward-links).</returns>
    public static LinearLinkage FromBackLinks(int[] lleb) {
      var result = new LinearLinkage(lleb.Length);
      for (var i = lleb.Length - 1; i > 0; i--) {
        if (lleb[i] == i) {
          if (result[i] == 0) result[i] = i;
          continue;
        }
        result[lleb[i]] = i;
        if (result[i] == 0) result[i] = i;
      }
      if (!Validate(result.array))
        throw new ArgumentException("Array is malformed and does not represent a valid LLE-b encoding (with back-links).", "lleb");
      return result;
    }

    /// <summary>
    /// Create a new LinearLinkage object by parsing an LLE-e representation
    /// and modifing the underlying array so that it is in LLE representation.
    /// </summary>
    /// <remarks>
    /// This operation runs in O(n) time, but requires additional memory
    /// in form of a int[].
    /// </remarks>
    /// <param name="llee">The LLE-e representation</param>
    /// <returns>The linear linkage encoding in LLE format (with forward-links).</returns>
    public static LinearLinkage FromEndLinks(int[] llee) {
      var result = new LinearLinkage(llee.Length);
      result.SetEndLinks(llee);
      return result;
    }

    /// <summary>
    /// Create a new LinearLinkage object by translating
    /// an enumeration of groups into the underlying array representation.
    /// </summary>
    /// <remarks>
    /// Throws an ArgumentException when there is an element assigned to
    /// multiple groups or elements that are not assigned to any group.
    /// </remarks>
    /// <param name="grouping">The grouping of the elements, each element must
    /// be part of exactly one group.</param>
    public static LinearLinkage FromGroups(int length, IEnumerable<IEnumerable<int>> grouping) {
      var result = new LinearLinkage(length);
      result.SetGroups(grouping);
      return result;
    }

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
    /// <exception cref="InvalidOperationException">If this object is not vaild LLE.</exception>
    /// <returns>An enumeration of all groups.</returns>
    public IEnumerable<List<int>> GetGroups() {
      var len = array.Length;
      var used = new bool[len];
      for (var i = 0; i < len; i++) {
        if (used[i]) continue;
        var curr = i;
        var next = array[curr];
        var group = new List<int> { curr };
        while (next > curr && next < len && !used[next]) {
          used[curr] = true;
          curr = next;
          next = array[next];
          group.Add(curr);
        }
        if (curr != next) throw new InvalidOperationException("Array is malformed and does not represent a valid LLE forward encoding.");
        used[curr] = true;
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
    /// <exception cref="ArgumentException">If <paramref name="grouping"/> cannot be converted
    /// to a valid LLE representation. For instance, because elements are too big or too small,
    /// or they're contained in multiple groups, or there are elements not assigned to any group.
    /// </exception>
    public void SetGroups(IEnumerable<IEnumerable<int>> grouping) {
      var len = array.Length;
      var used = new bool[len];
      foreach (var group in grouping) {
        var prev = -1;
        foreach (var g in group.OrderBy(x => x)) {
          if (g < prev || g >= len) throw new ArgumentException(string.Format("Element {0} is bigger than {1} or smaller than 0.", g, len - 1), "grouping");
          if (prev >= 0) array[prev] = g;
          prev = g;
          if (used[prev]) {
            throw new ArgumentException(string.Format("Element {0} is contained at least twice.", prev), "grouping");
          }
          used[prev] = true;
        }
        array[prev] = prev;
      }
      if (!used.All(x => x))
        throw new ArgumentException(string.Format("Elements are not assigned a group: {0}", string.Join(", ", used.Select((x, i) => new { x, i }).Where(x => !x.x).Select(x => x.i))));
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
      return Validate(array);
    }

    private static bool Validate(int[] array) {
      var len = array.Length;
      var used = new bool[len];
      for (var i = 0; i < len; i++) {
        if (used[i]) continue;
        var curr = i;
        var next = array[curr];
        while (next > curr && next < len && !used[next]) {
          used[curr] = true;
          curr = next;
          next = array[next];
        }
        if (curr != next) return false;
        used[curr] = true;
      }
      return true;
    }

    /// <summary>
    /// This method flattens tree structures that may be present in groups.
    /// These tree structures may be created by e.g. merging two groups by
    /// linking one end node to the end node of another.
    /// Consider following array: 5, 5, 6, 4, 4, 7, 7, 7, 8.
    /// This results in the following tree structure for group 7:
    ///      7
    ///    /   \
    ///   5     6
    ///  / \    |
    /// 0   1   2
    /// After this operation the array will be 1, 2, 5, 4, 4, 6, 7, 7, 8.
    /// Representing a tree with one branch: 0 -> 1 -> 2 -> 5 -> 6 -> 7.
    /// </summary>
    /// <remarks>
    /// The method first converts the array to LLE-e format and then
    /// linearizes the links. This requires two passes of the whole array
    /// as well as another copy of the underlying array.
    /// The runtime complexity is O(n).
    /// 
    /// The method assumes that there are no back links present.
    /// </remarks>
    public void LinearizeTreeStructures() {
      // Step 1: Convert the array into LLE-e
      ToEndLinksInplace(array);
      // Step 2: For all groups linearize the links
      SetEndLinks(array);
    }

    /// <summary>
    /// Creates a copy of the underlying array and turns it into LLE-e.
    /// </summary>
    /// <remarks>
    /// LLE-e is a special format where each element points to the
    /// ending item of a group.
    /// The LLE representation 1, 2, 4, 5, 4, 6, 7, 7 would become 
    /// 4, 4, 4, 7, 4, 7, 7, 7 in LLE-e.
    /// 
    /// This operation runs in O(n) time.
    /// </remarks>
    /// <exception cref="ArgumentException">In case, this object does not
    /// represent a valid LLE encoding.
    /// </exception>
    /// <returns>An integer array in LLE-e representation</returns>
    public int[] ToEndLinks() {
      var result = (int[])array.Clone();
      ToEndLinksInplace(result);
      return result;
    }

    private static void ToEndLinksInplace(int[] array) {
      var length = array.Length;
      for (var i = length - 1; i >= 0; i--) {
        var next = array[i];
        if (next > i) {
          array[i] = array[next];
        } else if (next < i) {
          throw new ArgumentException("Array is malformed and does not represent a valid LLE encoding.", "array");
        }
      }
    }

    /// <summary>
    /// Parses an LLE-e representation and modifies the underlying array
    /// so that it is in LLE representation.
    /// </summary>
    /// <remarks>
    /// This operation runs in O(n) time, but requires additional memory
    /// in form of a int[].
    /// </remarks>
    /// <param name="llee">The LLE-e representation</param>
    /// <exception cref="ArgumentException">
    /// If <paramref name="llee"/> does not contain a valid LLE-e representation or
    /// has a different length to the given instance.
    /// </exception>
    public void SetEndLinks(int[] llee) {
      var length = array.Length;
      if (length != llee.Length) {
        throw new ArgumentException(string.Format("Expected length {0} but length was {1}", length, llee.Length), "llee");
      }
      // If we are ok with mutating llee we can avoid this clone
      var lookup = (int[])llee.Clone();
      for (var i = length - 1; i >= 0; i--) {
        var end = llee[i];
        if (end == i) {
          array[i] = end;
        } else if (end > i && end < length) {
          array[i] = lookup[end];
          lookup[end] = i;
        } else {
          throw new ArgumentException("Array is malformed and does not represent a valid LLE-e end encoding.", "llee");
        }
      }
    }

    /// <summary>
    /// Creates a copy of the underlying array and turns it into LLE-b.
    /// </summary>
    /// <remarks>
    /// LLE-b is a special format where each element points to the
    /// predecessor instead of the successor.
    /// The LLE representation 1, 2, 4, 5, 4, 6, 7, 7
    /// would become           0, 0, 1, 3, 2, 3, 5, 6 in LLE-b.
    /// 
    /// This operation runs in O(n) time.
    /// </remarks>
    /// <returns>An integer array in LLE-b representation</returns>
    public int[] ToBackLinks() {
      var result = new int[array.Length];
      var zeroLink = array[0];
      for (var i = 0; i < array.Length; i++) {
        if (array[i] == i) {
          if (result[i] == 0 && i != zeroLink) result[i] = i;
          continue;
        }
        result[array[i]] = i;
        if (result[i] == 0 && i != zeroLink) result[i] = i;
      }
      return result;
    }
  }
}

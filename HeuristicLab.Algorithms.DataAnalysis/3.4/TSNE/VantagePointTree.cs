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

//Code is based on an implementation from Laurens van der Maaten

/*
*
* Copyright (c) 2014, Laurens van der Maaten (Delft University of Technology)
* All rights reserved.
*
* Redistribution and use in source and binary forms, with or without
* modification, are permitted provided that the following conditions are met:
* 1. Redistributions of source code must retain the above copyright
*    notice, this list of conditions and the following disclaimer.
* 2. Redistributions in binary form must reproduce the above copyright
*    notice, this list of conditions and the following disclaimer in the
*    documentation and/or other materials provided with the distribution.
* 3. All advertising materials mentioning features or use of this software
*    must display the following acknowledgement:
*    This product includes software developed by the Delft University of Technology.
* 4. Neither the name of the Delft University of Technology nor the names of
*    its contributors may be used to endorse or promote products derived from
*    this software without specific prior written permission.
*
* THIS SOFTWARE IS PROVIDED BY LAURENS VAN DER MAATEN ''AS IS'' AND ANY EXPRESS
* OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
* OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO
* EVENT SHALL LAURENS VAN DER MAATEN BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
* SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
* PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR
* BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
* CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING
* IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY
* OF SUCH DAMAGE.
*
*/
#endregion

using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Collections;
using HeuristicLab.Random;

namespace HeuristicLab.Algorithms.DataAnalysis {
  /// <summary>
  /// Vantage point tree  (or VP tree) is a metric tree that segregates data in a metric space by choosing 
  /// a position in the space (the "vantage point") and partitioning the data points into two parts: 
  /// those points that are nearer to the vantage point than a threshold, and those points that are not. 
  /// By recursively applying this procedure to partition the data into smaller and smaller sets, a tree 
  /// data structure is created where neighbors in the tree are likely to be neighbors in the space.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public class VantagePointTree<T> {
    private readonly List<T> items;
    private readonly Node root;
    private readonly IDistance<T> distance;

    public VantagePointTree(IDistance<T> distance, IEnumerable<T> items) : base() {
      root = null;
      this.distance = distance;
      this.items = items.Select(x => x).ToList();
      root = BuildFromPoints(0, this.items.Count);
    }

    /// <summary>
    /// provides the k-nearest neighbours to a certain target element
    /// </summary>
    /// <param name="target">The target element</param>
    /// <param name="k">How many neighbours</param>
    /// <param name="results">The nearest neighbouring elements</param>
    /// <param name="distances">The distances form the target corresponding to the neighbouring elements</param>
    public void Search(T target, int k, out IList<T> results, out IList<double> distances) {
      var heap = new PriorityQueue<double, IndexedItem<double>>(double.MaxValue, double.MinValue, k);
      var tau = double.MaxValue;
      Search(root, target, k, heap, ref tau);
      var res = new List<T>();
      var dist = new List<double>();
      while (heap.Size > 0) {
        res.Add(items[heap.PeekMinValue().Index]);// actually max distance
        dist.Add(heap.PeekMinValue().Value);
        heap.RemoveMin();
      }
      res.Reverse();
      dist.Reverse();
      results = res;
      distances = dist;
    }

    private void Search(Node node, T target, int k, PriorityQueue<double, IndexedItem<double>> heap, ref double tau) {
      if (node == null) return;
      var dist = distance.Get(items[node.index], target);
      if (dist < tau) {
        if (heap.Size == k) heap.RemoveMin();   // remove furthest node from result list (if we already have k results) 
        heap.Insert(-dist, new IndexedItem<double>(node.index, dist));
        if (heap.Size == k) tau = heap.PeekMinValue().Value;
      }
      if (node.left == null && node.right == null) return;

      if (dist < node.threshold) {
        if (dist - tau <= node.threshold) Search(node.left, target, k, heap, ref tau);   // if there can still be neighbors inside the ball, recursively search left child first
        if (dist + tau >= node.threshold) Search(node.right, target, k, heap, ref tau);  // if there can still be neighbors outside the ball, recursively search right child
      } else {
        if (dist + tau >= node.threshold) Search(node.right, target, k, heap, ref tau);  // if there can still be neighbors outside the ball, recursively search right child first
        if (dist - tau <= node.threshold) Search(node.left, target, k, heap, ref tau);   // if there can still be neighbors inside the ball, recursively search left child
      }
    }

    private Node BuildFromPoints(int lower, int upper) {
      if (upper == lower)      // indicates that we're done here!
        return null;

      // Lower index is center of current node
      var node = new Node { index = lower };
      var r = new MersenneTwister(); //outer behaviour does not change with the random seed => no need to take the IRandom from the algorithm 
      if (upper - lower <= 1) return node;

      // if we did not arrive at leaf yet
      // Choose an arbitrary point and move it to the start
      var i = (int)(r.NextDouble() * (upper - lower - 1)) + lower;
      items.Swap(lower, i);

      // Partition around the median distance
      var median = (upper + lower) / 2;
      items.PartialSort(lower + 1, upper - 1, median, distance.GetDistanceComparer(items[lower]));

      // Threshold of the new node will be the distance to the median
      node.threshold = distance.Get(items[lower], items[median]);

      // Recursively build tree
      node.index = lower;
      node.left = BuildFromPoints(lower + 1, median);
      node.right = BuildFromPoints(median, upper);

      // Return result
      return node;
    }

    private sealed class Node {
      public int index;
      public double threshold;
      public Node left;
      public Node right;

      internal Node() {
        index = 0;
        threshold = 0;
        left = null;
        right = null;
      }
    }
  }
}

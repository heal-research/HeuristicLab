#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Algorithms.DataAnalysis {

  // Code Taken from branch: RoutePlanning, Heap4
  // implementation based on C++ version from Peter Sanders
  // http://www.mpi-inf.mpg.de/~sanders/programs/spq/
  public sealed class PriorityQueue<TK, TV> where TK : IComparable {
    private class KNElement {
      public TK Key { get; set; }
      public TV Value { get; set; }
    }

    private readonly int capacity;
    private int size; // index of last used element
    private int finalLayerSize; // size of first layer with free space
    private int finalLayerDist; // distance to end of layer
                                //private KNElement[] rawData;
    private readonly KNElement[] data;    // aligned version of rawData

    public PriorityQueue(TK supremum, TK infimum, int cap) {
      capacity = cap;
      data = Enumerable.Range(0, capacity + 4).Select(x => new KNElement()).ToArray();

      data[0].Key = infimum; // sentinel
      data[capacity + 1].Key = supremum;
      data[capacity + 2].Key = supremum;
      data[capacity + 3].Key = supremum;
      Reset();
    }

    public int Size {
      get { return size; }
    }

    public TK Supremum {
      get { return data[capacity + 1].Key; }
    }

    public KeyValuePair<TK, TV> PeekMin() {
      if (size == 0) throw new InvalidOperationException("Heap is empty");
      return new KeyValuePair<TK, TV>(data[1].Key, data[1].Value);
    }

    public TK PeekMinKey() {
      return data[1].Key;
    }

    public TV PeekMinValue() {
      return data[1].Value;
    }

    public void RemoveMin() {
      // first move up elements on a min-path
      var hole = 1;
      var succ = 2;
      var layerSize = 4;
      var layerPos = 0;
      var sz = size;
      size = sz - 1;
      finalLayerDist++;
      if (finalLayerDist == finalLayerSize) {
        finalLayerSize >>= 2;
        finalLayerDist = 0;
      }

      while (succ < sz) {
        var minKey = data[succ].Key;
        var delta = 0;

        for (var i = 1; i <= 3; i++) {
          var otherKey = data[succ + i].Key;
          if (otherKey.CompareTo(minKey) >= 0) continue;
          minKey = otherKey;
          delta = i;
        }
        
        succ += delta;
        layerPos += delta;

        // move min successor up
        data[hole].Key = minKey;
        data[hole].Value = data[succ].Value;

        // step to next layer
        hole = succ;
        succ = succ - layerPos + layerSize; // beginning of next layer
        layerPos <<= 2;
        succ += layerPos; // now correct value
        layerSize <<= 2;
      }

      // bubble up rightmost element
      var bubble = data[sz].Key;
      layerSize >>= 2; // now size of hole's layer
      layerPos >>= 2; // now pos of hole within its layer

      var layerDist = layerSize - layerPos - 1; // hole's dist to end of layer
      var pred = hole + layerDist - layerSize; // end of pred's layer for now
      layerSize >>= 2; // now size of pred's layer
      layerDist >>= 2; // now pred's pos in layer
      pred = pred - layerDist; // finally preds index

      while (data[pred].Key.CompareTo(bubble) > 0) {  // must terminate since inf at root
        data[hole].Key = data[pred].Key;
        data[hole].Value = data[pred].Value;
        hole = pred;
        pred = hole + layerDist - layerSize; // end of hole's layer for now
        layerSize >>= 2;
        layerDist >>= 2;
        pred = pred - layerDist; // finally preds index
      }

      // finally move data to hole
      data[hole].Key = bubble;
      data[hole].Value = data[sz].Value;
      data[sz].Key = Supremum; // mark as deleted
    }

    public void Insert(TK key, TV value) {
      var layerSize = finalLayerSize;
      var layerDist = finalLayerDist;
      finalLayerDist--;

      if (finalLayerDist == -1) { // layer full
                                  // start next layer
        finalLayerSize <<= 2;
        finalLayerDist = finalLayerSize - 1;
      }

      size++;

      var hole = size;
      var pred = hole + layerDist - layerSize; // end of pred's layer for now
      layerSize >>= 2; // now size of pred's layer
      layerDist >>= 2;
      pred = pred - layerDist; // finally preds index
      var predKey = data[pred].Key;
      while (predKey.CompareTo(key) > 0) {
        data[hole].Key = predKey;
        data[hole].Value = data[pred].Value;
        hole = pred;
        pred = hole + layerDist - layerSize; // end of pred's layer for now
        layerSize >>= 2; // now isze of pred's layer
        layerDist >>= 2;
        pred = pred - layerDist; // finally preds index
        predKey = data[pred].Key;
      }

      // finally move data to hole
      data[hole].Key = key;
      data[hole].Value = value;
    }

    // reset size to 0 and fill data array with sentinels
    private void Reset() {
      size = 0;
      finalLayerSize = 1;
      finalLayerDist = 0;
      var sup = Supremum;
      var cap = capacity;
      for (var i = 1; i <= cap; i++) {
        data[i].Key = sup;
      }
    }
  }
}

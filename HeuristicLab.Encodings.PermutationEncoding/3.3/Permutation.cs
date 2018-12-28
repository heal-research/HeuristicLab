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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HEAL.Fossil;

namespace HeuristicLab.Encodings.PermutationEncoding {
  [Item("Permutation", "Represents a permutation of integer values.")]
  [StorableType("FBBCFA53-C1AE-4069-907B-99C720F5AC51")]
  public class Permutation : IntArray {
    [Storable]
    private PermutationTypes permutationType;
    /// <summary>
    /// Gets the type of the permutation (see <see cref="PermutationType"/>).
    /// </summary>
    public PermutationTypes PermutationType {
      get { return permutationType; }
      set {
        bool changed = (permutationType != value);
        permutationType = value;
        if (changed) OnPermutationTypeChanged();
      }
    }

    [StorableConstructor]
    protected Permutation(StorableConstructorFlag _) : base(_) { }
    protected Permutation(Permutation original, Cloner cloner)
      : base(original, cloner) {
      this.permutationType = original.permutationType;
    }
    public Permutation() : this(PermutationTypes.RelativeUndirected) { }
    public Permutation(PermutationTypes type)
      : base() {
      permutationType = type;
    }
    public Permutation(PermutationTypes type, int length)
      : base(length) {
      for (int i = 0; i < length; i++)
        this[i] = i;
      permutationType = type;
    }
    public Permutation(PermutationTypes type, int length, IRandom random)
      : this(type, length) {
      Randomize(random);
    }
    public Permutation(PermutationTypes type, int[] elements)
      : base(elements) {
      permutationType = type;
    }
    public Permutation(PermutationTypes type, IntArray elements)
      : this(type, elements.Length) {
      for (int i = 0; i < array.Length; i++)
        array[i] = elements[i];
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new Permutation(this, cloner);
    }

    public virtual bool Validate() {
      bool[] values = new bool[Length];
      int value;

      for (int i = 0; i < values.Length; i++)
        values[i] = false;
      for (int i = 0; i < Length; i++) {
        value = this[i];
        if ((value < 0) || (value >= values.Length)) return false;
        if (values[value]) return false;
        values[value] = true;
      }
      return true;
    }

    public virtual void Randomize(IRandom random, int startIndex, int length) {
      if (length > 1) {
        // Knuth shuffle
        int index1, index2;
        int val;
        for (int i = length - 1; i > 0; i--) {
          index1 = startIndex + i;
          index2 = startIndex + random.Next(i + 1);
          if (index1 != index2) {
            val = array[index1];
            array[index1] = array[index2];
            array[index2] = val;
          }
        }
        OnReset();
      }
    }
    public void Randomize(IRandom random) {
      Randomize(random, 0, Length);
    }

    public virtual int GetCircular(int position) {
      if (position >= Length) position = position % Length;
      while (position < 0) position += Length;
      return this[position];
    }

    public virtual void Swap(int i, int j) {
      var h = array[i];
      array[i] = array[j];
      array[j] = h;
      OnReset();
    }

    public virtual void Reverse(int startIndex, int length) {
      Array.Reverse(array, startIndex, length);
      if (length > 1) OnReset();
    }

    public virtual void Move(int startIndex, int endIndex, int insertIndex) {
      if (insertIndex == startIndex) return;
      if (insertIndex > startIndex && insertIndex <= endIndex) {
        var start = endIndex + 1;
        var end = endIndex + insertIndex - startIndex;
        insertIndex = startIndex;
        startIndex = start;
        endIndex = end;
      }
      var original = (int[])array.Clone();
      Array.Copy(original, startIndex, array, insertIndex, endIndex - startIndex + 1);
      if (insertIndex > endIndex)
        Array.Copy(original, endIndex + 1, array, startIndex, insertIndex - startIndex);
      else Array.Copy(original, insertIndex, array, insertIndex + endIndex - startIndex + 1, startIndex - insertIndex);
      OnReset();
    }

    public virtual void Replace(int startIndex, int[] replacement) {
      Array.Copy(replacement, 0, array, startIndex, replacement.Length);
      OnReset();
    }

    public event EventHandler PermutationTypeChanged;

    protected virtual void OnPermutationTypeChanged() {
      var handler = PermutationTypeChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }
  }
}

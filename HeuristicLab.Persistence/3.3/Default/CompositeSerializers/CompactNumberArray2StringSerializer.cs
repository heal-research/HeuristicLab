#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Collections;
using System.Collections.Generic;
using System.Text;
using HeuristicLab.Persistence.Auxiliary;
using HeuristicLab.Persistence.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Persistence.Interfaces;

namespace HeuristicLab.Persistence.Default.CompositeSerializers {

  [StorableClass]
  public sealed class CompactNumberArray2StringSerializer : ICompositeSerializer {

    private class ElementEnumerator : IEnumerator<string> {

      private IEnumerator<Tag> tagIt;
      private IEnumerator<string> valIt;

      public ElementEnumerator(IEnumerable<Tag> tags) {
        tagIt = tags.GetEnumerator();
      }

      public string Current {
        get {
          if (valIt == null)
            throw new InvalidOperationException("no current value");
          return valIt.Current;
        }
      }

      public void Dispose() {
        valIt.Dispose();
        valIt = null;
        tagIt.Dispose();
      }

      object IEnumerator.Current {
        get { return this.Current; }
      }

      public bool MoveNext() {
        if (valIt != null && valIt.MoveNext())
          return true;
        if (tagIt.MoveNext()) {
          if (valIt != null)
            valIt.Dispose();
          valIt = ((string)tagIt.Current.Value).GetSplitEnumerator(';');
          return MoveNext();
        }
        valIt.Dispose();
        valIt = null;
        return false;
      }

      public void Reset() {
        valIt.Dispose();
        tagIt.Reset();
      }
    }

    [StorableConstructor]
    private CompactNumberArray2StringSerializer(bool deserializing) { }
    public CompactNumberArray2StringSerializer() { }

    public const int SPLIT_THRESHOLD = 1024 * 1024;

    public int Priority {
      get { return 200; }
    }

    private static readonly Number2StringSerializer numberConverter =
      new Number2StringSerializer();

    public bool CanSerialize(Type type) {
      return
        (type.IsArray || type == typeof(Array)) &&
        numberConverter.CanSerialize(type.GetElementType());
    }

    public string JustifyRejection(Type type) {
      if (!type.IsArray && type != typeof(Array))
        return "not an array";
      return string.Format("number converter cannot serialize elements: " +
        numberConverter.JustifyRejection(type.GetElementType()));
    }

    public IEnumerable<Tag> CreateMetaInfo(object obj) {
      Array a = (Array)obj;
      int[] lengths = new int[a.Rank];
      int[] lowerBounds = new int[a.Rank];
      StringBuilder sb = new StringBuilder(a.Rank * 6);
      sb.Append(a.Rank).Append(';');
      for (int i = 0; i < a.Rank; i++) {
        sb.Append(a.GetLength(i)).Append(';');
        lengths[i] = a.GetLength(i);
      }
      for (int i = 0; i < a.Rank; i++) {
        sb.Append(a.GetLowerBound(i)).Append(';');
        lowerBounds[i] = a.GetLowerBound(i);
      }
      int nElements = 1;
      for (int i = 0; i < a.Rank; i++) {
        lowerBounds[i] = a.GetLowerBound(i);
        lengths[i] = a.GetLength(i);
        nElements *= lengths[i];
      }
      sb.Capacity += Math.Min(nElements * 3, SPLIT_THRESHOLD);
      int[] positions = (int[])lowerBounds.Clone();
      while (positions[a.Rank - 1] < lengths[a.Rank - 1] + lowerBounds[a.Rank - 1]) {
        sb.Append(numberConverter.Format(a.GetValue(positions))).Append(';');
        if (sb.Length > SPLIT_THRESHOLD && sb.Length > sb.Capacity - 18) {
          yield return new Tag(sb.ToString());
          sb = new StringBuilder(Math.Min(nElements * 3, SPLIT_THRESHOLD));
        }
        positions[0] += 1;
        for (int i = 0; i < a.Rank - 1; i++) {
          if (positions[i] >= lengths[i] + lowerBounds[i]) {
            positions[i] = lowerBounds[i];
            positions[i + 1] += 1;
          } else {
            break;
          }
        }
      }
      if (sb.Length > 0)
        yield return new Tag(sb.ToString());
    }

    private static Tag[] emptyTag = new Tag[0];
    public IEnumerable<Tag> Decompose(object obj) {
      return emptyTag;
    }

    public object CreateInstance(Type type, IEnumerable<Tag> metaInfo) {
      try {
        var valueIter = new ElementEnumerator(metaInfo);
        valueIter.MoveNext();
        int rank = int.Parse(valueIter.Current);
        int[] lengths = new int[rank];
        int[] lowerBounds = new int[rank];
        for (int i = 0; i < rank; i++) {
          valueIter.MoveNext();
          lengths[i] = int.Parse(valueIter.Current);
        }
        for (int i = 0; i < rank; i++) {
          valueIter.MoveNext();
          lowerBounds[i] = int.Parse(valueIter.Current);
        }
        Type elementType = type.GetElementType();
        Array a = Array.CreateInstance(elementType, lengths, lowerBounds);
        if (a == null) throw new PersistenceException("invalid instance data type, expected array");
        int[] positions = (int[])lowerBounds.Clone();

        while (valueIter.MoveNext()) {
          a.SetValue(numberConverter.Parse(valueIter.Current, elementType), positions);
          positions[0] += 1;
          for (int i = 0; i < a.Rank - 1; i++) {
            if (positions[i] >= lengths[i] + lowerBounds[i]) {
              positions[i + 1] += 1;
              positions[i] = lowerBounds[i];
            } else {
              break;
            }
          }
        }
        return a;
      }
      catch (InvalidOperationException e) {
        throw new PersistenceException("Insuffictient data to deserialize compact array", e);
      }
      catch (InvalidCastException e) {
        throw new PersistenceException("Invalid element data during compact array deserialization", e);
      }
    }

    public void Populate(object instance, IEnumerable<Tag> tags, Type type) {
      // Nothing to do. Arrays are populated during instance creation;
    }

  }

}
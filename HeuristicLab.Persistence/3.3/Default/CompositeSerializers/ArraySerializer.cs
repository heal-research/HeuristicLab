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
using HeuristicLab.Persistence.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Persistence.Interfaces;

namespace HeuristicLab.Persistence.Default.CompositeSerializers {

  [StorableClass]
  internal sealed class ArraySerializer : ICompositeSerializer {

    [StorableConstructor]
    private ArraySerializer(bool deserializing) { }
    public ArraySerializer() { }

    public int Priority {
      get { return 100; }
    }

    public bool CanSerialize(Type type) {
      return type.IsArray || type == typeof(Array);
    }

    public string JustifyRejection(Type type) {
      return "not an array and not of type System.Array";
    }

    public IEnumerable<Tag> CreateMetaInfo(object obj) {
      Array a = (Array)obj;
      yield return new Tag("rank", a.Rank);
      for (int i = 0; i < a.Rank; i++) {
        yield return new Tag("length_" + i, a.GetLength(i));
      }
      for (int i = 0; i < a.Rank; i++) {
        yield return new Tag("lowerBound_" + i, a.GetLowerBound(i));
      }
    }

    public IEnumerable<Tag> Decompose(object array) {
      Array a = (Array)array;
      int[] lengths = new int[a.Rank];
      int[] lowerBounds = new int[a.Rank];
      for (int i = 0; i < a.Rank; i++) {
        lengths[i] = a.GetLength(i);
      }
      for (int i = 0; i < a.Rank; i++) {
        lowerBounds[i] = a.GetLowerBound(i);
      }
      int[] positions = (int[])lowerBounds.Clone();
      while (positions[a.Rank - 1] < lengths[a.Rank - 1] + lowerBounds[a.Rank - 1]) {
        yield return new Tag(a.GetValue(positions));
        positions[0] += 1;
        for (int i = 0; i < a.Rank - 1; i++) {
          if (positions[i] >= lowerBounds[i] + lengths[i]) {
            positions[i] = lowerBounds[i];
            positions[i + 1] += 1;
          } else {
            break;
          }
        }
      }
    }

    public object CreateInstance(Type t, IEnumerable<Tag> metaInfo) {
      try {
        IEnumerator<Tag> e = metaInfo.GetEnumerator();
        e.MoveNext();
        int rank = (int)e.Current.Value;
        int[] lengths = new int[rank];
        for (int i = 0; i < rank; i++) {
          e.MoveNext();
          lengths[i] = (int)e.Current.Value;
        }
        int[] lowerBounds = new int[rank];
        for (int i = 0; i < rank; i++) {
          e.MoveNext();
          lowerBounds[i] = (int)e.Current.Value;
        }
        return Array.CreateInstance(t.GetElementType(), lengths, lowerBounds);
      }
      catch (InvalidOperationException x) {
        throw new PersistenceException("Insufficient meta information to construct array instance.", x);
      }
      catch (InvalidCastException x) {
        throw new PersistenceException("Invalid format of array metainfo.", x);
      }
    }

    public void Populate(object instance, IEnumerable<Tag> elements, Type t) {
      Array a = (Array)instance;
      int[] lengths = new int[a.Rank];
      int[] lowerBounds = new int[a.Rank];
      for (int i = 0; i < a.Rank; i++) {
        lengths[i] = a.GetLength(i);
      }
      for (int i = 0; i < a.Rank; i++) {
        lowerBounds[i] = a.GetLowerBound(i);
      }
      int[] positions = (int[])lowerBounds.Clone();
      IEnumerator<Tag> e = elements.GetEnumerator();
      try {
        while (e.MoveNext()) {
          int[] currentPositions = positions;
          a.SetValue(e.Current.Value, currentPositions);
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
      }
      catch (InvalidOperationException x) {
        throw new PersistenceException("Insufficient data to fill array instance", x);
      }
      catch (InvalidCastException x) {
        throw new PersistenceException("Invalid element data. Cannot fill array", x);
      }
      catch (IndexOutOfRangeException x) {
        throw new PersistenceException("Too many elements during array deserialization", x);
      }
    }
  }

}

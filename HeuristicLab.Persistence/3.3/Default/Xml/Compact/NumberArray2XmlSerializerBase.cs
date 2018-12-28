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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HEAL.Fossil;
using HeuristicLab.Persistence.Auxiliary;

namespace HeuristicLab.Persistence.Default.Xml.Compact {

  [StorableClass]
  internal abstract class NumberArray2XmlSerializerBase<T> : CompactXmlSerializerBase<T> where T : class {

    protected virtual char Separator { get { return ';'; } }
    protected abstract string FormatValue(object o);
    protected abstract object ParseValue(string o);

    public override XmlString Format(T t) {
      Array a = (Array)(object)t;
      int[] lengths = new int[a.Rank];
      int[] lowerBounds = new int[a.Rank];
      StringBuilder sb = new StringBuilder(3 + a.Rank * 3);
      sb.Append(a.Rank);
      int nElements = 1;
      for (int i = 0; i < a.Rank; i++) {
        sb.Append(Separator);
        sb.Append(a.GetLength(i));
        lengths[i] = a.GetLength(i);
        nElements *= lengths[i];
      }
      sb.EnsureCapacity(sb.Length + nElements * 3);
      for (int i = 0; i < a.Rank; i++) {
        sb.Append(Separator);
        sb.Append(a.GetLowerBound(i));
        lowerBounds[i] = a.GetLowerBound(i);
      }
      if (lengths.Any(l => l == 0))
        return new XmlString(sb.ToString());
      int[] positions = (int[])lowerBounds.Clone();
      while (positions[a.Rank - 1] < lengths[a.Rank - 1] + lowerBounds[a.Rank - 1]) {
        sb.Append(Separator);
        sb.Append(FormatValue(a.GetValue(positions)));
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
      return new XmlString(sb.ToString());
    }

    public override T Parse(XmlString x) {
      try {
        IEnumerator<string> values = x.Data.GetSplitEnumerator(Separator);
        values.MoveNext();
        int rank = int.Parse(values.Current);
        int[] lengths = new int[rank];
        for (int i = 0; i < rank; i++) {
          values.MoveNext();
          lengths[i] = int.Parse(values.Current);
        }
        int[] lowerBounds = new int[rank];
        for (int i = 0; i < rank; i++) {
          values.MoveNext();
          lowerBounds[i] = int.Parse(values.Current);
        }
        Array a = Array.CreateInstance(this.SourceType.GetElementType(), lengths, lowerBounds);
        int[] positions = (int[])lowerBounds.Clone();
        while (values.MoveNext()) {
          a.SetValue(ParseValue(values.Current), positions);
          positions[0] += 1;
          for (int i = 0; i < rank - 1; i++) {
            if (positions[i] >= lowerBounds[i] + lengths[i]) {
              positions[i] = lowerBounds[i];
              positions[i + 1] += 1;
            } else {
              break;
            }
          }
        }
        if (positions[rank - 1] != lowerBounds[rank - 1] + lengths[rank - 1] && lengths.All(l => l != 0))
          throw new PersistenceException("Insufficient number of elements while trying to fill number array.");
        return (T)(object)a;
      }
      catch (InvalidOperationException e) {
        throw new PersistenceException("Insufficient information to rebuild number array.", e);
      }
      catch (InvalidCastException e) {
        throw new PersistenceException("Invalid element data or meta data to reconstruct number array.", e);
      }
      catch (OverflowException e) {
        throw new PersistenceException("Overflow during element parsing while trying to reconstruct number array.", e);
      }
    }
  }

}
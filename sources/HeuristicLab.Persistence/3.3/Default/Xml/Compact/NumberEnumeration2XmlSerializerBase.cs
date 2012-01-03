#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Text;
using HeuristicLab.Persistence.Auxiliary;
using HeuristicLab.Persistence.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Persistence.Default.Xml.Compact {

  [StorableClass]
  internal abstract class NumberEnumeration2XmlSerializerBase<T> : CompactXmlSerializerBase<T> where T : IEnumerable {

    protected virtual char Separator { get { return ';'; } }
    protected abstract void Add(IEnumerable enumeration, object o);
    protected abstract IEnumerable Instantiate();
    protected abstract string FormatValue(object o);
    protected abstract object ParseValue(string o);

    public override XmlString Format(T t) {
      StringBuilder sb = new StringBuilder();
      foreach (var value in (IEnumerable)t) {
        sb.Append(FormatValue(value));
        sb.Append(Separator);
      }
      return new XmlString(sb.ToString());
    }

    public override T Parse(XmlString x) {
      try {
        IEnumerable enumeration = Instantiate();
        foreach (var value in x.Data.EnumerateSplit(Separator)) {
          Add(enumeration, ParseValue(value));
        }
        return (T)enumeration;
      }
      catch (InvalidCastException e) {
        throw new PersistenceException("Invalid element data during reconstruction of number enumerable.", e);
      }
      catch (OverflowException e) {
        throw new PersistenceException("Overflow during element parsing while trying to reconstruct number enumerable.", e);
      }
    }
  }

}
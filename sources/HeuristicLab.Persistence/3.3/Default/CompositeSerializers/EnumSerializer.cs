#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  internal sealed class EnumSerializer : ICompositeSerializer {

    [StorableConstructor]
    private EnumSerializer(bool deserializing) { }
    public EnumSerializer() { }

    public int Priority {
      get { return 100; }
    }

    public bool CanSerialize(Type type) {
      return type.IsEnum || type == typeof(Enum);
    }

    public string JustifyRejection(Type type) {
      return "not an enum and not System.Enum";
    }

    public IEnumerable<Tag> CreateMetaInfo(object obj) {
      yield return new Tag(Enum.Format(obj.GetType(), obj, "G"));
    }

    public IEnumerable<Tag> Decompose(object obj) {
      return new Tag[] { };
    }

    public object CreateInstance(Type t, IEnumerable<Tag> metaInfo) {
      IEnumerator<Tag> it = metaInfo.GetEnumerator();
      try {
        it.MoveNext();
        return Enum.Parse(t, (string)it.Current.Value);
      }
      catch (InvalidOperationException e) {
        throw new PersistenceException("not enough meta information to recstruct enum", e);
      }
      catch (InvalidCastException e) {
        throw new PersistenceException("invalid meta information found while trying to reconstruct enum", e);
      }
    }

    public void Populate(object instance, IEnumerable<Tag> elements, Type t) {
      // Enums are already populated during instance creation.
    }
  }
}
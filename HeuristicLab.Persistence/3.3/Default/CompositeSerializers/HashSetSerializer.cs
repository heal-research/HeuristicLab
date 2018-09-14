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
using System.Linq;
using System.Reflection;
using HeuristicLab.Persistence.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Persistence.Interfaces;

namespace HeuristicLab.Persistence.Default.CompositeSerializers {

  [StorableClass]
  internal sealed class HashSetSerializer : ICompositeSerializer {

    [StorableConstructor]
    private HashSetSerializer(bool deserializing) { }
    public HashSetSerializer() { }

    public int Priority {
      get { return 250; }
    }

    public bool CanSerialize(Type type) {
      return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(HashSet<>);
    }

    public string JustifyRejection(Type type) {
      return "Type is not a generic HashSet<>";
    }

    public IEnumerable<Tag> CreateMetaInfo(object o) {
      yield return new Tag("Comparer", o.GetType().GetProperty("Comparer").GetValue(o, null));
    }

    public IEnumerable<Tag> Decompose(object obj) {
      foreach (object o in (IEnumerable)obj) {
        yield return new Tag(o);
      }
    }

    public object CreateInstance(Type type, IEnumerable<Tag> metaInfo) {
      return Activator.CreateInstance(type, metaInfo.First().Value);
    }

    public void Populate(object instance, IEnumerable<Tag> tags, Type type) {
      MethodInfo addMethod = type.GetMethod("Add");
      try {
        foreach (var tag in tags)
          addMethod.Invoke(instance, new[] { tag.Value });
      } catch (Exception e) {
        throw new PersistenceException("Exception caught while trying to populate enumerable.", e);
      }
    }

    public object HashSet { get; set; }
  }
}
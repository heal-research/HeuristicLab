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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HeuristicLab.Persistence.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Persistence.Interfaces;

namespace HeuristicLab.Persistence.Default.CompositeSerializers {

  [StorableClass]
  internal sealed class KeyValuePairSerializer : ICompositeSerializer {

    [StorableConstructor]
    private KeyValuePairSerializer(bool deserializing) { }
    public KeyValuePairSerializer() { }

    public int Priority {
      get { return 100; }
    }

    private static readonly Type genericKeyValuePairType =
      typeof(KeyValuePair<int, int>).GetGenericTypeDefinition();

    public bool CanSerialize(Type type) {
      return type.IsGenericType &&
             type.GetGenericTypeDefinition() == genericKeyValuePairType;
    }

    public string JustifyRejection(Type type) {
      if (!type.IsGenericType)
        return "not even generic";
      return "not generic KeyValuePair<,>";
    }

    public IEnumerable<Tag> CreateMetaInfo(object o) {
      return new Tag[] { };
    }

    public IEnumerable<Tag> Decompose(object o) {
      Type t = o.GetType();
      Tag key, value;
      try {
        key = new Tag("key", t.GetProperty("Key").GetValue(o, null));
      }
      catch (Exception e) {
        throw new PersistenceException("Exception caught during KeyValuePair decomposition", e);
      }
      yield return key;
      try {
        value = new Tag("value", t.GetProperty("Value").GetValue(o, null));
      }
      catch (Exception e) {
        throw new PersistenceException("Exception caught during KeyValuePair decomposition", e);
      }
      yield return value;
    }

    public object CreateInstance(Type type, IEnumerable<Tag> metaInfo) {
      return Activator.CreateInstance(type, true);
    }

    public void Populate(object instance, IEnumerable<Tag> o, Type t) {
      IEnumerator<Tag> iter = o.GetEnumerator();
      try {
        iter.MoveNext();
        t.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
          .Single(fi => fi.Name == "key").SetValue(instance, iter.Current.Value);
        iter.MoveNext();
        t.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
          .Single(fi => fi.Name == "value").SetValue(instance, iter.Current.Value);
      }
      catch (InvalidOperationException e) {
        throw new PersistenceException("Not enough components to populate KeyValuePair instance", e);
      }
      catch (Exception e) {
        throw new PersistenceException("Exception caught during KeyValuePair reconstruction", e);
      }
    }
  }
}

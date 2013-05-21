#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Persistence.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Persistence.Interfaces;

namespace HeuristicLab.Persistence.Default.CompositeSerializers {

  [StorableClass]
  internal sealed class ConcreteDictionarySerializer : ICompositeSerializer {

    [StorableConstructor]
    private ConcreteDictionarySerializer(bool deserializing) { }
    public ConcreteDictionarySerializer() { }

    public int Priority {
      get { return 150; }
    }


    public bool CanSerialize(Type type) {
      return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>);
    }

    public string JustifyRejection(Type type) {
      return "Type is not a generic Dictionary<>";
    }

    public IEnumerable<Tag> CreateMetaInfo(object o) {
      yield return new Tag("Comparer", o.GetType().GetProperty("Comparer").GetValue(o, null));
    }

    public IEnumerable<Tag> Decompose(object o) {
      IDictionary dict = (IDictionary)o;
      foreach (DictionaryEntry entry in dict) {
        yield return new Tag("key", entry.Key);
        yield return new Tag("value", entry.Value);
      }
    }

    public object CreateInstance(Type t, IEnumerable<Tag> metaInfo) {
      return Activator.CreateInstance(t, metaInfo.First().Value);
    }

    public void Populate(object instance, IEnumerable<Tag> o, Type t) {
      IDictionary dict = (IDictionary)instance;
      IEnumerator<Tag> iter = o.GetEnumerator();
      try {
        while (iter.MoveNext()) {
          Tag key = iter.Current;
          iter.MoveNext();
          Tag value = iter.Current;
          dict.Add(key.Value, value.Value);
        }
      } catch (InvalidOperationException e) {
        throw new PersistenceException("Dictionaries must contain an even number of elements (key+value).", e);
      } catch (NotSupportedException e) {
        throw new PersistenceException("The serialized dictionary type was read-only or had a fixed size and cannot be deserialized.", e);
      } catch (ArgumentNullException e) {
        throw new PersistenceException("Dictionary key was null.", e);
      } catch (ArgumentException e) {
        throw new PersistenceException("Duplicate dictionary key.", e);
      }
    }
  }

}

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
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using HeuristicLab.Persistence.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Persistence.Interfaces;

namespace HeuristicLab.Persistence.Default.CompositeSerializers {

  [StorableClass]
  internal sealed class StackSerializer : ICompositeSerializer {

    [StorableConstructor]
    private StackSerializer(bool deserializing) { }
    public StackSerializer() { }

    public int Priority {
      get { return 100; }
    }


    public bool CanSerialize(Type type) {
      return type == typeof(Stack) ||
        type.IsGenericType &&
        type.GetGenericTypeDefinition() == typeof(Stack<>);
    }

    public string JustifyRejection(Type type) {
      return "not Stack or generic Stack<>";
    }

    public IEnumerable<Tag> CreateMetaInfo(object o) {
      return new Tag[] { };
    }

    public IEnumerable<Tag> Decompose(object obj) {
      MethodInfo addMethod = obj.GetType().GetMethod("Push");
      object reverseStack = Activator.CreateInstance(obj.GetType(), true);
      foreach (object o in (IEnumerable)obj) {
        addMethod.Invoke(reverseStack, new[] { o });
      }
      foreach (object o in (IEnumerable)reverseStack) {
        yield return new Tag(null, o);
      }
    }

    public object CreateInstance(Type type, IEnumerable<Tag> metaInfo) {
      return Activator.CreateInstance(type, true);
    }

    public void Populate(object instance, IEnumerable<Tag> tags, Type type) {
      MethodInfo addMethod = type.GetMethod("Push");
      try {
        foreach (var tag in tags)
          addMethod.Invoke(instance, new[] { tag.Value });
      }
      catch (Exception e) {
        throw new PersistenceException("Exception caught while trying to populate enumerable.", e);
      }
    }
  }
}

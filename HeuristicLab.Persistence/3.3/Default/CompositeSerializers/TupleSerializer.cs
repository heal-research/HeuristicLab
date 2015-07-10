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
using System.Linq;
using System.Reflection;
using HeuristicLab.Persistence.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Persistence.Interfaces;

namespace HeuristicLab.Persistence.Default.CompositeSerializers {

  [StorableClass]
  internal sealed class TupleSerializer : ICompositeSerializer {

    [StorableConstructor]
    private TupleSerializer(bool deserializing) { }
    public TupleSerializer() { }

    public int Priority {
      get { return 50; }
    }

    public bool CanSerialize(Type type) {
      if (!type.IsGenericType)
        return false;
      Type t = type.GetGenericTypeDefinition();
      return t == typeof(Tuple<>) ||
        t == typeof(Tuple<,>) ||
        t == typeof(Tuple<,,>) ||
        t == typeof(Tuple<,,,>) ||
        t == typeof(Tuple<,,,,>) ||
        t == typeof(Tuple<,,,,,>) ||
        t == typeof(Tuple<,,,,,,>) ||
        t == typeof(Tuple<,,,,,,,>);
    }

    public string JustifyRejection(Type type) {
      if (!type.IsGenericType)
        return "not a generic type";
      return "type is not a tuple type";
    }

    public IEnumerable<Tag> CreateMetaInfo(object obj) {
      Type t = obj.GetType();
      for (int i = 1; i <= t.GetGenericArguments().Length; i++) {
        string name = string.Format("Item{0}", i);
        yield return new Tag(name, t.GetProperty(name).GetValue(obj, null));
      }
    }

    public IEnumerable<Tag> Decompose(object obj) {
      return null;
    }

    private static BindingFlags public_static = BindingFlags.Public | BindingFlags.Static;

    private static MethodInfo[] CreateMethods = new MethodInfo[8];

    static TupleSerializer() {
      foreach (MethodInfo mi in typeof(Tuple).GetMethods(public_static).Where(mi => mi.Name == "Create")) {
        CreateMethods[mi.GetGenericArguments().Length - 1] = mi;
      }
    }

    public object CreateInstance(Type type, IEnumerable<Tag> metaInfo) {
      var values = metaInfo.Select(t => t.Value).ToArray();

      MethodInfo createMethod = CreateMethods[values.Length - 1].MakeGenericMethod(type.GetGenericArguments());
      return createMethod.Invoke(null, values);
    }

    public void Populate(object instance, IEnumerable<Tag> tags, Type type) {
      // nothing to do
    }

  }
}

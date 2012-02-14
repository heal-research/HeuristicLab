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
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Threading;

namespace HeuristicLab.Common {
  public static class ObjectExtensions {
    public static IEnumerable<object> GetObjectGraphObjects(this object obj, HashSet<string> excludedMembers = null, bool excludeStaticMembers = false) {
      if (obj == null) return Enumerable.Empty<object>();
      if (excludedMembers == null) excludedMembers = new HashSet<string>();

      var objects = new HashSet<object>();
      var stack = new Stack<object>();

      stack.Push(obj);
      while (stack.Count > 0) {
        object current = stack.Pop();
        objects.Add(current);

        foreach (object o in GetChildObjects(current, excludedMembers, excludeStaticMembers)) {
          if (o != null && !objects.Contains(o) && !ExcludeType(o.GetType()))
            stack.Push(o);
        }
      }

      return objects;
    }

    /// <summary>
    /// Types not collected:
    ///   * System.Delegate
    ///   * System.Reflection.Pointer
    ///   * Primitives (Boolean, Byte, SByte, Int16, UInt16, Int32, UInt32, Int64, UInt64, IntPtr, UIntPtr, Char, Double, Single)
    ///   * string, decimal
    ///   * Arrays of types not collected
    ///   
    /// Dictionaries and HashSets are treated specially, because it is cheaper to iterate over their keys and values
    /// compared to traverse their internal data structures.
    /// </summary>
    private static bool ExcludeType(Type type) {
      return type.IsPrimitive ||
             type == typeof(string) ||
             type == typeof(decimal) ||
             typeof(Delegate).IsAssignableFrom(type) ||
             typeof(Pointer).IsAssignableFrom(type) ||
             (type.HasElementType && ExcludeType(type.GetElementType()));
    }
    private static IEnumerable<object> GetChildObjects(object obj, HashSet<string> excludedMembers, bool excludeStaticMembers) {
      Type type = obj.GetType();

      if (type.IsSubclassOfRawGeneric(typeof(ThreadLocal<>))) {
        PropertyInfo info = type.GetProperty("Value");
        object value = info.GetValue(obj, null);
        if (value != null) yield return value;
      } else if (type.IsSubclassOfRawGeneric(typeof(Dictionary<,>)) ||
           type.IsSubclassOfRawGeneric(typeof(SortedDictionary<,>)) ||
           type.IsSubclassOfRawGeneric(typeof(SortedList<,>)) ||
           obj is SortedList ||
           obj is OrderedDictionary ||
           obj is ListDictionary ||
           obj is Hashtable) {
        var dictionary = obj as IDictionary;
        foreach (object value in dictionary.Keys)
          yield return value;
        foreach (object value in dictionary.Values)
          yield return value;
      } else if (type.IsArray || type.IsSubclassOfRawGeneric(typeof(HashSet<>))) {
        var enumerable = obj as IEnumerable;
        foreach (var value in enumerable)
          yield return value;
      } else {
        foreach (FieldInfo f in type.GetAllFields()) {
          if (excludedMembers.Contains(f.Name)) continue;
          if (excludeStaticMembers && f.IsStatic) continue;
          object fieldValue;
          try {
            fieldValue = f.GetValue(obj);
          }
          catch (SecurityException) {
            continue;
          }
          yield return fieldValue;
        }
      }
    }
  }
}

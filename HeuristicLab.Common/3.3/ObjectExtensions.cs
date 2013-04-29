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
    public static IEnumerable<T> ToEnumerable<T>(this T obj) {
      yield return obj;
    }

    public static IEnumerable<object> GetObjectGraphObjects(this object obj, HashSet<object> excludedMembers = null, bool excludeStaticMembers = false) {
      if (obj == null) return Enumerable.Empty<object>();
      if (excludedMembers == null) excludedMembers = new HashSet<object>();

      var objects = new HashSet<object>();
      var stack = new Stack<object>();

      stack.Push(obj);
      while (stack.Count > 0) {
        object current = stack.Pop();
        objects.Add(current);

        foreach (object o in GetChildObjects(current, excludedMembers, excludeStaticMembers)) {
          if (o == null) continue;
          if (ExcludeType(o.GetType())) continue;
          if (objects.Contains(o)) continue;
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
    ///   * string, decimal, DateTime
    ///   * Arrays of types not collected
    /// </summary>
    private static bool ExcludeType(Type type) {
      return type.IsPrimitive ||
             type == typeof(string) ||
             type == typeof(string[]) ||
             type == typeof(decimal) ||
             type == typeof(decimal[]) ||
             type == typeof(DateTime) ||
             type == typeof(DateTime[]) ||
             typeof(Delegate).IsAssignableFrom(type) ||
             typeof(Pointer).IsAssignableFrom(type) ||
             type == typeof(System.Reflection.Emit.SignatureHelper) ||
             (type.HasElementType && ExcludeType(type.GetElementType()));
    }

    private static IEnumerable<object> GetChildObjects(object obj, HashSet<object> excludedMembers, bool excludeStaticMembers) {
      Type type = obj.GetType();

      if (type.IsSubclassOfRawGeneric(typeof(ThreadLocal<>))) {
        PropertyInfo info = type.GetProperty("Value");
        object value = info.GetValue(obj, null);
        if (value != null && !excludedMembers.Contains(value))
          yield return value;
      } else if (type.IsSubclassOfRawGeneric(typeof(Dictionary<,>)) ||
           type.IsSubclassOfRawGeneric(typeof(SortedDictionary<,>)) ||
           type.IsSubclassOfRawGeneric(typeof(SortedList<,>)) ||
           obj is SortedList ||
           obj is OrderedDictionary ||
           obj is ListDictionary ||
           obj is Hashtable) {
        var dictionary = obj as IDictionary;
        foreach (object value in dictionary.Keys) {
          if (excludedMembers.Contains(value)) continue;
          yield return value;
        }
        foreach (object value in dictionary.Values) {
          if (excludedMembers.Contains(value)) continue;
          yield return value;
        }
      } else if (type.IsArray || type.IsSubclassOfRawGeneric(typeof(HashSet<>))) {
        var enumerable = obj as IEnumerable;
        foreach (var value in enumerable) {
          if (excludedMembers.Contains(value)) continue;
          yield return value;
        }
      } else {
        foreach (FieldInfo f in type.GetAllFields()) {
          if (excludeStaticMembers && f.IsStatic) continue;
          object fieldValue;
          try {
            fieldValue = f.GetValue(obj);
          }
          catch (SecurityException) {
            continue;
          }
          if (excludedMembers.Contains(fieldValue)) continue;
          yield return fieldValue;
        }
      }
    }
  }
}

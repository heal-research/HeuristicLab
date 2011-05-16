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
using System.Reflection;
using System.Collections;

namespace HeuristicLab.Common {
  public static class ObjectExtensions {
    public static IEnumerable<object> GetObjectGraphObjects(this object obj) {
      var objects = new HashSet<object>();
      obj.CollectObjectGraphObjects(objects);
      return objects;
    }
    /// <summary>
    /// Types not collected:
    ///   * System.Delegate
    ///   * System.EventHandler (+ System.EventHandler<T>)
    ///   * Primitives (Boolean, Byte, SByte, Int16, UInt16, Int32, UInt32, Int64, UInt64, IntPtr, UIntPtr, Char, Double, Single)
    ///   * string, decimal
    ///   * Arrays of primitives (+ string, decimal)
    /// Types of which the fields are not further collected:
    ///   * System.Type
    ///   * System.Threading.ThreadLocal<T>
    /// </summary>
    private static void CollectObjectGraphObjects(this object obj, HashSet<object> objects) {
      if (obj == null || objects.Contains(obj)) return;
      if (obj is Delegate || obj is EventHandler) return;
      if (obj is Pointer) return;
      Type type = obj.GetType();
      if (type.IsSubclassOfRawGeneric(typeof(EventHandler<>))) return;
      if (type.IsPrimitive || type == typeof(string) || type == typeof(decimal)) return;
      if (type.HasElementType) {
        Type elementType = type.GetElementType();
        if (elementType.IsPrimitive || elementType == typeof(string) || elementType == typeof(decimal)) return;
        //TODO check all types
      }

      objects.Add(obj);

      //if (typeof(Type).IsInstanceOfType(obj)) return; // avoid infinite recursion
      //if (type.IsSubclassOfRawGeneric(typeof(ThreadLocal<>))) return; // avoid stack overflow when the field `ConcurrentStack<int> s_availableIndices` grows large

      // performance critical to handle dictionaries in a special way
      var dictionary = obj as IDictionary;
      if (dictionary != null) {
        foreach (object value in dictionary.Keys)
          CollectObjectGraphObjects(value, objects);
        foreach (object value in dictionary.Values)
          CollectObjectGraphObjects(value, objects);
        return;
      } else if (type.IsArray) {
        var array = obj as Array;
        foreach (object value in array)
          CollectObjectGraphObjects(value, objects);
        return;
      }

      foreach (FieldInfo f in type.GetAllFields()) {
        f.GetValue(obj).CollectObjectGraphObjects(objects);
      }
    }
  }
}

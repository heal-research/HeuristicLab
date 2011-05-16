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
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reflection;
using System.Threading;

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
    ///   * System.Reflection.Pointer
    ///   * Primitives (Boolean, Byte, SByte, Int16, UInt16, Int32, UInt32, Int64, UInt64, IntPtr, UIntPtr, Char, Double, Single)
    ///   * string, decimal
    ///   * Arrays of types not collected
    ///   
    /// Dictionaries and HashSets are treated specially, because it is cheaper to iterate over their keys and values
    /// compared to traverse their internal data structures.
    /// </summary>
    private static void CollectObjectGraphObjects(this object obj, HashSet<object> objects) {
      if (obj == null || objects.Contains(obj)) return;
      Type type = obj.GetType();
      if (ExcludeType(type)) return;
      if (type.HasElementType && ExcludeType(type.GetElementType())) return;

      objects.Add(obj);

      if (type.IsSubclassOfRawGeneric(typeof(ThreadLocal<>))) return; // avoid stack overflow when the field `ConcurrentStack<int> s_availableIndices` too grows large
      
      // performance critical to handle dictionaries, hashsets and hashtables in a special way
      if (type.IsSubclassOfRawGeneric(typeof(Dictionary<,>)) || 
          type.IsSubclassOfRawGeneric(typeof(SortedDictionary<,>)) || 
          type.IsSubclassOfRawGeneric(typeof(SortedList<,>)) ||
          obj is SortedList || 
          obj is OrderedDictionary || 
          obj is ListDictionary || 
          obj is Hashtable) {        
        var dictionary = obj as IDictionary;
        foreach (object value in dictionary.Keys)
          CollectObjectGraphObjects(value, objects);
        foreach (object value in dictionary.Values)
          CollectObjectGraphObjects(value, objects);
        return;
      } else if (type.IsArray || type.IsSubclassOfRawGeneric(typeof(HashSet<>))) {
        var enumerable = obj as IEnumerable;
        foreach (var value in enumerable)
          CollectObjectGraphObjects(value, objects);
        return;
      }

      foreach (FieldInfo f in type.GetAllFields()) {
        f.GetValue(obj).CollectObjectGraphObjects(objects);
      }
    }

    private static bool ExcludeType(Type type) {
      return type.IsPrimitive ||
             type == typeof(string) || 
             type == typeof(decimal) ||
             typeof(Delegate).IsAssignableFrom(type) || 
             typeof(Pointer).IsAssignableFrom(type);
    }
  }
}

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
using System.Reflection;

namespace HeuristicLab.Common {
  public static class ObjectExtensions {
    public static IEnumerable<object> GetObjectGraphObjects(this object obj) {
      var objects = new HashSet<object>();
      obj.CollectObjectGraphObjects(objects);
      return objects;
    }

    private static void CollectObjectGraphObjects(this object obj, HashSet<object> objects) {
      if (obj == null || objects.Contains(obj)) return;
      if (obj is ValueType || obj is string) return;
      if (obj is Delegate || obj is EventHandler) return;
      if (obj.GetType().IsSubclassOfRawGeneric(typeof(EventHandler<>))) return;
      if (obj.GetType().GetElementType() != null) {
        Type elementType = obj.GetType().GetElementType();
        if (elementType.IsPrimitive || elementType == typeof(string) || elementType == typeof(decimal)) return;
      }

      objects.Add(obj);

      IEnumerable enumerable = obj as IEnumerable;
      if (enumerable != null) {
        foreach (object value in enumerable) {
          value.CollectObjectGraphObjects(objects);
        }
      } else {
        foreach (FieldInfo f in obj.GetType().GetAllFields()) {
          f.GetValue(obj).CollectObjectGraphObjects(objects);
        }
      }
    }
  }
}

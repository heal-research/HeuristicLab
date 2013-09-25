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
using System.Collections.Generic;
using System.Linq;

namespace HeuristicLab.MainForm {
  internal static class TypeExtension {
    internal static bool IsAssignableTo(this Type type, Type other) {
      if (other.IsAssignableFrom(type))
        return true;

      if (RecursiveCheckGenericTypes(type, other))
        return true;

      IEnumerable<Type> implementedInterfaces = type.GetInterfaces().Where(t => t.IsGenericType);
      foreach (Type implementedInterface in implementedInterfaces) {
        if (implementedInterface.CheckGenericTypes(other))
          return true;
      }

      return false;
    }

    private static bool RecursiveCheckGenericTypes(Type type, Type other) {
      if (type.CheckGenericTypes(other))
        return true;

      if (type.BaseType == null)
        return false;

      return RecursiveCheckGenericTypes(type.BaseType, other);
    }

    internal static bool CheckGenericTypes(this Type type, Type other) {
      if (!type.IsGenericType || !other.IsGenericType)
        return false;

      if (type.GetGenericTypeDefinition() == other.GetGenericTypeDefinition())
        return type.CheckGenericArguments(other);

      return false;
    }

    private static bool CheckGenericArguments(this Type type, Type other) {
      Type[] typeGenericArguments = type.GetGenericArguments();
      Type[] otherGenericArguments = other.GetGenericArguments();

      if (typeGenericArguments.Length != otherGenericArguments.Length)
        return false;

      for (int i = 0; i < typeGenericArguments.Length; i++) {
        if (typeGenericArguments[i] != otherGenericArguments[i]
          && !otherGenericArguments[i].IsGenericParameter)
          return false;
      }
      return true;
    }
  }
}

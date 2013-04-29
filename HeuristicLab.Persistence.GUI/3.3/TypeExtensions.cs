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
using System.Text;

namespace HeuristicLab.Persistence.GUI {

  internal static class TypeExtensions {

    public static string SimpleFullName(this Type type) {
      StringBuilder sb = new StringBuilder();
      SimpleFullName(type, sb);
      return sb.ToString();
    }

    private static void SimpleFullName(Type type, StringBuilder sb) {
      if (type.IsGenericType) {
        sb.Append(type.Name, 0, type.Name.LastIndexOf('`'));
        sb.Append("<");
        foreach (Type t in type.GetGenericArguments()) {
          SimpleFullName(t, sb);
          sb.Append(", ");
        }
        sb.Remove(sb.Length - 2, 2);
        sb.Append(">");
      } else {
        sb.Append(type.Name);
      }
    }

  }

}

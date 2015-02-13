#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Core {
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
  public sealed class CreatableAttribute : Attribute {
    private string category;
    public string Category {
      get {
        return category;
      }
      set {
        if (value == null) throw new ArgumentNullException("Category", "CreataleAttribute.Category must not be null");
        category = value;
      }
    }

    public CreatableAttribute() {
      Category = "Other Items";
    }
    public CreatableAttribute(string category)
      : this() {
      Category = category;
    }

    public static bool IsCreatable(Type type) {
      object[] attribs = type.GetCustomAttributes(typeof(CreatableAttribute), false);
      return attribs.Length > 0;
    }
    public static string GetCategory(Type type) {
      object[] attribs = type.GetCustomAttributes(typeof(CreatableAttribute), false);
      if (attribs.Length > 0) return ((CreatableAttribute)attribs[0]).Category;
      else return null;
    }
  }
}

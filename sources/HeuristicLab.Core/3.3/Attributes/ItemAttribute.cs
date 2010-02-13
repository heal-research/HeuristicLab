#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Text;
using System.Resources;
using System.Drawing;

namespace HeuristicLab.Core {
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
  public sealed class ItemAttribute : Attribute {
    public string Name { get; set; }
    public string Description { get; set; }

    public ItemAttribute() {
      Name = null;
      Description = null;
    }
    public ItemAttribute(string name, string description) {
      Name = name;
      Description = description;
    }

    public static string GetName(Type type) {
      object[] attribs = type.GetCustomAttributes(typeof(ItemAttribute), true);
      if (attribs.Length > 0) return ((ItemAttribute)attribs[0]).Name;
      else return null;
    }
    public static string GetDescription(Type type) {
      object[] attribs = type.GetCustomAttributes(typeof(ItemAttribute), true);
      if (attribs.Length > 0) return ((ItemAttribute)attribs[0]).Description;
      else return null;
    }
  }
}

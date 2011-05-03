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
using System.Reflection;

namespace HeuristicLab.MainForm {
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
  public sealed class ViewAttribute : Attribute {
    public ViewAttribute(string name) {
      this.name = name;
    }

    private string name;
    public string Name {
      get { return this.name; }
      set { this.name = value; }
    }

    public static bool HasViewAttribute(MemberInfo viewType) {
      ViewAttribute[] attributes = (ViewAttribute[])viewType.GetCustomAttributes(typeof(ViewAttribute), false);
      return attributes.Length != 0;
    }

    public static string GetViewName(MemberInfo viewType) {
      ViewAttribute[] attributes = (ViewAttribute[])viewType.GetCustomAttributes(typeof(ViewAttribute), false);
      if (attributes.Length == 1)
        return attributes[0].Name;
      return viewType.Name;
    }
  }
}
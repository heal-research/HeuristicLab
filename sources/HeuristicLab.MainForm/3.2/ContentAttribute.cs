#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Text;

namespace HeuristicLab.MainForm {
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
  public class ContentAttribute : Attribute {
    private Type type;
    public ContentAttribute(Type type) {
      this.type = type;
    }

    public ContentAttribute(Type type, bool isDefaultView)
      : this(type) {
      this.isDefaultView = isDefaultView;
    }

    private bool isDefaultView;
    public bool IsDefaultView {
      get { return this.isDefaultView; }
      set { this.isDefaultView = value; }
    }

    public static bool HasContentAttribute(Type viewType) {
      ContentAttribute[] attributes = (ContentAttribute[])viewType.GetCustomAttributes(typeof(ContentAttribute), false);
      return attributes.Length != 0;
    }

    public static bool CanViewType(Type viewType, Type content) {
      ContentAttribute[] attributes = (ContentAttribute[])viewType.GetCustomAttributes(typeof(ContentAttribute), false);
      return attributes.Any(a => content.IsAssignableTo(a.type));
    }

    internal static IEnumerable<Type> GetDefaultViewableTypes(Type viewType) {
      ContentAttribute[] attributes = (ContentAttribute[])viewType.GetCustomAttributes(typeof(ContentAttribute), false);
      return from a in attributes
             where a.isDefaultView
             select a.type;
    }
  }
}

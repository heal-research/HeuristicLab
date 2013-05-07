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
using System.Reflection;

namespace HeuristicLab.MainForm {
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
  public sealed class ContentAttribute : Attribute {
    public ContentAttribute(Type contentType) {
      this.contentType = contentType;
    }

    public ContentAttribute(Type contentType, bool isDefaultView)
      : this(contentType) {
      this.isDefaultView = isDefaultView;
    }

    private bool isDefaultView;
    public bool IsDefaultView {
      get { return this.isDefaultView; }
      set { this.isDefaultView = value; }
    }

    private Type contentType;
    public Type ContentType {
      get { return this.contentType; }
    }

    public static bool HasContentAttribute(MemberInfo viewType) {
      ContentAttribute[] attributes = (ContentAttribute[])viewType.GetCustomAttributes(typeof(ContentAttribute), false);
      return attributes.Length != 0;
    }

    public static bool CanViewType(MemberInfo viewType, Type content) {
      ContentAttribute[] attributes = (ContentAttribute[])viewType.GetCustomAttributes(typeof(ContentAttribute), false);
      return attributes.Any(a => content.IsAssignableTo(a.contentType));
    }

    internal static IEnumerable<Type> GetDefaultViewableTypes(Type viewType) {
      ContentAttribute[] attributes = (ContentAttribute[])viewType.GetCustomAttributes(typeof(ContentAttribute), false);
      return from a in attributes
             where a.isDefaultView
             select a.contentType;
    }

    internal static IEnumerable<Type> GetViewableTypes(Type viewType) {
      ContentAttribute[] attributes = (ContentAttribute[])viewType.GetCustomAttributes(typeof(ContentAttribute), false);
      return from a in attributes
             select a.contentType;
    }
  }
}

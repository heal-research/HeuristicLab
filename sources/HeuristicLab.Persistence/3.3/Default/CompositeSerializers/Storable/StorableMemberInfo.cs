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
using System.Reflection;
using System.Text;

namespace HeuristicLab.Persistence.Default.CompositeSerializers.Storable {

  internal sealed class StorableMemberInfo {
    public MemberInfo MemberInfo { get; private set; }
    public string DisentangledName { get; private set; }
    public object DefaultValue { get; private set; }
    public string FullyQualifiedMemberName {
      get {
        return new StringBuilder()
          .Append(MemberInfo.ReflectedType.FullName)
          .Append('.')
          .Append(MemberInfo.Name)
          .ToString();
      }
    }
    public StorableMemberInfo(StorableAttribute attribute, MemberInfo memberInfo) {
      DisentangledName = attribute.Name;
      DefaultValue = attribute.DefaultValue;
      MemberInfo = memberInfo;
    }
    public StorableMemberInfo(MemberInfo memberInfo) {
      MemberInfo = memberInfo;
    }
    public void SetDisentangledName(string name) {
      if (DisentangledName == null)
        DisentangledName = name;
    }
    public Type GetPropertyDeclaringBaseType() {
      return ((PropertyInfo)MemberInfo).GetGetMethod(true).GetBaseDefinition().DeclaringType;
    }
  }
}
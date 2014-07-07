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
using System.Reflection;
using System.Text;
using HeuristicLab.Persistence.Core;

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
      if (!attribute.AllowOneWay)
        CheckPropertyAccess(memberInfo as PropertyInfo);
    }
    public StorableMemberInfo(MemberInfo memberInfo, bool allowOneWay) {
      MemberInfo = memberInfo;
      if (!allowOneWay)
        CheckPropertyAccess(memberInfo as PropertyInfo);
    }
    private static void CheckPropertyAccess(PropertyInfo propertyInfo) {
      if (propertyInfo == null)
        return;
      if (!propertyInfo.CanRead || !propertyInfo.CanWrite)
        throw new PersistenceException("Properties must be readable and writable or explicity enable one way serialization.");
    }
    public void SetDisentangledName(string name) {
      if (DisentangledName == null)
        DisentangledName = name;
    }
    /// <summary>
    /// Gets the type who first defined this property in the class hierarchy when the
    /// property has subsequently been overridden but not shadowed with <code>new</code>.
    /// </summary>
    /// <returns>The properties base type.</returns>
    public Type GetPropertyDeclaringBaseType() {
      PropertyInfo pi = MemberInfo as PropertyInfo;
      if (pi == null)
        throw new PersistenceException("fields don't have a declaring base type, directly use FullyQualifiedMemberName instead");
      if (pi.CanRead)
        return pi.GetGetMethod(true).GetBaseDefinition().DeclaringType;
      if (pi.CanWrite)
        return pi.GetSetMethod(true).GetBaseDefinition().DeclaringType;
      throw new InvalidOperationException("property has neigher a getter nor a setter.");
    }
  }
}
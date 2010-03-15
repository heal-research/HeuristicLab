using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Persistence.Interfaces;
using HeuristicLab.Persistence.Core;
using System.Reflection;
using HeuristicLab.Persistence.Auxiliary;
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
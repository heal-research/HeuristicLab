using System;
using System.Collections.Generic;
using System.Reflection;

namespace HeuristicLab.Persistence.Core {

  
  [AttributeUsage(
    AttributeTargets.Class,
    AllowMultiple=false,
    Inherited=false)]
  public class EmptyStorableClassAttribute : Attribute {

    private static readonly Dictionary<Type, bool> emptyTypeInfo = new Dictionary<Type, bool>();
    public static bool IsEmptyStorable(Type type) {      
      if (emptyTypeInfo.ContainsKey(type))
        return emptyTypeInfo[type];
      foreach (var attribute in type.GetCustomAttributes(false)) {
        EmptyStorableClassAttribute empty = attribute as EmptyStorableClassAttribute;
        if (empty != null) {
          emptyTypeInfo.Add(type, true);
          return true;
        }
      }
      int nFields = 0;
      foreach ( MemberInfo memberInfo in type.GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) ) {
        if (memberInfo.MemberType == MemberTypes.Field ||
          memberInfo.MemberType == MemberTypes.Property)
          nFields += 1;
      }
      if (nFields == 0) {
        emptyTypeInfo.Add(type, true);
        return true;
      }
      emptyTypeInfo.Add(type, false);
      return false;
    }
  }  
}

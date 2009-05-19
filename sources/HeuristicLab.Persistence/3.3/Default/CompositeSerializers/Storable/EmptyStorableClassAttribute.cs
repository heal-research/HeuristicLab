using System;
using System.Collections.Generic;
using System.Reflection;

namespace HeuristicLab.Persistence.Default.CompositeSerializers.Storable {

  [AttributeUsage(
    AttributeTargets.Class,
    AllowMultiple = false,
    Inherited = false)]
  public class EmptyStorableClassAttribute : Attribute {

    private static readonly Dictionary<Type, bool> emptyTypeInfo = new Dictionary<Type, bool>();

    private const BindingFlags BINDING_FLAGS =
      BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

    /// <summary>
    /// Check if supplied type is empty (i.e. no properties or fields) or is marked as empty storable.
    /// This method is not recusrice and does not check if base types are also empty.
    /// </summary>    
    public static bool IsEmptyStorable(Type type) {
      if (emptyTypeInfo.ContainsKey(type))
        return emptyTypeInfo[type];
      if (type == typeof(object)) {
        return true;
      }
      foreach (var attribute in type.GetCustomAttributes(false)) {        
        if (attribute as EmptyStorableClassAttribute != null) {
          emptyTypeInfo.Add(type, true);
          return true;
        }
      }      
      foreach (MemberInfo memberInfo in type.GetMembers(BINDING_FLAGS)) {
        if ( 
          memberInfo.MemberType == MemberTypes.Field && IsModifiableField((FieldInfo)memberInfo) ||          
          memberInfo.MemberType == MemberTypes.Property && IsModifiableProperty((PropertyInfo)memberInfo) ) {
          emptyTypeInfo.Add(type, false);
          return false;
        }          
      }      
      emptyTypeInfo.Add(type, true);
      return true;            
    }

    public static bool IsModifiableField(FieldInfo fi) {      
      return !fi.IsLiteral && !fi.IsInitOnly;
    }
    public static bool IsModifiableProperty(PropertyInfo pi) {
      return pi.CanWrite;
    }
  }
}

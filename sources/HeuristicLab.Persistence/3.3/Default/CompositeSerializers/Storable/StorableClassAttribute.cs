using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace HeuristicLab.Persistence.Default.CompositeSerializers.Storable {


  /// <summary>
  /// Specifies which memebrs are selected for serialization by the StorableSerializer
  /// </summary>
  public enum StorableClassType {

    /// <summary>
    /// Serialize the class completely empty
    /// (ignore further [Storable] attributes inside the class)
    /// </summary>
    Empty,

    /// <summary>
    /// Serialize only fields and properties that have been marked
    /// with the [Storable] attribute
    /// </summary>
    MarkedOnly,

    /// <summary>
    /// Serialize all fields but ignore the 
    /// [Storable] attribute on properties.
    /// </summary>
    [Obsolete("not implemented yet")]
    AllFields,

    /// <summary>
    /// Serialize all properties but ignore the
    /// [Storable] attirbute on fields.
    /// </summary>
    [Obsolete("not implemented yet")]
    AllProperties,

    /// <summary>
    /// Serialize all fields and all properties
    /// but ignore the [Storable] on all members.
    /// </summary>
    [Obsolete("not implemnted yet")]
    AllFieldsAndAllProperties
  };


  /// <summary>
  /// Mark a class to be considered by the <code>StorableSerializer</code>.
  /// </summary>
  [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
  public sealed class StorableClassAttribute : Attribute {


    /// <summary>
    /// Specify how members are selected for serialization.
    /// </summary>
    public StorableClassType Type { get; set; }    

    /// <summary>
    /// Mark a class to be serialize by the <code>StorableSerizlier</code>    
    /// </summary>    
    public StorableClassAttribute(StorableClassType type) {
      Type = type;
    }

    /// <summary>
    /// Check that the type is either empty i.e. has no fields or properties
    /// or conatins proper parameterization through the storable attribute.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="recusrive"></param>
    /// <returns></returns>
    public static bool IsStorableType(Type type, bool recusrive) {
      if (IsEmptyType(type, recusrive))
        return true;
      StorableClassAttribute attribute = type
        .GetCustomAttributes(typeof(StorableClassAttribute), false)
        .Cast<StorableClassAttribute>().SingleOrDefault();
      if (attribute == null)
        return false;
      if (!recusrive || type.BaseType == null)
        return true;
      else
        return IsStorableType(type.BaseType, true);
    }

    private const BindingFlags allDeclaredMembers =
      BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;


    public static bool IsEmptyType(Type type, bool recursive) {
      foreach (MemberInfo memberInfo in type.GetMembers(allDeclaredMembers)) {
        if (
          memberInfo.MemberType == MemberTypes.Field && IsModifiableField((FieldInfo)memberInfo) ||
          memberInfo.MemberType == MemberTypes.Property && IsModifiableProperty((PropertyInfo)memberInfo)) {
          return false;
        }
      }
      if (!recursive || type.BaseType == null)
        return true;
      else
        return IsEmptyType(type.BaseType, true);
    }

    private static bool IsModifiableField(FieldInfo fi) {
      return !fi.IsLiteral && !fi.IsInitOnly;
    }

    private static bool IsModifiableProperty(PropertyInfo pi) {
      return pi.CanWrite;
    }
  }
}


  

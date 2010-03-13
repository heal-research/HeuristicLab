using System;
using System.Reflection;
using HeuristicLab.Persistence.Core;

namespace HeuristicLab.Persistence.Default.CompositeSerializers.Storable {

  /// <summary>
  /// Encapsulation and abstraction for access a data member of an object
  /// regardless of it being a property or field. Addicionally a
  /// default value and an alternate name can be specified.
  /// </summary>
  public class DataMemberAccessor {

    /// <summary>
    /// The function to get the value of the data member.
    /// </summary>
    public readonly Func<object> Get;

    /// <summary>
    /// The function to set the value of the data member.
    /// </summary>
    public readonly Action<object> Set;

    /// <summary>
    /// The name of the data member.
    /// </summary>
    public readonly string Name;

    /// <summary>
    /// The default value of the data member, can remain <c>null</c>
    /// if no default value. If left null, this will also leave the
    /// default value for value types (e.g. 0 for <c>int</c>).
    /// </summary>
    public readonly object DefaultValue;


    /// <summary>
    /// Create a <see cref="DataMemberAccessor"/> from a FieldInfo or
    /// PropertyInfo for the give object.
    /// </summary>
    /// <param name="memberInfo">The member info.</param>
    /// <param name="name">The name.</param>
    /// <param name="defaultvalue">The defaultvalue.</param>
    /// <param name="obj">The object.</param>
    public DataMemberAccessor(MemberInfo memberInfo, string name, object defaultvalue, object obj) {
      Name = name;
      DefaultValue = defaultvalue;
      if (memberInfo.MemberType == MemberTypes.Field) {
        FieldInfo fieldInfo = (FieldInfo)memberInfo;
        Get = () => fieldInfo.GetValue(obj);
        Set = value => fieldInfo.SetValue(obj, value);
      } else if (memberInfo.MemberType == MemberTypes.Property) {
        PropertyInfo propertyInfo = (PropertyInfo)memberInfo;
        if (!propertyInfo.CanRead || !propertyInfo.CanWrite) {
          throw new PersistenceException(
            "Storable properties must implement both a Get and a Set Accessor. ");
        }
        Get = () => propertyInfo.GetValue(obj, null);
        Set = value => propertyInfo.SetValue(obj, value, null);
      } else {
        throw new PersistenceException(
          "The Storable attribute can only be applied to fields and properties.");
      }
    }

    /// <summary>
    /// Wrap existing getter and setter functions.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="defaultValue">The default value.</param>
    /// <param name="getter">The getter.</param>
    /// <param name="setter">The setter.</param>
    public DataMemberAccessor(string name, object defaultValue,
        Func<object> getter, Action<object> setter) {
      Name = name;
      DefaultValue = defaultValue;
      Get = getter;
      Set = setter;
    }

    /// <summary>
    /// Create an empty accessor that just encapsulates an object
    /// without access.
    /// </summary>
    /// <param name="o">The object</param>
    public DataMemberAccessor(object o) {
      Name = null;
      DefaultValue = null;
      Get = () => o;
      Set = null;
    }

    /// <summary>
    /// Create an empty accessor that just encapsulates an object
    /// without access.
    /// </summary>
    /// <param name="o">The object</param>
    /// <param name="name">The object's name.</param>
    public DataMemberAccessor(object o, string name) {
      Name = name;
      DefaultValue = null;
      Get = () => o;
      Set = null;
    }

    /// <summary>
    /// Returns a <see cref="System.String"/> that represents this instance.
    /// </summary>
    /// <returns>
    /// A <see cref="System.String"/> that represents this instance.
    /// </returns>
    public override string ToString() {
      return String.Format("DataMemberAccessor({0}, {1}, {2}, {3})",
        Name,
        DefaultValue ?? "<null>",
        Get.Method, Set.Method);
    }
  }

}
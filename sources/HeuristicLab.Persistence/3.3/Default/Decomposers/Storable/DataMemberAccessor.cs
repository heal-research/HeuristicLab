using System;
using System.Reflection;

namespace HeuristicLab.Persistence.Default.Decomposers.Storable {

  public class DataMemberAccessor {
    
    public readonly Func<object> Get;    
    public readonly Action<object> Set;
    public readonly string Name;
    public readonly object DefaultValue;

    public DataMemberAccessor(
        MemberInfo memberInfo,
        StorableAttribute storableAttribute,
        object obj) {
      if (memberInfo.MemberType == MemberTypes.Field) {
        FieldInfo fieldInfo = (FieldInfo)memberInfo;
        Get = () => fieldInfo.GetValue(obj);
        Set = value => fieldInfo.SetValue(obj, value);
      } else if (memberInfo.MemberType == MemberTypes.Property) {
        PropertyInfo propertyInfo = (PropertyInfo)memberInfo;
        if (!propertyInfo.CanRead || !propertyInfo.CanWrite) {
          throw new NotSupportedException(
            "Storable properties must implement both a Get and a Set Accessor. ");
        }
        Get = () => propertyInfo.GetValue(obj, null);
        Set = value => propertyInfo.SetValue(obj, value, null);
      } else {
        throw new NotSupportedException(
          "The Storable attribute can only be applied to fields and properties.");
      }
      Name = storableAttribute.Name ?? memberInfo.Name;
      DefaultValue = storableAttribute.DefaultValue;
    }

    public DataMemberAccessor(
        string name, object defaultValue,
        Func<object> getter, Action<object> setter) {
      Name = name;
      DefaultValue = defaultValue;
      Get = getter;
      Set = setter;
    }

    public DataMemberAccessor(object o) {
      Name = null;
      DefaultValue = null;
      Get = () => o;
      Set = null;
    }

    public DataMemberAccessor(object o, string name) {
      Name = name;
      DefaultValue = null;
      Get = () => o;
      Set = null;
    }


    public override string ToString() {
      return String.Format("DataMember({0}, {1}, {2}, {3})",
        Name,
        DefaultValue ?? "<null>",
        Get.Method, Set.Method);
    }
  }

}
#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Reflection.Emit;

namespace HeuristicLab.Persistence.Core {

  /// <summary>
  /// Encapsulation and abstraction for access a data member of an object
  /// regardless of it being a property or field. Additionally a
  /// default value and an alternate name can be specified.
  /// </summary>
  public sealed class DataMemberAccessor {

    #region fields

    /// <summary>
    /// The function to get the value of the data member.
    /// </summary>
    public readonly Func<object, object> Get;

    /// <summary>
    /// The function to set the value of the data member.
    /// </summary>
    public readonly Action<object, object> Set;

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

    #endregion

    #region constructors

    /// <summary>
    /// Create a <see cref="DataMemberAccessor"/> from a FieldInfo or
    /// PropertyInfo for the give object.
    /// </summary>
    /// <param name="memberInfo">The member info.</param>
    /// <param name="name">The name.</param>
    /// <param name="defaultvalue">The defaultvalue.</param>
    public DataMemberAccessor(MemberInfo memberInfo, string name, object defaultvalue) {
      Get = GenerateGetter(memberInfo);
      Set = GenerateSetter(memberInfo);
      Name = name;
      DefaultValue = defaultvalue;
    }

    /// <summary>
    /// Create an empty accessor that just encapsulates an object
    /// without access.
    /// </summary>    
    public DataMemberAccessor() {
      Name = null;
      DefaultValue = null;
      Get = Id;
    }

    /// <summary>
    /// Create an empty accessor that just encapsulates an object
    /// without access.
    /// </summary>    
    /// <param name="name">The object's name.</param>
    public DataMemberAccessor(string name) {
      Name = name;
      DefaultValue = null;
      Get = Id;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DataMemberAccessor"/> class using the
    /// getter and setter from an exisiting instance but with a new name and default value.
    /// </summary>
    /// <param name="dma">The existing DataMemberAccessor.</param>
    /// <param name="name">The new name.</param>
    /// <param name="defaultValue">The new default value.</param>
    public DataMemberAccessor(DataMemberAccessor dma, string name, object defaultValue) {
      Get = dma.Get;
      Set = dma.Set;
      this.Name = name;
      this.DefaultValue = defaultValue;
    }

    #endregion

    #region auxiliary methods

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

    /// <summary>
    /// The identity function
    /// </summary>
    /// <param name="o">An object.</param>
    /// <returns>its argument o unmodified.</returns>
    public static object Id(object o) {
      return o;
    }

    #endregion

    #region static methods (code generators)

    /// <summary>
    /// Generate a getter for the given field or property
    /// </summary>
    /// <param name="memberInfo">The member info.</param>
    /// <returns></returns>
    public static Func<object, object> GenerateGetter(MemberInfo memberInfo) {
      if (memberInfo.MemberType == MemberTypes.Field) {
        FieldInfo fieldInfo = (FieldInfo)memberInfo;
        return GenerateFieldGetter(fieldInfo);
      } else if (memberInfo.MemberType == MemberTypes.Property) {
        return GeneratePropertyGetter((PropertyInfo)memberInfo);
      } else {
        throw new PersistenceException(
          "The Storable attribute can only be applied to fields and properties.");
      }
    }

    /// <summary>
    /// Generates a setter for the given field or property.
    /// </summary>
    /// <param name="memberInfo">The member info.</param>
    /// <returns></returns>
    public static Action<object, object> GenerateSetter(MemberInfo memberInfo) {
      if (memberInfo.MemberType == MemberTypes.Field) {
        FieldInfo fieldInfo = (FieldInfo)memberInfo;
        return GenerateFieldSetter(fieldInfo);
      } else if (memberInfo.MemberType == MemberTypes.Property) {
        return GeneratePropertySetter((PropertyInfo)memberInfo);
      } else {
        throw new PersistenceException(
          "The Storable attribute can only be applied to fields and properties.");
      }
    }

    /// <summary>
    /// Generates a dynamically compiled getter to access fields (even private ones).
    /// </summary>
    /// <param name="fieldInfo">The field info.</param>
    /// <returns>A Func&lt;object, object&gt;</returns>
    public static Func<object, object> GenerateFieldGetter(FieldInfo fieldInfo) {
      DynamicMethod dm = new DynamicMethod("", typeof(object), new Type[] { typeof(object) }, fieldInfo.DeclaringType, true);
      ILGenerator ilgen = dm.GetILGenerator();
      ilgen.Emit(OpCodes.Ldarg_0);
      ilgen.Emit(OpCodes.Castclass, fieldInfo.DeclaringType);
      ilgen.Emit(OpCodes.Ldfld, fieldInfo);
      ilgen.Emit(OpCodes.Box, fieldInfo.FieldType);
      ilgen.Emit(OpCodes.Ret);
      return (Func<object, object>)dm.CreateDelegate(typeof(Func<object, object>));
    }

    /// <summary>
    /// Generates a dynamically compiled setter to access fields (even private ones).
    /// </summary>
    /// <param name="fieldInfo">The field info.</param>
    /// <returns>An Action&lt;object, object%gt;</returns>
    public static Action<object, object> GenerateFieldSetter(FieldInfo fieldInfo) {
      DynamicMethod dm = new DynamicMethod("", null, new Type[] { typeof(object), typeof(object) }, fieldInfo.DeclaringType, true);
      ILGenerator ilgen = dm.GetILGenerator();
      ilgen.Emit(OpCodes.Ldarg_0);
      ilgen.Emit(OpCodes.Castclass, fieldInfo.DeclaringType);
      ilgen.Emit(OpCodes.Ldarg_1);
      ilgen.Emit(OpCodes.Unbox_Any, fieldInfo.FieldType);
      ilgen.Emit(OpCodes.Stfld, fieldInfo);
      ilgen.Emit(OpCodes.Ret);
      return (Action<object, object>)dm.CreateDelegate(typeof(Action<object, object>));
    }

    /// <summary>
    /// Generates a dynamically compiled getter to access properties (even private ones).
    /// </summary>
    /// <param name="propertyInfo">The property info.</param>
    /// <returns>A Func&lt;object, object&gt;</returns>
    public static Func<object, object> GeneratePropertyGetter(PropertyInfo propertyInfo) {
      MethodInfo getter = propertyInfo.GetGetMethod(true);
      if (getter == null)
        return null;
      DynamicMethod dm = new DynamicMethod("", typeof(object), new Type[] { typeof(object) }, propertyInfo.DeclaringType, true);
      ILGenerator ilgen = dm.GetILGenerator();
      ilgen.Emit(OpCodes.Ldarg_0);
      ilgen.Emit(OpCodes.Castclass, propertyInfo.DeclaringType);
      ilgen.Emit(OpCodes.Callvirt, getter);
      ilgen.Emit(OpCodes.Box, propertyInfo.PropertyType);
      ilgen.Emit(OpCodes.Ret);
      return (Func<object, object>)dm.CreateDelegate(typeof(Func<object, object>));
    }

    /// <summary>
    /// Generates a dynamically compiled setter to access properties (even private ones).
    /// </summary>
    /// <param name="propertyInfo">The property info.</param>
    /// <returns>An Action&lt;object, object%gt;</returns>
    public static Action<object, object> GeneratePropertySetter(PropertyInfo propertyInfo) {
      MethodInfo setter = propertyInfo.GetSetMethod(true);
      if (setter == null)
        return null;
      DynamicMethod dm = new DynamicMethod("", null, new Type[] { typeof(object), typeof(object) }, propertyInfo.DeclaringType, true);
      ILGenerator ilgen = dm.GetILGenerator();
      ilgen.Emit(OpCodes.Ldarg_0);
      ilgen.Emit(OpCodes.Castclass, propertyInfo.DeclaringType);
      ilgen.Emit(OpCodes.Ldarg_1);
      ilgen.Emit(OpCodes.Unbox_Any, propertyInfo.PropertyType);
      ilgen.Emit(OpCodes.Callvirt, setter);
      ilgen.Emit(OpCodes.Ret);
      return (Action<object, object>)dm.CreateDelegate(typeof(Action<object, object>));
    }

    #endregion
  }

}
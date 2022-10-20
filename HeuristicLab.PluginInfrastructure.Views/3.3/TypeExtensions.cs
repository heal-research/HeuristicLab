#region License Information

/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.PluginInfrastructure {
  internal static class TypeExtensions {
    internal static bool IsNonDiscoverableType(this Type t) {
      return t.GetCustomAttributes(typeof(NonDiscoverableTypeAttribute), false).Any();
    }

    /// <summary>
    /// Constructs a concrete type from a given proto type.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="protoType"></param>
    /// <returns>The constructed type, a generic type definition or null, if a type construction is not possible</returns>
    /// <remarks>This method does not work with nested generic types</remarks>
    internal static Type BuildType(this Type type, Type protoType) {
      if (type == null || protoType == null) throw new ArgumentNullException();

      if (!type.IsGenericTypeDefinition) return type;
      if (protoType.IsGenericTypeDefinition) return type;
      if (!protoType.IsGenericType) return type;

      var typeGenericArguments = type.GetGenericArguments();
      var protoTypeGenericArguments = protoType.GetGenericArguments();
      if (typeGenericArguments.Length != protoTypeGenericArguments.Length) return null;

      for (int i = 0; i < typeGenericArguments.Length; i++) {
        var typeGenericArgument = typeGenericArguments[i];
        var protoTypeGenericArgument = protoTypeGenericArguments[i];

        //check class contraint on generic type parameter 
        if (typeGenericArgument.GenericParameterAttributes.HasFlag(GenericParameterAttributes.ReferenceTypeConstraint))
          if (!protoTypeGenericArgument.IsClass && !protoTypeGenericArgument.IsInterface && !protoType.IsArray) return null;

        //check default constructor constraint on generic type parameter 
        if (typeGenericArgument.GenericParameterAttributes.HasFlag(GenericParameterAttributes.DefaultConstructorConstraint))
          if (!protoTypeGenericArgument.IsValueType && protoTypeGenericArgument.GetConstructor(Type.EmptyTypes) == null) return null;

        //check type restrictions on generic type parameter
        foreach (var constraint in typeGenericArgument.GetGenericParameterConstraints())
          if (!constraint.IsAssignableFrom(protoTypeGenericArgument)) return null;
      }

      try {
        return type.MakeGenericType(protoTypeGenericArguments);
      }
      catch (Exception) {
        return null;
      }
    }

    internal static bool IsSubTypeOf(this Type subType, Type baseType) {
      if (baseType.IsAssignableFrom(subType)) return true;
      if (!baseType.IsGenericType) return false;

      if (RecursiveCheckGenericTypes(baseType, subType)) return true;
      IEnumerable<Type> implementedInterfaces = subType.GetInterfaces().Where(t => t.IsGenericType);
      foreach (var implementedInterface in implementedInterfaces.Where(i => i.IsGenericType)) {
        if (baseType.CheckGenericTypes(implementedInterface)) return true;
      }

      return false;
    }

    private static bool RecursiveCheckGenericTypes(Type baseType, Type subType) {
      if (!baseType.IsGenericType) return false;
      if (!subType.IsGenericType) return false;
      if (baseType.CheckGenericTypes(subType)) return true;
      if (subType.BaseType == null) return false;

      return RecursiveCheckGenericTypes(baseType, subType.BaseType);
    }

    private static bool CheckGenericTypes(this Type baseType, Type subType) {
      var baseTypeGenericTypeDefinition = baseType.GetGenericTypeDefinition();
      var subTypeGenericTypeDefinition = subType.GetGenericTypeDefinition();
      if (baseTypeGenericTypeDefinition != subTypeGenericTypeDefinition) return false;
      var baseTypeGenericArguments = baseType.GetGenericArguments();
      var subTypeGenericArguments = subType.GetGenericArguments();

      for (int i = 0; i < baseTypeGenericArguments.Length; i++) {
        var baseTypeGenericArgument = baseTypeGenericArguments[i];
        var subTypeGenericArgument = subTypeGenericArguments[i];

        if (baseTypeGenericArgument.IsGenericParameter ^ subTypeGenericArgument.IsGenericParameter) return false;
        if (baseTypeGenericArgument == subTypeGenericArgument) continue;
        if (!baseTypeGenericArgument.IsGenericParameter && !subTypeGenericArgument.IsGenericParameter) return false;

        if (baseTypeGenericArgument.GenericParameterAttributes.HasFlag(GenericParameterAttributes.ReferenceTypeConstraint) &&
            !subTypeGenericArgument.GenericParameterAttributes.HasFlag(GenericParameterAttributes.ReferenceTypeConstraint)) return false;
        if (baseTypeGenericArgument.GenericParameterAttributes.HasFlag(GenericParameterAttributes.DefaultConstructorConstraint) &&
            !subTypeGenericArgument.GenericParameterAttributes.HasFlag(GenericParameterAttributes.DefaultConstructorConstraint)) return false;
        if (baseTypeGenericArgument.GenericParameterAttributes.HasFlag(GenericParameterAttributes.NotNullableValueTypeConstraint) &&
            !subTypeGenericArgument.GenericParameterAttributes.HasFlag(GenericParameterAttributes.NotNullableValueTypeConstraint)) return false;

        foreach (var baseTypeGenericParameterConstraint in baseTypeGenericArgument.GetGenericParameterConstraints()) {
          if (!subTypeGenericArgument.GetGenericParameterConstraints().Any(t => baseTypeGenericParameterConstraint.IsAssignableFrom(t))) return false;
        }
      }
      return true;
    }
  }
}

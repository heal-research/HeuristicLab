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

    internal static bool IsAssignableTo(this Type subType, Type baseType) {
      if (baseType.IsAssignableFrom(subType)) return true;

      //check generics
      if (!baseType.IsGenericType) return false;
      if (RecursiveCheckGenericTypes(baseType, subType)) return true;

      //check generic interfaces
      IEnumerable<Type> implementedInterfaces = subType.GetInterfaces().Where(t => t.IsGenericType);
      return implementedInterfaces.Any(implementedInterface => baseType.CheckGenericTypes(implementedInterface));
    }

    private static bool RecursiveCheckGenericTypes(Type baseType, Type subType) {
      if (!baseType.IsGenericType) return false;
      if (subType.IsGenericType && baseType.CheckGenericTypes(subType)) return true;
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

        //no generic parameters => concrete types => check for type equality (ignore co- and contravariance)
        //for example List<int> is only a List<int>, IParameter<IItem> is not a base type of IParameter<DoubleValue>
        if (!baseTypeGenericArgument.IsGenericParameter && !subTypeGenericArgument.IsGenericParameter) {
          if (baseTypeGenericArgument == subTypeGenericArgument) continue;
          return false;
        }

        //baseTypeGenericArgument is a concrete type and the subTypeGenericArgument is a generic parameter
        //for example List<int> is not a base type of List<T>
        if (!baseTypeGenericArgument.IsGenericParameter && subTypeGenericArgument.IsGenericParameter) return false;

        //baseTypeGenericArugment is a generic parameter and the subTypeGenericArgument is a concrete type => check type constraints
        //for example IParameter<T> is a base type of IParameter<IItem> if all generic contraints on T are fulfilled
        if (baseTypeGenericArgument.IsGenericParameter && !subTypeGenericArgument.IsGenericParameter) {
          if (baseTypeGenericArgument.GenericParameterAttributes.HasFlag(GenericParameterAttributes.ReferenceTypeConstraint) &&
            subTypeGenericArgument.IsValueType) return false;
          if (baseTypeGenericArgument.GenericParameterAttributes.HasFlag(GenericParameterAttributes.DefaultConstructorConstraint) &&
            subTypeGenericArgument.GetConstructor(Type.EmptyTypes) == null) return false;
          if (baseTypeGenericArgument.GenericParameterAttributes.HasFlag(GenericParameterAttributes.NotNullableValueTypeConstraint)) {
            if (!subTypeGenericArgument.IsValueType) return false;
            if (subTypeGenericArgument.IsGenericType && subTypeGenericArgument.GetGenericTypeDefinition() == typeof(Nullable<>))
              return false;
          }

          //not assignable if the subTypeGenericArgument is not assignable to all of the constraints of the base type
          if (baseTypeGenericArgument.GetGenericParameterConstraints().Any(baseTypeGenericParameterConstraint =>
            !baseTypeGenericParameterConstraint.IsAssignableFrom(subTypeGenericArgument)))
            return false;
        }

        //both generic arguments are generic parameters => check type constraints
        //for example IParameter<T> is a base type of IParameter<T>
        if (baseTypeGenericArgument.IsGenericParameter && subTypeGenericArgument.IsGenericParameter) {
          if (baseTypeGenericArgument.GenericParameterAttributes.HasFlag(GenericParameterAttributes.ReferenceTypeConstraint) &&
              !subTypeGenericArgument.GenericParameterAttributes.HasFlag(GenericParameterAttributes.ReferenceTypeConstraint)) return false;
          if (baseTypeGenericArgument.GenericParameterAttributes.HasFlag(GenericParameterAttributes.DefaultConstructorConstraint) &&
              !subTypeGenericArgument.GenericParameterAttributes.HasFlag(GenericParameterAttributes.DefaultConstructorConstraint)) return false;
          if (baseTypeGenericArgument.GenericParameterAttributes.HasFlag(GenericParameterAttributes.NotNullableValueTypeConstraint) &&
              !subTypeGenericArgument.GenericParameterAttributes.HasFlag(GenericParameterAttributes.NotNullableValueTypeConstraint)) return false;

          //not assignable if any of the constraints is not assignable to the constraints of the base type
          if (baseTypeGenericArgument.GetGenericParameterConstraints().Any(baseTypeGenericParameterConstraint => !subTypeGenericArgument.GetGenericParameterConstraints().Any(t => baseTypeGenericParameterConstraint.IsAssignableFrom(t)))) {
            return false;
          }
        }
      }
      return true;
    }
  }
}

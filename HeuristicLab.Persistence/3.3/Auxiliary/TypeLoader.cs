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
using System.Reflection;
using HEAL.Attic;
using HeuristicLab.Tracing;

namespace HeuristicLab.Persistence.Auxiliary {
  internal class TypeLoader {
    #region Mono Compatibility
    private static TypeName cachedMonoRuntimeType;
    private static TypeName cachedWindowsRuntimeType;
    private static TypeName cachedMonoObjectEqualityComparerType;
    private static TypeName cachedWindowsObjectEqualityComparerType;

    private static bool MonoInstalled {
      get { return Type.GetType("Mono.Runtime") != null; }
    }

    static TypeLoader() {
      // we use Int32 here because we get all the information about Mono's mscorlib and just have to change the class name
      cachedMonoRuntimeType = TypeNameParser.Parse(typeof(System.Int32).AssemblyQualifiedName);
      cachedMonoRuntimeType = new TypeName(cachedMonoRuntimeType, "MonoType");

      cachedWindowsRuntimeType = TypeNameParser.Parse(typeof(System.Int32).AssemblyQualifiedName);
      cachedWindowsRuntimeType = new TypeName(cachedWindowsRuntimeType, "RuntimeType");

      // we need the information about the Persistence assembly, so we use TypeName here because it is contained in this assembly
      cachedMonoObjectEqualityComparerType = TypeNameParser.Parse(typeof(TypeName).AssemblyQualifiedName);
      cachedMonoObjectEqualityComparerType = new TypeName(cachedMonoObjectEqualityComparerType, "ObjectEqualityComparer", "HeuristicLab.Persistence.Mono");

      cachedWindowsObjectEqualityComparerType = TypeNameParser.Parse(typeof(System.Int32).AssemblyQualifiedName);
      cachedWindowsObjectEqualityComparerType = new TypeName(cachedWindowsObjectEqualityComparerType, "ObjectEqualityComparer", "System.Collections.Generic");
    }
    #endregion

    public static Type Load(string typeNameString) {
      TypeName typeName = null;
      try {
        typeName = TypeNameParser.Parse(typeNameString);
      }
      catch (Exception) {
        throw new PersistenceException(String.Format(
           "Could not parse type string \"{0}\"",
           typeNameString));
      }

      try {
        // try to load type normally
        return LoadInternal(typeName);
      }
      catch (PersistenceException) {
        #region Mono Compatibility
        // if that fails, try to convert to the corresponding Mono or .NET type
        if (MonoInstalled) {
          typeName = GetMonoType(typeName);
          Logger.Info(String.Format(@"Trying to load Mono type ""{0}"" instead of .NET type ""{1}""",
                                    typeName, typeNameString));
        } else {
          typeName = GetDotNetType(typeName);
          Logger.Info(String.Format(@"Trying to load .NET type ""{0}"" instead of Mono type ""{1}""",
                                    typeName, typeNameString));

        }
        return LoadInternal(typeName);
        #endregion
      }
    }

    private static Type LoadInternal(TypeName typeName) {
      Type type;
      try {
        type = Type.GetType(typeName.ToString(true, true), true);
      }
      catch (Exception) {
        Logger.Warn(String.Format(
          "Cannot load type \"{0}\", falling back to partial name", typeName.ToString(true, true)));
        type = LoadWithPartialName(typeName);
        CheckCompatibility(typeName, type);
      }
      return type;
    }

    private static Type LoadWithPartialName(TypeName typeName) {
      try {
#pragma warning disable 0618
        Assembly a = Assembly.LoadWithPartialName(typeName.AssemblyName);
        // the suggested Assembly.Load() method fails to load assemblies outside the GAC
#pragma warning restore 0618
        return a.GetType(typeName.ToString(false, false), true);
      }
      catch (Exception) {
        throw new PersistenceException(String.Format(
          "Could not load type \"{0}\"",
          typeName.ToString(true, true)));
      }
    }

    private static void CheckCompatibility(TypeName typeName, Type type) {
      try {
        TypeName loadedTypeName = TypeNameParser.Parse(type.AssemblyQualifiedName);
        if (!typeName.IsCompatible(loadedTypeName))
          throw new PersistenceException(String.Format(
            "Serialized type is incompatible with available type: serialized: {0}, loaded: {1}",
            typeName.ToString(true, true),
            type.AssemblyQualifiedName));
        if (typeName.IsNewerThan(loadedTypeName))
          throw new PersistenceException(String.Format(
            "Serialized type is newer than available type: serialized: {0}, loaded: {1}",
            typeName.ToString(true, true),
            type.AssemblyQualifiedName));
      }
      catch (PersistenceException) {
        throw;
      }
      catch (Exception e) {
        Logger.Warn(String.Format(
          "Could not perform version check requested type was {0} while loaded type is {1}:",
          typeName.ToString(true, true),
          type.AssemblyQualifiedName),
                    e);
      }
    }

    #region Mono Compatibility
    /// <summary>
    /// Returns the corresponding type for the Mono runtime
    /// </summary>
    /// <returns>
    /// The remapped typeName, or the original typeName if no mapping was found
    /// </returns>
    private static TypeName GetMonoType(TypeName typeName) {
      // map System.RuntimeType to System.MonoType
      if (typeName.Namespace == "System" && typeName.ClassName == "RuntimeType") {
        return cachedMonoRuntimeType;
        // map System.Collections.Generic.ObjectEqualityComparer to HeuristicLab.Mono.ObjectEqualityComparer
      } else if (typeName.Namespace == "System.Collections.Generic" && typeName.ClassName == "ObjectEqualityComparer") {
        TypeName newTypeName = new TypeName(cachedMonoObjectEqualityComparerType);
        newTypeName.GenericArgs = new List<TypeName>(typeName.GenericArgs);
        return newTypeName;
      }
      return typeName;
    }

    /// <summary>
    /// Returns the corresponding type for the .NET runtime
    /// </summary>
    /// <returns>
    /// The remapped typeName, or the original typeName if no mapping was found
    /// </returns>
    private static TypeName GetDotNetType(TypeName typeName) {
      // map System.MonoType to System.RuntimeType
      if (typeName.Namespace == "System" && typeName.ClassName == "MonoType") {
        return cachedWindowsRuntimeType;
        // maps Mono's string comparer to System.Collections.Generic.ObjectEqualityComparer<string>
      } else if (typeName.Namespace == "System.Collections.Generic" && typeName.ClassName == "InternalStringComparer") {
        TypeName newTypeName = new TypeName(cachedWindowsObjectEqualityComparerType);
        var genericArgsList = new List<TypeName>();
        genericArgsList.Add(new TypeName(typeof(String).Namespace, "String"));
        newTypeName.GenericArgs = genericArgsList;
        return newTypeName;
      } else if (typeName.Namespace == "System.Collections.Generic" && typeName.ClassName == "EqualityComparer+DefaultComparer") {
        TypeName newTypeName = new TypeName(cachedWindowsObjectEqualityComparerType);
        newTypeName.GenericArgs = new List<TypeName>(typeName.GenericArgs);
        return newTypeName;
      }
      return typeName;
    }
    #endregion
  }
}
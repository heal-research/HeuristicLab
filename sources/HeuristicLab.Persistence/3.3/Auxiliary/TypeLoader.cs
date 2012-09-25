#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Persistence.Core;
using HeuristicLab.Tracing;

namespace HeuristicLab.Persistence.Auxiliary {
  internal class TypeLoader {
    #region Mono Compatibility
    private static TypeName cachedRuntimeType;
    private static TypeName cachedObjectEqualityComparerType;

    static TypeLoader() {
      // we use Int32 here because we get all the information about Mono's mscorlib and just have to change the class name
      cachedRuntimeType = TypeNameParser.Parse(typeof(System.Int32).AssemblyQualifiedName);
      cachedRuntimeType = new TypeName(cachedRuntimeType, "MonoType");

      // we need the information about the Persistence assembly, so we use TypeName here because it is contained in this assembly
      cachedObjectEqualityComparerType = TypeNameParser.Parse(typeof(TypeName).AssemblyQualifiedName);
      cachedObjectEqualityComparerType = new TypeName(cachedObjectEqualityComparerType, "ObjectEqualityComparer", "HeuristicLab.Persistence.Mono");
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
        // if that fails, try to load Mono type
        TypeName monoTypeName = GetMonoType(typeName);
        Logger.Info(String.Format(@"Trying to load Mono type ""{0}"" instead of type ""{1}""",
                                  monoTypeName, typeNameString));
        return LoadInternal(monoTypeName);
      }
        #endregion
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
    /// The remapped typeNameString, or the original string if no mapping was found
    /// </returns>
    private static TypeName GetMonoType(TypeName typeName) {
      // map System.RuntimeType to System.MonoType
      if (typeName.Namespace == "System" && typeName.ClassName == "RuntimeType") {
        return cachedRuntimeType;
        // map System.Collections.Generic.ObjectEqualityComparer to HeuristicLab.Mono.ObjectEqualityComparer
      } else if (typeName.Namespace == "System.Collections.Generic" && typeName.ClassName == "ObjectEqualityComparer") {
        TypeName newTypeName = new TypeName(cachedObjectEqualityComparerType);
        newTypeName.GenericArgs = new List<TypeName>(typeName.GenericArgs);
        return newTypeName;
      }
      return typeName;
    }
    #endregion
  }
}
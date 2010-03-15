using System.Collections.Generic;
using System;
using HeuristicLab.Persistence.Interfaces;
using HeuristicLab.Persistence.Core.Tokens;
using HeuristicLab.Persistence.Auxiliary;
using HeuristicLab.Tracing;
using System.Reflection;
using HeuristicLab.Persistence.Core;

namespace HeuristicLab.Persistence.Auxiliary {

  internal class TypeLoader {

    public static Type Load(string typeNameString) {
      Type type;
      try {
        type = Type.GetType(typeNameString, true);
      } catch (Exception) {
        Logger.Warn(String.Format(
          "Cannot load type \"{0}\", falling back to partial name", typeNameString));
        try {
          TypeName typeName = TypeNameParser.Parse(typeNameString);             
#pragma warning disable 0618
          Assembly a = Assembly.LoadWithPartialName(typeName.AssemblyName);          
          // the suggested Assembly.Load() method fails to load assemblies outside the GAC
#pragma warning restore 0618
          type = a.GetType(typeName.ToString(false, false), true);
        } catch (Exception) {
          throw new PersistenceException(String.Format(
            "Could not load type \"{0}\"",
            typeNameString));
        }
        try {
          TypeName requestedTypeName = TypeNameParser.Parse(typeNameString);
          TypeName loadedTypeName = TypeNameParser.Parse(type.AssemblyQualifiedName);
          if (!requestedTypeName.IsCompatible(loadedTypeName))
            throw new PersistenceException(String.Format(
              "Serialized type is incompatible with available type: serialized: {0}, loaded: {1}",
              typeNameString,
              type.AssemblyQualifiedName));
          if (requestedTypeName.IsNewerThan(loadedTypeName))
            throw new PersistenceException(String.Format(
              "Serialized type is newer than available type: serialized: {0}, loaded: {1}",
              typeNameString,
              type.AssemblyQualifiedName));
        } catch (PersistenceException) {
          throw;
        } catch (Exception e) {
          Logger.Warn(String.Format(
            "Could not perform version check requested type was {0} while loaded type is {1}:",
            typeNameString,
            type.AssemblyQualifiedName),
            e);
        }
      }
      return type;
    }
  }
}
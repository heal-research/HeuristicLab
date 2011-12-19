#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Persistence.Core;
using HeuristicLab.Tracing;

namespace HeuristicLab.Persistence.Auxiliary {

  internal class TypeLoader {

    public static Type Load(string typeNameString) {
      Type type;
      try {
        type = Type.GetType(typeNameString, true);
      }
      catch (Exception) {
        Logger.Warn(String.Format(
          "Cannot load type \"{0}\", falling back to partial name", typeNameString));
        try {
          TypeName typeName = TypeNameParser.Parse(typeNameString);
#pragma warning disable 0618
          Assembly a = Assembly.LoadWithPartialName(typeName.AssemblyName);
          // the suggested Assembly.Load() method fails to load assemblies outside the GAC
#pragma warning restore 0618
          type = a.GetType(typeName.ToString(false, false), true);
        }
        catch (Exception) {
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
        }
        catch (PersistenceException) {
          throw;
        }
        catch (Exception e) {
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
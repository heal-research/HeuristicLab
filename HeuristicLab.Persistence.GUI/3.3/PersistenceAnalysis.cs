#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Windows.Forms;
using HeuristicLab.Persistence.Core;

namespace HeuristicLab.Persistence.GUI {
  public class PersistenceAnalysis {

    public static bool IsSerializable(Type type, Configuration config) {
      if (config.PrimitiveSerializers.Any(ps => ps.SourceType == type))
        return true;
      foreach (var cs in config.CompositeSerializers) {
        if (cs.CanSerialize(type))
          return true;
      }
      return false;
    }

    private static bool DerivesFrom(Type baseType, Type type) {
      if (type == baseType)
        return true;
      if (type.BaseType == null)
        return false;
      return DerivesFrom(baseType, type.BaseType);
    }

    public static IEnumerable<Type> NonSerializableTypes(Configuration config) {
      var types = new List<Type>();
      var storableInconsistentcy = new List<Type>();
      foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies()) {
        if (assembly.FullName.StartsWith("System.") ||
          assembly.FullName.StartsWith("HeuristicLab.PluginInfrastructure") ||
          assembly.FullName.StartsWith("log4net") ||
          assembly.FullName.StartsWith("WindowsBase") ||
          assembly.FullName.StartsWith("WeifenLuo") ||
          assembly.FullName.StartsWith("ICSharpCode") ||
          assembly.FullName.StartsWith("Mono") ||
          assembly.FullName.StartsWith("Netron"))
          continue;
        foreach (var type in assembly.GetTypes()) {
          if (type.IsInterface || type.IsAbstract ||
              type.FullName.StartsWith("System.") ||
              type.FullName.StartsWith("Microsoft.") ||
              type.FullName.Contains("<") ||
              type.FullName.Contains(">") ||
              DerivesFrom(typeof(Exception), type) ||
              DerivesFrom(typeof(Control), type) ||
              DerivesFrom(typeof(System.EventArgs), type) ||
              DerivesFrom(typeof(System.Attribute), type) ||
              type.GetInterface("HeuristicLab.MainForm.IUserInterfaceItem") != null
            )
            continue;
          try {
            if (!IsSerializable(type, config))
              types.Add(type);
            /* if (!IsCorrectlyStorable(type))
              storableInconsistentcy.Add(type); */
          }
          catch {
            types.Add(type);
          }
        }
      }
      return types;
    }

    /* private static bool IsCorrectlyStorable(Type type) {
      if (StorableAttribute.GetStorableMembers(type).Count() > 0) {
        if (!StorableClassAttribute.IsStorableType(type, true))
          return false;
        if (type.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public, null, Type.EmptyTypes, null) == null &&
          StorableConstructorAttribute.GetStorableConstructor(type) == null)
          return false;
      }
      return true;
    }  */
  }
}

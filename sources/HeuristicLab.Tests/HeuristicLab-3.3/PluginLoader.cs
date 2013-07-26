#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.IO;
using System.Linq;
using System.Reflection;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Tests {
  public static class PluginLoader {
    public const string ExecutableExtension = ".exe";
    public const string AssemblyExtension = ".dll";
    public const string TestAccessorAssemblyExtension = "_Accessor.dll";
    public const string TestAssemblyExtension = ".Tests.dll";
    public static List<Assembly> Assemblies;

    static PluginLoader() {
      foreach (string path in Directory.EnumerateFiles(Environment.CurrentDirectory).Where(IsRelevantAssemblyPath)) {
        try {
          Assembly.LoadFrom(path);
        } catch (BadImageFormatException) { }
      }
      // test relevant path again to exclude previously loaded assemblies
      Assemblies = AppDomain.CurrentDomain.GetAssemblies()
        .Where(asm => IsRelevantAssemblyPath(asm.Location))
        .ToList();
    }

    private static bool IsRelevantAssemblyPath(string path) {
      bool valid = true;
      valid = valid && path.EndsWith(ExecutableExtension, StringComparison.OrdinalIgnoreCase) || path.EndsWith(AssemblyExtension, StringComparison.OrdinalIgnoreCase);
      valid = valid && !path.EndsWith(TestAccessorAssemblyExtension, StringComparison.OrdinalIgnoreCase) && !path.EndsWith(TestAssemblyExtension, StringComparison.OrdinalIgnoreCase);
      return valid;
    }

    public static bool IsPluginAssembly(Assembly assembly) {
      return assembly.GetExportedTypes().Any(t => typeof(IPlugin).IsAssignableFrom(t) && !t.IsAbstract && !t.IsInterface);
    }
  }
}

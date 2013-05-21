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
using System.Text;
using HeuristicLab.PluginInfrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab_33.Tests {
  [TestClass]
  public class PluginDependenciesTest {
    private static Dictionary<Assembly, Type> loadedPlugins;
    private static Dictionary<string, string> pluginNames;
    private static Dictionary<string, Assembly> pluginFilesToPluginLookup = new Dictionary<string, Assembly>();

    // Use ClassInitialize to run code before running the first test in the class
    [ClassInitialize]
    public static void MyClassInitialize(TestContext testContext) {
      loadedPlugins = PluginLoader.Assemblies.Where(PluginLoader.IsPluginAssembly).ToDictionary(a => a, GetPluginFromAssembly);
      pluginNames = loadedPlugins.ToDictionary(a => a.Key.GetName().FullName, a => GetPluginName(a.Value));

      foreach (Assembly pluginAssembly in loadedPlugins.Keys) {
        Type pluginType = GetPluginFromAssembly(pluginAssembly);
        var pluginFileAttributes = Attribute.GetCustomAttributes(pluginType, false).OfType<PluginFileAttribute>();
        foreach (var pluginFileAttribute in pluginFileAttributes) {
          string fillNameWithoutExtension = Path.GetFileNameWithoutExtension(pluginFileAttribute.FileName);
          pluginFilesToPluginLookup.Add(fillNameWithoutExtension, pluginAssembly);
        }
      }
    }

    [TestMethod]
    public void CheckReferenceAssembliesForPluginDependencies() {
      StringBuilder errorMessage = new StringBuilder();
      foreach (Assembly pluginAssembly in loadedPlugins.Keys) {
        Type plugin = loadedPlugins[pluginAssembly];
        Dictionary<string, PluginDependencyAttribute> pluginDependencies = Attribute.GetCustomAttributes(plugin, false).OfType<PluginDependencyAttribute>().ToDictionary(a => a.Dependency);

        foreach (AssemblyName referencedAssemblyName in pluginAssembly.GetReferencedAssemblies()) {
          if (IsPluginAssemblyName(referencedAssemblyName)) {
            if (!pluginDependencies.ContainsKey(pluginNames[referencedAssemblyName.FullName]))
              errorMessage.AppendLine("Missing dependency in plugin " + plugin + " to referenced plugin " + pluginNames[referencedAssemblyName.FullName] + ".");
          } else { //no plugin assembly => test if the assembly is delivered by another plugin
            if (pluginFilesToPluginLookup.ContainsKey(referencedAssemblyName.Name)) {
              string containingPluginFullName = pluginFilesToPluginLookup[referencedAssemblyName.Name].FullName;
              if (containingPluginFullName != pluginAssembly.FullName && !pluginDependencies.ContainsKey(pluginNames[containingPluginFullName]))
                errorMessage.AppendLine("Missing dependency in plugin " + plugin + " to plugin " + pluginNames[containingPluginFullName] + " due to a reference to " + referencedAssemblyName.FullName + ".");
            }
          }
        }
      }
      Assert.IsTrue(errorMessage.Length == 0, errorMessage.ToString());
    }

    [TestMethod]
    public void CheckPluginDependenciesForReferencedAssemblies() {
      StringBuilder errorMessage = new StringBuilder();
      foreach (Assembly pluginAssembly in loadedPlugins.Keys) {
        Type plugin = loadedPlugins[pluginAssembly];
        Dictionary<PluginDependencyAttribute, string> pluginDependencies = Attribute.GetCustomAttributes(plugin, false).OfType<PluginDependencyAttribute>().ToDictionary(a => a, a => a.Dependency);

        foreach (PluginDependencyAttribute attribute in pluginDependencies.Keys) {
          string pluginDependencyName = pluginDependencies[attribute];

          if (pluginDependencyName == "HeuristicLab.MathJax") continue; //is never referenced as this plugin contains HTML files
          var referencedPluginAssemblies = pluginAssembly.GetReferencedAssemblies().Where(IsPluginAssemblyName);
          if (referencedPluginAssemblies.Any(a => pluginNames[a.FullName] == pluginDependencyName)) continue;

          var referencedNonPluginAssemblies = pluginAssembly.GetReferencedAssemblies().Where(a => !IsPluginAssemblyName(a));
          bool found = (from referencedNonPluginAssembly in referencedNonPluginAssemblies
                        select referencedNonPluginAssembly.Name into assemblyName
                        where pluginFilesToPluginLookup.ContainsKey(assemblyName)
                        select GetPluginFromAssembly(pluginFilesToPluginLookup[assemblyName]) into pluginType
                        select GetPluginName(pluginType)).Any(pluginName => pluginName == pluginDependencyName);

          if (!found) errorMessage.AppendLine("Unnecessary plugin dependency in " + GetPluginName(plugin) + " to " + pluginDependencyName + ".");
        }
      }
      Assert.IsTrue(errorMessage.Length == 0, errorMessage.ToString());
    }

    private static Type GetPluginFromAssembly(Assembly assembly) {
      return assembly.GetExportedTypes().Where(t => typeof(IPlugin).IsAssignableFrom(t) && !t.IsAbstract && !t.IsInterface).FirstOrDefault();
    }

    private static string GetPluginName(Type plugin) {
      string name = string.Empty;
      PluginAttribute pluginAttribute = (PluginAttribute)Attribute.GetCustomAttribute(plugin, typeof(PluginAttribute));
      if (pluginAttribute != null)
        name = pluginAttribute.Name;
      return name;
    }

    private static bool IsPluginAssemblyName(AssemblyName assemblyName) {
      return pluginNames.ContainsKey(assemblyName.FullName);
    }
  }
}

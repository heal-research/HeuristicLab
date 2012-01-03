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
    private static HashSet<string> extLibPluginNames;

    // Use ClassInitialize to run code before running the first test in the class
    [ClassInitialize]
    public static void MyClassInitialize(TestContext testContext) {
      loadedPlugins = PluginLoader.Assemblies.Where(x => PluginLoader.IsPluginAssembly(x)).ToDictionary(a => a, GetPluginFromAssembly);
      pluginNames = loadedPlugins.ToDictionary(a => a.Key.GetName().FullName, a => GetPluginName(a.Value));

      extLibPluginNames = new HashSet<string>();
      extLibPluginNames.Add("HeuristicLab.ALGLIB");
      extLibPluginNames.Add("HeuristicLab.LibSVM");
      extLibPluginNames.Add("HeuristicLab.log4net");
      extLibPluginNames.Add("HeuristicLab.Netron");
      extLibPluginNames.Add("HeuristicLab.ProtobufCS");
      extLibPluginNames.Add("HeuristicLab.SharpDevelop");
      extLibPluginNames.Add("HeuristicLab.WinFormsUI");
    }

    [TestMethod]
    public void CheckReferenceAssembliesForPluginDependencies() {
      StringBuilder errorMessage = new StringBuilder();
      foreach (Assembly pluginAssembly in loadedPlugins.Keys) {
        Type plugin = loadedPlugins[pluginAssembly];
        Dictionary<string, PluginDependencyAttribute> pluginDependencies =
          Attribute.GetCustomAttributes(plugin, false).OfType<PluginDependencyAttribute>().ToDictionary(a => a.Dependency);
        foreach (AssemblyName referencedPluginName in pluginAssembly.GetReferencedAssemblies())
          if (pluginNames.ContainsKey(referencedPluginName.FullName)) {  //check if reference assembly is a plugin
            if (!pluginDependencies.ContainsKey(pluginNames[referencedPluginName.FullName]))
              errorMessage.AppendLine("Missing dependency in plugin " + plugin + " to referenced plugin " + pluginNames[referencedPluginName.FullName] + ".");
          }
      }

      Assert.IsTrue(errorMessage.Length == 0, errorMessage.ToString());
    }

    [TestMethod]
    public void CheckPluginDependenciesForReferencedAssemblies() {
      StringBuilder errorMessage = new StringBuilder();
      foreach (Assembly pluginAssembly in loadedPlugins.Keys) {
        Type plugin = loadedPlugins[pluginAssembly];
        Dictionary<PluginDependencyAttribute, string> pluginDependencies =
          Attribute.GetCustomAttributes(plugin, false).OfType<PluginDependencyAttribute>().ToDictionary(a => a, a => a.Dependency);

        foreach (PluginDependencyAttribute attribute in pluginDependencies.Keys) {
          string pluginDependencyName = pluginDependencies[attribute];
          //do not check extlib plugins, because the transport assemblies are never referenced in the assemblies
          if (extLibPluginNames.Contains(pluginDependencyName)) continue;
          if (pluginAssembly.GetReferencedAssemblies().Where(a => pluginNames.ContainsKey(a.FullName))
            .All(a => pluginNames[a.FullName] != pluginDependencyName)) {
            errorMessage.AppendLine("Unnecessary plugin dependency in " + GetPluginName(plugin) + " to " + pluginDependencyName + ".");
          }
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
  }
}

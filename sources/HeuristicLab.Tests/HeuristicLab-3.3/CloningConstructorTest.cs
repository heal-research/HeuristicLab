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
using System.Linq;
using System.Reflection;
using System.Text;
using HeuristicLab.Common;
using HeuristicLab.PluginInfrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Tests {
  [TestClass]
  public class CloningConstructorTest {
    // Use ClassInitialize to run code before running the first test in the class
    [ClassInitialize]
    public static void MyClassInitialize(TestContext testContext) {
      PluginLoader.Assemblies.Any();
    }

    [TestMethod]
    public void TestCloningConstructor() {
      StringBuilder errorMessage = new StringBuilder();

      foreach (Type deepCloneableType in ApplicationManager.Manager.GetTypes(typeof(IDeepCloneable))) {
        //test only types contained in HL plugin assemblies
        if (!PluginLoader.Assemblies.Contains(deepCloneableType.Assembly)) continue;

        bool found = false;
        foreach (ConstructorInfo constructor in deepCloneableType.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)) {
          ParameterInfo[] parameters = constructor.GetParameters();
          if (parameters.Length == 2 && parameters[0].ParameterType == deepCloneableType && parameters[1].ParameterType == typeof(Cloner)) {
            found = true;
            if (deepCloneableType.IsSealed && !constructor.IsPrivate)
              errorMessage.Append(Environment.NewLine + deepCloneableType.ToString() + ": Cloning constructor must be private in sealed classes.");
            else if (!deepCloneableType.IsSealed && !(constructor.IsFamily || constructor.IsPublic))
              errorMessage.Append(Environment.NewLine + deepCloneableType.ToString() + ": Cloning constructor must be protected.");
            break;
          }
        }
        if (!found)
          errorMessage.Append(Environment.NewLine + deepCloneableType.ToString() + ": No cloning constructor is defined.");

        if (!deepCloneableType.IsAbstract) {
          MethodInfo cloneMethod = deepCloneableType.GetMethod("Clone", new Type[] { typeof(Cloner) });
          if (cloneMethod == null)
            errorMessage.Append(Environment.NewLine + deepCloneableType.ToString() + ": No virtual cloning method is defined.");
        }
      }
      Assert.IsTrue(errorMessage.Length == 0, errorMessage.ToString());
    }
  }
}

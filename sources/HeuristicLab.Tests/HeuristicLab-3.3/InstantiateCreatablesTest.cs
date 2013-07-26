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
using System.Linq;
using HeuristicLab.Core;
using HeuristicLab.PluginInfrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Tests {

  [TestClass]
  public class InstantiateCreatablesTest {
    // Use ClassInitialize to run code before running the first test in the class
    [ClassInitialize]
    public static void MyClassInitialize(TestContext testContext) {
      PluginLoader.Assemblies.Any();
    }
    [TestMethod]
    [TestCategory("General")]
    [TestCategory("Essential")]
    [TestProperty("Time", "medium")]
    public void InstantiateAllCreatables() {
      var exceptions = new List<string>();

      var types = from t in ApplicationManager.Manager.GetTypes(typeof(IItem))
                  where CreatableAttribute.IsCreatable(t)
                  select t;

      Assert.IsTrue(types.Any(), "No creatables found!");

      foreach (var t in types) {
        try {
          var instance = Activator.CreateInstance(t);
        }
        catch {
          exceptions.Add("Error instantiating " + t.FullName);
        }
      }

      Assert.IsTrue(!exceptions.Any(), string.Join(Environment.NewLine, exceptions));
    }
  }
}

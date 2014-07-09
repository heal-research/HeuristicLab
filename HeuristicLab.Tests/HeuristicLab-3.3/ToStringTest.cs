#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Core;
using HeuristicLab.PluginInfrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Tests {
  [TestClass]
  public class ToStringTest {

    private TestContext testContextInstance;
    public TestContext TestContext {
      get { return testContextInstance; }
      set { testContextInstance = value; }
    }

    // Use ClassInitialize to run code before running the first test in the class
    [ClassInitialize]
    public static void MyClassInitialize(TestContext testContext) {
      PluginLoader.Assemblies.Any();
    }

    [TestMethod]
    [TestCategory("General")]
    [TestCategory("Essential")]
    [TestProperty("Time", "long")]
    public void TestToString() {
      bool success = true;
      // just test for all IItems that the ToString method doesn't throw an exception
      foreach (object item in ApplicationManager.Manager.GetInstances(typeof(IItem))) {
        try {
          item.ToString();
        }
        catch (Exception e) {
          TestContext.WriteLine(item.GetType() + " throws a " + e.GetType() + " in the ToString method.");
          success = false;
        }
      }
      Assert.IsTrue(success, "There are potential errors in the ToString methods of  objects.");
    }
  }
}

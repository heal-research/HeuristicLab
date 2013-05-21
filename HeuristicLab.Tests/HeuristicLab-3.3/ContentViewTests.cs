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
using HeuristicLab.Common;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;
using HeuristicLab.PluginInfrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab_33.Tests {
  [TestClass]
  public class ContentViewTests {
    public ContentViewTests() {
    }

    private TestContext testContextInstance;

    /// <summary>
    ///Gets or sets the test context which provides
    ///information about and functionality for the current test run.
    ///</summary>
    public TestContext TestContext {
      get {
        return testContextInstance;
      }
      set {
        testContextInstance = value;
      }
    }

    // Use ClassInitialize to run code before running the first test in the class
    [ClassInitialize]
    public static void MyClassInitialize(TestContext testContext) {
      PluginLoader.Assemblies.Any();
    }


    [TestMethod]
    public void ContentViewAttributeTest() {
      //get all non-generic and instantiable classes which implement IContentView
      foreach (Type viewType in ApplicationManager.Manager.GetTypes(typeof(IContentView))) {
        //get all ContentAttributes on the instantiable view
        foreach (ContentAttribute attribute in viewType.GetCustomAttributes(typeof(ContentAttribute), false).Cast<ContentAttribute>()) {
          Assert.IsTrue(attribute.ContentType == typeof(IContent) || attribute.ContentType.GetInterfaces().Contains(typeof(IContent)),
            "The type specified in the ContentAttribute of {0} must implement IContent.", viewType);
        }
        //check if view can handle null as content by calling OnContentChanged
        IContentView view = (IContentView)Activator.CreateInstance(viewType);
        ContentView_Accessor accessor = new ContentView_Accessor(new PrivateObject(view));
        try {
          accessor.OnContentChanged();
        }
        catch (Exception ex) {
          Assert.Fail(viewType.ToString() + Environment.NewLine + ex.Message);
        }
      }
    }
  }
}

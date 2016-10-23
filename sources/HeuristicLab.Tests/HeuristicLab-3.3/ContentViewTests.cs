#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.PluginInfrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Tests {
  [TestClass]
  public class ContentViewTests {
    [TestMethod]
    [TestCategory("General")]
    [TestCategory("Essential")]
    [TestProperty("Time", "long")]
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
        var accessor = new PrivateObject(view);
        try {
          accessor.Invoke("OnContentChanged");
        }
        catch (Exception ex) {
          Assert.Fail(viewType.ToString() + Environment.NewLine + ex.Message);
        }
      }
    }
  }
}

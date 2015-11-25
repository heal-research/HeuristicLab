#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Tests {
  [TestClass]
  public class StorableClassTest {
    [TestMethod]
    [TestCategory("General")]
    [TestCategory("Essential")]
    [TestProperty("Time", "short")]
    public void TestStorableClass() {
      var errorMessage = new StringBuilder();

      foreach (var type in from type in ApplicationManager.Manager.GetTypes(typeof(object), onlyInstantiable: false, includeGenericTypeDefinitions: true)
                           let members = type.GetMembers(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly)
                           where members.Any(m => Attribute.IsDefined(m, typeof(StorableAttribute), false))
                           where !StorableClassAttribute.IsStorableClass(type)
                           select type) {
        errorMessage.Append(Environment.NewLine + type.GetPrettyName() + ": Contains at least one storable member but type is not a storable class.");
      }
      Assert.IsTrue(errorMessage.Length == 0, errorMessage.ToString());
    }

  }
}
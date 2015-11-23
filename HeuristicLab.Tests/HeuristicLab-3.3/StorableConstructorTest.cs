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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Tests {
  [TestClass]
  public class StorableConstructorTest {
    [TestMethod]
    [TestCategory("General")]
    [TestCategory("Essential")]
    [TestProperty("Time", "short")]
    public void TestStorableConstructor() {
      StringBuilder errorMessage = new StringBuilder();

      foreach (Type storableType in ApplicationManager.Manager.GetTypes(typeof(object))
        .Where(StorableClassAttribute.IsStorableClass)) {
        //test only types contained in HL plugin assemblies
        if (storableType.Namespace.Contains(".Tests")) continue;
        if (!PluginLoader.Assemblies.Contains(storableType.Assembly)) continue;

        IEnumerable<ConstructorInfo> ctors = storableType.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        ConstructorInfo storableConstructor = ctors.Where(c => c.GetParameters().Count() == 1 && c.GetParameters().First().ParameterType == typeof(bool)).FirstOrDefault();
        if (storableConstructor == null) errorMessage.Append(Environment.NewLine + storableType.ToString() + ": No storable constructor is defined.");
        else {
          if (storableType.IsSealed && !storableConstructor.IsPrivate)
            errorMessage.Append(Environment.NewLine + storableType.ToString() + ": Storable constructor must be private in sealed classes.");
          else if (!storableType.IsSealed && !(storableConstructor.IsFamily || storableConstructor.IsPublic))
            errorMessage.Append(Environment.NewLine + storableType.ToString() + ": Storable constructor must be protected (can be public in rare cases).");
        }
      }
      Assert.IsTrue(errorMessage.Length == 0, errorMessage.ToString());
    }
  }
}

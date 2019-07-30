#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.PluginInfrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Tests {
  [TestClass]
  public class StorableTest {
    [TestMethod]
    [TestCategory("General")]
    [TestCategory("Essential")]
    [TestProperty("Time", "short")]
    public void TestStorableConstructor() {
      StringBuilder errorMessage = new StringBuilder();

      foreach (Type storableType in ApplicationManager.Manager.GetTypes(typeof(object))
        .Where(StorableTypeAttribute.IsStorableType)) {
        //test only types contained in HL plugin assemblies
        if (!storableType.Namespace.StartsWith("HeuristicLab")) continue;
        if (storableType.Namespace.Contains(".Tests")) continue;
        if (!PluginLoader.Assemblies.Contains(storableType.Assembly)) continue;

        if (storableType.IsEnum || storableType.IsInterface) continue;
        var storableFields = storableType
          .GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
          .Where(fi => StorableAttribute.IsStorable(fi));
        var storableProps = storableType
          .GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
          .Where(fi => StorableAttribute.IsStorable(fi));

        // a storable constructor should be given but is not absolutely required.
        // when there are no storable fields then a storable ctor has no real purpose.
        if (!storableFields.Any() && !storableProps.Any()) continue;

        IEnumerable<ConstructorInfo> ctors = storableType.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        ConstructorInfo storableConstructor = ctors.Where(c => c.GetParameters().Count() == 1 && c.GetParameters().First().ParameterType == typeof(HEAL.Attic.StorableConstructorFlag)).FirstOrDefault();
        if (storableConstructor == null) errorMessage.Append(Environment.NewLine + storableType.ToString() + ": No storable constructor is defined.");
        else {
          if (storableType.IsSealed && !storableConstructor.IsPrivate)
            errorMessage.Append(Environment.NewLine + storableType.Namespace + "." + storableType.GetPrettyName() + ": Storable constructor must be private in sealed classes.");
          else if (!storableType.IsSealed && !(storableConstructor.IsFamily || storableConstructor.IsPublic))
            errorMessage.Append(Environment.NewLine + storableType.Namespace + "." + storableType.GetPrettyName() + ": Storable constructor must be protected (can be public in rare cases).");
        }
      }
      Assert.IsTrue(errorMessage.Length == 0, errorMessage.ToString());
    }

    [TestMethod]
    [TestCategory("General")]
    [TestCategory("Essential")]
    [TestProperty("Time", "short")]
    public void TestStorableClass() {
      var errorMessage = new StringBuilder();

      foreach (var type in ApplicationManager.Manager.GetTypes(typeof(object), onlyInstantiable: false, includeGenericTypeDefinitions: true)
        .Where(t => t.Namespace != null && !t.Namespace.Contains(".Tests"))
        .Where(t => !StorableTypeAttribute.IsStorableType(t))) {
        var members = type.GetMembers(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly);
        var storableConstructor = members.SingleOrDefault(m => Attribute.IsDefined(m, typeof(StorableConstructorAttribute), inherit: false));
        var storableMembers = members.Where(m => Attribute.IsDefined(m, typeof(StorableAttribute), inherit: false));

        if (storableConstructor != null) {
          errorMessage.Append(Environment.NewLine + type.Namespace + "." + type.GetPrettyName() + ": Contains a storable constructor but is not a storable type.");
        } else if (storableMembers.Any()) {
          errorMessage.Append(Environment.NewLine + type.Namespace + "." + type.GetPrettyName() + ": Contains at least one storable member but is not a storable type.");
        }
      }
      Assert.IsTrue(errorMessage.Length == 0, errorMessage.ToString());
    }
  }
}

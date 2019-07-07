#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Core;
using HeuristicLab.PluginInfrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Tests {
  [TestClass]
  public class ParameterVisibilityTest {
    private TestContext testContextInstance;
    public TestContext TestContext {
      get { return testContextInstance; }
      set { testContextInstance = value; }
    }

    [TestMethod]
    [TestCategory("General")]
    [TestCategory("Essential")]
    [TestProperty("Time", "long")]
    public void TestParameterVisibility() {
      StringBuilder errorMessage = new StringBuilder();

      foreach (var parameterizedItem in ApplicationManager.Manager.GetInstances<IParameterizedItem>()) {
        foreach (var parameter in parameterizedItem.Parameters) {
          var parameterType = parameter.GetType().GetInterfaces().FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IConstrainedValueParameter<>));
          if (parameterType == null) continue;

          var parameterGenericTypeArgument = parameterType.GetGenericArguments().First();
          var parameterGenericTypeDefinition = typeof(IConstrainedValueParameter<>);

          var paramProperty = parameterizedItem.GetType().GetProperty(parameter.Name + "Parameter",
            BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public);
          var valueProperty = parameterizedItem.GetType().GetProperty(parameter.Name,
                                                                 BindingFlags.GetProperty | BindingFlags.Instance |
                                                                 BindingFlags.Public);
          if (paramProperty == null)
            errorMessage.Append(Environment.NewLine + parameterizedItem.GetType() +
              ": public property " + parameter.Name + "Parameter is missing.");
          else if (paramProperty.PropertyType.GetGenericTypeDefinition() != parameterGenericTypeDefinition)
            errorMessage.Append(Environment.NewLine + parameterizedItem.GetType() +
              ": public property " + parameter.Name + "Parameter type must be " + parameterGenericTypeDefinition.Name);
          else if (paramProperty.PropertyType.GetGenericArguments().First() != parameterGenericTypeArgument)
            errorMessage.Append(Environment.NewLine + parameterizedItem.GetType() +
              ": public property " + parameter.Name + "Parameter generic type argument does not match the generic type argument of the parameter.");

          if (valueProperty == null)
            TestContext.WriteLine(parameterizedItem.GetType() + ": public property " + parameter.Name + " is missing.");
          else if (valueProperty.PropertyType != parameterGenericTypeArgument) {
            TestContext.WriteLine(parameterizedItem.GetType() + ": " + parameter.Name + " property type does not match the generic type argument of the parameter.");
          } else if (!valueProperty.CanRead || !valueProperty.CanWrite)
            TestContext.WriteLine(parameterizedItem.GetType() + ": public property " + parameter.Name + " must have a getter and a setter.");
        }
      }
      Assert.IsTrue(errorMessage.Length == 0, errorMessage.ToString());
    }
  }
}

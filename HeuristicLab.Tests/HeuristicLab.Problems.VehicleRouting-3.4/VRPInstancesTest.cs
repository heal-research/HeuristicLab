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
using System.Text;
using HeuristicLab.Problems.Instances;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Problems.VehicleRouting.Tests {
  [TestClass]
  public class VRPInstancesTest {
    [TestMethod]
    [TestCategory("Problems.VehicleRouting")]
    [TestProperty("Time", "long")]
    public void TestVRPInstances() {
      var vrp = new VehicleRoutingProblem();
      var providers = ProblemInstanceManager.GetProviders(vrp);
      var failedInstances = new StringBuilder();

      Assert.IsTrue(providers.Any(), "No providers could be found.");

      foreach (var provider in providers) {
        IEnumerable<IDataDescriptor> instances = ((dynamic)provider).GetDataDescriptors();
        Assert.IsTrue(instances.Any(), string.Format("No instances could be found in {0}.", provider.Name));

        foreach (var instance in instances) {
          try {
            // throws InvalidOperationException if zero or more than one interpreter is found
            ((dynamic)vrp).Load(((dynamic)provider).LoadData(instance));
          } catch (Exception exc) {
            failedInstances.AppendLine(instance.Name + ": " + exc.Message);
          }
        }
      }

      Assert.IsTrue(failedInstances.Length == 0, "Following instances failed: " + Environment.NewLine + failedInstances);
    }
  }
}

#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Problems.QuadraticAssignment.Tests_33 {
  [TestClass]
  public class QAPLIBInstancesTest {
    private static string InstancePrefix = "HeuristicLab.Tests.HeuristicLab.Problems.QuadraticAssignment_3._3.QAPLIB.";

    private IEnumerable<string> EmbeddedInstances {
      get {
        return Assembly.GetExecutingAssembly()
          .GetManifestResourceNames()
          .Where(x => x.EndsWith(".dat"))
          .OrderBy(x => x)
          .Select(x => x.Replace(".dat", String.Empty))
          .Select(x => x.Replace(InstancePrefix, String.Empty));
      }
    }

    [TestMethod]
    public void TestQAPLIBInstances() {
      var qap = new QuadraticAssignmentProblem();
      var failedInstances = new StringBuilder();
      string tempPath = Path.GetTempPath();

      Assert.IsTrue(EmbeddedInstances.Any(), "No instances could be found.");

      foreach (string instance in EmbeddedInstances) {
        WriteEmbeddedResourceToFile(InstancePrefix + instance + ".dat", File.Create(Path.Combine(tempPath, "instance.dat")));

        bool solutionExists = Assembly.GetExecutingAssembly().GetManifestResourceNames().Any(x => x == InstancePrefix + instance + ".sln");
        if (solutionExists)
          WriteEmbeddedResourceToFile(InstancePrefix + instance + ".sln", File.Create(Path.Combine(tempPath, "instance.sln")));

        try {
          qap.LoadInstanceFromFile(Path.Combine(tempPath, "instance.dat"));
        } catch (Exception ex) {
          failedInstances.AppendLine(instance + ": " + ex.Message);
          solutionExists = false; // not necessary to test solution as well
        }

        if (solutionExists) {
          try {
            qap.LoadInstanceFromFile(Path.Combine(tempPath, "instance.dat"), Path.Combine(tempPath, "instance.sln"));
            if (qap.BestKnownSolution == null)
              failedInstances.AppendLine(instance + " (sln): Given solution and reported quality cannot be reproduced.");
          } catch (Exception ex) {
            failedInstances.AppendLine(instance + " (+sln):" + ex.Message);
          }
        }
      }
      Assert.IsTrue(failedInstances.Length == 0, "Following instances failed: " + Environment.NewLine + failedInstances.ToString());
    }

    private void WriteEmbeddedResourceToFile(string resource, FileStream file) {
      try {
        using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resource)) {
          int token;
          while ((token = stream.ReadByte()) >= 0) {
            file.WriteByte((byte)token);
          }
        }
      } finally { file.Close(); }
    }
  }
}

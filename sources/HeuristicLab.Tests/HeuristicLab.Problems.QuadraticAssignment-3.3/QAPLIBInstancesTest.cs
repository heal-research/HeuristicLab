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
using HeuristicLab.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Problems.QuadraticAssignment.Tests_33 {
  [TestClass]
  public class QAPLIBInstancesTest {
    [TestMethod]
    public void LoadAllEmbeddedInstances() {
      QuadraticAssignmentProblem qap = new QuadraticAssignmentProblem();
      StringBuilder failedInstances = new StringBuilder();
      foreach (string instance in qap.EmbeddedInstances) {
        try {
          qap.LoadEmbeddedInstance(instance);
        }
        catch (Exception ex) {
          failedInstances.AppendLine(instance + ": " + ex.Message);
        }
      }
      Assert.IsTrue(failedInstances.Length == 0, "Following instances failed to load: " + Environment.NewLine + failedInstances.ToString());
    }

    [TestMethod]
    public void LoadAllEmbeddedSolutions() {
      IEnumerable<string> solutionFiles = Assembly.GetAssembly(typeof(QuadraticAssignmentProblem))
          .GetManifestResourceNames()
          .Where(x => x.EndsWith(".sln"));
      QAPLIBSolutionParser parser = new QAPLIBSolutionParser();
      StringBuilder failedInstances = new StringBuilder();
      foreach (string solution in solutionFiles) {
        using (Stream stream = Assembly.GetAssembly(typeof(QuadraticAssignmentProblem)).GetManifestResourceStream(solution)) {
          parser.Reset();
          parser.Parse(stream, true);
          if (parser.Error != null)
            failedInstances.AppendLine(solution + ": " + parser.Error.Message);
        }
      }
      Assert.IsTrue(failedInstances.Length == 0, "Following instances failed to load: " + Environment.NewLine + failedInstances.ToString());
    }

    [TestMethod]
    public void TestReportedSolutionQuality() {
      StringBuilder failedInstances = new StringBuilder();
      QuadraticAssignmentProblem qap = new QuadraticAssignmentProblem();
      foreach (string instance in qap.EmbeddedInstances) {
        try {
          qap.LoadEmbeddedInstance(instance);
        }
        catch {
          Assert.Fail("Not all instances load correctly");
        }
        if (qap.BestKnownSolution != null) {
          double quality = double.NaN;
          try {
            quality = QAPEvaluator.Apply(qap.BestKnownSolution, qap.Weights, qap.Distances);
          }
          catch (Exception ex) {
            failedInstances.AppendLine("An unknown problem occurred evaluating solution of instance " + instance + ": " + ex.Message);
          }
          if (!quality.IsAlmost(qap.BestKnownQuality.Value)) {
            failedInstances.AppendLine(instance + ": Reported quality: " + qap.BestKnownQuality.Value.ToString() + ", evaluated fitness: " + quality.ToString() + ".");
          }
        } else if (qap.BestKnownQuality != null) {
          failedInstances.AppendLine(instance + ": The solution failed to load, only the quality value is available!");
        }

      }
      Assert.IsTrue(failedInstances.Length == 0, "Following instances report divergent fitness values: " + Environment.NewLine + failedInstances.ToString());
    }
  }
}

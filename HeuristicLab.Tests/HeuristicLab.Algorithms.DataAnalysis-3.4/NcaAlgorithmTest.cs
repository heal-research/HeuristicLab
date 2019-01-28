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

using System.Linq;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Problems.DataAnalysis;
using HeuristicLab.Problems.Instances.DataAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Algorithms.DataAnalysis.Tests {
  [TestClass]
  public class NcaAlgorithmTest {
    [TestMethod]
    [TestCategory("Algorithms.DataAnalysis")]
    [TestProperty("Time", "medium")]
    public void RunNcaAlgorithmTest() {
      var uci = new UCIInstanceProvider();
      var nca = new NcaAlgorithm() {
        Iterations = 15,
        SetSeedRandomly = false,
        Seed = 620668657
      };
      var iris = uci.LoadData(uci.GetDataDescriptors().Single(x => x.Name.StartsWith("Iris")));
      ((ClassificationProblem)nca.Problem).Load(iris);
      nca.Start();
      Assert.AreEqual(Core.ExecutionState.Stopped, nca.ExecutionState);
      IResult qualityResult;
      Assert.IsTrue(nca.Results.TryGetValue("Quality", out qualityResult));
      var quality = qualityResult.Value as DoubleValue;
      Assert.IsNotNull(quality);
      Assert.IsTrue(quality.Value < -98);
    }
  }
}

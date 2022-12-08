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

using System.Linq;
using HeuristicLab.Optimization;
using HeuristicLab.Problems.DataAnalysis;
using HeuristicLab.Problems.Instances.DataAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HeuristicLab.Problems.DataAnalysis.Symbolic.Regression;

namespace HeuristicLab.Algorithms.DataAnalysis.Tests {
  [TestClass]
  public class CrossValidationTest {
    [TestMethod]
    [TestCategory("Algorithms.DataAnalysis")]
    public void RunCrossValidationTest() {
      var instanceProvider = new VariousInstanceProvider();
      
      var poly10 = instanceProvider.LoadData(instanceProvider.GetDataDescriptors().Single(x => x.Name.StartsWith("Poly")));
      var symRegProb = new SymbolicRegressionSingleObjectiveProblem();
      symRegProb.Load(poly10);
      symRegProb.EstimationLimits.Lower = -1;
      symRegProb.EstimationLimits.Upper = 1;

      var ga = new GeneticAlgorithm.GeneticAlgorithm();
      ga.Problem = symRegProb;
      // minimal configuration
      // we only need to check how estimation limits are set in the CV folds.
      ga.PopulationSize.Value = 10;
      ga.MaximumGenerations.Value = 1;


      var cv = new CrossValidation();
      cv.Algorithm = ga;
      cv.Start();

      Assert.AreEqual(Core.ExecutionState.Stopped, cv.ExecutionState);
      IResult cvFoldsResult;
      Assert.IsTrue(cv.Results.TryGetValue("CrossValidation Folds", out cvFoldsResult));
      var runs = (RunCollection)cvFoldsResult.Value;
      var foldEstimationLimits = (DoubleLimit)runs.First().Parameters["EstimationLimits"];

      Assert.AreEqual(symRegProb.EstimationLimits.Lower, foldEstimationLimits.Lower); // -1
      Assert.AreEqual(symRegProb.EstimationLimits.Upper, foldEstimationLimits.Upper); // 1
    }
  }
}

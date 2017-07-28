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
using HeuristicLab.Problems.DataAnalysis;
using HeuristicLab.Problems.Instances.DataAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Algorithms.DataAnalysis.Tests {
  [TestClass]
  public class GaussianProcessRegressionTest {
    public GaussianProcessRegressionTest() { }

    private TestContext testContextInstance;
    public TestContext TestContext {
      get { return testContextInstance; }
      set { testContextInstance = value; }
    }

    private Exception ex;

    [TestMethod]
    [TestCategory("Algorithms.DataAnalysis")]
    [TestProperty("Time", "long")]
    public void GaussianProcessRegressionPerformanceTest() {
      ex = null;

      var alg = new GaussianProcessRegression();
      alg.Engine = new HeuristicLab.SequentialEngine.SequentialEngine();
      alg.SetSeedRandomly = false;

      alg.Problem = new RegressionProblem();
      var provider = new RegressionCSVInstanceProvider();
      var problemData = (RegressionProblemData)provider.ImportData(@"Test Resources\co2.txt");
      problemData.TargetVariableParameter.ActualValue = problemData.TargetVariableParameter.ValidValues.First(x => x.Value == "interpolated");
      problemData.InputVariables.SetItemCheckedState(problemData.InputVariables.First(x => x.Value == "year"), false);
      problemData.InputVariables.SetItemCheckedState(problemData.InputVariables.First(x => x.Value == "month"), false);
      problemData.InputVariables.SetItemCheckedState(problemData.InputVariables.First(x => x.Value == "average"), false);
      problemData.InputVariables.SetItemCheckedState(problemData.InputVariables.First(x => x.Value == "interpolated"), false);
      problemData.InputVariables.SetItemCheckedState(problemData.InputVariables.First(x => x.Value == "trend"), false);
      problemData.InputVariables.SetItemCheckedState(problemData.InputVariables.First(x => x.Value == "#days"), false);

      alg.Problem.ProblemDataParameter.Value = problemData;

      alg.ExceptionOccurred += new EventHandler<EventArgs<Exception>>(cv_ExceptionOccurred);

      alg.Prepare();
      alg.Start();
      if (ex != null) throw ex;

      TestContext.WriteLine("Runtime: {0}", alg.ExecutionTime.ToString());
    }

    private void cv_ExceptionOccurred(object sender, EventArgs<Exception> e) {
      ex = e.Value;
    }
  }
}

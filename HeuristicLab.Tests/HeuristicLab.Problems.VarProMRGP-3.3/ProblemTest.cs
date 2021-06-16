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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Problems.VarProMRGP {
  [TestClass]
  public class ProblemTest {
    private TestContext testContextInstance;
    /// <summary>
    ///Gets or sets the test context which provides
    ///information about and functionality for the current test run.
    ///</summary>
    public TestContext TestContext {
      get { return testContextInstance; }
      set { testContextInstance = value; }
    }

    [TestMethod]
    [TestCategory("Problems.DataAnalysis")]
    [TestProperty("Time", "long")]
    public void TestOptimization() {
      var ga = new Algorithms.GeneticAlgorithm.GeneticAlgorithm();
      ga.PopulationSize.Value = 100;
      ga.MaximumGenerations.Value = 100;
      ga.MutationProbability.Value = 0.15;

      var prov = new HeuristicLab.Problems.Instances.DataAnalysis.VariousInstanceProvider();
      var poly10Desc = prov.GetDataDescriptors().Single(dd => dd.Name.Contains("Poly-10"));
      var varProProblem = new VarProMRGP.Problem();
      varProProblem.Load(prov.LoadData(poly10Desc));
      varProProblem.MaxSize = 10;
      varProProblem.MaxDepth = 6;
      ga.SetSeedRandomly.Value = false;
      ga.Seed.Value = 31415;
      ga.Problem = varProProblem;
      ga.ExceptionOccurred += (sender, e) => throw e.Value;
      ga.Stopped += (sender, e) => Console.WriteLine("Stopped");
      
      ga.Prepare();
      ga.Start();

    }
  }
}

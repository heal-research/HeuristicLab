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
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Tests {
  [TestClass]
  public class VarProMRGPTest {

    [TestMethod]
    [TestCategory("Algorithms.DataAnalysis")]
    [TestProperty("Time", "medium")]
    public void VarProMRGPPoly10() {
      var prov = new HeuristicLab.Problems.Instances.DataAnalysis.VariousInstanceProvider();
      var poly10 = prov.GetDataDescriptors().First(dd => dd.Name.Contains("Poly"));
      var problemData = prov.LoadData(poly10);
      var prob = new HeuristicLab.Problems.VarProMRGP.Problem();
      prob.RegressionProblemData = problemData;
      var alg = new HeuristicLab.Algorithms.GeneticAlgorithm.GeneticAlgorithm();
      alg.Problem = prob;

      alg.MaximumGenerations.Value = 2;
      alg.PopulationSize.Value = 10;
      alg.MutationProbability.Value = 0.1;
      alg.SetSeedRandomly.Value = false;
      alg.Seed.Value = 1234;

      alg.Start();


    }
  }
}

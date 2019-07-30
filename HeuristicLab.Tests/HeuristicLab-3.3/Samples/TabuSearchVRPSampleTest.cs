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

using System.IO;
using System.Linq;
using HEAL.Attic;
using HeuristicLab.Algorithms.TabuSearch;
using HeuristicLab.Problems.Instances.VehicleRouting;
using HeuristicLab.Problems.VehicleRouting;
using HeuristicLab.Problems.VehicleRouting.Encodings.Potvin;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Tests {
  [TestClass]
  public class TabuSearchVRPSampleTest {
    private const string SampleFileName = "TS_VRP";
    private static readonly ProtoBufSerializer serializer = new ProtoBufSerializer();

    [TestMethod]
    [TestCategory("Samples.Create")]
    [TestProperty("Time", "medium")]
    public void CreateTabuSearchVRPSampleTest() {
      var vrp = CreateTabuSearchVrpSample();
      string path = Path.Combine(SamplesUtils.SamplesDirectory, SampleFileName + SamplesUtils.SampleFileExtension);
      serializer.Serialize(vrp, path);
    }
    [TestMethod]
    [TestCategory("Samples.Execute")]
    [TestProperty("Time", "long")]
    public void RunTabuSearchVRPSampleTest() {
      var vrp = CreateTabuSearchVrpSample();
      vrp.SetSeedRandomly.Value = false;
      SamplesUtils.RunAlgorithm(vrp);
      Assert.AreEqual(1473, SamplesUtils.GetDoubleResult(vrp, "BestQuality"));
      Assert.AreEqual(2102.1192622950812, SamplesUtils.GetDoubleResult(vrp, "CurrentAverageQuality"));
      Assert.AreEqual(4006, SamplesUtils.GetDoubleResult(vrp, "CurrentWorstQuality"));
      Assert.AreEqual(119072, SamplesUtils.GetIntResult(vrp, "EvaluatedMoves"));
    }

    private TabuSearch CreateTabuSearchVrpSample() {
      TabuSearch ts = new TabuSearch();
      #region Problem Configuration
      var provider = new AugeratInstanceProvider();
      var instance = provider.GetDataDescriptors().Where(x => x.Name == "A-n62-k8").Single();
      VehicleRoutingProblem vrpProblem = new VehicleRoutingProblem();
      vrpProblem.Load(provider.LoadData(instance));
      #endregion
      #region Algorithm Configuration
      ts.Name = "Tabu Search - VRP";
      ts.Description = "A tabu search algorithm that solves the \"A-n62-k8\" VRP (imported from Augerat)";
      ts.Problem = vrpProblem;

      ts.MaximumIterations.Value = 200;
      // move generator has to be set first
      var moveGenerator = ts.MoveGeneratorParameter.ValidValues
        .OfType<PotvinCustomerRelocationExhaustiveMoveGenerator>()
        .Single();
      ts.MoveGenerator = moveGenerator;
      var moveEvaluator = ts.MoveEvaluatorParameter.ValidValues
        .OfType<PotvinCustomerRelocationMoveEvaluator>()
        .Single();
      ts.MoveEvaluator = moveEvaluator;
      var moveMaker = ts.MoveMakerParameter.ValidValues
        .OfType<PotvinCustomerRelocationMoveMaker>()
        .Single();
      ts.MoveMaker = moveMaker;
      ts.SampleSize.Value = 1000;
      ts.Seed.Value = 0;
      ts.SetSeedRandomly.Value = true;

      var tabuChecker = ts.TabuCheckerParameter.ValidValues
        .OfType<PotvinCustomerRelocationMoveTabuCriterion>()
        .Single();
      tabuChecker.UseAspirationCriterion.Value = false;
      ts.TabuChecker = tabuChecker;

      var tabuMaker = ts.TabuMakerParameter.ValidValues
        .OfType<PotvinCustomerRelocationMoveTabuMaker>()
        .Single();
      ts.TabuMaker = tabuMaker;
      ts.TabuTenure.Value = 6;

      #endregion
      ts.Engine = new ParallelEngine.ParallelEngine();
      return ts;
    }
  }
}

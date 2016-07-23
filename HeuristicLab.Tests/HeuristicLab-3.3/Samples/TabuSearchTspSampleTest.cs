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

using System.IO;
using System.Linq;
using HeuristicLab.Algorithms.TabuSearch;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Persistence.Default.Xml;
using HeuristicLab.Problems.Instances.TSPLIB;
using HeuristicLab.Problems.TravelingSalesman;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Tests {
  [TestClass]
  public class TabuSearchTspSampleTest {
    private const string SampleFileName = "TS_TSP";

    [TestMethod]
    [TestCategory("Samples.Create")]
    [TestProperty("Time", "medium")]
    public void CreateTabuSearchTspSampleTest() {
      var ts = CreateTabuSearchTspSample();
      string path = Path.Combine(SamplesUtils.SamplesDirectory, SampleFileName + SamplesUtils.SampleFileExtension);
      XmlGenerator.Serialize(ts, path);
    }
    [TestMethod]
    [TestCategory("Samples.Execute")]
    [TestProperty("Time", "long")]
    public void RunTabuSearchTspSampleTest() {
      var ts = CreateTabuSearchTspSample();
      ts.SetSeedRandomly.Value = false;
      SamplesUtils.RunAlgorithm(ts);
      Assert.AreEqual(6294, SamplesUtils.GetDoubleResult(ts, "BestQuality"));
      Assert.AreEqual(7380.0386666666664, SamplesUtils.GetDoubleResult(ts, "CurrentAverageQuality"));
      Assert.AreEqual(8328, SamplesUtils.GetDoubleResult(ts, "CurrentWorstQuality"));
      Assert.AreEqual(750000, SamplesUtils.GetIntResult(ts, "EvaluatedMoves"));
    }

    private TabuSearch CreateTabuSearchTspSample() {
      TabuSearch ts = new TabuSearch();
      #region Problem Configuration
      var provider = new TSPLIBTSPInstanceProvider();
      var instance = provider.GetDataDescriptors().Where(x => x.Name == "ch130").Single();
      TravelingSalesmanProblem tspProblem = new TravelingSalesmanProblem();
      tspProblem.Load(provider.LoadData(instance));
      tspProblem.UseDistanceMatrix.Value = true;
      #endregion
      #region Algorithm Configuration
      ts.Name = "Tabu Search - TSP";
      ts.Description = "A tabu search algorithm that solves the \"ch130\" TSP (imported from TSPLIB)";
      ts.Problem = tspProblem;

      ts.MaximumIterations.Value = 1000;
      // move generator has to be set first
      var moveGenerator = ts.MoveGeneratorParameter.ValidValues
        .OfType<StochasticInversionMultiMoveGenerator>()
        .Single();
      ts.MoveGenerator = moveGenerator;
      var moveEvaluator = ts.MoveEvaluatorParameter.ValidValues
        .OfType<TSPInversionMoveRoundedEuclideanPathEvaluator>()
        .Single();
      ts.MoveEvaluator = moveEvaluator;
      var moveMaker = ts.MoveMakerParameter.ValidValues
        .OfType<InversionMoveMaker>()
        .Single();
      ts.MoveMaker = moveMaker;
      ts.SampleSize.Value = 750;
      ts.Seed.Value = 0;
      ts.SetSeedRandomly.Value = true;

      var tabuChecker = ts.TabuCheckerParameter.ValidValues
        .OfType<InversionMoveSoftTabuCriterion>()
        .Single();
      tabuChecker.UseAspirationCriterion.Value = true;
      ts.TabuChecker = tabuChecker;

      var tabuMaker = ts.TabuMakerParameter.ValidValues
        .OfType<InversionMoveTabuMaker>()
        .Single();
      ts.TabuMaker = tabuMaker;
      ts.TabuTenure.Value = 60;

      #endregion
      ts.Engine = new ParallelEngine.ParallelEngine();
      return ts;
    }
  }
}

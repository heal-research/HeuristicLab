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
using HeuristicLab.Algorithms.GeneticAlgorithm;
using HeuristicLab.Problems.Instances;
using HeuristicLab.Problems.Instances.VehicleRouting;
using HeuristicLab.Problems.VehicleRouting;
using HeuristicLab.Problems.VehicleRouting.Encodings.General;
using HeuristicLab.Problems.VehicleRouting.Encodings.Potvin;
using HeuristicLab.Problems.VehicleRouting.ProblemInstances;
using HeuristicLab.Selection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Tests {
  [TestClass]
  public class GAVrpSampleTest {
    private const string SampleFileName = "GA_VRP";

    private static readonly ProtoBufSerializer serializer = new ProtoBufSerializer();

    [TestMethod]
    [TestCategory("Samples.Create")]
    [TestProperty("Time", "medium")]
    public void CreateGaVrpSampleTest() {
      var ga = CreateGaVrpSample();
      string path = Path.Combine(SamplesUtils.SamplesDirectory, SampleFileName + SamplesUtils.SampleFileExtension);
      serializer.Serialize(ga, path);
    }

    [TestMethod]
    [TestCategory("Samples.Execute")]
    [TestProperty("Time", "long")]
    public void RunGaVrpSampleTest() {
      var ga = CreateGaVrpSample();
      ga.SetSeedRandomly.Value = false;
      SamplesUtils.RunAlgorithm(ga);
      Assert.AreEqual(1828.9368669428338, SamplesUtils.GetDoubleResult(ga, "BestQuality"));
      Assert.AreEqual(1830.1444308908331, SamplesUtils.GetDoubleResult(ga, "CurrentAverageQuality"));
      Assert.AreEqual(1871.7128510304112, SamplesUtils.GetDoubleResult(ga, "CurrentWorstQuality"));
      Assert.AreEqual(99100, SamplesUtils.GetIntResult(ga, "EvaluatedSolutions"));
    }

    private GeneticAlgorithm CreateGaVrpSample() {
      GeneticAlgorithm ga = new GeneticAlgorithm();

      #region Problem Configuration
      VehicleRoutingProblem vrpProblem = new VehicleRoutingProblem();

      SolomonFormatInstanceProvider instanceProvider = new SolomonInstanceProvider();
      CVRPTWData data = instanceProvider.Import(@"Test Resources\C101.txt", @"Test Resources\C101.opt.txt") as CVRPTWData;
      vrpProblem.Load(data);
      vrpProblem.Name = "C101 VRP (imported from Solomon)";
      vrpProblem.Description = "Represents a Vehicle Routing Problem.";
      CVRPTWProblemInstance instance = vrpProblem.ProblemInstance as CVRPTWProblemInstance;
      instance.DistanceFactor.Value = 1;
      instance.FleetUsageFactor.Value = 100;
      instance.OverloadPenalty.Value = 100;
      instance.TardinessPenalty.Value = 100;
      instance.TimeFactor.Value = 0;
      vrpProblem.MaximizationParameter.Value.Value = false;
      instance.UseDistanceMatrix.Value = true;
      instance.Vehicles.Value = 25;
      #endregion
      #region Algorithm Configuration
      ga.Name = "Genetic Algorithm - VRP";
      ga.Description = "A genetic algorithm which solves the \"C101\" vehicle routing problem (imported from Solomon)";
      ga.Problem = vrpProblem;
      SamplesUtils.ConfigureGeneticAlgorithmParameters<TournamentSelector, MultiVRPSolutionCrossover, MultiVRPSolutionManipulator>(
        ga, 100, 1, 1000, 0.05, 3);

      var xOver = (MultiVRPSolutionCrossover)ga.Crossover;
      foreach (var op in xOver.Operators) {
        xOver.Operators.SetItemCheckedState(op, false);
      }
      xOver.Operators.SetItemCheckedState(xOver.Operators
        .OfType<PotvinRouteBasedCrossover>()
        .Single(), true);
      xOver.Operators.SetItemCheckedState(xOver.Operators
        .OfType<PotvinSequenceBasedCrossover>()
        .Single(), true);

      var manipulator = (MultiVRPSolutionManipulator)ga.Mutator;
      foreach (var op in manipulator.Operators) {
        manipulator.Operators.SetItemCheckedState(op, false);
      }
      manipulator.Operators.SetItemCheckedState(manipulator.Operators
        .OfType<PotvinOneLevelExchangeMainpulator>()
        .Single(), true);
      manipulator.Operators.SetItemCheckedState(manipulator.Operators
        .OfType<PotvinTwoLevelExchangeManipulator>()
        .Single(), true);
      #endregion

      return ga;
    }
  }
}

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
using HeuristicLab.Algorithms.VariableNeighborhoodSearch;
using HeuristicLab.Data;
using HeuristicLab.Encodings.IntegerVectorEncoding;
using HeuristicLab.Persistence.Default.Xml;
using HeuristicLab.Problems.Orienteering;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Tests {
  [TestClass]
  public class VnsOpSampleTest {
    private const string SampleFileName = "VNS_OP";

    [TestMethod]
    [TestCategory("Samples.Create")]
    [TestProperty("Time", "medium")]
    public void CreateVnsOpSampleTest() {
      var vns = CreateVnsOpSample();
      string path = Path.Combine(SamplesUtils.SamplesDirectory, SampleFileName + SamplesUtils.SampleFileExtension);
      XmlGenerator.Serialize(vns, path);
    }
    [TestMethod]
    [TestCategory("Samples.Execute")]
    [TestProperty("Time", "long")]
    public void RunVnsOpSampleTest() {
      var vns = CreateVnsOpSample();
      vns.SetSeedRandomly = false;
      SamplesUtils.RunAlgorithm(vns);
      Assert.AreEqual(1182, SamplesUtils.GetDoubleResult(vns, "BestQuality"));
      Assert.AreEqual(1182, SamplesUtils.GetDoubleResult(vns, "CurrentAverageQuality"));
      Assert.AreEqual(1182, SamplesUtils.GetDoubleResult(vns, "CurrentWorstQuality"));
      Assert.AreEqual(42651753, SamplesUtils.GetIntResult(vns, "EvaluatedSolutions"));
    }

    private VariableNeighborhoodSearch CreateVnsOpSample() {
      VariableNeighborhoodSearch vns = new VariableNeighborhoodSearch();
      #region Problem Configuration
      OrienteeringProblem opProblem = new OrienteeringProblem();
      opProblem.BestKnownQuality = new DoubleValue(1188);
      opProblem.BestKnownSolution = new IntegerVector(new[] {
0, 1, 3, 6, 11, 17, 24, 18, 13, 19, 14, 20, 26, 34, 27, 35, 42, 48, 53, 57, 52, 47, 41, 33, 25, 32, 40, 46, 39, 31, 38, 50, 44, 37, 30, 23, 16, 10, 15, 22, 29, 21, 28, 36, 43, 49, 54, 58, 61, 63 
      });
      opProblem.Coordinates = new DoubleMatrix(new double[,] {
{ 7, 0 },{ 6, 1 },{ 8, 1 },{ 5, 2 },{ 7, 2 },{ 9, 2 },{ 4, 3 },{ 6, 3 },{ 8, 3 },{ 10, 3 },{ 3, 4 },{ 5, 4 },{ 7, 4 },{ 9, 4 },{ 11, 4 },{ 2, 5 },{ 4, 5 },{ 6, 5 },{ 8, 5 },{ 10, 5 },{ 12, 5 },{ 1, 6 },{ 3, 6 },{ 5, 6 },{ 7, 6 },{ 9, 6 },{ 11, 6 },{ 13, 6 },{ 0, 7 },{ 2, 7 },{ 4, 7 },{ 6, 7 },{ 8, 7 },{ 10, 7 },{ 12, 7 },{ 14, 7 },{ 1, 8 },{ 3, 8 },{ 5, 8 },{ 7, 8 },{ 9, 8 },{ 11, 8 },{ 13, 8 },{ 2, 9 },{ 4, 9 },{ 6, 9 },{ 8, 9 },{ 10, 9 },{ 12, 9 },{ 3, 10 },{ 5, 10 },{ 7, 10 },{ 9, 10 },{ 11, 10 },{ 4, 11 },{ 6, 11 },{ 8, 11 },{ 10, 11 },{ 5, 12 },{ 7, 12 },{ 9, 12 },{ 6, 13 },{ 8, 13 },{ 7, 14 }
      });
      opProblem.MaximumDistance = 70;
      opProblem.PointVisitingCosts = 0;
      opProblem.Scores = new DoubleArray(new double[] {
0, 6, 6, 12, 6, 12, 18, 12, 12, 18, 24, 18, 12, 18, 24, 30, 24, 18, 18, 24, 30, 36, 30, 24, 18, 24, 30, 36, 42, 36, 30, 24, 24, 30, 36, 42, 36, 30, 24, 18, 24, 30, 36, 30, 24, 18, 18, 24, 30, 24, 18, 12, 18, 24, 18, 12, 12, 18, 12, 6, 12, 6, 6, 0
      });
      opProblem.StartingPoint = 0;
      opProblem.TerminalPoint = 63;

      opProblem.Name = "1_p64_t070";
      opProblem.Description = "Represents an instance of an orienteering problem.";
      #endregion
      #region Algorithm Configuration
      vns.Name = "Variable Neighborhood Search - OP";
      vns.Description = "A variable neighborhood search algorithm which solves an orienteering problem instance";
      vns.Problem = opProblem;

      vns.LocalImprovement = vns.LocalImprovementParameter.ValidValues.OfType<OrienteeringLocalImprovementOperator>().Single();
      vns.LocalImprovementMaximumIterations = 200;
      vns.MaximumIterations = 25;
      vns.Seed = 0;
      vns.SetSeedRandomly = true;
      vns.ShakingOperator = vns.ShakingOperatorParameter.ValidValues.OfType<OrienteeringShakingOperator>().Single();
      #endregion
      vns.Engine = new ParallelEngine.ParallelEngine();
      return vns;
    }
  }
}

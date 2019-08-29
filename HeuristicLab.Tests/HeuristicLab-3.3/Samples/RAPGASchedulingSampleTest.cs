//#region License Information
///* HeuristicLab
// * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
// *
// * This file is part of HeuristicLab.
// *
// * HeuristicLab is free software: you can redistribute it and/or modify
// * it under the terms of the GNU General Public License as published by
// * the Free Software Foundation, either version 3 of the License, or
// * (at your option) any later version.
// *
// * HeuristicLab is distributed in the hope that it will be useful,
// * but WITHOUT ANY WARRANTY; without even the implied warranty of
// * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// * GNU General Public License for more details.
// *
// * You should have received a copy of the GNU General Public License
// * along with HeuristicLab. If not, see <http://www.gnu.org/licenses/>.
// */
//#endregion

//using System.IO;
//using System.Linq;
//using HEAL.Attic;
//using HeuristicLab.Algorithms.RAPGA;
//using HeuristicLab.Encodings.ScheduleEncoding;
//using HeuristicLab.Problems.Scheduling;
//using Microsoft.VisualStudio.TestTools.UnitTesting;

//namespace HeuristicLab.Tests {
//  [TestClass]
//  public class RAPGASchedulingSampleTest {
//    private const string SampleFileName = "RAPGA_JSSP";
//    private static readonly ProtoBufSerializer serializer = new ProtoBufSerializer();

//    [TestMethod]
//    [TestCategory("Samples.Create")]
//    [TestProperty("Time", "medium")]
//    public void CreateRAPGASchedulingSampleTest() {
//      var ss = CreateRAPGASchedulingSample();
//      string path = Path.Combine(SamplesUtils.SamplesDirectory, SampleFileName + SamplesUtils.SampleFileExtension);
//      serializer.Serialize(ss, path);
//    }

//    [TestMethod]
//    [TestCategory("Samples.Execute")]
//    [TestProperty("Time", "long")]
//    public void RunRAPGASchedulingSampleTest() {
//      var rapga = CreateRAPGASchedulingSample();
//      rapga.SetSeedRandomly.Value = false;
//      SamplesUtils.RunAlgorithm(rapga);
//      Assert.AreEqual(988.00, SamplesUtils.GetDoubleResult(rapga, "BestQuality"));
//      Assert.AreEqual(988.00, SamplesUtils.GetDoubleResult(rapga, "CurrentAverageQuality"));
//      Assert.AreEqual(988.00, SamplesUtils.GetDoubleResult(rapga, "CurrentWorstQuality"));
//      Assert.AreEqual(27100, SamplesUtils.GetIntResult(rapga, "EvaluatedSolutions"));
//    }

//    private RAPGA CreateRAPGASchedulingSample() {
//      #region Problem Configuration
//      var problem = new JobShopSchedulingProblemNew();
//      #endregion

//      #region Algorithm Configuration
//      RAPGA rapga = new RAPGA();
//      rapga.Engine = new SequentialEngine.SequentialEngine();
//      rapga.Name = "RAPGA - Job Shop Scheduling";
//      rapga.Description = "A relevant alleles preserving genetic algorithm which solves a job shop scheduling problem";
//      rapga.Problem = problem;
//      rapga.Mutator = rapga.MutatorParameter.ValidValues.OfType<JSMSwapManipulator>().First();
//      rapga.Seed.Value = 0;
//      return rapga;
//      #endregion
//    }
//  }
//}

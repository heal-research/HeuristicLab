using System;
using System.IO;
using HEAL.Attic;
using HeuristicLab.Algorithms.GeneticAlgorithm;
using HeuristicLab.Optimization;
using HeuristicLab.Problems.DataAnalysis.Symbolic;
using HeuristicLab.Problems.DataAnalysis.Symbolic.Regression;
using HeuristicLab.Problems.Instances.DataAnalysis;
using HeuristicLab.Selection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Tests {
  [TestClass]
  public class StructureTemplateRegressionSampleTest {
    private const string SampleFileName = "GP_Structure_Template_Regression";
    private static readonly ProtoBufSerializer serializer = new ProtoBufSerializer();

    [TestMethod]
    [TestCategory("Samples.Execute")]
    [TestCategory("Run.Daily")]
    [TestProperty("Time", "long")]
    public void RunStructureTemplateRegressionSampleTest() {
      var ga = CreateStructureTemplateRegressionSample();
      ga.SetSeedRandomly.Value = false;
      SamplesUtils.RunAlgorithm(ga);

      if (Environment.Is64BitProcess) {
        Assert.AreEqual(5.0286947997733353E-07, SamplesUtils.GetDoubleResult(ga, "BestQuality"), 1E-8);
        Assert.AreEqual(79.100605440090362, SamplesUtils.GetDoubleResult(ga, "CurrentAverageQuality"), 1E-8);
        Assert.AreEqual(7905.465554758197, SamplesUtils.GetDoubleResult(ga, "CurrentWorstQuality"), 1E-8);
        Assert.AreEqual(5050, SamplesUtils.GetIntResult(ga, "EvaluatedSolutions"));
      } else {
        Assert.AreEqual(5.45285034915826E-07, SamplesUtils.GetDoubleResult(ga, "BestQuality"), 1E-8);
        Assert.AreEqual(0.25447047591366467, SamplesUtils.GetDoubleResult(ga, "CurrentAverageQuality"), 1E-8);
        Assert.AreEqual(23.211139545787372, SamplesUtils.GetDoubleResult(ga, "CurrentWorstQuality"), 1E-8);
        Assert.AreEqual(5050, SamplesUtils.GetIntResult(ga, "EvaluatedSolutions"));
      }
    }

    [TestMethod]
    [TestCategory("Samples.Create")]
    [TestProperty("Time", "medium")]
    public void CreateStructureTemplateRegressionSampleTest() {
      var ga = CreateStructureTemplateRegressionSample();
      string path = Path.Combine(SamplesUtils.SamplesDirectory, SampleFileName + SamplesUtils.SampleFileExtension);
      serializer.Serialize(ga, path);
    }

    public static GeneticAlgorithm CreateStructureTemplateRegressionSample() {
      var alg = new GeneticAlgorithm();
      var provider = new PhysicsInstanceProvider(seed: 0);
      var descriptor = new SheetBendingProcess(seed: 0);
      var problem = new StructureTemplateSymbolicRegressionProblem();
      problem.Load(provider.LoadData(descriptor));
      problem.StructureTemplate.Template =
        "(" +
          "(210000 / (210000 + h)) * ((sigma_y * t * t) / (wR * Rt * t)) + " +
          "PlasticHardening(_) - Elasticity(_)" +
        ")" +
        " * C(_)";
      foreach (var subFunction in problem.StructureTemplate.SubFunctions) {
        subFunction.MaximumSymbolicExpressionTreeDepth = 8;
        subFunction.MaximumSymbolicExpressionTreeLength = 20;
        subFunction.Grammar = new ArithmeticExpressionGrammar();
      }

      #region Algorithm Configuration
      alg.Name = "Genetic Programming - Structure Template";
      alg.Description = "A standard genetic programming algorithm to solve a regression problem with a predefined structure template (Sheet Bending Process)";
      alg.Problem = problem;

      SamplesUtils.ConfigureGeneticAlgorithmParameters<TournamentSelector, MultiEncodingCrossover, MultiEncodingManipulator>
        (alg, popSize: 100, elites: 1, maxGens: 50, mutationRate: 0.25, tournGroupSize: 3);

      alg.Seed.Value = 0;
      alg.Engine = new ParallelEngine.ParallelEngine();
      #endregion

      return alg;
    }
  }
}

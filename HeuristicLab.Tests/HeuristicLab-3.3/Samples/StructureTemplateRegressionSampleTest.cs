using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HEAL.Attic;
using HeuristicLab.Algorithms.GeneticAlgorithm;
using HeuristicLab.Problems.Instances.DataAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HeuristicLab.Problems.DataAnalysis.Symbolic.Regression;
using HeuristicLab.Problems.DataAnalysis.Symbolic;
using HeuristicLab.Selection;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Optimization;

namespace HeuristicLab.Tests {
  [TestClass]
  public class StructureTemplateRegressionSampleTest {
    private const string SampleFileName = "GP_Structure_Template_Regression";
    private static readonly ProtoBufSerializer serializer = new ProtoBufSerializer();

    [TestMethod]
    [TestCategory("Samples.Execute")]
    [TestProperty("Time", "long")]
    public void RunStructureTemplateRegressionSampleTest() {
      var ga = CreateStructureTemplateRegressionSample();
      ga.SetSeedRandomly.Value = false;
      SamplesUtils.RunAlgorithm(ga);

      if (Environment.Is64BitProcess) {
        Assert.AreEqual(4.2920235018589167E-05, SamplesUtils.GetDoubleResult(ga, "BestQuality"), 1E-8);
        Assert.AreEqual(54.076796020975593, SamplesUtils.GetDoubleResult(ga, "CurrentAverageQuality"), 1E-8);
        Assert.AreEqual(5104.3828108493908, SamplesUtils.GetDoubleResult(ga, "CurrentWorstQuality"), 1E-8);
        Assert.AreEqual(5050, SamplesUtils.GetIntResult(ga, "EvaluatedSolutions"));
      } else {
        Assert.AreEqual(4.29202350185872E-05, SamplesUtils.GetDoubleResult(ga, "BestQuality"), 1E-8);
        Assert.AreEqual(54.076796020975593, SamplesUtils.GetDoubleResult(ga, "CurrentAverageQuality"), 1E-8);
        Assert.AreEqual(5104.3828108493908, SamplesUtils.GetDoubleResult(ga, "CurrentWorstQuality"), 1E-8);
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
      var problem = new StructuredSymbolicRegressionSingleObjectiveProblem();
      problem.Load(provider.LoadData(descriptor));
      problem.StructureTemplate.Template =
        "(" +
          "(210000 / (210000 + h)) * ((sigma_y * t * t) / (wR * Rt * t)) + " +
          "PlasticHardening(_) - Elasticity(_)" +
        ")" +
        " * C(_)";
      foreach(var subFunction in problem.StructureTemplate.SubFunctions) {
        subFunction.MaximumSymbolicExpressionTreeDepth = 8;
        subFunction.MaximumSymbolicExpressionTreeLength = 20;
        subFunction.Grammar = new ArithmeticExpressionGrammar();
      }
      var evaluator = problem.TreeEvaluatorParameter.ValidValues
        .OfType<NMSESingleObjectiveConstraintsEvaluator>()
        .First();
      evaluator.OptimizeParameters = true;
      problem.TreeEvaluatorParameter.Value = evaluator;

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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using HeuristicLab.Algorithms.DataAnalysis;
using HeuristicLab.Common;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Problems.DataAnalysis.Symbolic;
using HeuristicLab.Problems.DataAnalysis.Symbolic.Classification;
using HeuristicLab.Problems.Instances.DataAnalysis;
using HeuristicLab.Random;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Problems.DataAnalysis.Tests {

  [TestClass()]
  public class ClassificationVariableImpactCalculationTest {
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
    [TestProperty("Time", "short")]
    public void ConstantModelVariableImpactTest() {
      IClassificationProblemData problemData = LoadIrisProblem();
      IClassificationModel model = new ConstantModel(5, "y");
      IClassificationSolution solution = new ClassificationSolution(model, problemData);
      Dictionary<string, double> expectedImpacts = GetExpectedValuesForConstantModel();

      CheckDefaultAsserts(solution, expectedImpacts);
    }

    [TestMethod]
    [TestCategory("Problems.DataAnalysis")]
    [TestProperty("Time", "short")]
    public void KNNIrisVariableImpactTest() {
      IClassificationProblemData problemData = LoadIrisProblem();
      IClassificationSolution solution = NearestNeighbourClassification.CreateNearestNeighbourClassificationSolution(problemData, 3);
      ClassificationSolutionVariableImpactsCalculator.CalculateImpacts(solution);
      Dictionary<string, double> expectedImpacts = GetExpectedValuesForIrisKNNModel();

      CheckDefaultAsserts(solution, expectedImpacts);
    }


    [TestMethod]
    [TestCategory("Problems.DataAnalysis")]
    [TestProperty("Time", "short")]
    public void LDAIrisVariableImpactTest() {
      IClassificationProblemData problemData = LoadIrisProblem();
      IClassificationSolution solution = LinearDiscriminantAnalysis.CreateLinearDiscriminantAnalysisSolution(problemData);
      ClassificationSolutionVariableImpactsCalculator.CalculateImpacts(solution);
      Dictionary<string, double> expectedImpacts = GetExpectedValuesForIrisLDAModel();

      CheckDefaultAsserts(solution, expectedImpacts);
    }


    [TestMethod]
    [TestCategory("Problems.DataAnalysis")]
    [TestProperty("Time", "short")]
    public void CustomModelVariableImpactTest() {
      IClassificationProblemData problemData = CreateDefaultProblem();
      ISymbolicExpressionTree tree = CreateCustomExpressionTree();
      var model = new SymbolicNearestNeighbourClassificationModel(problemData.TargetVariable, 3, tree, new SymbolicDataAnalysisExpressionTreeInterpreter());
      model.RecalculateModelParameters(problemData, problemData.TrainingIndices);
      IClassificationSolution solution = new ClassificationSolution(model, (IClassificationProblemData)problemData.Clone());
      Dictionary<string, double> expectedImpacts = GetExpectedValuesForCustomProblem();

      CheckDefaultAsserts(solution, expectedImpacts);
    }

    [TestMethod]
    [TestCategory("Problems.DataAnalysis")]
    [TestProperty("Time", "short")]
    public void CustomModelVariableImpactNoInfluenceTest() {
      IClassificationProblemData problemData = CreateDefaultProblem();
      ISymbolicExpressionTree tree = CreateCustomExpressionTreeNoInfluenceX1();
      var model = new SymbolicNearestNeighbourClassificationModel(problemData.TargetVariable, 3, tree, new SymbolicDataAnalysisExpressionTreeInterpreter());
      model.RecalculateModelParameters(problemData, problemData.TrainingIndices);
      IClassificationSolution solution = new ClassificationSolution(model, (IClassificationProblemData)problemData.Clone());
      Dictionary<string, double> expectedImpacts = GetExpectedValuesForCustomProblemNoInfluence();

      CheckDefaultAsserts(solution, expectedImpacts);
    }

    [TestMethod]
    [TestCategory("Problems.DataAnalysis")]
    [TestProperty("Time", "short")]
    [ExpectedException(typeof(ArgumentException))]
    public void WrongDataSetVariableImpactClassificationTest() {
      IClassificationProblemData problemData = LoadIrisProblem();
      IClassificationSolution solution = NearestNeighbourClassification.CreateNearestNeighbourClassificationSolution(problemData, 3);
      ClassificationSolutionVariableImpactsCalculator.CalculateImpacts(solution);
      Dictionary<string, double> expectedImpacts = GetExpectedValuesForIrisKNNModel();

      solution.ProblemData = LoadMammographyProblem();
      ClassificationSolutionVariableImpactsCalculator.CalculateImpacts(solution);
    }


    [TestMethod]
    [TestCategory("Problems.DataAnalysis")]
    [TestProperty("Time", "medium")]
    public void PerformanceVariableImpactClassificationTest() {
      int rows = 1500;
      int columns = 77;
      IClassificationProblemData problemData = CreateDefaultProblem(rows, columns);
      IClassificationSolution solution = NearestNeighbourClassification.CreateNearestNeighbourClassificationSolution(problemData, 3);

      Stopwatch watch = new Stopwatch();
      watch.Start();
      var results = ClassificationSolutionVariableImpactsCalculator.CalculateImpacts(solution);
      watch.Stop();

      TestContext.WriteLine("");
      TestContext.WriteLine("Calculated cells per millisecond: {0}.", rows * columns / watch.ElapsedMilliseconds);
    }

    #region Load ClassificationProblemData
    private IClassificationProblemData LoadIrisProblem() {
      UCIInstanceProvider provider = new UCIInstanceProvider();
      var instance = provider.GetDataDescriptors().Where(x => x.Name.Equals("Iris, M. Marshall, 1988")).Single();
      return provider.LoadData(instance);
    }
    private IClassificationProblemData LoadMammographyProblem() {
      UCIInstanceProvider provider = new UCIInstanceProvider();
      var instance = provider.GetDataDescriptors().Where(x => x.Name.Equals("Mammography, M. Elter, 2007")).Single();
      return provider.LoadData(instance);
    }
    private IClassificationProblemData CreateDefaultProblem() {
      List<string> allowedInputVariables = new List<string>() { "x1", "x2", "x3", "x4", "x5" };
      string targetVariable = "y";
      var variableNames = allowedInputVariables.Union(targetVariable.ToEnumerable());
      double[,] variableValues = new double[100, variableNames.Count()];

      FastRandom random = new FastRandom(12345);
      int len0 = variableValues.GetLength(0);
      int len1 = variableValues.GetLength(1);
      for (int i = 0; i < len0; i++) {
        for (int j = 0; j < len1; j++) {
          if (j == len1 - 1) {
            variableValues[i, j] = (j + i) % 2;
          } else {
            variableValues[i, j] = random.Next(1, 100);
          }
        }
      }

      Dataset dataset = new Dataset(variableNames, variableValues);
      var ret = new ClassificationProblemData(dataset, allowedInputVariables, targetVariable);

      ret.SetClassName(0, "NOK");
      ret.SetClassName(1, "OK");
      return ret;
    }

    private IClassificationProblemData CreateDefaultProblem(int rows, int columns) {
      List<string> allowedInputVariables = Enumerable.Range(0, columns - 1).Select(x => "x" + x.ToString()).ToList();
      string targetVariable = "y";
      var variableNames = allowedInputVariables.Union(targetVariable.ToEnumerable());
      double[,] variableValues = new double[rows, columns];

      FastRandom random = new FastRandom(12345);
      int len0 = variableValues.GetLength(0);
      int len1 = variableValues.GetLength(1);
      for (int i = 0; i < len0; i++) {
        for (int j = 0; j < len1; j++) {
          if (j == len1 - 1) {
            variableValues[i, j] = (j + i) % 2;
          } else {
            variableValues[i, j] = random.Next(1, 100);
          }
        }
      }

      Dataset dataset = new Dataset(variableNames, variableValues);
      var ret = new ClassificationProblemData(dataset, allowedInputVariables, targetVariable);

      ret.SetClassName(0, "NOK");
      ret.SetClassName(1, "OK");
      return ret;
    }

    #endregion

    #region Create SymbolicExpressionTree
    private ISymbolicExpressionTree CreateCustomExpressionTree() {
      return new InfixExpressionParser().Parse("x1*x2 - x2*x2 + x3*x3 + x4*x4 - x5*x5 + 14/12");
    }
    private ISymbolicExpressionTree CreateCustomExpressionTreeNoInfluenceX1() {
      return new InfixExpressionParser().Parse("x1/x1*x2 - x2*x2 + x3*x3 + x4*x4 - x5*x5 + 14/12");
    }
    #endregion

    #region Get Expected Values      
    private Dictionary<string, double> GetExpectedValuesForConstantModel() {
      Dictionary<string, double> expectedImpacts = new Dictionary<string, double>();
      expectedImpacts.Add("petal_length", 0);
      expectedImpacts.Add("petal_width", 0);
      expectedImpacts.Add("sepal_length", 0);
      expectedImpacts.Add("sepal_width", 0);

      return expectedImpacts;
    }
    private Dictionary<string, double> GetExpectedValuesForIrisKNNModel() {
      Dictionary<string, double> expectedImpacts = new Dictionary<string, double>();
      expectedImpacts.Add("petal_length", 0.21);
      expectedImpacts.Add("petal_width", 0.25);
      expectedImpacts.Add("sepal_length", 0.05);
      expectedImpacts.Add("sepal_width", 0.05);

      return expectedImpacts;
    }
    private Dictionary<string, double> GetExpectedValuesForCustomProblem() {
      Dictionary<string, double> expectedImpacts = new Dictionary<string, double>();
      expectedImpacts.Add("x1", 0.04);
      expectedImpacts.Add("x2", 0.22);
      expectedImpacts.Add("x3", 0.26);
      expectedImpacts.Add("x4", 0.24);
      expectedImpacts.Add("x5", 0.2);

      return expectedImpacts;
    }
    private Dictionary<string, double> GetExpectedValuesForCustomProblemNoInfluence() {
      Dictionary<string, double> expectedImpacts = new Dictionary<string, double>();
      expectedImpacts.Add("x1", 0);
      expectedImpacts.Add("x2", 0.22);
      expectedImpacts.Add("x3", 0.14);
      expectedImpacts.Add("x4", 0.3);
      expectedImpacts.Add("x5", 0.44);

      return expectedImpacts;
    }
    private Dictionary<string, double> GetExpectedValuesForIrisLDAModel() {
      Dictionary<string, double> expectedImpacts = new Dictionary<string, double>();
      expectedImpacts.Add("sepal_width", 0.01);
      expectedImpacts.Add("sepal_length", 0.03);
      expectedImpacts.Add("petal_width", 0.2);
      expectedImpacts.Add("petal_length", 0.5);

      return expectedImpacts;
    }
    #endregion

    private void CheckDefaultAsserts(IClassificationSolution solution, Dictionary<string, double> expectedImpacts) {
      IClassificationProblemData problemData = solution.ProblemData;
      IEnumerable<double> estimatedValues = solution.GetEstimatedClassValues(solution.ProblemData.TrainingIndices);

      var solutionImpacts = ClassificationSolutionVariableImpactsCalculator.CalculateImpacts(solution);
      var modelImpacts = ClassificationSolutionVariableImpactsCalculator.CalculateImpacts(solution.Model, problemData, estimatedValues, problemData.TrainingIndices);

      //Both ways should return equal results
      Assert.IsTrue(solutionImpacts.SequenceEqual(modelImpacts));

      //Check if impacts are as expected
      Assert.AreEqual(modelImpacts.Count(), expectedImpacts.Count);
      Assert.IsTrue(modelImpacts.All(v => v.Item2.IsAlmost(expectedImpacts[v.Item1])));
    }
  }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using HeuristicLab.Algorithms.DataAnalysis;
using HeuristicLab.Common;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Problems.DataAnalysis.Symbolic;
using HeuristicLab.Problems.DataAnalysis.Symbolic.Regression;
using HeuristicLab.Problems.Instances.DataAnalysis;
using HeuristicLab.Random;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Problems.DataAnalysis.Tests {

  [TestClass()]
  public class RegressionVariableImpactCalculationTest {
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
      IRegressionProblemData problemData = LoadDefaultTowerProblem();
      IRegressionModel model = new ConstantModel(5, "y");
      IRegressionSolution solution = new RegressionSolution(model, problemData);
      Dictionary<string, double> expectedImpacts = GetExpectedValuesForConstantModel();

      CheckDefaultAsserts(solution, expectedImpacts);
    }

    [TestMethod]
    [TestCategory("Problems.DataAnalysis")]
    [TestProperty("Time", "short")]
    public void LinearRegressionModelVariableImpactTowerTest() {
      IRegressionProblemData problemData = LoadDefaultTowerProblem();
      double rmsError;
      double cvRmsError;
      var solution = LinearRegression.CreateSolution(problemData, out rmsError, out cvRmsError);
      Dictionary<string, double> expectedImpacts = GetExpectedValuesForLRTower();

      CheckDefaultAsserts(solution, expectedImpacts);
    }

    [TestMethod]
    [TestCategory("Problems.DataAnalysis")]
    [TestProperty("Time", "short")]
    public void LinearRegressionModelVariableImpactMibaTest() {
      IRegressionProblemData problemData = LoadDefaultMibaProblem();
      double rmsError;
      double cvRmsError;
      var solution = LinearRegression.CreateSolution(problemData, out rmsError, out cvRmsError);
      Dictionary<string, double> expectedImpacts = GetExpectedValuesForLRMiba();

      CheckDefaultAsserts(solution, expectedImpacts);
    }

    [TestMethod]
    [TestCategory("Problems.DataAnalysis")]
    [TestProperty("Time", "short")]
    public void RandomForestModelVariableImpactTowerTest() {
      IRegressionProblemData problemData = LoadDefaultTowerProblem();
      double rmsError;
      double avgRelError;
      double outOfBagRmsError;
      double outofBagAvgRelError;
      var solution = RandomForestRegression.CreateRandomForestRegressionSolution(problemData, 50, 0.2, 0.5, 1234, out rmsError, out avgRelError, out outOfBagRmsError, out outofBagAvgRelError);
      Dictionary<string, double> expectedImpacts = GetExpectedValuesForRFTower();

      CheckDefaultAsserts(solution, expectedImpacts);
    }

    [TestMethod]
    [TestCategory("Problems.DataAnalysis")]
    [TestProperty("Time", "short")]
    public void CustomModelVariableImpactTest() {
      IRegressionProblemData problemData = CreateDefaultProblem();
      ISymbolicExpressionTree tree = CreateCustomExpressionTree();
      IRegressionModel model = new SymbolicRegressionModel(problemData.TargetVariable, tree, new SymbolicDataAnalysisExpressionTreeInterpreter());
      IRegressionSolution solution = new RegressionSolution(model, (IRegressionProblemData)problemData.Clone());
      Dictionary<string, double> expectedImpacts = GetExpectedValuesForCustomProblem();

      CheckDefaultAsserts(solution, expectedImpacts);
    }

    [TestMethod]
    [TestCategory("Problems.DataAnalysis")]
    [TestProperty("Time", "short")]
    public void CustomModelVariableImpactNoInfluenceTest() {
      IRegressionProblemData problemData = CreateDefaultProblem();
      ISymbolicExpressionTree tree = CreateCustomExpressionTreeNoInfluenceX1();
      IRegressionModel model = new SymbolicRegressionModel(problemData.TargetVariable, tree, new SymbolicDataAnalysisExpressionTreeInterpreter());
      IRegressionSolution solution = new RegressionSolution(model, (IRegressionProblemData)problemData.Clone());
      Dictionary<string, double> expectedImpacts = GetExpectedValuesForCustomProblemNoInfluence();

      CheckDefaultAsserts(solution, expectedImpacts);
    }

    [TestMethod]
    [TestCategory("Problems.DataAnalysis")]
    [TestProperty("Time", "short")]
    [ExpectedException(typeof(ArgumentException))]
    public void WrongDataSetVariableImpactRegressionTest() {
      IRegressionProblemData problemData = LoadDefaultTowerProblem();
      double rmsError;
      double cvRmsError;
      var solution = LinearRegression.CreateSolution(problemData, out rmsError, out cvRmsError);
      solution.ProblemData = LoadDefaultMibaProblem();
      RegressionSolutionVariableImpactsCalculator.CalculateImpacts(solution);

    }

    [TestMethod]
    [TestCategory("Problems.DataAnalysis")]
    [TestProperty("Time", "medium")]
    public void PerformanceVariableImpactRegressionTest() {
      int rows = 20000;
      int columns = 77;
      var dataSet = OnlineCalculatorPerformanceTest.CreateRandomDataset(new MersenneTwister(1234), rows, columns);
      IRegressionProblemData problemData = new RegressionProblemData(dataSet, dataSet.VariableNames.Except("y".ToEnumerable()), "y");
      double rmsError;
      double cvRmsError;
      var solution = LinearRegression.CreateSolution(problemData, out rmsError, out cvRmsError);

      Stopwatch watch = new Stopwatch();
      watch.Start();
      var results = RegressionSolutionVariableImpactsCalculator.CalculateImpacts(solution);
      watch.Stop();

      TestContext.WriteLine("");
      TestContext.WriteLine("Calculated cells per millisecond: {0}.", rows * columns / watch.ElapsedMilliseconds);

    }

    #region Load RegressionProblemData
    private IRegressionProblemData LoadDefaultTowerProblem() {
      RegressionRealWorldInstanceProvider provider = new RegressionRealWorldInstanceProvider();
      var tower = new HeuristicLab.Problems.Instances.DataAnalysis.Tower();
      return provider.LoadData(tower);
    }
    private IRegressionProblemData LoadDefaultMibaProblem() {
      MibaFrictionRegressionInstanceProvider provider = new MibaFrictionRegressionInstanceProvider();
      var cf1 = new HeuristicLab.Problems.Instances.DataAnalysis.CF1();
      return provider.LoadData(cf1);
    }
    private IRegressionProblemData CreateDefaultProblem() {
      List<string> allowedInputVariables = new List<string>() { "x1", "x2", "x3", "x4", "x5" };
      string targetVariable = "y";
      var variableNames = allowedInputVariables.Union(targetVariable.ToEnumerable());
      double[,] variableValues = new double[100, variableNames.Count()];

      FastRandom random = new FastRandom(12345);
      for (int i = 0; i < variableValues.GetLength(0); i++) {
        for (int j = 0; j < variableValues.GetLength(1); j++) {
          variableValues[i, j] = random.Next(1, 100);
        }
      }

      Dataset dataset = new Dataset(variableNames, variableValues);
      return new RegressionProblemData(dataset, allowedInputVariables, targetVariable);
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
      expectedImpacts.Add("x1", 0);
      expectedImpacts.Add("x10", 0);
      expectedImpacts.Add("x11", 0);
      expectedImpacts.Add("x12", 0);
      expectedImpacts.Add("x13", 0);
      expectedImpacts.Add("x14", 0);
      expectedImpacts.Add("x15", 0);
      expectedImpacts.Add("x16", 0);
      expectedImpacts.Add("x17", 0);
      expectedImpacts.Add("x18", 0);
      expectedImpacts.Add("x19", 0);
      expectedImpacts.Add("x2", 0);
      expectedImpacts.Add("x20", 0);
      expectedImpacts.Add("x21", 0);
      expectedImpacts.Add("x22", 0);
      expectedImpacts.Add("x23", 0);
      expectedImpacts.Add("x24", 0);
      expectedImpacts.Add("x25", 0);
      expectedImpacts.Add("x3", 0);
      expectedImpacts.Add("x4", 0);
      expectedImpacts.Add("x5", 0);
      expectedImpacts.Add("x6", 0);
      expectedImpacts.Add("x7", 0);
      expectedImpacts.Add("x8", 0);
      expectedImpacts.Add("x9", 0);

      return expectedImpacts;
    }
    private Dictionary<string, double> GetExpectedValuesForLRTower() {
      Dictionary<string, double> expectedImpacts = new Dictionary<string, double>();
      expectedImpacts.Add("x1", 0.639933657675427);
      expectedImpacts.Add("x10", 0.0127006885259798);
      expectedImpacts.Add("x11", 0.648236047877475);
      expectedImpacts.Add("x12", 0.248350173524562);
      expectedImpacts.Add("x13", 0.550889987109547);
      expectedImpacts.Add("x14", 0.0882824237877192);
      expectedImpacts.Add("x15", 0.0391276799061169);
      expectedImpacts.Add("x16", 0.743632451088798);
      expectedImpacts.Add("x17", 0.00254276857715308);
      expectedImpacts.Add("x18", 0.0021548147614302);
      expectedImpacts.Add("x19", 0.00513473927463037);
      expectedImpacts.Add("x2", 0.0107583487931443);
      expectedImpacts.Add("x20", 0.18085069746933);
      expectedImpacts.Add("x21", 0.138053600700762);
      expectedImpacts.Add("x22", 0.000339539790460086);
      expectedImpacts.Add("x23", 0.362111965467117);
      expectedImpacts.Add("x24", 0.0320167935572304);
      expectedImpacts.Add("x25", 0.57460423230969);
      expectedImpacts.Add("x3", 0.688142635515862);
      expectedImpacts.Add("x4", 0.000176632348454664);
      expectedImpacts.Add("x5", 0.0213915503114581);
      expectedImpacts.Add("x6", 0.807976486909701);
      expectedImpacts.Add("x7", 0.716217843319252);
      expectedImpacts.Add("x8", 0.772701841392564);
      expectedImpacts.Add("x9", 0.178418730050997);

      return expectedImpacts;
    }
    private Dictionary<string, double> GetExpectedValuesForLRMiba() {
      Dictionary<string, double> expectedImpacts = new Dictionary<string, double>();
      expectedImpacts.Add("Grooving", 0.0380558091030508);
      expectedImpacts.Add("Material", 0.02195836766156);
      expectedImpacts.Add("Material_Cat", 0.000338687689067418);
      expectedImpacts.Add("Oil", 0.363464994447857);
      expectedImpacts.Add("x10", 0.0015309669014415);
      expectedImpacts.Add("x11", -3.60432578908609E-05);
      expectedImpacts.Add("x12", 0.00118953859087612);
      expectedImpacts.Add("x13", 0.00164240977191832);
      expectedImpacts.Add("x14", 0.000688363685380056);
      expectedImpacts.Add("x15", -4.75067203969948E-05);
      expectedImpacts.Add("x16", 0.00130388206125076);
      expectedImpacts.Add("x17", 0.132351838646134);
      expectedImpacts.Add("x2", -2.47981401556574E-05);
      expectedImpacts.Add("x20", 0.716541716605016);
      expectedImpacts.Add("x22", 0.174959377282835);
      expectedImpacts.Add("x3", -2.65979754026091E-05);
      expectedImpacts.Add("x4", -1.24764212947603E-05);
      expectedImpacts.Add("x5", 0.001184959455798);
      expectedImpacts.Add("x6", 0.000743336665237626);
      expectedImpacts.Add("x7", 0.00188965927889773);
      expectedImpacts.Add("x8", 0.00415201581536351);
      expectedImpacts.Add("x9", 0.00365653880518491);

      return expectedImpacts;
    }
    private Dictionary<string, double> GetExpectedValuesForRFTower() {
      Dictionary<string, double> expectedImpacts = new Dictionary<string, double>();
      expectedImpacts.Add("x1", 0.0460043373592669);
      expectedImpacts.Add("x2", 0.0771677998074048);
      expectedImpacts.Add("x3", 0.00619316631767142);
      expectedImpacts.Add("x4", 0.0173307649229608);
      expectedImpacts.Add("x5", 0.00165078943166208);
      expectedImpacts.Add("x6", 0.393797136971534);
      expectedImpacts.Add("x7", 0.0103041010136826);
      expectedImpacts.Add("x8", 0.0218489557817256);
      expectedImpacts.Add("x9", 0.0033320239547433);
      expectedImpacts.Add("x10", 0.0386869026272391);
      expectedImpacts.Add("x11", 0.0194313200667007);
      expectedImpacts.Add("x12", 0.00685957264555537);
      expectedImpacts.Add("x13", 0.00996030046057605);
      expectedImpacts.Add("x14", 0.00290837005063926);
      expectedImpacts.Add("x15", 0.00487537076065259);
      expectedImpacts.Add("x16", 0.00469764322452204);
      expectedImpacts.Add("x17", 0.0373542975924628);
      expectedImpacts.Add("x18", 0.00208482408757715);
      expectedImpacts.Add("x19", 0.00250463259329947);
      expectedImpacts.Add("x20", 0.00211501042751294);
      expectedImpacts.Add("x21", 0.0028823562854704);
      expectedImpacts.Add("x22", 0.0113878041309722);
      expectedImpacts.Add("x23", 0.0454532954039953);
      expectedImpacts.Add("x24", 0.0115989415893684);
      expectedImpacts.Add("x25", 0.0196479481586532);

      return expectedImpacts;
    }
    private Dictionary<string, double> GetExpectedValuesForCustomProblem() {
      Dictionary<string, double> expectedImpacts = new Dictionary<string, double>();
      expectedImpacts.Add("x1", -0.000573340275115796);
      expectedImpacts.Add("x2", 0.000781819784095592);
      expectedImpacts.Add("x3", -0.000390473234921058);
      expectedImpacts.Add("x4", -0.00116083274627995);
      expectedImpacts.Add("x5", -0.00036161186207545);

      return expectedImpacts;
    }
    private Dictionary<string, double> GetExpectedValuesForCustomProblemNoInfluence() {
      Dictionary<string, double> expectedImpacts = new Dictionary<string, double>();
      expectedImpacts.Add("x1", 0);
      expectedImpacts.Add("x2", 0.00263393690342982);
      expectedImpacts.Add("x3", -0.00053248037514929);
      expectedImpacts.Add("x4", 0.00450365819257568);
      expectedImpacts.Add("x5", -0.000550911612888904);

      return expectedImpacts;
    }
    #endregion

    private void CheckDefaultAsserts(IRegressionSolution solution, Dictionary<string, double> expectedImpacts) {
      IRegressionProblemData problemData = solution.ProblemData;
      IEnumerable<double> estimatedValues = solution.GetEstimatedValues(solution.ProblemData.TrainingIndices);

      var solutionImpacts = RegressionSolutionVariableImpactsCalculator.CalculateImpacts(solution);
      var modelImpacts = RegressionSolutionVariableImpactsCalculator.CalculateImpacts(solution.Model, problemData, estimatedValues, problemData.TrainingIndices);

      //Both ways should return equal results
      Assert.IsTrue(solutionImpacts.SequenceEqual(modelImpacts));

      //Check if impacts are as expected
      Assert.AreEqual(modelImpacts.Count(), expectedImpacts.Count);
      Assert.IsTrue(modelImpacts.All(v => v.Item2.IsAlmost(expectedImpacts[v.Item1])));
    }
  }
}

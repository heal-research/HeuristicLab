using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Algorithms.ALPS;
using HeuristicLab.Algorithms.DataAnalysis;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Problems.DataAnalysis.Symbolic;
using HeuristicLab.Problems.DataAnalysis.Symbolic.Regression;
using HeuristicLab.Problems.Instances.DataAnalysis;
using HeuristicLab.Selection;
using HeuristicLab.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Problems.DataAnalysis.Tests {

  [TestClass()]
  public class VariableImpactCalculationTest {
    [TestMethod]
    [TestCategory("Problems.DataAnalysis")]
    [TestProperty("Time", "medium")]
    public void RunAlpsSymRegFactoryVariableMibaC1Test() {
      var alpsGA = CreateAlpsGaSymRegMibaC1Sample();
      alpsGA.Start();
      var ers = alpsGA.Results.FirstOrDefault(v => v.Name == "Variable impacts").Value as DoubleMatrix;
      Assert.IsNotNull(ers);
      Assert.IsTrue(ers.Rows == 22);
      List<string> expectedVariableOrder = new List<string> { "Oil", "Grooving", "Material_Cat", "x20", "Material", "x14", "x12", "x3", "x7", "x2", "x16", "x15", "x8", "x10", "x11", "x22", "x4", "x5", "x6", "x9", "x17", "x13" };
      List<double> expectedVariableImpacts = new List<double> { 0.206, 0.179, 0.136, 0.099, 0.092, 0.07, 0.048, 0.033, 0.029, 0.026, 0.017, 0.01, 0.007, 0.007, 0.007, 0.006, 0.005, 0.005, 0.005, 0.005, 0.005, 0.003 };
      Assert.IsTrue(ers.RowNames.SequenceEqual(expectedVariableOrder));
      Assert.IsTrue(ers.SequenceEqual(expectedVariableImpacts));
    }

    [TestMethod]
    [TestCategory("Problems.DataAnalysis")]
    [TestProperty("Time", "medium")]
    public void RunAlpsSymRegFactoryVariableMibaWear1Test() {
      var alpsGA = CreateAlpsGaSymRegMibaWear1Sample();
      alpsGA.Start();
      var ers = alpsGA.Results.FirstOrDefault(v => v.Name == "Variable impacts").Value as DoubleMatrix;
      Assert.IsNotNull(ers);
      Assert.IsTrue(ers.Rows == 27);
      List<string> expectedVariableOrder = new List<string> { "Oil", "Material_Cat", "Material", "Grooving", "x8", "x1", "x14", "x11", "x19", "x10", "x5", "x9", "x17", "x6", "x18", "x7", "Source1", "x2", "x3", "x4", "x16", "x20", "x12", "x13", "x15", "x22", "x21" };
      List<double> expectedVariableImpacts = new List<double> { 0.194, 0.184, 0.163, 0.155, 0.022, 0.019, 0.019, 0.018, 0.017, 0.016, 0.015, 0.015, 0.015, 0.014, 0.014, 0.013, 0.012, 0.011, 0.011, 0.011, 0.011, 0.011, 0.01, 0.009, 0.009, 0.008, 0.006 };
      Assert.IsTrue(ers.RowNames.SequenceEqual(expectedVariableOrder));
      Assert.IsTrue(ers.SequenceEqual(expectedVariableImpacts));
    }

    [TestMethod]
    [TestCategory("Problems.DataAnalysis")]
    [TestProperty("Time", "medium")]
    public void RunAlpsSymRegFactoryVariableRealworldTowerTest() {
      var alpsGA = CreateAlpsGaSymReRealworldTowerSample();
      alpsGA.Start();
      var ers = alpsGA.Results.FirstOrDefault(v => v.Name == "Variable impacts").Value as DoubleMatrix;
      Assert.IsNotNull(ers);
      Assert.IsTrue(ers.Rows == 25);
      List<string> expectedVariableOrder = new List<string> { "x5", "x4", "x21", "x22", "x13", "x3", "x9", "x19", "x1", "x12", "x8", "x10", "x25", "x11", "x23", "x7", "x24", "x6", "x14", "x15", "x20", "x2", "x16", "x18", "x17" };
      List<double> expectedVariableImpacts = new List<double> { 0.057, 0.051, 0.047, 0.047, 0.046, 0.045, 0.044, 0.044, 0.043, 0.043, 0.041, 0.04, 0.04, 0.038, 0.038, 0.037, 0.037, 0.036, 0.036, 0.034, 0.033, 0.031, 0.031, 0.031, 0.03 };
      Assert.IsTrue(ers.RowNames.SequenceEqual(expectedVariableOrder));
      Assert.IsTrue(ers.SequenceEqual(expectedVariableImpacts));
    }

    [TestMethod]
    [TestCategory("Problems.DataAnalysis")]
    [TestProperty("Time", "short")]
    public void RunRandomForestPolyTenTest() {
      var randomForestRegression = new RandomForestRegression() {
        M = 0.4,
        R = 0.3,
        NumberOfTrees = 50,
        SetSeedRandomly = false,
        Seed = 42
      };

      var symbRegProblem = new SymbolicRegressionSingleObjectiveProblem();
      symbRegProblem.ProblemData = new PolyTen(555000).GenerateRegressionData();
      randomForestRegression.Problem = symbRegProblem;
      randomForestRegression.Start();

      var rfs = randomForestRegression.Results.FirstOrDefault(v => v.Name == "Random forest regression solution").Value as RandomForestRegressionSolution;
      Assert.IsNotNull(rfs);
      var result = new RegressionSolutionVariableImpactsCalculator().Calculate(rfs);

      //Set expected impacts
      List<Tuple<string, double>> aux = new List<Tuple<string, double>>();
      aux.Add(new Tuple<string, double>("X6", 0.14984398650032071));
      aux.Add(new Tuple<string, double>("X5", 0.14361275357221248));
      aux.Add(new Tuple<string, double>("X1", 0.10607502238071009));
      aux.Add(new Tuple<string, double>("X3", 0.1056849194554953));
      aux.Add(new Tuple<string, double>("X4", 0.0906750779077492));
      aux.Add(new Tuple<string, double>("X2", 0.089817766590950532));
      aux.Add(new Tuple<string, double>("X8", 0.042797597332305481));
      aux.Add(new Tuple<string, double>("X9", 0.038609311722408446));
      aux.Add(new Tuple<string, double>("X10", 0.036202503161574362));
      aux.Add(new Tuple<string, double>("X7", 0.033539038256832465));
      Assert.IsTrue(result.SequenceEqual(aux));
    }

    [TestMethod]
    [TestCategory("Problems.DataAnalysis")]
    [TestProperty("Time", "short")]
    public void RunLinearRegressionBreimanITest() {
      LinearRegression lr = new LinearRegression();
      var symbRegProblem = new SymbolicRegressionSingleObjectiveProblem();
      symbRegProblem.ProblemData = new BreimanOne(1234).GenerateRegressionData();

      lr.Problem = symbRegProblem;
      lr.Start();

      var lrs = lr.Results.FirstOrDefault(v => v.Name == "Linear regression solution").Value as IRegressionSolution;
      Assert.IsNotNull(lrs);
      var result = new RegressionSolutionVariableImpactsCalculator().Calculate(lrs);

      //Set expected impacts
      List<Tuple<string, double>> aux = new List<Tuple<string, double>>();
      aux.Add(new Tuple<string, double>("X1", 0.43328518823918716));
      aux.Add(new Tuple<string, double>("X2", 0.073284548674773631));
      aux.Add(new Tuple<string, double>("X5", 0.070306657566311159));
      aux.Add(new Tuple<string, double>("X3", 0.035352205426012917));
      aux.Add(new Tuple<string, double>("X6", 0.031710492680145475));
      aux.Add(new Tuple<string, double>("X4", 0.0081659530036176653));
      aux.Add(new Tuple<string, double>("X7", 0.0070869550705255913));
      aux.Add(new Tuple<string, double>("X9", 8.0546619615096127E-05));
      aux.Add(new Tuple<string, double>("X8", 6.2072921147349192E-05));
      aux.Add(new Tuple<string, double>("X10", 1.9113559758254794E-06));

      Assert.IsTrue(result.SequenceEqual(aux));
    }

    private AlpsGeneticAlgorithm CreateAlpsGaSymRegMibaC1Sample() {
      AlpsGeneticAlgorithm alpsGa = new AlpsGeneticAlgorithm();
      #region Problem Configuration
      var provider = new MibaFrictionRegressionInstanceProvider();
      var instance = provider.GetDataDescriptors().Single(x => x.Name.StartsWith("CF1"));
      var symbRegProblem = new SymbolicRegressionSingleObjectiveProblem();
      symbRegProblem.Load(provider.LoadData(instance));

      symbRegProblem.MaximumSymbolicExpressionTreeDepth.Value = 35;
      symbRegProblem.MaximumSymbolicExpressionTreeLength.Value = 35;

      var grammar = (TypeCoherentExpressionGrammar)symbRegProblem.SymbolicExpressionTreeGrammar;
      grammar.Symbols.OfType<Exponential>().Single().Enabled = false;
      grammar.Symbols.OfType<Logarithm>().Single().Enabled = false;

      #endregion
      #region Algorithm Configuration
      alpsGa.Name = "ALPS Genetic Programming - Symbolic Regression";
      alpsGa.Description = "An ALPS-GP to solve a symbolic regression problem";
      alpsGa.Problem = symbRegProblem;
      SamplesUtils.ConfigureAlpsGeneticAlgorithmParameters<GeneralizedRankSelector, SubtreeCrossover, MultiSymbolicExpressionTreeManipulator>(alpsGa,
        numberOfLayers: 1000,
        popSize: 100,
        mutationRate: 0.25,
        elites: 1,
        plusSelection: false,
        agingScheme: AgingScheme.Polynomial,
        ageGap: 15,
        ageInheritance: 1.0,
        maxGens: 10);

      alpsGa.SetSeedRandomly.Value = false;
      alpsGa.Seed.Value = 1234;
      #endregion
      return alpsGa;
    }
    private AlpsGeneticAlgorithm CreateAlpsGaSymRegMibaWear1Sample() {
      AlpsGeneticAlgorithm alpsGa = new AlpsGeneticAlgorithm();
      #region Problem Configuration
      var provider = new MibaFrictionRegressionInstanceProvider();
      var instance = provider.GetDataDescriptors().Single(x => x.Name.StartsWith("Wear1"));
      var symbRegProblem = new SymbolicRegressionSingleObjectiveProblem();
      symbRegProblem.Load(provider.LoadData(instance));

      symbRegProblem.MaximumSymbolicExpressionTreeDepth.Value = 77;
      symbRegProblem.MaximumSymbolicExpressionTreeLength.Value = 77;

      var grammar = (TypeCoherentExpressionGrammar)symbRegProblem.SymbolicExpressionTreeGrammar;

      #endregion
      #region Algorithm Configuration
      alpsGa.Name = "ALPS Genetic Programming - Symbolic Regression";
      alpsGa.Description = "An ALPS-GP to solve a symbolic regression problem";
      alpsGa.Problem = symbRegProblem;
      SamplesUtils.ConfigureAlpsGeneticAlgorithmParameters<RandomSelector, SubtreeCrossover, MultiSymbolicExpressionTreeManipulator>(alpsGa,
        numberOfLayers: 1000,
        popSize: 200,
        mutationRate: 0.25,
        elites: 1,
        plusSelection: false,
        agingScheme: AgingScheme.Polynomial,
        ageGap: 15,
        ageInheritance: 1.0,
        maxGens: 10);

      alpsGa.SetSeedRandomly.Value = false;
      alpsGa.Seed.Value = 11121314;
      #endregion
      return alpsGa;
    }
    private AlpsGeneticAlgorithm CreateAlpsGaSymReRealworldTowerSample() {
      AlpsGeneticAlgorithm alpsGa = new AlpsGeneticAlgorithm();
      #region Problem Configuration
      var provider = new RegressionRealWorldInstanceProvider();
      var instance = provider.GetDataDescriptors().Single(x => x.Name.StartsWith("Tower"));
      var symbRegProblem = new SymbolicRegressionSingleObjectiveProblem();
      symbRegProblem.Load(provider.LoadData(instance));

      symbRegProblem.MaximumSymbolicExpressionTreeDepth.Value = 77;
      symbRegProblem.MaximumSymbolicExpressionTreeLength.Value = 77;

      var grammar = (TypeCoherentExpressionGrammar)symbRegProblem.SymbolicExpressionTreeGrammar;

      #endregion
      #region Algorithm Configuration
      alpsGa.Name = "ALPS Genetic Programming - Symbolic Regression";
      alpsGa.Description = "An ALPS-GP to solve a symbolic regression problem";
      alpsGa.Problem = symbRegProblem;
      SamplesUtils.ConfigureAlpsGeneticAlgorithmParameters<RandomSelector, SubtreeCrossover, MultiSymbolicExpressionTreeManipulator>(alpsGa,
        numberOfLayers: 1000,
        popSize: 200,
        mutationRate: 0.25,
        elites: 1,
        plusSelection: false,
        agingScheme: AgingScheme.Polynomial,
        ageGap: 15,
        ageInheritance: 1.0,
        maxGens: 10);

      alpsGa.SetSeedRandomly.Value = false;
      alpsGa.Seed.Value = 1111;
      #endregion
      return alpsGa;
    }

    //TODO: Add Function-Tests once the Branch of #2904 is done
  }
}

using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HeuristicLab.Algorithms.GeneticAlgorithm;
using HeuristicLab.Problems.ArtificialAnt;
using HeuristicLab.Selection;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Persistence.Default.Xml;
using HeuristicLab.Optimization;
using System.Threading;
using HeuristicLab.ParallelEngine;
using HeuristicLab.Problems.DataAnalysis.Symbolic.Regression;
using HeuristicLab.Problems.DataAnalysis;
using HeuristicLab.Problems.DataAnalysis.Symbolic;
using System.IO;
using HeuristicLab.Problems.DataAnalysis.Symbolic.Classification;
using HeuristicLab.Problems.TravelingSalesman;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Problems.VehicleRouting;
using HeuristicLab.Problems.VehicleRouting.Encodings.Potvin;
using HeuristicLab.Problems.VehicleRouting.Encodings;
using HeuristicLab.Problems.VehicleRouting.Encodings.General;

namespace HeuristicLab_33.Tests {
  [TestClass]
  public class GeneticAlgorithmSamplesTest {
    [TestMethod]
    public void CreateTSPSample() {
      GeneticAlgorithm ga = new GeneticAlgorithm();
      #region problem configuration
      TravelingSalesmanProblem tspProblem = new TravelingSalesmanProblem();
      // import and configure TSP data
      string ch130FileName = Path.GetTempFileName() + ".tsp";// for silly parser constraints
      using (var writer = File.CreateText(ch130FileName)) {
        writer.Write(HeuristicLab_33.Tests.Properties.Resources.ch130);
      }
      string ch130OptTourFileName = Path.GetTempFileName() + ".opt.tour"; // for silly parser constraints
      using (var writer = File.CreateText(ch130OptTourFileName)) {
        writer.Write(HeuristicLab_33.Tests.Properties.Resources.ch130_opt);
      }

      tspProblem.ImportFromTSPLIB(ch130FileName, ch130OptTourFileName, 6110);
      tspProblem.Evaluator = new TSPRoundedEuclideanPathEvaluator();
      tspProblem.SolutionCreator = new RandomPermutationCreator();
      tspProblem.UseDistanceMatrix.Value = true;
      tspProblem.Name = "ch130 TSP (imported from TSPLIB)";
      tspProblem.Description = "130 city problem (Churritz)";
      #endregion
      #region algorithm configuration
      ga.Name = "Genetic Algorithm - TSP";
      ga.Description = "A genetic algorithm which solves the \"ch130\" traveling salesman problem (imported from TSPLIB)";
      ga.Problem = tspProblem;
      ConfigureGeneticAlgorithmParameters<ProportionalSelector, OrderCrossover2, InversionManipulator>(
        ga, 100, 1, 1000, 0.05);

      ga.Analyzer.Operators.SetItemCheckedState(ga.Analyzer.Operators
        .OfType<TSPAlleleFrequencyAnalyzer>()
        .Single(), false);
      ga.Analyzer.Operators.SetItemCheckedState(ga.Analyzer.Operators
        .OfType<TSPPopulationDiversityAnalyzer>()
        .Single(), false);
      #endregion

      XmlGenerator.Serialize(ga, "../../GA_TSP.hl");

      RunAlgorithm(ga);
    }
    [TestMethod]
    public void CreateVRPSample() {
      GeneticAlgorithm ga = new GeneticAlgorithm();
      #region problem configuration
      VehicleRoutingProblem vrpProblem = new VehicleRoutingProblem();
      // import and configure VRP data
      string c101FileName = Path.GetTempFileName();
      using (var writer = File.CreateText(c101FileName)) {
        writer.Write(HeuristicLab_33.Tests.Properties.Resources.C101);
      }
      // import and configure VRP data
      string c101BestSolutionFileName = Path.GetTempFileName();
      using (var writer = File.CreateText(c101BestSolutionFileName)) {
        writer.Write(HeuristicLab_33.Tests.Properties.Resources.C101_opt);
      }

      vrpProblem.ImportFromSolomon(c101FileName);
      vrpProblem.ImportSolution(c101BestSolutionFileName);
      vrpProblem.Name = "C101 VRP (imported from Solomon)";
      vrpProblem.Description = "Represents a Vehicle Routing Problem.";
      vrpProblem.DistanceFactorParameter.Value.Value = 1;
      vrpProblem.FleetUsageFactorParameter.Value.Value = 100;
      vrpProblem.OverloadPenaltyParameter.Value.Value = 100;
      vrpProblem.TardinessPenaltyParameter.Value.Value = 100;
      vrpProblem.TimeFactorParameter.Value.Value = 0;
      vrpProblem.Evaluator = new VRPEvaluator();
      vrpProblem.MaximizationParameter.Value.Value = false;
      vrpProblem.SolutionCreator = new RandomCreator();
      vrpProblem.UseDistanceMatrix.Value = true;
      vrpProblem.Vehicles.Value = 25;
      #endregion
      #region algorithm configuration
      ga.Name = "Genetic Algorithm - VRP";
      ga.Description = "A genetic algorithm which solves the \"C101\" vehicle routing problem (imported from Solomon)";
      ga.Problem = vrpProblem;
      ConfigureGeneticAlgorithmParameters<TournamentSelector, MultiVRPSolutionCrossover, MultiVRPSolutionManipulator>(
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

      XmlGenerator.Serialize(ga, "../../GA_VRP.hl");

      RunAlgorithm(ga);
    }

    [TestMethod]
    public void CreateArtificialAntSample() {
      GeneticAlgorithm ga = new GeneticAlgorithm();
      #region problem configuration
      ArtificialAntProblem antProblem = new ArtificialAntProblem();
      antProblem.BestKnownQuality.Value = 89;
      antProblem.MaxExpressionDepth.Value = 10;
      antProblem.MaxExpressionLength.Value = 100;
      antProblem.MaxFunctionArguments.Value = 3;
      antProblem.MaxFunctionDefinitions.Value = 3;
      antProblem.MaxTimeSteps.Value = 600;
      #endregion
      #region algorithm configuration
      ga.Name = "Genetic Programming - Artificial Ant";
      ga.Description = "A standard genetic programming algorithm to solve the artificial ant problem (Santa-Fe trail)";
      ga.Problem = antProblem;
      ConfigureGeneticAlgorithmParameters<TournamentSelector, SubtreeCrossover, MultiSymbolicExpressionTreeArchitectureManipulator>(
        ga, 1000, 1, 100, 0.15, 5);
      var mutator = (MultiSymbolicExpressionTreeArchitectureManipulator)ga.Mutator;
      mutator.Operators.SetItemCheckedState(mutator.Operators
        .OfType<FullTreeShaker>()
        .Single(), false);
      mutator.Operators.SetItemCheckedState(mutator.Operators
        .OfType<OnePointShaker>()
        .Single(), false);
      mutator.Operators.SetItemCheckedState(mutator.Operators
        .OfType<ArgumentDeleter>()
        .Single(), false);
      mutator.Operators.SetItemCheckedState(mutator.Operators
        .OfType<SubroutineDeleter>()
        .Single(), false);
      #endregion

      XmlGenerator.Serialize(ga, "../../SGP_SantaFe.hl");

      RunAlgorithm(ga);
    }

    [TestMethod]
    public void CreateSymbolicRegressionSample() {
      GeneticAlgorithm ga = new GeneticAlgorithm();
      #region problem configuration
      SymbolicRegressionSingleObjectiveProblem symbRegProblem = new SymbolicRegressionSingleObjectiveProblem();
      symbRegProblem.Name = "Tower Symbolic Regression Problem";
      symbRegProblem.Description = "Tower Dataset (downloaded from: http://vanillamodeling.com/realproblems.html)";
      // import and configure problem data
      string filename = Path.GetTempFileName();
      using (var writer = File.CreateText(filename)) {
        writer.Write(HeuristicLab_33.Tests.Properties.Resources.TowerData);
      }
      var towerProblemData = RegressionProblemData.ImportFromFile(filename);
      towerProblemData.TargetVariableParameter.Value = towerProblemData.TargetVariableParameter.ValidValues
        .First(v => v.Value == "towerResponse");
      towerProblemData.InputVariables.SetItemCheckedState(
        towerProblemData.InputVariables.Single(x => x.Value == "x1"), true);
      towerProblemData.InputVariables.SetItemCheckedState(
        towerProblemData.InputVariables.Single(x => x.Value == "x7"), false);
      towerProblemData.InputVariables.SetItemCheckedState(
        towerProblemData.InputVariables.Single(x => x.Value == "x11"), false);
      towerProblemData.InputVariables.SetItemCheckedState(
        towerProblemData.InputVariables.Single(x => x.Value == "x16"), false);
      towerProblemData.InputVariables.SetItemCheckedState(
        towerProblemData.InputVariables.Single(x => x.Value == "x21"), false);
      towerProblemData.InputVariables.SetItemCheckedState(
        towerProblemData.InputVariables.Single(x => x.Value == "x25"), false);
      towerProblemData.InputVariables.SetItemCheckedState(
        towerProblemData.InputVariables.Single(x => x.Value == "towerResponse"), false);
      towerProblemData.TrainingPartition.Start = 0;
      towerProblemData.TrainingPartition.End = 4000;
      towerProblemData.TestPartition.Start = 4000;
      towerProblemData.TestPartition.End = 4999;
      towerProblemData.Name = "Data imported from towerData.txt";
      towerProblemData.Description = "Chemical concentration at top of distillation tower, dataset downloaded from: http://vanillamodeling.com/realproblems.html, best R² achieved with nu-SVR = 0.97";
      symbRegProblem.ProblemData = towerProblemData;

      // configure grammar
      var grammar = new TypeCoherentExpressionGrammar();
      grammar.Symbols.OfType<Sine>().Single().InitialFrequency = 0.0;
      grammar.Symbols.OfType<Cosine>().Single().InitialFrequency = 0.0;
      grammar.Symbols.OfType<Tangent>().Single().InitialFrequency = 0.0;
      grammar.Symbols.OfType<IfThenElse>().Single().InitialFrequency = 0.0;
      grammar.Symbols.OfType<GreaterThan>().Single().InitialFrequency = 0.0;
      grammar.Symbols.OfType<LessThan>().Single().InitialFrequency = 0.0;
      grammar.Symbols.OfType<And>().Single().InitialFrequency = 0.0;
      grammar.Symbols.OfType<Or>().Single().InitialFrequency = 0.0;
      grammar.Symbols.OfType<Not>().Single().InitialFrequency = 0.0;
      grammar.Symbols.OfType<TimeLag>().Single().InitialFrequency = 0.0;
      grammar.Symbols.OfType<Integral>().Single().InitialFrequency = 0.0;
      grammar.Symbols.OfType<Derivative>().Single().InitialFrequency = 0.0;
      grammar.Symbols.OfType<LaggedVariable>().Single().InitialFrequency = 0.0;
      grammar.Symbols.OfType<VariableCondition>().Single().InitialFrequency = 0.0;
      var varSymbol = grammar.Symbols.OfType<Variable>().Where(x => !(x is LaggedVariable)).Single();
      varSymbol.WeightMu = 1.0;
      varSymbol.WeightSigma = 1.0;
      varSymbol.WeightManipulatorMu = 0.0;
      varSymbol.WeightManipulatorSigma = 0.05;
      varSymbol.MultiplicativeWeightManipulatorSigma = 0.03;
      var constSymbol = grammar.Symbols.OfType<Constant>().Single();
      constSymbol.MaxValue = 20;
      constSymbol.MinValue = -20;
      constSymbol.ManipulatorMu = 0.0;
      constSymbol.ManipulatorSigma = 1;
      constSymbol.MultiplicativeManipulatorSigma = 0.03;
      symbRegProblem.SymbolicExpressionTreeGrammar = grammar;

      // configure remaining problem parameters
      symbRegProblem.BestKnownQuality.Value = 0.97;
      symbRegProblem.FitnessCalculationPartition.Start = 0;
      symbRegProblem.FitnessCalculationPartition.End = 2800;
      symbRegProblem.ValidationPartition.Start = 2800;
      symbRegProblem.ValidationPartition.End = 4000;
      symbRegProblem.RelativeNumberOfEvaluatedSamples.Value = 0.3;
      symbRegProblem.MaximumSymbolicExpressionTreeLength.Value = 150;
      symbRegProblem.MaximumSymbolicExpressionTreeDepth.Value = 12;
      symbRegProblem.MaximumFunctionDefinitions.Value = 0;
      symbRegProblem.MaximumFunctionArguments.Value = 0;

      symbRegProblem.EvaluatorParameter.Value = new SymbolicRegressionSingleObjectivePearsonRSquaredEvaluator();
      #endregion
      #region algorithm configuration
      ga.Problem = symbRegProblem;
      ga.Name = "Genetic Programming - Symbolic Regression";
      ga.Description = "A standard genetic programming algorithm to solve a symbolic regression problem (tower dataset)";
      ConfigureGeneticAlgorithmParameters<TournamentSelector, SubtreeCrossover, MultiSymbolicExpressionTreeManipulator>(
        ga, 1000, 1, 100, 0.15, 5);
      var mutator = (MultiSymbolicExpressionTreeManipulator)ga.Mutator;
      mutator.Operators.OfType<FullTreeShaker>().Single().ShakingFactor = 0.1;
      mutator.Operators.OfType<OnePointShaker>().Single().ShakingFactor = 1.0;

      ga.Analyzer.Operators.SetItemCheckedState(
        ga.Analyzer.Operators
        .OfType<SymbolicRegressionSingleObjectiveOverfittingAnalyzer>()
        .Single(), false);
      #endregion

      XmlGenerator.Serialize(ga, "../../SGP_SymbReg.hl");

      RunAlgorithm(ga);
    }

    [TestMethod]
    public void CreateSymbolicClassificationSample() {
      GeneticAlgorithm ga = new GeneticAlgorithm();
      #region problem configuration
      SymbolicClassificationSingleObjectiveProblem symbClassProblem = new SymbolicClassificationSingleObjectiveProblem();
      symbClassProblem.Name = "Mammography Classification Problem";
      symbClassProblem.Description = "Mammography dataset imported from the UCI machine learning repository (http://archive.ics.uci.edu/ml/datasets/Mammographic+Mass)";
      // import and configure problem data
      string filename = Path.GetTempFileName();
      using (var writer = File.CreateText(filename)) {
        writer.Write(HeuristicLab_33.Tests.Properties.Resources.MammographicMasses);
      }
      var mammoData = ClassificationProblemData.ImportFromFile(filename);
      mammoData.TargetVariableParameter.Value = mammoData.TargetVariableParameter.ValidValues
        .First(v => v.Value == "Severity");
      mammoData.InputVariables.SetItemCheckedState(
        mammoData.InputVariables.Single(x => x.Value == "BI-RADS"), false);
      mammoData.InputVariables.SetItemCheckedState(
        mammoData.InputVariables.Single(x => x.Value == "Age"), true);
      mammoData.InputVariables.SetItemCheckedState(
        mammoData.InputVariables.Single(x => x.Value == "Shape"), true);
      mammoData.InputVariables.SetItemCheckedState(
        mammoData.InputVariables.Single(x => x.Value == "Margin"), true);
      mammoData.InputVariables.SetItemCheckedState(
        mammoData.InputVariables.Single(x => x.Value == "Density"), true);
      mammoData.InputVariables.SetItemCheckedState(
        mammoData.InputVariables.Single(x => x.Value == "Severity"), false);
      mammoData.TrainingPartition.Start = 0;
      mammoData.TrainingPartition.End = 800;
      mammoData.TestPartition.Start = 800;
      mammoData.TestPartition.End = 961;
      mammoData.Name = "Data imported from mammographic_masses.csv";
      mammoData.Description = "Original dataset: http://archive.ics.uci.edu/ml/datasets/Mammographic+Mass, missing values have been replaced with median values.";
      symbClassProblem.ProblemData = mammoData;

      // configure grammar
      var grammar = new TypeCoherentExpressionGrammar();
      grammar.Symbols.OfType<Sine>().Single().InitialFrequency = 0.0;
      grammar.Symbols.OfType<Cosine>().Single().InitialFrequency = 0.0;
      grammar.Symbols.OfType<Tangent>().Single().InitialFrequency = 0.0;
      grammar.Symbols.OfType<Power>().Single().InitialFrequency = 0.0;
      grammar.Symbols.OfType<Root>().Single().InitialFrequency = 0.0;
      grammar.Symbols.OfType<TimeLag>().Single().InitialFrequency = 0.0;
      grammar.Symbols.OfType<Integral>().Single().InitialFrequency = 0.0;
      grammar.Symbols.OfType<Derivative>().Single().InitialFrequency = 0.0;
      grammar.Symbols.OfType<LaggedVariable>().Single().InitialFrequency = 0.0;
      grammar.Symbols.OfType<VariableCondition>().Single().InitialFrequency = 0.0;
      var varSymbol = grammar.Symbols.OfType<Variable>().Where(x => !(x is LaggedVariable)).Single();
      varSymbol.WeightMu = 1.0;
      varSymbol.WeightSigma = 1.0;
      varSymbol.WeightManipulatorMu = 0.0;
      varSymbol.WeightManipulatorSigma = 0.05;
      varSymbol.MultiplicativeWeightManipulatorSigma = 0.03;
      var constSymbol = grammar.Symbols.OfType<Constant>().Single();
      constSymbol.MaxValue = 20;
      constSymbol.MinValue = -20;
      constSymbol.ManipulatorMu = 0.0;
      constSymbol.ManipulatorSigma = 1;
      constSymbol.MultiplicativeManipulatorSigma = 0.03;
      symbClassProblem.SymbolicExpressionTreeGrammar = grammar;

      // configure remaining problem parameters
      symbClassProblem.BestKnownQuality.Value = 0.0;
      symbClassProblem.FitnessCalculationPartition.Start = 0;
      symbClassProblem.FitnessCalculationPartition.End = 400;
      symbClassProblem.ValidationPartition.Start = 400;
      symbClassProblem.ValidationPartition.End = 800;
      symbClassProblem.RelativeNumberOfEvaluatedSamples.Value = 1;
      symbClassProblem.MaximumSymbolicExpressionTreeLength.Value = 100;
      symbClassProblem.MaximumSymbolicExpressionTreeDepth.Value = 10;
      symbClassProblem.MaximumFunctionDefinitions.Value = 0;
      symbClassProblem.MaximumFunctionArguments.Value = 0;
      symbClassProblem.EvaluatorParameter.Value = new SymbolicClassificationSingleObjectiveMeanSquaredErrorEvaluator();
      #endregion
      #region algorithm configuration
      ga.Problem = symbClassProblem;
      ga.Name = "Genetic Programming - Symbolic Classification";
      ga.Description = "A standard genetic programming algorithm to solve a classification problem (Mammographic+Mass dataset)";
      ConfigureGeneticAlgorithmParameters<TournamentSelector, SubtreeCrossover, MultiSymbolicExpressionTreeManipulator>(
        ga, 1000, 1, 100, 0.15, 5
        );

      var mutator = (MultiSymbolicExpressionTreeManipulator)ga.Mutator;
      mutator.Operators.OfType<FullTreeShaker>().Single().ShakingFactor = 0.1;
      mutator.Operators.OfType<OnePointShaker>().Single().ShakingFactor = 1.0;

      ga.Analyzer.Operators.SetItemCheckedState(
        ga.Analyzer.Operators
        .OfType<SymbolicClassificationSingleObjectiveOverfittingAnalyzer>()
        .Single(), false);
      #endregion

      XmlGenerator.Serialize(ga, "../../SGP_SymbClass.hl");

      RunAlgorithm(ga);
    }

    private void ConfigureGeneticAlgorithmParameters<S, C, M>(GeneticAlgorithm ga, int popSize, int elites, int maxGens, double mutationRate, int tournGroupSize = 0)
      where S : ISelector
      where C : ICrossover
      where M : IManipulator {
      ga.Elites.Value = elites;
      ga.MaximumGenerations.Value = maxGens;
      ga.MutationProbability.Value = mutationRate;
      ga.PopulationSize.Value = popSize;
      ga.Seed.Value = 0;
      ga.SetSeedRandomly.Value = true;
      ga.Selector = ga.SelectorParameter.ValidValues
        .OfType<S>()
        .Single();

      ga.Crossover = ga.CrossoverParameter.ValidValues
        .OfType<C>()
        .Single();

      ga.Mutator = ga.MutatorParameter.ValidValues
        .OfType<M>()
        .Single();

      var tSelector = ga.Selector as TournamentSelector;
      if (tSelector != null) {
        tSelector.GroupSizeParameter.Value.Value = 5;
      }
      ga.Engine = new ParallelEngine();
    }


    private void RunAlgorithm(IAlgorithm a) {
      var trigger = new EventWaitHandle(false, EventResetMode.ManualReset);
      Exception ex = null;
      a.Stopped += (src, e) => { trigger.Set(); };
      a.ExceptionOccurred += (src, e) => { ex = e.Value; };
      a.Prepare();
      a.Start();
      trigger.WaitOne();

      Assert.AreEqual(ex, null);
    }
  }
}

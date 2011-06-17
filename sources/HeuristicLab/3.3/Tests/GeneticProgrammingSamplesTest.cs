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

namespace HeuristicLab_33.Tests {
  [TestClass]
  public class GeneticProgrammingSamplesTest {
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
      ga.PopulationSize.Value = 1000;
      ga.MaximumGenerations.Value = 100;
      ga.MutationProbability.Value = 0.15;
      ga.Elites.Value = 1;
      var tSelector = ga.SelectorParameter.ValidValues
        .OfType<TournamentSelector>()
        .Single();
      tSelector.GroupSizeParameter.Value = new IntValue(5);
      ga.Selector = tSelector;
      var mutator = ga.MutatorParameter.ValidValues
        .OfType<MultiSymbolicExpressionTreeArchitectureManipulator>()
        .Single();
      mutator.Operators.SetItemCheckedState(mutator.Operators
        .OfType<FullTreeShaker>()
        .Single(), false);
      mutator.Operators.SetItemCheckedState(mutator.Operators
        .OfType<OnePointShaker>()
        .Single(), false);
      ga.Mutator = mutator;

      ga.SetSeedRandomly.Value = false;
      ga.Seed.Value = 0;
      ga.Engine = new ParallelEngine();
      #endregion

      XmlGenerator.Serialize(ga, "../../SGP_SantaFe.hl");

      RunAlgorithm(ga);
    }

    [TestMethod]
    public void CreateSymbolicRegressionSample() {
      GeneticAlgorithm ga = new GeneticAlgorithm();
      #region problem configuration
      SymbolicRegressionSingleObjectiveProblem symbRegProblem = new SymbolicRegressionSingleObjectiveProblem();
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
      symbRegProblem.EvaluatorParameter.Value = new SymbolicRegressionSingleObjectivePearsonRSquaredEvaluator();
      #endregion
      #region algorithm configuration
      ga.Problem = symbRegProblem;
      ga.Name = "Genetic Programming - Symbolic Regression";
      ga.Description = "A standard genetic programming algorithm to solve a symbolic regression problem (tower dataset)";
      ga.Elites.Value = 1;
      ga.MaximumGenerations.Value = 100;
      ga.MutationProbability.Value = 0.15;
      ga.PopulationSize.Value = 1000;
      ga.Seed.Value = 0;
      ga.SetSeedRandomly.Value = false;
      var tSelector = ga.SelectorParameter.ValidValues
        .OfType<TournamentSelector>()
        .Single();
      tSelector.GroupSizeParameter.Value.Value = 5;
      ga.Selector = tSelector;
      var mutator = ga.MutatorParameter.ValidValues
        .OfType<MultiSymbolicExpressionTreeManipulator>()
        .Single();
      mutator.Operators.OfType<FullTreeShaker>().Single().ShakingFactor = 0.1;
      mutator.Operators.OfType<OnePointShaker>().Single().ShakingFactor = 1.0;
      ga.Mutator = mutator;

      ga.Analyzer.Operators.SetItemCheckedState(
        ga.Analyzer.Operators
        .OfType<SymbolicRegressionSingleObjectiveOverfittingAnalyzer>()
        .Single(), false);
      ga.Engine = new ParallelEngine();
      #endregion

      XmlGenerator.Serialize(ga, "../../SGP_SymbReg.hl");

      RunAlgorithm(ga);
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

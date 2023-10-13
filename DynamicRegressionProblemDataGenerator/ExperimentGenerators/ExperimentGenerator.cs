using System.Linq;
using HeuristicLab.Algorithms.GeneticAlgorithm;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Problems.DataAnalysis.Dynamic;
using HeuristicLab.Problems.DataAnalysis.Dynamic.ProblemInstances;
using HeuristicLab.Problems.DataAnalysis.Symbolic;
using HeuristicLab.Problems.Dynamic;
using HeuristicLab.Selection;
using Xunit;

namespace DynamicRegressionProblemDataGenerator.ExperimentGenerators {
  public class ExperimentGenerator {

    [Fact]
    [Trait("Generate", "Experiment")]
    void GenerateExperiment_F1_GA() {
      ContentManager.Initialize(new PersistenceContentManager());
      
      var alg = new GeneticAlgorithm();
      var problem = new DynamicSymbolicRegressionProblem();
      alg.Problem = problem;

      var csvProvider = new DynSymRegCsvInstanceProvider();
      var problemData = csvProvider.ImportData(@"C:\Users\P41107\Desktop\F1.csv");
      problemData.PartitionsUpdate = PartitionsUpdateMode.KeepLast;
      problem.Load(problemData);

      var generationClock = problem.EpochClockParameter.ValidValues.OfType<CountingClock>().Single(c => c.Name == "Generation Counting Clock");
      problem.EpochClockParameter.Value = generationClock;
      generationClock.IntervalSize = 1;

      problem.Grammar.GetSymbol("Exponential and Logarithmic Functions").Enabled = false;
      problem.MaximumTreeDepth = 10;
      problem.MaximumTreeLength = 25;

      problem.OptimizeParameters = true;

      alg.PopulationSize.Value = 100;
      
      alg.Selector = alg.SelectorParameter.ValidValues.Single(s => s.Name == "ProportionalSelector");

      alg.Crossover = alg.CrossoverParameter.ValidValues.Single(c => c.Name == "SubtreeSwappingCrossover");
      
      alg.MutationProbability.Value = 0.15;
      alg.Mutator = alg.MutatorParameter.ValidValues.Single(m => m.Name == "MultiSymbolicExpressionTreeManipulator");

      alg.Analyzer.AddOperator(new SymbolicDataAnalysisVariableFrequencyAnalyzer());
      problem.ImpactFactorRepetitionsBestSolutionParameter.Value.Value = 5;
      problem.ImpactFactorRepetitionsPopulationParameter.Value.Value = 1;

      alg.MaximumGenerations.Value = 700;

      ContentManager.Save(alg, @"C:\Users\P41107\Desktop\F1_GA.hl", compressed: true);
    }
    
    
    [Fact]
    [Trait("Generate", "Experiment")]
    void GenerateExperiment_F2_GA() {
      ContentManager.Initialize(new PersistenceContentManager());
      
      var alg = new GeneticAlgorithm();
      var problem = new DynamicSymbolicRegressionProblem();
      alg.Problem = problem;

      var csvProvider = new DynSymRegCsvInstanceProvider();
      var problemData = csvProvider.ImportData(@"C:\Users\P41107\Desktop\F2.csv");
      problemData.PartitionsUpdate = PartitionsUpdateMode.KeepLast;
      problem.Load(problemData);

      var generationClock = problem.EpochClockParameter.ValidValues.OfType<CountingClock>().Single(c => c.Name == "Generation Counting Clock");
      problem.EpochClockParameter.Value = generationClock;
      generationClock.IntervalSize = 1;

      problem.Grammar.GetSymbol("Exponential and Logarithmic Functions").Enabled = false;
      problem.MaximumTreeDepth = 20;
      problem.MaximumTreeLength = 40;

      problem.OptimizeParameters = true;

      alg.PopulationSize.Value = 200;
      
      //alg.Selector = alg.SelectorParameter.ValidValues.Single(s => s.Name == "ProportionalSelector");
      var selector = alg.SelectorParameter.ValidValues.OfType<TournamentSelector>().Single();
      selector.GroupSizeParameter.Value = new IntValue(3);
      alg.Selector = selector;

      alg.Crossover = alg.CrossoverParameter.ValidValues.Single(c => c.Name == "SubtreeSwappingCrossover");
      
      alg.MutationProbability.Value = 0.15;
      alg.Mutator = alg.MutatorParameter.ValidValues.Single(m => m.Name == "MultiSymbolicExpressionTreeManipulator");

      alg.Analyzer.AddOperator(new SymbolicDataAnalysisVariableFrequencyAnalyzer());
      problem.ImpactFactorRepetitionsBestSolutionParameter.Value.Value = 5;
      problem.ImpactFactorRepetitionsPopulationParameter.Value.Value = 1;

      alg.MaximumGenerations.Value = 700;

      ContentManager.Save(alg, @"C:\Users\P41107\Desktop\F2_GA.hl", compressed: true);
    }
    
    [Fact]
    [Trait("Generate", "Experiment")]
    void GenerateExperiment_F3_GA() {
      ContentManager.Initialize(new PersistenceContentManager());
      
      var alg = new GeneticAlgorithm();
      var problem = new DynamicSymbolicRegressionProblem();
      alg.Problem = problem;

      var csvProvider = new DynSymRegCsvInstanceProvider();
      var problemData = csvProvider.ImportData(@"C:\Users\P41107\Desktop\F3.csv");
      problemData.PartitionsUpdate = PartitionsUpdateMode.KeepLast;
      problem.Load(problemData);

      var generationClock = problem.EpochClockParameter.ValidValues.OfType<CountingClock>().Single(c => c.Name == "Generation Counting Clock");
      problem.EpochClockParameter.Value = generationClock;
      generationClock.IntervalSize = 1;

      problem.Grammar.GetSymbol("Exponential and Logarithmic Functions").Enabled = false;
      problem.MaximumTreeDepth = 50;
      problem.MaximumTreeLength = 70;

      problem.OptimizeParameters = true;

      alg.PopulationSize.Value = 500;
      
      //alg.Selector = alg.SelectorParameter.ValidValues.Single(s => s.Name == "ProportionalSelector");
      var selector = alg.SelectorParameter.ValidValues.OfType<TournamentSelector>().Single();
      selector.GroupSizeParameter.Value = new IntValue(4);
      alg.Selector = selector;

      alg.Crossover = alg.CrossoverParameter.ValidValues.Single(c => c.Name == "SubtreeSwappingCrossover");
      
      alg.MutationProbability.Value = 0.15;
      alg.Mutator = alg.MutatorParameter.ValidValues.Single(m => m.Name == "MultiSymbolicExpressionTreeManipulator");

      alg.Analyzer.AddOperator(new SymbolicDataAnalysisVariableFrequencyAnalyzer());
      problem.ImpactFactorRepetitionsBestSolutionParameter.Value.Value = 5;
      problem.ImpactFactorRepetitionsPopulationParameter.Value.Value = 1;

      alg.MaximumGenerations.Value = 700;

      ContentManager.Save(alg, @"C:\Users\P41107\Desktop\F3_GA.hl", compressed: true);
    }
    
    [Fact]
    [Trait("Generate", "Experiment")]
    void GenerateExperiment_Friedman_Test_3_0_00GA() {
      ContentManager.Initialize(new PersistenceContentManager());
      
      var alg = new GeneticAlgorithm();
      var problem = new DynamicSymbolicRegressionProblem();
      alg.Problem = problem;

      var csvProvider = new DynSymRegCsvInstanceProvider();
      var problemData = csvProvider.ImportData(@"C:\Users\P41107\Desktop\Friedman_Test_3_0.00.csv");
      problemData.PartitionsUpdate = PartitionsUpdateMode.KeepLast;
      problem.Load(problemData);

      var generationClock = problem.EpochClockParameter.ValidValues.OfType<CountingClock>().Single(c => c.Name == "Generation Counting Clock");
      problem.EpochClockParameter.Value = generationClock;
      generationClock.IntervalSize = 150;

      problem.Grammar.GetSymbol("Exponential and Logarithmic Functions").Enabled = false;
      problem.MaximumTreeDepth = 35;
      problem.MaximumTreeLength = 70;

      problem.OptimizeParameters = true;

      alg.PopulationSize.Value = 200;
      
      //alg.Selector = alg.SelectorParameter.ValidValues.Single(s => s.Name == "ProportionalSelector");
      var selector = alg.SelectorParameter.ValidValues.OfType<TournamentSelector>().Single();
      selector.GroupSizeParameter.Value = new IntValue(3);
      alg.Selector = selector;

      alg.Crossover = alg.CrossoverParameter.ValidValues.Single(c => c.Name == "SubtreeSwappingCrossover");
      
      alg.MutationProbability.Value = 0.15;
      alg.Mutator = alg.MutatorParameter.ValidValues.Single(m => m.Name == "MultiSymbolicExpressionTreeManipulator");

      alg.Analyzer.AddOperator(new SymbolicDataAnalysisVariableFrequencyAnalyzer());
      problem.ImpactFactorRepetitionsBestSolutionParameter.Value.Value = 5;
      problem.ImpactFactorRepetitionsPopulationParameter.Value.Value = 1;

      alg.MaximumGenerations.Value = 1_500;

      ContentManager.Save(alg, @"C:\Users\P41107\Desktop\Friedman_Test_3_0.00_GA.hl", compressed: true);
    }
    
    [Fact]
    [Trait("Generate", "Experiment")]
    void GenerateExperiment_Friedman_Test_6_0_00GA() {
      ContentManager.Initialize(new PersistenceContentManager());
      
      var alg = new GeneticAlgorithm();
      var problem = new DynamicSymbolicRegressionProblem();
      alg.Problem = problem;

      var csvProvider = new DynSymRegCsvInstanceProvider();
      var problemData = csvProvider.ImportData(@"C:\Users\P41107\Desktop\Friedman_Test_6_0.00.csv");
      problemData.PartitionsUpdate = PartitionsUpdateMode.KeepLast;
      problem.Load(problemData);

      var generationClock = problem.EpochClockParameter.ValidValues.OfType<CountingClock>().Single(c => c.Name == "Generation Counting Clock");
      problem.EpochClockParameter.Value = generationClock;
      generationClock.IntervalSize = 200;

      problem.Grammar.GetSymbol("Exponential and Logarithmic Functions").Enabled = false;
      problem.MaximumTreeDepth = 50;
      problem.MaximumTreeLength = 100;

      problem.OptimizeParameters = true;

      alg.PopulationSize.Value = 500;
      
      //alg.Selector = alg.SelectorParameter.ValidValues.Single(s => s.Name == "ProportionalSelector");
      var selector = alg.SelectorParameter.ValidValues.OfType<TournamentSelector>().Single();
      selector.GroupSizeParameter.Value = new IntValue(4);
      alg.Selector = selector;

      alg.Crossover = alg.CrossoverParameter.ValidValues.Single(c => c.Name == "SubtreeSwappingCrossover");
      
      alg.MutationProbability.Value = 0.15;
      alg.Mutator = alg.MutatorParameter.ValidValues.Single(m => m.Name == "MultiSymbolicExpressionTreeManipulator");

      alg.Analyzer.AddOperator(new SymbolicDataAnalysisVariableFrequencyAnalyzer());
      problem.ImpactFactorRepetitionsBestSolutionParameter.Value.Value = 5;
      problem.ImpactFactorRepetitionsPopulationParameter.Value.Value = 1;

      alg.MaximumGenerations.Value = 2_000;

      ContentManager.Save(alg, @"C:\Users\P41107\Desktop\Friedman_Test_6_0.00_GA.hl", compressed: true);
    }
  }
}

using System.Linq;
using HeuristicLab.Algorithms.GeneticAlgorithm;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Problems.DataAnalysis.Dynamic;
using HeuristicLab.Problems.DataAnalysis.Dynamic.ProblemInstances;
using HeuristicLab.Problems.DataAnalysis.Symbolic;
using HeuristicLab.Problems.Dynamic;
using Xunit;

namespace DynamicRegressionProblemDataGenerator.ExperimentGenerators {
  public class ExperimentGenerator {

    [Fact]
    void GenerateExperiment_F1_GA() {
      ContentManager.Initialize(new PersistenceContentManager());
      
      var alg = new GeneticAlgorithm();
      var problem = new DynamicSymbolicRegressionProblem();
      alg.Problem = problem;

      var csvProvider = new DynSymRegCsvInstanceProvider();
      var problemData = csvProvider.ImportData(@"C:\Users\P41107\Desktop\F1.csv");
      problemData.PartitionsUpdate = PartitionsUpdateMode.Wrapping;
      problem.Load(problemData);

      ((CountingClock)problem.EpochClock).IntervalSize = 100;

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

      alg.MaximumGenerations.Value = 10_000;

      ContentManager.Save(alg, @"C:\Users\P41107\Desktop\F1_GA.hl", compressed: true);
    }
  }
}

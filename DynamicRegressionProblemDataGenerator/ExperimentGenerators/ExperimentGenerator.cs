using System;
using System.IO;
using System.Linq;
using HeuristicLab.Algorithms.ALPS;
using HeuristicLab.Algorithms.GeneticAlgorithm;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Optimization;
using HeuristicLab.Problems.DataAnalysis.Dynamic;
using HeuristicLab.Problems.DataAnalysis.Dynamic.ProblemInstances;
using HeuristicLab.Problems.DataAnalysis.Symbolic;
using HeuristicLab.Problems.Dynamic;
using HeuristicLab.Selection;
using Xunit;

namespace DynamicRegressionProblemDataGenerator.ExperimentGenerators {
  public class ExperimentGenerator {

    static ExperimentGenerator() {
      ContentManager.Initialize(new PersistenceContentManager());
    }
    private const string RootDirectory = @"C:\Users\P41107\Desktop";


    [Theory]
    [Trait("Generate", "Algorithm")]
    [MemberData(nameof(Generate_GA))]
    [MemberData(nameof(Generate_ALPS))]
    void GenerateAlgorithm<TAlgorithm>(string outputFile, Action<DynamicSymbolicRegressionProblem> parameterizeProblem, Func<TAlgorithm> createAlgorithm, Action<TAlgorithm> parameterizeAlgorithm)
      where TAlgorithm : IAlgorithm, IStorableContent
    {
      var alg = createAlgorithm();
      var problem = new DynamicSymbolicRegressionProblem();
      alg.Problem = problem;

      parameterizeProblem(problem);
      parameterizeAlgorithm(alg);
      
      ContentManager.Save(alg, Path.Combine(RootDirectory, outputFile), compressed: true);
    }
    public static TheoryData<string, Action<DynamicSymbolicRegressionProblem>, Func<GeneticAlgorithm>, Action<GeneticAlgorithm>> Generate_GA => new() {
      { "F1_GA.hl", Problem_F1(), () => new GeneticAlgorithm(), GA(100, ProportionalSelection(), 0.15, 700) },
      { "F2_GA.hl", Problem_F2(), () => new GeneticAlgorithm(), GA(200, TournamentSelection(3), 0.15, 700) },
      { "F3_GA.hl", Problem_F3(), () => new GeneticAlgorithm(), GA(500, TournamentSelection(4), 0.15, 700)},
      { "Friedman_Test_3_0.00_GA.hl", Problem_Friedman_Test_3_0_00(), () => new GeneticAlgorithm(), GA(200, TournamentSelection(3), 0.15, 1_500) },
      { "Friedman_Test_6_0.00_GA.hl", Problem_Friedman_Test_6_0_00(), () => new GeneticAlgorithm(), GA(500, TournamentSelection(4), 0.15, 2_000) },
    };
    public static TheoryData<string, Action<DynamicSymbolicRegressionProblem>, Func<AlpsGeneticAlgorithm>, Action<AlpsGeneticAlgorithm>> Generate_ALPS => new() {
      { "F1_ALPS.hl", Problem_F1(), () => new AlpsGeneticAlgorithm(), ALPS(100, 10, 0.15, 700) },
      { "F2_ALPS.hl", Problem_F2(), () => new AlpsGeneticAlgorithm(), ALPS(100, 10, 0.15, 700) },
      { "F3_ALPS.hl", Problem_F3(), () => new AlpsGeneticAlgorithm(), ALPS(100, 10, 0.15, 700) },
      { "Friedman_Test_3_0.00_ALPS.hl", Problem_Friedman_Test_3_0_00(), () => new AlpsGeneticAlgorithm(), ALPS(100, 10, 0.15, 1_500) },
      { "Friedman_Test_6_0.00_ALPS.hl", Problem_Friedman_Test_6_0_00(), () => new AlpsGeneticAlgorithm(), ALPS(100, 10, 0.15, 2_000) },
    };
    
    static Action<DynamicSymbolicRegressionProblem> Problem_F1() => Problem("F1.csv", GenerationalClock(1), 10, 25);
    static Action<DynamicSymbolicRegressionProblem> Problem_F2() => Problem("F2.csv", GenerationalClock(1), 20, 40);
    static Action<DynamicSymbolicRegressionProblem> Problem_F3() => Problem("F3.csv", GenerationalClock(1), 50, 70);
    
    static Action<DynamicSymbolicRegressionProblem> Problem_Friedman_Test_3_0_00() => Problem("Friedman_Test_3_0.00.csv", GenerationalClock(150), 35, 70);
    static Action<DynamicSymbolicRegressionProblem> Problem_Friedman_Test_6_0_00() => Problem("Friedman_Test_6_0.00.csv", GenerationalClock(200), 50, 100);
    

    #region Configure Problems
    static Action<DynamicSymbolicRegressionProblem> Problem(string fileName, Action<IConstrainedValueParameter<IEpochClock>> configureEpochClock, int maximumTreeDepth, int maximumTreeLength, int impactFactorRepetitionsBestSolution = 5, int impactFactorRepetitionsPopulation = 1, bool optimizeParameters = true) => (problem) => {
      var csvProvider = new DynSymRegCsvInstanceProvider();
      
      var problemData = csvProvider.ImportData(Path.Combine(RootDirectory, fileName));
      
      problemData.PartitionsUpdate = PartitionsUpdateMode.KeepLast;
      problem.Load(problemData);
      
      configureEpochClock(problem.EpochClockParameter);
      
      problem.Grammar.GetSymbol("Exponential and Logarithmic Functions").Enabled = false;
      problem.MaximumTreeDepth = maximumTreeDepth;
      problem.MaximumTreeLength = maximumTreeLength;
      
      problem.ImpactFactorRepetitionsBestSolutionParameter.Value.Value = impactFactorRepetitionsBestSolution;
      problem.ImpactFactorRepetitionsPopulationParameter.Value.Value = impactFactorRepetitionsPopulation;
      
      problem.OptimizeParameters = optimizeParameters;
    };
    
    static Action<IConstrainedValueParameter<IEpochClock>> EvaluationClock(int interval) => (clockParameter) => {
      var clock = clockParameter.ValidValues.OfType<CountingClock>().Single(c => c.Name == "Evaluation Counting Clock");
      clock.IntervalSize = interval;
      clockParameter.Value = clock;
    };
    static Action<IConstrainedValueParameter<IEpochClock>> GenerationalClock(int interval) => (clockParameter) => {
      var clock = clockParameter.ValidValues.OfType<CountingClock>().Single(c => c.Name == "Generation Counting Clock");
      clock.IntervalSize = interval;
      clockParameter.Value = clock;
    };
    #endregion
    
    
    #region Configure Algorithms
    static Action<GeneticAlgorithm> GA(int populationSize, Action<IConstrainedValueParameter<ISelector>> configureSelector, double mutationProbability, int maximumGenerations) => (alg) => {
      alg.PopulationSize.Value = populationSize;

      configureSelector(alg.SelectorParameter);

      alg.Crossover = alg.CrossoverParameter.ValidValues.Single(c => c.Name == "SubtreeSwappingCrossover");
      
      alg.MutationProbability.Value = mutationProbability;
      alg.Mutator = alg.MutatorParameter.ValidValues.Single(m => m.Name == "MultiSymbolicExpressionTreeManipulator");

      alg.Analyzer.AddOperator(new SymbolicDataAnalysisVariableFrequencyAnalyzer());
      
      alg.MaximumGenerations.Value = maximumGenerations;
    };
    
    static Action<AlpsGeneticAlgorithm> ALPS(int layerSize, int maxLayers/*, Action<IConstrainedValueParameter<ISelector>> configureSelector*/, double mutationProbability, int maximumGenerations) => (alg) => {
      alg.PopulationSize.Value = layerSize;
      alg.NumberOfLayers.Value = maxLayers;

      //configureSelector(alg.SelectorParameter);
      
      alg.MutationProbability.Value = mutationProbability;
      alg.Mutator = alg.MutatorParameter.ValidValues.Single(m => m.Name == "MultiSymbolicExpressionTreeManipulator");

      alg.Analyzer.AddOperator(new SymbolicDataAnalysisVariableFrequencyAnalyzer());

      alg.MaximumGenerations = maximumGenerations;
    };
    
    static Action<IConstrainedValueParameter<ISelector>> ProportionalSelection() => (selectorParameter) => {
      var selector = selectorParameter.ValidValues.OfType<ProportionalSelector>().Single();
      selectorParameter.Value = selector;
    };
    static Action<IConstrainedValueParameter<ISelector>> TournamentSelection(int groupSize) => (selectorParameter) => {
      var selector = selectorParameter.ValidValues.OfType<TournamentSelector>().Single();
      selector.GroupSizeParameter.Value.Value = groupSize;
      selectorParameter.Value = selector;
    };
    #endregion
  }
}

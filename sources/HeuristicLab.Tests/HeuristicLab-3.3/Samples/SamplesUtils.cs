using System;
using System.Linq;
using HeuristicLab.Algorithms.ALPS;
using HeuristicLab.Algorithms.EvolutionStrategy;
using HeuristicLab.Algorithms.GeneticAlgorithm;
using HeuristicLab.Algorithms.OffspringSelectionEvolutionStrategy;
using HeuristicLab.Algorithms.OffspringSelectionGeneticAlgorithm;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Selection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Tests {
  public static class SamplesUtils {
    public const string SamplesDirectory = @"Samples\";
    public const string SampleFileExtension = ".hl";

    public static void RunAlgorithm(IAlgorithm a) {
      Exception ex = null;
      a.ExceptionOccurred += (sender, e) => { ex = e.Value; };
      a.Prepare();
      a.Start();
      Assert.IsNull(ex);
    }

    public static double GetDoubleResult(IAlgorithm a, string resultName) {
      return ((DoubleValue)a.Results[resultName].Value).Value;
    }

    public static int GetIntResult(IAlgorithm a, string resultName) {
      return ((IntValue)a.Results[resultName].Value).Value;
    }

    public static void ConfigureEvolutionStrategyParameters<R, M, SC, SR, SM>(EvolutionStrategy es, int popSize, int children, int parentsPerChild, int maxGens, bool plusSelection)
      where R : ICrossover
      where M : IManipulator
      where SC : IStrategyParameterCreator
      where SR : IStrategyParameterCrossover
      where SM : IStrategyParameterManipulator {
      es.PopulationSize.Value = popSize;
      es.Children.Value = children;
      es.ParentsPerChild.Value = parentsPerChild;
      es.MaximumGenerations.Value = maxGens;
      es.PlusSelection.Value = false;

      es.Seed.Value = 0;
      es.SetSeedRandomly.Value = true;

      es.Recombinator = es.RecombinatorParameter.ValidValues
        .OfType<R>()
        .Single();

      es.Mutator = es.MutatorParameter.ValidValues
        .OfType<M>()
        .Single();

      es.StrategyParameterCreator = es.StrategyParameterCreatorParameter.ValidValues
        .OfType<SC>()
        .Single();
      es.StrategyParameterCrossover = es.StrategyParameterCrossoverParameter.ValidValues
        .OfType<SR>()
        .Single();
      es.StrategyParameterManipulator = es.StrategyParameterManipulatorParameter.ValidValues
        .OfType<SM>()
        .Single();
      es.Engine = new ParallelEngine.ParallelEngine();
    }

    public static void ConfigureOffspringSelectionEvolutionStrategyParameters<R, M, SC, SR, SM>(OffspringSelectionEvolutionStrategy es, int popSize,
      double successRatio, double comparisonFactor, double maxSelPres, int parentsPerChild, int maxGens, bool plusSelection)
      where R : ICrossover
      where M : IManipulator
      where SC : IStrategyParameterCreator
      where SR : IStrategyParameterCrossover
      where SM : IStrategyParameterManipulator {
      es.PopulationSize.Value = popSize;
      es.ComparisonFactor.Value = comparisonFactor;
      es.SuccessRatio.Value = successRatio;
      es.MaximumSelectionPressure.Value = maxSelPres;
      es.ParentsPerChild.Value = parentsPerChild;
      es.SelectedParents.Value = popSize * parentsPerChild;
      es.MaximumGenerations.Value = maxGens;
      es.PlusSelection.Value = false;

      es.Seed.Value = 0;
      es.SetSeedRandomly.Value = true;

      es.Recombinator = es.RecombinatorParameter.ValidValues
        .OfType<R>()
        .Single();

      es.Mutator = es.MutatorParameter.ValidValues
        .OfType<M>()
        .Single();

      es.StrategyParameterCreator = es.StrategyParameterCreatorParameter.ValidValues
        .OfType<SC>()
        .Single();
      es.StrategyParameterCrossover = es.StrategyParameterCrossoverParameter.ValidValues
        .OfType<SR>()
        .Single();
      es.StrategyParameterManipulator = es.StrategyParameterManipulatorParameter.ValidValues
        .OfType<SM>()
        .Single();
      es.Engine = new ParallelEngine.ParallelEngine();
    }

    public static void ConfigureGeneticAlgorithmParameters<S, C, M>(GeneticAlgorithm ga, int popSize, int elites, int maxGens, double mutationRate, int tournGroupSize = 0)
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
        .First();

      ga.Crossover = ga.CrossoverParameter.ValidValues
        .OfType<C>()
        .First();

      ga.Mutator = ga.MutatorParameter.ValidValues
        .OfType<M>()
        .First();

      var tSelector = ga.Selector as TournamentSelector;
      if (tSelector != null) {
        tSelector.GroupSizeParameter.Value.Value = tournGroupSize;
      }
      ga.Engine = new ParallelEngine.ParallelEngine();
    }

    public static void ConfigureOsGeneticAlgorithmParameters<S, C, M>(OffspringSelectionGeneticAlgorithm ga, int popSize, int elites, int maxGens, double mutationRate = 0.05, double maxSelPres = 100, int tournGroupSize = 0)
      where S : ISelector
      where C : ICrossover
      where M : IManipulator {
      ga.Elites.Value = elites;
      ga.MaximumGenerations.Value = maxGens;
      ga.MutationProbability.Value = mutationRate;
      ga.PopulationSize.Value = popSize;
      ga.MaximumSelectionPressure.Value = maxSelPres;
      ga.Seed.Value = 0;
      ga.SetSeedRandomly.Value = true;
      ga.ComparisonFactorLowerBound.Value = 1;
      ga.ComparisonFactorUpperBound.Value = 1;

      ga.Selector = ga.SelectorParameter.ValidValues
        .OfType<S>()
        .First();

      ga.Crossover = ga.CrossoverParameter.ValidValues
        .OfType<C>()
        .First();

      ga.Mutator = ga.MutatorParameter.ValidValues
        .OfType<M>()
        .First();

      var tSelector = ga.Selector as TournamentSelector;
      if (tSelector != null) {
        tSelector.GroupSizeParameter.Value.Value = tournGroupSize;
      }
      ga.Engine = new ParallelEngine.ParallelEngine();
    }

    public static void ConfigureIslandGeneticAlgorithmParameters<S, C, M, Mi, MiS, MiR>(IslandGeneticAlgorithm ga, int popSize, int elites, int maxGens, double mutationRate, int numberOfIslands, int migrationInterval, double migrationRate)
      where S : ISelector
      where C : ICrossover
      where M : IManipulator
      where Mi : IMigrator
      where MiS : ISelector
      where MiR : IReplacer {
      ga.Elites.Value = elites;
      ga.MaximumGenerations.Value = maxGens;
      ga.MutationProbability.Value = mutationRate;
      ga.PopulationSize.Value = popSize;
      ga.NumberOfIslands.Value = numberOfIslands;
      ga.MigrationInterval.Value = migrationInterval;
      ga.MigrationRate.Value = migrationRate;
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
      ga.Migrator = ga.MigratorParameter.ValidValues
        .OfType<Mi>()
        .Single();
      ga.EmigrantsSelector = ga.EmigrantsSelectorParameter.ValidValues
        .OfType<MiS>()
        .Single();
      ga.ImmigrationReplacer = ga.ImmigrationReplacerParameter.ValidValues
        .OfType<MiR>()
        .Single();
      ga.Engine = new ParallelEngine.ParallelEngine();
    }

    public static void ConfigureAlpsGeneticAlgorithmParameters<S, C, M>(AlpsGeneticAlgorithm ga, int numberOfLayers, int popSize, double mutationRate, int elites, bool plusSelection, AgingScheme agingScheme, int ageGap, double ageInheritance, int maxGens)
      where S : ISelector
      where C : ICrossover
      where M : IManipulator {
      ga.Seed.Value = 0;
      ga.SetSeedRandomly.Value = true;

      ga.NumberOfLayers.Value = numberOfLayers;
      ga.PopulationSize.Value = popSize;

      ga.Selector = ga.SelectorParameter.ValidValues.OfType<S>().Single();
      ga.Crossover = ga.CrossoverParameter.ValidValues.OfType<C>().Single();
      ga.Mutator = ga.MutatorParameter.ValidValues.OfType<M>().Single();
      ga.MutationProbability.Value = mutationRate;
      ga.Elites.Value = elites;
      ga.PlusSelection = plusSelection;

      ga.AgingScheme = new EnumValue<AgingScheme>(agingScheme);
      ga.AgeGap.Value = ageGap;
      ga.AgeInheritance.Value = ageInheritance;

      ga.MaximumGenerations = maxGens;

      ga.Engine = new ParallelEngine.ParallelEngine();
    }
  }
}
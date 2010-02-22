#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
 *
 * This file is part of HeuristicLab.
 *
 * HeuristicLab is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * HeuristicLab is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with HeuristicLab. If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

using System;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Collections;
using HeuristicLab.Parameters;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Evolutionary;

namespace HeuristicLab.SGA {
  /// <summary>
  /// A standard genetic algorithm.
  /// </summary>
  [Item("SGA", "A standard genetic algorithm.")]
  [Creatable("Algorithms")]
  [EmptyStorableClass]
  public sealed class SGA : EngineAlgorithm {
    public new IScope GlobalScope {
      get { return base.GlobalScope; }
    }

    public SGA()
      : base() {
//      Parameters.Add(new ValueLookupParameter<BoolData>("Maximization", "True if the problem is a maximization problem, otherwise false."));
//      Parameters.Add(new SubScopesLookupParameter<DoubleData>("Quality", "The value which represents the quality of a solution."));
      Parameters.Add(new ValueParameter<IntData>("Seed", "The random seed used to initialize the new pseudo random number generator.", new IntData(0)));
      Parameters.Add(new ValueParameter<BoolData>("SetSeedRandomly", "True if the random seed should be set to a random value, otherwise false.", new BoolData(true)));
      Parameters.Add(new ValueParameter<IntData>("PopulationSize", "The size of the population of solutions.", new IntData(100)));
      Parameters.Add(new OperatorParameter("CrossoverOperator", "The operator used to cross solutions."));
      Parameters.Add(new ValueParameter<DoubleData>("MutationProbability", "The probability that the mutation operator is applied on a solution.", new DoubleData(0.05)));
      Parameters.Add(new OperatorParameter("MutationOperator", "The operator used to mutate solutions."));
//      Parameters.Add(new ValueLookupParameter<IOperator>("SolutionEvaluator", "The operator used to evaluate solutions."));
      Parameters.Add(new ValueParameter<IntData>("Elites", "The numer of elite solutions which are kept in each generation.", new IntData(1)));
      Parameters.Add(new ValueParameter<IntData>("MaximumGenerations", "The maximum number of generations which should be processed.", new IntData(1000)));

      RandomCreator randomCreator = new RandomCreator();
      PopulationCreator populationCreator = new PopulationCreator();
      SGAOperator sgaOperator = new SGAOperator();

      randomCreator.RandomParameter.ActualName = "Random";
      randomCreator.SeedParameter.ActualName = "Seed";
      randomCreator.SeedParameter.Value = null;
      randomCreator.SetSeedRandomlyParameter.ActualName = "SetSeedRandomly";
      randomCreator.SetSeedRandomlyParameter.Value = null;
      randomCreator.Successor = populationCreator;

      populationCreator.PopulationSizeParameter.ActualName = "PopulationSize";
      populationCreator.PopulationSizeParameter.Value = null;
      populationCreator.SolutionCreatorParameter.ActualName = "SolutionCreator";
      populationCreator.SolutionEvaluatorParameter.ActualName = "SolutionEvaluator";
      populationCreator.Successor = sgaOperator;

      sgaOperator.CrossoverOperatorParameter.ActualName = "CrossoverOperator";
      sgaOperator.ElitesParameter.ActualName = "Elites";
      sgaOperator.MaximizationParameter.ActualName = "Maximization";
      sgaOperator.MaximumGenerationsParameter.ActualName = "MaximumGenerations";
      sgaOperator.MutationOperatorParameter.ActualName = "MutationOperator";
      sgaOperator.MutationProbabilityParameter.ActualName = "MutationProbability";
      sgaOperator.QualityParameter.ActualName = "Quality";
      sgaOperator.RandomParameter.ActualName = "Random";
      sgaOperator.SolutionEvaluatorParameter.ActualName = "SolutionEvaluator";

      OperatorGraph.InitialOperator = randomCreator;
    }
  }
}

#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
 
using System.Collections.Generic;
using HeuristicLab.Algorithms.MemPR.Binary;
using HeuristicLab.Algorithms.MemPR.LinearLinkage;
using HeuristicLab.Algorithms.MemPR.Permutation;
using HeuristicLab.Core;
using HeuristicLab.Encodings.BinaryVectorEncoding;
using HeuristicLab.Optimization;

namespace HeuristicLab.Algorithms.MemPR.Interfaces {

  /*************************************************
   * ********************************************* *
   *                    DATA                       *
   * ********************************************* *
   *************************************************/

  public interface ISolutionModel<TSolution> : IItem {
    TSolution Sample();
  }

  public interface ISolutionSubspace<TSolution> : IItem { }

  /*************************************************
   * ********************************************* *
   *                  OPERATORS                    *
   * ********************************************* *
   *************************************************/
   
  public interface ISolutionModelTrainer<TContext> : IItem {
    void TrainModel(TContext context);
  }

  public interface ILocalSearch<TContext> : IItem {
    void Optimize(TContext context);
  }
  
  /*************************************************
   * ********************************************* *
   *                  CONTEXTS                     *
   * ********************************************* *
   *************************************************/

  public interface IHeuristicAlgorithmContext<TProblem, TSolution> : IExecutionContext
      where TProblem : class, ISingleObjectiveProblemDefinition {
    TProblem Problem { get; }
    IRandom Random { get; }
    int Iterations { get; set; }
    int EvaluatedSolutions { get; }
    void IncrementEvaluatedSolutions(int byEvaluations);
    double BestQuality { get; set; }
    TSolution BestSolution { get; set; }
  }

  public interface IPopulationBasedHeuristicAlgorithmContext<TProblem, TSolution> : IHeuristicAlgorithmContext<TProblem, TSolution>
      where TProblem : class, ISingleObjectiveProblemDefinition {
    IEnumerable<ISingleObjectiveSolutionScope<TSolution>> Population { get; }
  }

  public interface ISingleSolutionHeuristicAlgorithmContext<TProblem, TSolution> : IHeuristicAlgorithmContext<TProblem, TSolution>
      where TProblem : class, ISingleObjectiveProblemDefinition {
    ISingleObjectiveSolutionScope<TSolution> Solution { get; }
  }

  public interface ISolutionModelContext<TSolution> : IExecutionContext {
    ISolutionModel<TSolution> Model { get; set; }
  }

  public interface ISolutionSubspaceContext<TSolution> : IExecutionContext {
    ISolutionSubspace<TSolution> Subspace { get; }
  }
  public interface IBinaryVectorSubspaceContext : ISolutionSubspaceContext<BinaryVector> {
    new BinarySolutionSubspace Subspace { get; }
  }
  public interface IPermutationSubspaceContext : ISolutionSubspaceContext<Encodings.PermutationEncoding.Permutation> {
    new PermutationSolutionSubspace Subspace { get; }
  }
  public interface ILinearLinkageSubspaceContext : ISolutionSubspaceContext<Encodings.LinearLinkageEncoding.LinearLinkage> {
    new LinearLinkageSolutionSubspace Subspace { get; }
  }


  /*************************************************
   * ********************************************* *
   *                   SCOPES                      *
   * ********************************************* *
   *************************************************/

  public interface ISingleObjectiveSolutionScope<TSolution> : IScope {
    TSolution Solution { get; set; }
    double Fitness { get; set; }

    void Adopt(ISingleObjectiveSolutionScope<TSolution> orphan);
  }
}

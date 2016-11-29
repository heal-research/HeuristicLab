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

using System;
using System.Collections.Generic;
using HeuristicLab.Algorithms.MemPR.Binary;
using HeuristicLab.Core;
using HeuristicLab.Encodings.BinaryVectorEncoding;

/*************************************************
 * ********************************************* *
 *                    DATA                       *
 * ********************************************* *
 *************************************************/

namespace HeuristicLab.Optimization.SolutionModel {

  public interface ISolutionModel<TSolution> : IItem {
    TSolution Sample();
  }
}

namespace HeuristicLab.Optimization {

  public interface ISolutionSubspace : IItem { }
}

/*************************************************
   * ********************************************* *
   *                  OPERATORS                    *
   * ********************************************* *
   *************************************************/

namespace HeuristicLab.Optimization.SolutionModel {

  public interface ISolutionModelTrainer<TSolution> : IOperator
      where TSolution : class, IItem {
    IScopeTreeLookupParameter<TSolution> SolutionParameter { get; }
    ILookupParameter<ISolutionModel<TSolution>> SamplingModelParameter { get; }
  }

  public interface ISolutionModelTrainer<TSolution, TContext> : ISolutionModelTrainer<TSolution>
      where TSolution : class, IItem
      where TContext : ISolutionModelContext<TSolution> {

    void TrainModel(TContext context);
  }

  public interface IBinarySolutionModelTrainer<TContext> : ISolutionModelTrainer<BinaryVector, TContext>
      where TContext : ISolutionModelContext<BinaryVector> { }
}

namespace HeuristicLab.Optimization.LocalSearch {

  public interface ILocalSearch<TSolution> : IOperator
      where TSolution : class, IItem {
    ILookupParameter<TSolution> SolutionParameter { get; }

    Func<TSolution, double> EvaluateFunc { get; set; }
  }

  public interface ILocalSearch<TSolution, TContext> : ILocalSearch<TSolution>
      where TSolution : class, IItem {

    void Optimize(TContext context);
  }

  public interface IBinaryLocalSearch<TContext> : ILocalSearch<BinaryVector, TContext> {
    
  }

}

/*************************************************
   * ********************************************* *
   *                  CONTEXTS                     *
   * ********************************************* *
   *************************************************/

namespace HeuristicLab.Optimization {
  
  public interface IStochasticContext : IExecutionContext {
    IRandom Random { get; }
  }

  public interface IMaximizationContext : IExecutionContext {
    bool Maximization { get; }
  }

  public interface IEvaluatedSolutionsContext : IExecutionContext {
    int EvaluatedSolutions { get; set; }
  }

  public interface IBestQualityContext : IExecutionContext {
    double BestQuality { get; set; }
  }

  public interface IBestSolutionContext<TSolution> : IExecutionContext {
    TSolution BestSolution { get; set; }
  }

  public interface IIterationsContext : IExecutionContext {
    int Iterations { get; }
  }

  public interface IIterationsManipulationContext : IIterationsContext {
    new int Iterations { get; set; }
  }

  public interface IPopulationContext : IExecutionContext {
    IEnumerable<IScope> Population { get; }
  }

  public interface IPopulationContext<TSolution> : IPopulationContext {
    new IEnumerable<ISolutionScope<TSolution>> Population { get; }
  }

  public interface ISingleObjectivePopulationContext<TSolution> : IPopulationContext<TSolution> {
    new IEnumerable<ISingleObjectiveSolutionScope<TSolution>> Population { get; }
  }

  public interface ISolutionContext : IExecutionContext {
    IScope Solution { get; }
  }

  public interface ISolutionContext<TSolution> : ISolutionContext {
    new ISolutionScope<TSolution> Solution { get; }
  }

  public interface ISingleObjectiveSolutionContext<TSolution> : ISolutionContext<TSolution> {
    new ISingleObjectiveSolutionScope<TSolution> Solution { get; }
  }

  /* Probably a setter is not needed anyway, since there is Adopt() also
  public interface ISolutionManipulationContext : ISolutionContext {
    new IScope Solution { get; set; }
  }

  public interface ISolutionManipulationContext<TSolution> : ISolutionManipulationContext, ISolutionContext<TSolution> {
    new ISolutionScope<TSolution> Solution { get; set; }
  }

  public interface ISingleObjectiveSolutionManipulationContext<TSolution> : ISolutionManipulationContext<TSolution>, ISingleObjectiveSolutionContext<TSolution> {
    new ISingleObjectiveSolutionScope<TSolution> Solution { get; set; }
  }
  */

  public interface ISolutionSubspaceContext : IExecutionContext {
    ISolutionSubspace Subspace { get; }
  }
  public interface IBinarySolutionSubspaceContext : ISolutionSubspaceContext {
    new BinarySolutionSubspace Subspace { get; }
  }
}

namespace HeuristicLab.Optimization.SolutionModel {
  public interface ISolutionModelContext<TSolution> : IExecutionContext {
    ISolutionModel<TSolution> Model { get; set; }
  }
}


/*************************************************
   * ********************************************* *
   *                   SCOPES                      *
   * ********************************************* *
   *************************************************/
  
namespace HeuristicLab.Core {
  public interface ISolutionScope<TSolution> : IScope {
    TSolution Solution { get; set; }

    void Adopt(ISolutionScope<TSolution> orphan);
  }

  public interface ISingleObjectiveSolutionScope<TSolution> : ISolutionScope<TSolution> {
    double Fitness { get; set; }

    void Adopt(ISingleObjectiveSolutionScope<TSolution> orphan);
  }
}

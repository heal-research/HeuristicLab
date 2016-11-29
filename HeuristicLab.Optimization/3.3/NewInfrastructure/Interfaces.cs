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
using System.Threading;
using HeuristicLab.Core;
using HeuristicLab.Optimization;

/*************************************************
 * ********************************************* *
 *                    DATA                       *
 * ********************************************* *
 *************************************************/



/*************************************************
   * ********************************************* *
   *                  OPERATORS                    *
   * ********************************************* *
   *************************************************/


namespace HeuristicLab.Optimization.LocalSearch {
  public interface ILocalSearch<TContext> : IItem {
    void Optimize(TContext context);
  }
}

namespace HeuristicLab.Optimization.Crossover {
  public interface ICrossover<TContext> : IItem {

    void Cross(TContext context);
  }
}

namespace HeuristicLab.Optimization.Manipulation {
  public interface IManipulator<TContext> : IItem {
    void Manipulate(TContext context);
  }
}

namespace HeuristicLab.Optimization.Selection {
  public interface ISelector<TContext> : IItem {

    void Select(TContext context, int n, bool withRepetition);
  }
}

/*************************************************
   * ********************************************* *
   *                  CONTEXTS                     *
   * ********************************************* *
   *************************************************/

namespace HeuristicLab.Optimization {

  public interface ILongRunningOperationContext : IExecutionContext {
    CancellationToken CancellationToken { get; set; }
  }

  public interface IStochasticContext : IExecutionContext {
    IRandom Random { get; }
  }

  public interface IEvaluatedSolutionsContext : IExecutionContext {
    int EvaluatedSolutions { get; }
    void IncEvaluatedSolutions(int inc);
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

  public interface IImprovementStepsContext : IExecutionContext {
    int ImprovementSteps { get; set; }
  }

  public interface IProblemContext<TProblem, TEncoding, TSolution> : IExecutionContext
      where TProblem : class, ISingleObjectiveProblem<TEncoding, TSolution>
      where TEncoding : class, IEncoding<TSolution>
      where TSolution : class, ISolution {
    TProblem Problem { get; }
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

  public interface IMatingContext<TSolution> : IExecutionContext {
    Tuple<ISolutionScope<TSolution>, ISolutionScope<TSolution>>  Parents { get; }
    ISolutionScope<TSolution> Child { get; }
  }

  public interface ISingleObjectiveMatingContext<TSolution> : IMatingContext<TSolution> {
    new Tuple<ISingleObjectiveSolutionScope<TSolution>, ISingleObjectiveSolutionScope<TSolution>> Parents { get; }
    new ISingleObjectiveSolutionScope<TSolution> Child { get; }
  }

  public interface IMatingpoolContext<TSolution> : IExecutionContext
      where TSolution : class, ISolution {
    IEnumerable<ISolutionScope<TSolution>> MatingPool { get; set; }
  }

  public interface ISingleObjectiveMatingpoolContext<TSolution> : IMatingpoolContext<TSolution>
      where TSolution : class, ISolution {
    new IEnumerable<ISingleObjectiveSolutionScope<TSolution>> MatingPool { get; set; }
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

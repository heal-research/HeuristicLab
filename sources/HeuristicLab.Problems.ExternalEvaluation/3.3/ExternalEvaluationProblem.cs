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

using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Parameters;
using System.Drawing;
using HeuristicLab.Optimization;
using HeuristicLab.Data;
using HeuristicLab.Operators;

namespace HeuristicLab.Problems.ExternalEvaluation {
  [Item("External Evaluation Problem", "A problem that is evaluated in a different process.")]
  [StorableClass]
  public class ExternalEvaluationProblem : ParameterizedNamedItem, ISingleObjectiveProblem {
    public override Image ItemImage {
      get { return HeuristicLab.Common.Resources.VS2008ImageLibrary.Type; }
    }

    #region Parameters
    private IValueParameter<IExternalEvaluationDriver> DriverParameter {
      get { return (IValueParameter<IExternalEvaluationDriver>)Parameters["Driver"]; }
    }
    private IValueParameter<IExternalEvaluationProblemEvaluator> EvaluatorParameter {
      get { return (IValueParameter<IExternalEvaluationProblemEvaluator>)Parameters["Evaluator"]; }
    }
    public ValueParameter<BoolValue> MaximizationParameter {
      get { return (ValueParameter<BoolValue>)Parameters["Maximization"]; }
    }
    IParameter ISingleObjectiveProblem.MaximizationParameter {
      get { return MaximizationParameter; }
    }
    public ValueParameter<IOperator> SolutionCreatorParameter {
      get { return (ValueParameter<IOperator>)Parameters["SolutionCreator"]; }
    }
    IParameter IProblem.SolutionCreatorParameter {
      get { return SolutionCreatorParameter; }
    }
    IParameter IProblem.EvaluatorParameter {
      get { return EvaluatorParameter; }
    }
    public OptionalValueParameter<DoubleValue> BestKnownQualityParameter {
      get { return (OptionalValueParameter<DoubleValue>)Parameters["BestKnownQuality"]; }
    }
    IParameter ISingleObjectiveProblem.BestKnownQualityParameter {
      get { return BestKnownQualityParameter; }
    }
    public OptionalValueParameter<IScope> BestKnownSolutionParameter {
      get { return (OptionalValueParameter<IScope>)Parameters["BestKnownSolution"]; }
    }
    #endregion

    public ExternalEvaluationProblem()
      : base() {
      ExternalEvaluator evaluator = new ExternalEvaluator();
      EmptyOperator solutionCreator = new EmptyOperator();

      Parameters.Add(new ValueParameter<IExternalEvaluationDriver>("Driver", "The communication driver that is used to exchange data with the external process."));
      Parameters.Add(new ValueParameter<IExternalEvaluationProblemEvaluator>("Evaluator", "The evaluator that collects the values to exchange.", evaluator));
      Parameters.Add(new ValueParameter<IOperator>("SolutionCreator", "An operator to create the solution components.", solutionCreator));
    }
  }
}

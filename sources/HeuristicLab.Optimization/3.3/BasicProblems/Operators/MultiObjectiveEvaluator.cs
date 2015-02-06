#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Optimization {
  [Item("Multi-objective Evaluator", "Calls the Evaluate method of the problem definition and writes the return value into the scope.")]
  [StorableClass]
  public class MultiObjectiveEvaluator : SingleSuccessorOperator, IMultiObjectiveEvaluationOperator, IStochasticOperator {

    public ILookupParameter<IRandom> RandomParameter {
      get { return (ILookupParameter<IRandom>)Parameters["Random"]; }
    }

    public ILookupParameter<IEncoding> EncodingParameter {
      get { return (ILookupParameter<IEncoding>)Parameters["Encoding"]; }
    }

    public ILookupParameter<DoubleArray> QualitiesParameter {
      get { return (ILookupParameter<DoubleArray>)Parameters["Qualities"]; }
    }

    public Func<Individual, IRandom, double[]> EvaluateFunc { get; set; }

    [StorableConstructor]
    protected MultiObjectiveEvaluator(bool deserializing) : base(deserializing) { }
    protected MultiObjectiveEvaluator(MultiObjectiveEvaluator original, Cloner cloner) : base(original, cloner) { }
    public MultiObjectiveEvaluator() {
      Parameters.Add(new LookupParameter<IRandom>("Random", "The random number generator to use."));
      Parameters.Add(new LookupParameter<IEncoding>("Encoding", "An item that holds the problem's encoding."));
      Parameters.Add(new LookupParameter<DoubleArray>("Qualities", "The qualities of the parameter vector."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new MultiObjectiveEvaluator(this, cloner);
    }

    public override IOperation Apply() {
      var random = RandomParameter.ActualValue;
      var encoding = EncodingParameter.ActualValue;
      var individual = encoding.GetIndividual(ExecutionContext.Scope);
      QualitiesParameter.ActualValue = new DoubleArray(EvaluateFunc(individual, random));
      return base.Apply();
    }
  }
}

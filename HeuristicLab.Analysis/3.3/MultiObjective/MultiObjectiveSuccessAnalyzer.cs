#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HEAL.Attic;


namespace HeuristicLab.Analysis {
  [StorableType("3659291a-927c-4016-88be-8a0f65476660")]
  public abstract class MultiObjectiveSuccessAnalyzer : SingleSuccessorOperator, IAnalyzer, IMultiObjectiveOperator {
    public virtual bool EnabledByDefault {
      get { return true; }
    }
    public abstract string ResultName { get; }

    public IScopeTreeLookupParameter<DoubleArray> QualitiesParameter {
      get { return (IScopeTreeLookupParameter<DoubleArray>)Parameters["Qualities"]; }
    }

    public ILookupParameter<BoolArray> MaximizationParameter {
      get { return (ILookupParameter<BoolArray>)Parameters["Maximization"]; }
    }

    public ResultParameter<DoubleValue> ResultParameter {
      get { return (ResultParameter<DoubleValue>)Parameters[ResultName]; }
    }

    protected MultiObjectiveSuccessAnalyzer(MultiObjectiveSuccessAnalyzer original, Cloner cloner) : base(original, cloner) { }
    [StorableConstructor]
    protected MultiObjectiveSuccessAnalyzer(StorableConstructorFlag _) : base(_) { }
    protected MultiObjectiveSuccessAnalyzer() {
      Parameters.Add(new ScopeTreeLookupParameter<DoubleArray>("Qualities", "The qualities of the parameter vector."));
      Parameters.Add(new LookupParameter<DoubleMatrix>("BestKnownFront", "The currently best known Pareto front"));
      Parameters.Add(new LookupParameter<BoolArray>("Maximization", ""));
    }
  }
}
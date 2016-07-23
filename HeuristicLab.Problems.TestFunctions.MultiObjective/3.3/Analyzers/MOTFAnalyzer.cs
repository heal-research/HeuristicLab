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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;


namespace HeuristicLab.Problems.TestFunctions.MultiObjective {

  [StorableClass]
  public abstract class MOTFAnalyzer : SingleSuccessorOperator, IMultiObjectiveTestFunctionAnalyzer {
    public virtual bool EnabledByDefault { get { return true; } }

    public IScopeTreeLookupParameter<DoubleArray> QualitiesParameter {
      get { return (IScopeTreeLookupParameter<DoubleArray>)Parameters["Qualities"]; }
    }

    public ILookupParameter<ResultCollection> ResultsParameter {
      get { return (ILookupParameter<ResultCollection>)Parameters["Results"]; }
    }

    public ILookupParameter<IMultiObjectiveTestFunction> TestFunctionParameter {
      get { return (ILookupParameter<IMultiObjectiveTestFunction>)Parameters["TestFunction"]; }
    }

    public ILookupParameter<DoubleMatrix> BestKnownFrontParameter {
      get { return (ILookupParameter<DoubleMatrix>)Parameters["BestKnownFront"]; }
    }

    protected MOTFAnalyzer(MOTFAnalyzer original, Cloner cloner) : base(original, cloner) { }

    [StorableConstructor]
    protected MOTFAnalyzer(bool deserializing) : base(deserializing) { }
    protected MOTFAnalyzer() {
      Parameters.Add(new ScopeTreeLookupParameter<DoubleArray>("Qualities", "The qualities of the parameter vector."));
      Parameters.Add(new LookupParameter<ResultCollection>("Results", "The results collection to write to."));
      Parameters.Add(new LookupParameter<IMultiObjectiveTestFunction>("TestFunction", "The Testfunction that is analyzed"));
      Parameters.Add(new LookupParameter<DoubleMatrix>("BestKnownFront", "The currently best known Pareto front"));
    }
  }
}

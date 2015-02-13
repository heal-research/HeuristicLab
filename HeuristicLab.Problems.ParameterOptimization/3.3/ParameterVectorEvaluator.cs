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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.RealVectorEncoding;
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.ParameterOptimization {
  [Item("ParameterVectorEvaluator", "An base class for other parameter vector evaluators.")]
  [StorableClass]
  public abstract class ParameterVectorEvaluator : SingleSuccessorOperator, IParameterVectorEvaluator {
    private const string QualityParameterName = "Quality";
    private const string ProblemSizeParameterName = "ProblemSize";
    private const string ParameterVectorParameterName = "RealVector";
    private const string ParameterNamesParameterName = "ParameterNames";

    #region parameters
    public ILookupParameter<DoubleValue> QualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters[QualityParameterName]; }
    }
    public ILookupParameter<IntValue> ProblemSizeParameter {
      get { return (ILookupParameter<IntValue>)Parameters[ProblemSizeParameterName]; }
    }
    public ILookupParameter<RealVector> ParameterVectorParameter {
      get { return (ILookupParameter<RealVector>)Parameters[ParameterVectorParameterName]; }
    }
    public ILookupParameter<StringArray> ParameterNamesParameter {
      get { return (ILookupParameter<StringArray>)Parameters[ParameterNamesParameterName]; }
    }
    #endregion


    [StorableConstructor]
    protected ParameterVectorEvaluator(bool deserializing) : base(deserializing) { }
    protected ParameterVectorEvaluator(ParameterVectorEvaluator original, Cloner cloner)
      : base(original, cloner) {
    }

    protected ParameterVectorEvaluator()
      : base() {
      Parameters.Add(new LookupParameter<DoubleValue>(QualityParameterName, "Result of the evaluation of a parameter vector."));
      Parameters.Add(new LookupParameter<IntValue>(ProblemSizeParameterName, "The dimension of the parameter vector that is to be optimized."));
      Parameters.Add(new LookupParameter<RealVector>(ParameterVectorParameterName, "The parameter vector which should be evaluated."));
      Parameters.Add(new LookupParameter<StringArray>(ParameterNamesParameterName, "The name of the parameters that should be optimized."));
    }
  }
}

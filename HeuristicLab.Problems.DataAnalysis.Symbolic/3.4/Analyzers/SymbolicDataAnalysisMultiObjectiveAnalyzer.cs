#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Optimization;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  /// <summary>
  /// Abstract base class for multi objective symbolic data analysis analyzers.
  /// </summary>
  [StorableClass]
  public abstract class SymbolicDataAnalysisMultiObjectiveAnalyzer : SymbolicDataAnalysisAnalyzer, ISymbolicDataAnalysisMultiObjectiveAnalyzer {
    private const string QualitiesParameterName = "Qualities";
    private const string MaximizationParameterName = "Maximization";
    #region parameter properties
    public IScopeTreeLookupParameter<DoubleArray> QualitiesParameter {
      get { return (IScopeTreeLookupParameter<DoubleArray>)Parameters[QualitiesParameterName]; }
    }
    public ILookupParameter<BoolArray> MaximizationParameter {
      get { return (ILookupParameter<BoolArray>)Parameters[MaximizationParameterName]; }
    }
    #endregion
    #region properties
    public ItemArray<DoubleArray> Qualities {
      get { return QualitiesParameter.ActualValue; }
    }
    public BoolArray Maximization {
      get { return MaximizationParameter.ActualValue; }
    }
    #endregion
    [StorableConstructor]
    protected SymbolicDataAnalysisMultiObjectiveAnalyzer(bool deserializing) : base(deserializing) { }
    protected SymbolicDataAnalysisMultiObjectiveAnalyzer(SymbolicDataAnalysisMultiObjectiveAnalyzer original, Cloner cloner)
      : base(original, cloner) {
    }
    public SymbolicDataAnalysisMultiObjectiveAnalyzer()
      : base() {
      Parameters.Add(new ScopeTreeLookupParameter<DoubleArray>(QualitiesParameterName, "The qualities of the trees that should be analyzed."));
      Parameters.Add(new LookupParameter<BoolArray>(MaximizationParameterName, "The directions of optimization for each dimension."));
    }
  }
}

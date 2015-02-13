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
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  /// <summary>
  /// Abstract base class for symbolic data analysis analyzers.
  /// </summary>
  [StorableClass]
  public abstract class SymbolicDataAnalysisAnalyzer : SingleSuccessorOperator, ISymbolicDataAnalysisAnalyzer {
    private const string SymbolicExpressionTreeParameterName = "SymbolicExpressionTree";
    private const string ResultCollectionParameterName = "Results";

    #region parameter properties
    public IScopeTreeLookupParameter<ISymbolicExpressionTree> SymbolicExpressionTreeParameter {
      get { return (IScopeTreeLookupParameter<ISymbolicExpressionTree>)Parameters[SymbolicExpressionTreeParameterName]; }
    }
    public ILookupParameter<ResultCollection> ResultCollectionParameter {
      get { return (ILookupParameter<ResultCollection>)Parameters[ResultCollectionParameterName]; }
    }
    #endregion
    #region properties
    public virtual bool EnabledByDefault {
      get { return true; }
    }
    public ItemArray<ISymbolicExpressionTree> SymbolicExpressionTree {
      get { return SymbolicExpressionTreeParameter.ActualValue; }
    }
    public ResultCollection ResultCollection {
      get { return ResultCollectionParameter.ActualValue; }
    }
    #endregion
    [StorableConstructor]
    protected SymbolicDataAnalysisAnalyzer(bool deserializing) : base(deserializing) { }
    protected SymbolicDataAnalysisAnalyzer(SymbolicDataAnalysisAnalyzer original, Cloner cloner)
      : base(original, cloner) {
    }
    public SymbolicDataAnalysisAnalyzer()
      : base() {
      Parameters.Add(new ScopeTreeLookupParameter<ISymbolicExpressionTree>(SymbolicExpressionTreeParameterName, "The symbolic expression trees that should be analyzed."));
      Parameters.Add(new LookupParameter<ResultCollection>(ResultCollectionParameterName, "The result collection to store the analysis results."));
    }
  }
}

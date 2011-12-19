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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  public abstract class SymbolicDataAnalysisMultiObjectiveEvaluator<T> : SymbolicDataAnalysisEvaluator<T>, ISymbolicDataAnalysisMultiObjectiveEvaluator<T>
   where T : class, IDataAnalysisProblemData {
    private const string QualitiesParameterName = "Qualities";
    public ILookupParameter<DoubleArray> QualitiesParameter {
      get { return (ILookupParameter<DoubleArray>)Parameters[QualitiesParameterName]; }
    }

    public abstract IEnumerable<bool> Maximization { get; }

    [StorableConstructor]
    protected SymbolicDataAnalysisMultiObjectiveEvaluator(bool deserializing) : base(deserializing) { }
    protected SymbolicDataAnalysisMultiObjectiveEvaluator(SymbolicDataAnalysisMultiObjectiveEvaluator<T> original, Cloner cloner)
      : base(original, cloner) {
    }

    protected SymbolicDataAnalysisMultiObjectiveEvaluator()
      : base() {
      Parameters.Add(new LookupParameter<DoubleArray>(QualitiesParameterName, "The qualities of the evaluated symbolic data analysis solution."));
    }

    public abstract double[] Evaluate(IExecutionContext context, ISymbolicExpressionTree tree, T problemData, IEnumerable<int> rows);
  }
}

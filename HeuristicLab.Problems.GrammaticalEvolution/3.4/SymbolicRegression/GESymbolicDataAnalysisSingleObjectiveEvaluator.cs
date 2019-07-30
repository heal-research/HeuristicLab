#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
 * 
 * Author: Sabine Winkler
 */

#endregion

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HEAL.Attic;
using HeuristicLab.Problems.DataAnalysis;

namespace HeuristicLab.Problems.GrammaticalEvolution {
  [StorableType("BEC10226-1F0C-4D42-ABDF-38E604C0B2F2")]
  public abstract class GESymbolicDataAnalysisSingleObjectiveEvaluator<T> : GESymbolicDataAnalysisEvaluator<T>, IGESymbolicDataAnalysisSingleObjectiveEvaluator<T>
   where T : class, IDataAnalysisProblemData {
    private const string QualityParameterName = "Quality";
    #region parameter properties
    public ILookupParameter<DoubleValue> QualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters[QualityParameterName]; }
    }
    #endregion
    #region properties
    public abstract bool Maximization { get; }
    #endregion
    [StorableConstructor]
    protected GESymbolicDataAnalysisSingleObjectiveEvaluator(StorableConstructorFlag _) : base(_) { }
    protected GESymbolicDataAnalysisSingleObjectiveEvaluator(GESymbolicDataAnalysisSingleObjectiveEvaluator<T> original, Cloner cloner)
      : base(original, cloner) {
    }

    protected GESymbolicDataAnalysisSingleObjectiveEvaluator()
      : base() {
      Parameters.Add(new LookupParameter<DoubleValue>(QualityParameterName, "The quality of the evaluated symbolic data analysis solution."));
    }
  }
}

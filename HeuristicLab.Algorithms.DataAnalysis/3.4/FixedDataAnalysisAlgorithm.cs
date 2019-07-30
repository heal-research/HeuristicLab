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
 */
#endregion

using System;
using HeuristicLab.Common;
using HeuristicLab.Optimization;
using HEAL.Attic;
using HeuristicLab.Problems.DataAnalysis;

namespace HeuristicLab.Algorithms.DataAnalysis {
  [StorableType("E7BEE142-B6F1-4293-AC2A-B19EB163AB2E")]
  public abstract class FixedDataAnalysisAlgorithm<T> : BasicAlgorithm, IDataAnalysisAlgorithm<T> where T : class, IDataAnalysisProblem {
    #region Properties
    public override Type ProblemType {
      get { return typeof(T); }
    }
    public new T Problem {
      get { return (T)base.Problem; }
      set { base.Problem = value; }
    }
    #endregion

    public override bool SupportsPause { get { return false; } }

    [StorableConstructor]
    protected FixedDataAnalysisAlgorithm(StorableConstructorFlag _) : base(_) { }
    protected FixedDataAnalysisAlgorithm(FixedDataAnalysisAlgorithm<T> original, Cloner cloner) : base(original, cloner) { }
    public FixedDataAnalysisAlgorithm() : base() { }

  }
}

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

using System;
using HeuristicLab.Common;
using HeuristicLab.Optimization;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.DataAnalysis;

namespace HeuristicLab.Algorithms.DataAnalysis {
  [StorableClass]
  public abstract class FixedDataAnalysisAlgorithm<T> : BasicAlgorithm where T : class, IDataAnalysisProblem {
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
    protected FixedDataAnalysisAlgorithm(bool deserializing) : base(deserializing) { }
    protected FixedDataAnalysisAlgorithm(FixedDataAnalysisAlgorithm<T> original, Cloner cloner) : base(original, cloner) { }
    public FixedDataAnalysisAlgorithm() : base() { }

  }
}

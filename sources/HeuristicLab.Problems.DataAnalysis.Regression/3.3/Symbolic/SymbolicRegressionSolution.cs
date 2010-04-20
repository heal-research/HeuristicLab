#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using System.Collections.Generic;
using System.Linq;

namespace HeuristicLab.Problems.DataAnalysis.Regression.Symbolic {
  /// <summary>
  /// Represents a solution for a symbolic regression problem which can be visualized in the GUI.
  /// </summary>
  [Item("SymbolicRegressionSolution", "Represents a solution for a symbolic regression problem which can be visualized in the GUI.")]
  [StorableClass]
  public sealed class SymbolicRegressionSolution : DataAnalysisSolution {
    public new SymbolicRegressionModel Model {
      get { return (SymbolicRegressionModel)base.Model; }
      set { base.Model = value; }
    }

    public SymbolicRegressionSolution() : base() { }
    public SymbolicRegressionSolution(DataAnalysisProblemData problemData, SymbolicRegressionModel model)
      : base(problemData, model) {
    }
  }
}

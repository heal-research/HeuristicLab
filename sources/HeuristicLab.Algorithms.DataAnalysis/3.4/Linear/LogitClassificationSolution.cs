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

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.DataAnalysis;

namespace HeuristicLab.Algorithms.DataAnalysis {
  /// <summary>
  /// Represents a multinomial logit solution for a classification problem which can be visualized in the GUI.
  /// </summary>
  [Item("LogitClassificationSolution", "Represents a multinomial logit solution for a classification problem which can be visualized in the GUI.")]
  [StorableClass]
  public sealed class LogitClassificationSolution : ClassificationSolution {

    public new LogitModel Model {
      get { return (LogitModel)base.Model; }
      set { base.Model = value; }
    }

    [StorableConstructor]
    private LogitClassificationSolution(bool deserializing) : base(deserializing) { }
    private LogitClassificationSolution(LogitClassificationSolution original, Cloner cloner)
      : base(original, cloner) {
    }
    public LogitClassificationSolution(IClassificationProblemData problemData, LogitModel logitModel)
      : base(logitModel, problemData) {
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new LogitClassificationSolution(this, cloner);
    }
  }
}

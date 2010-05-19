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

using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Parameters;

namespace HeuristicLab.Problems.ExternalEvaluation {
  [Item("External Evaluation Problem", "A problem that is evaluated in a different process.")]
  [StorableClass]
  public class ExternalEvaluationProblem : ParameterizedNamedItem {
    #region Parameters
    #endregion

    public ExternalEvaluationProblem()
      : base() {
      Parameters.Add(new ValueParameter<IExternalEvaluationDriver>("Driver", "The communication driver that is used to exchange data with the external process."));
    }
  }
}

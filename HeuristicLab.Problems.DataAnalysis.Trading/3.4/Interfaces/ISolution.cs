#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HEAL.Fossil;

namespace HeuristicLab.Problems.DataAnalysis.Trading {
  [StorableType("f0f78fec-6361-4032-b6d1-e36fbbf63381")]
  public interface ISolution : IDataAnalysisSolution {
    new IModel Model { get; }
    new IProblemData ProblemData { get; }

    IEnumerable<double> Signals { get; }
    IEnumerable<double> TrainingSignals { get; }
    IEnumerable<double> TestSignals { get; }
    IEnumerable<double> GetSignals(IEnumerable<int> rows);

    double TrainingSharpeRatio { get; }
    double TestSharpeRatio { get; }
  }
}

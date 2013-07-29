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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.DataAnalysis {
  [StorableClass]
  [Item("Trading Problem", "A general trading problem.")]
  [Creatable("Problems")]
  public class TradingProblem : DataAnalysisProblem<ITradingProblemData>, ITradingProblem {
    [StorableConstructor]
    protected TradingProblem(bool deserializing) : base(deserializing) { }
    protected TradingProblem(TradingProblem original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) { 
      return new TradingProblem(this, cloner); }

    public TradingProblem()
      : base() {
      ProblemData = new TradingProblemData();
    }

    public override void ImportProblemDataFromFile(string fileName) {
      TradingProblemData problemData = TradingProblemData.ImportFromFile(fileName);
      ProblemData = problemData;
    }
  }
}

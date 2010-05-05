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

using System.Linq;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Data;
using HeuristicLab.Core;

namespace HeuristicLab.Algorithms.TabuSearch {
  public class TabuNeighborhoodAnalyzer : SingleSuccessorOperator, IAnalyzer {
    public SubScopesLookupParameter<BoolValue> IsTabuParameter {
      get { return (SubScopesLookupParameter<BoolValue>)Parameters["IsTabu"]; }
    }
    public LookupParameter<PercentValue> PercentTabuParameter {
      get { return (LookupParameter<PercentValue>)Parameters["PercentTabu"]; }
    }
    public LookupParameter<ResultCollection> ResultsParameter {
      get { return (LookupParameter<ResultCollection>)Parameters["Results"]; }
    }

    public TabuNeighborhoodAnalyzer()
      : base() {
      Parameters.Add(new SubScopesLookupParameter<BoolValue>("IsTabu", "A value that determines if a move is tabu or not."));
      Parameters.Add(new LookupParameter<PercentValue>("PercentTabu", "Indicates how much of the neighborhood is tabu."));
      Parameters.Add(new LookupParameter<ResultCollection>("Results", "The result collection where the value should be stored."));
    }

    public override IOperation Apply() {
      ItemArray<BoolValue> tabu = IsTabuParameter.ActualValue;
      PercentValue value = PercentTabuParameter.ActualValue;
      if (value == null) {
        value = new PercentValue();
        PercentTabuParameter.ActualValue = value;
      }
      value.Value = tabu.Where(x => x.Value).Count() / (double)tabu.Length;
      ResultCollection results = ResultsParameter.ActualValue;
      if (results != null) {
        IResult result = null;
        results.TryGetValue(PercentTabuParameter.ActualName, out result);
        if (result != null)
          result.Value = value;
        else
          results.Add(new Result(PercentTabuParameter.ActualName, "Indicates how much of the neighborhood is tabu.", (IItem)value.Clone()));
      }
      return base.Apply();
    }
  }
}

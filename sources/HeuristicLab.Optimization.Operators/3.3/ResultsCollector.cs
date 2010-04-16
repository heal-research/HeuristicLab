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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Optimization.Operators {
  /// <summary>
  /// An operator which collects the actual values of parameters and adds them to a collection of results.
  /// </summary>
  [Item("ResultsCollector", "An operator which collects the actual values of parameters and adds them to a collection of results.")]
  [StorableClass]
  public class ResultsCollector : ValuesCollector {
    public ValueLookupParameter<ResultCollection> ResultsParameter {
      get { return (ValueLookupParameter<ResultCollection>)Parameters["Results"]; }
    }

    public ResultsCollector()
      : base() {
      Parameters.Add(new ValueLookupParameter<ResultCollection>("Results", "The result collection where the collected values should be stored."));
    }

    public override IOperation Apply() {
      ResultCollection results = ResultsParameter.ActualValue;
      IResult result;
      foreach (IParameter param in CollectedValues) {
        IItem value = param.ActualValue;
        if (value != null) {
          results.TryGetValue(param.Name, out result);
          if (result != null)
            result.Value = value;
          else
            results.Add(new Result(param.Name, param.Description, value));
        }
      }
      return base.Apply();
    }
  }
}

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
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Operators {
  /// <summary>
  /// An operator which collects the actual values of parameters and adds them to a collection of variables.
  /// </summary>
  [Item("ResultsCollector", "An operator which collects the actual values of parameters and adds them to a collection of variables.")]
  [StorableClass]
  public class ResultsCollector : ValuesCollector {
    public ValueLookupParameter<VariableCollection> ResultsParameter {
      get { return (ValueLookupParameter<VariableCollection>)Parameters["Results"]; }
    }

    public ResultsCollector()
      : base() {
      Parameters.Add(new ValueLookupParameter<VariableCollection>("Results", "The variable collection where the collected values should be stored."));
    }

    public override IOperation Apply() {
      VariableCollection results = ResultsParameter.ActualValue;
      IVariable var;
      foreach (IParameter param in CollectedValues) {
        IItem value = param.ActualValue;
        if (value != null) {
          results.TryGetValue(param.Name, out var);
          if (var != null)
            var.Value = value;
          else
            results.Add(new Variable(param.Name, param.Description, value));
        }
      }
      return base.Apply();
    }
  }
}

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

using System.Collections;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HEAL.Attic;

namespace HeuristicLab.Optimization.Operators {
  /// <summary>
  /// An operator which collects the actual values of parameters and adds them to a collection of results.
  /// </summary>
  [Item("ResultsCollector", "An operator which collects the actual values of parameters and adds them to a collection of results.")]
  [StorableType("AAC0CE73-E492-48FB-9645-7AC282ECD860")]
  public class ResultsCollector : ValuesCollector {
    public ValueLookupParameter<ResultCollection> ResultsParameter {
      get { return (ValueLookupParameter<ResultCollection>)Parameters["Results"]; }
    }
    public ValueParameter<BoolValue> CopyValueParameter {
      get { return (ValueParameter<BoolValue>)Parameters["CopyValue"]; }
    }

    public BoolValue CopyValue {
      get { return CopyValueParameter.Value; }
      set { CopyValueParameter.Value = value; }
    }

    [StorableConstructor]
    protected ResultsCollector(StorableConstructorFlag _) : base(_) { }
    protected ResultsCollector(ResultsCollector original, Cloner cloner) : base(original, cloner) { }
    public ResultsCollector()
      : base() {
      Parameters.Add(new ValueLookupParameter<ResultCollection>("Results", "The result collection where the collected values should be stored."));
      Parameters.Add(new ValueParameter<BoolValue>("CopyValue", "True if the collected result value should be copied, otherwise false.", new BoolValue(false)));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new ResultsCollector(this, cloner);
    }

    public override IOperation Apply() {
      bool copy = CopyValueParameter.Value.Value;
      ResultCollection results = ResultsParameter.ActualValue;
      IResult result;
      foreach (IParameter param in CollectedValues) {
        IItem value = param.ActualValue;
        if (value != null) {
          ILookupParameter lookupParam = param as ILookupParameter;
          string name = lookupParam != null ? lookupParam.TranslatedName : param.Name;

          IScopeTreeLookupParameter scopeTreeLookupParam = param as IScopeTreeLookupParameter;
          if ((scopeTreeLookupParam != null) && (scopeTreeLookupParam.Depth == 0)) {
            IEnumerator enumerator = ((IEnumerable)value).GetEnumerator();
            if (enumerator.MoveNext())
              value = (IItem)enumerator.Current;
          }

          results.TryGetValue(name, out result);
          if (result != null)
            result.Value = copy ? (IItem)value.Clone() : value;
          else
            results.Add(new Result(name, param.Description, copy ? (IItem)value.Clone() : value));
        }
      }
      return base.Apply();
    }
  }
}

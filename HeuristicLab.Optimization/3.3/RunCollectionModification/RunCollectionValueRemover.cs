#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HEAL.Attic;
using HeuristicLab.Collections;

namespace HeuristicLab.Optimization {

  [Item("RunCollection Value Remover", "Modifies a RunCollection by removing results or parameters.")]
  [StorableType("300726A9-3E81-4F8E-A11F-4A5B3CDCA796")]
  public class RunCollectionValueRemover : ParameterizedNamedItem, IRunCollectionModifier {
    
    public ValueParameter<CheckedItemCollection<StringValue>> ValuesParameter {
      get { return (ValueParameter<CheckedItemCollection<StringValue>>)Parameters["Values"]; }
    }

    public IFixedValueParameter<BoolValue> InvertParameter {
      get { return (IFixedValueParameter<BoolValue>)Parameters["Invert"]; }
    }

    public IEnumerable<string> Values {
      get { return ValuesParameter.Value.CheckedItems.Select(v => v.Value); }
    }

    public bool Invert => InvertParameter.Value.Value;

    #region Construction & Cloning    
    [StorableConstructor]
    protected RunCollectionValueRemover(StorableConstructorFlag _) : base(_) { }
    protected RunCollectionValueRemover(RunCollectionValueRemover original, Cloner cloner)
      : base(original, cloner) {
    }
    public RunCollectionValueRemover() {
      Parameters.Add(new ValueParameter<CheckedItemCollection<StringValue>>("Values", "The result or parameter values to be removed from each run."));
      Parameters.Add(new FixedValueParameter<BoolValue>("Invert", "Inverts the filter strategy: Blacklist <-> Whitelist (Default: Blacklist)", new BoolValue(false)));
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new RunCollectionValueRemover(this, cloner);
    }    
    #endregion    

    public void Modify(List<IRun> runs) {
      foreach (var run in runs) {
        if (Invert) { //Whitebox
          var parametersCopy = new ObservableDictionary<string, IItem>(run.Parameters);
          var resultsCopy = new ObservableDictionary<string, IItem>(run.Results);
          foreach(var param in parametersCopy)
            if (!Values.Any(x => x == param.Key))
              run.Parameters.Remove(param.Key);
          foreach (var result in resultsCopy)
            if (!Values.Any(x => x == result.Key))
              run.Results.Remove(result.Key);
        } else { //Blackbox
          foreach (var value in Values) {
            run.Parameters.Remove(value);
            run.Results.Remove(value);
          }
        }
      }   
    }
    
  }
}

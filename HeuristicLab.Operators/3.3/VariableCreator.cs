#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Operators {
  /// <summary>
  /// An operator which collects the actual values of parameters and clones them into the current scope.
  /// </summary>
  [Item("VariableCreator", "An operator which collects the actual values of parameters and clones them into the current scope.")]
  [StorableClass]
  public class VariableCreator : ValuesCollector {
    protected ScopeParameter CurrentScopeParameter {
      get { return (ScopeParameter)Parameters["CurrentScope"]; }
    }
    public IScope CurrentScope {
      get { return CurrentScopeParameter.ActualValue; }
    }

    [StorableConstructor]
    protected VariableCreator(bool deserializing) : base(deserializing) { }
    protected VariableCreator(VariableCreator original, Cloner cloner)
      : base(original, cloner) {
    }
    public VariableCreator()
      : base() {
      Parameters.Add(new ScopeParameter("CurrentScope", "The current scope into which the parameter values are cloned."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new VariableCreator(this, cloner);
    }

    public override IOperation Apply() {
      IVariable var;
      foreach (IParameter param in CollectedValues) {
        ILookupParameter lookupParam = param as ILookupParameter;
        string name = lookupParam != null ? lookupParam.TranslatedName : param.Name;

        CurrentScope.Variables.TryGetValue(name, out var);
        IItem value = param.ActualValue;
        if (var != null)
          var.Value = value == null ? null : (IItem)value.Clone();
        else
          CurrentScope.Variables.Add(new Variable(name, param.Description, value == null ? null : (IItem)value.Clone()));
      }
      return base.Apply();
    }
  }
}

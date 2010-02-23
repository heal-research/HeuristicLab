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

using System;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Parameters {
  /// <summary>
  /// A parameter whose value is retrieved from the scope.
  /// </summary>
  [Item("LookupParameter<T>", "A parameter whose value is retrieved from or written to a scope.")]
  public class LookupParameter<T> : Parameter, ILookupParameter<T> where T : class, IItem {
    [Storable]
    private string actualName;
    public string ActualName {
      get { return actualName; }
      set {
        if (value == null) throw new ArgumentNullException();
        if (!actualName.Equals(value)) {
          actualName = value;
          OnActualNameChanged();
        }
      }
    }
    public new T ActualValue {
      get { return (T)GetActualValue(); }
      set { SetActualValue(value); }
    }

    public LookupParameter()
      : base("Anonymous", typeof(T)) {
      actualName = Name;
    }
    public LookupParameter(string name)
      : base(name, typeof(T)) {
      actualName = Name;
    }
    public LookupParameter(string name, string description)
      : base(name, description, typeof(T)) {
      actualName = Name;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      LookupParameter<T> clone = (LookupParameter<T>)base.Clone(cloner);
      clone.actualName = actualName;
      return clone;
    }

    public override string ToString() {
      return string.Format("{0}: {1} ({2})", Name, ActualName, DataType.GetPrettyName());
    }

    private IValueParameter GetParameter(out string name) {
      IValueParameter valueParam = this as IValueParameter;
      ILookupParameter lookupParam = this as ILookupParameter;
      IExecutionContext current = ExecutionContext;

      name = Name;
      while ((valueParam != null) || (lookupParam != null)) {
        if ((valueParam != null) && (valueParam.Value != null)) return valueParam;
        if (lookupParam != null) name = lookupParam.ActualName;

        current = current.Parent;
        while ((current != null) && !current.Parameters.ContainsKey(name))
          current = current.Parent;

        if (current != null) {
          valueParam = current.Parameters[name] as IValueParameter;
          lookupParam = current.Parameters[name] as ILookupParameter;
          if ((valueParam == null) && (lookupParam == null))
            throw new InvalidOperationException(
              string.Format("Parameter look-up chain broken. Parameter \"{0}\" is not an \"{1}\" or an \"{2}\".",
                            name,
                            typeof(IValueParameter).GetPrettyName(),
                            typeof(ILookupParameter).GetPrettyName())
            );
        } else {
          valueParam = null;
          lookupParam = null;
        }
      }
      return null;
    }
    private IVariable LookupVariable(string name) {
      IScope scope = ExecutionContext.Scope;
      while ((scope != null) && !scope.Variables.ContainsKey(name))
        scope = scope.Parent;
      return scope != null ? scope.Variables[actualName] : null;
    }
    protected override IItem GetActualValue() {
      string name;
      // try to get value from context stack
      IValueParameter param = GetParameter(out name);
      if (param != null) return param.Value;

      // try to get variable from scope
      IVariable var = LookupVariable(name);
      if (var != null) {
        if (!(var.Value is T))
          throw new InvalidOperationException(
            string.Format("Type mismatch. Variable \"{0}\" does not contain a \"{1}\".",
                          name,
                          typeof(T).GetPrettyName())
          );
        return var.Value;
      }
      return null;
    }
    protected override void SetActualValue(IItem value) {
      if (!(value is T))
        throw new InvalidOperationException(
          string.Format("Type mismatch. Value is not a \"{0}\".",
                        typeof(T).GetPrettyName())
        );
      // try to set value in context stack
      string name;
      IValueParameter param = GetParameter(out name);
      if (param != null) {
        param.Value = value;
        return;
      }

      // try to set value in scope
      IVariable var = LookupVariable(name);
      if (var != null) {
        var.Value = value;
        return;
      }

      // create new variable
      ExecutionContext.Scope.Variables.Add(new Variable(name, value));
    }

    public event EventHandler ActualNameChanged;
    private void OnActualNameChanged() {
      if (ActualNameChanged != null)
        ActualNameChanged(this, EventArgs.Empty);
      OnChanged();
    }

    public static string TranslateName(string name, IExecutionContext context) {
      string currentName = name;
      IExecutionContext currentContext = context;
      IParameter param;
      ILookupParameter lookupParam;

      while (currentContext != null) {
        currentContext.Parameters.TryGetValue(currentName, out param);
        if (param != null) {
          lookupParam = param as ILookupParameter;
          if (lookupParam == null)
            throw new InvalidOperationException(
              string.Format("Parameter look-up chain broken. Parameter \"{0}\" is not an \"{1}\".",
                            currentName,
                            typeof(ILookupParameter).GetPrettyName())
            );
          currentName = lookupParam.ActualName;
        }
        currentContext = currentContext.Parent;
      }
      return currentName;
    }
  }
}

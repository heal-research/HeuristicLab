#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Collections.Generic;
using System.Text;
using System.Xml;
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
      return string.Format("{0}: {1} ({2})", Name, ActualName, DataType.Name);
    }

    private IVariable LookupVariable(string name) {
      IScope scope = ExecutionContext.Scope;
      while ((scope != null) && !scope.Variables.ContainsKey(name))
        scope = scope.Parent;
      return scope != null ? scope.Variables[actualName] : null;
    }
    protected override IItem GetActualValue() {
      string name = TranslateName(Name, ExecutionContext);
      IVariable var = LookupVariable(name);
      if (var != null) {
        T value = var.Value as T;
        if (value == null)
          throw new InvalidOperationException(
            string.Format("Type mismatch. Variable \"{0}\" does not contain a \"{1}\".",
                          name,
                          typeof(T).GetPrettyName())
          );
        return value;
      }
      return null;
    }
    protected override void SetActualValue(IItem value) {
      T val = value as T;
      if (val == null)
        throw new InvalidOperationException(
          string.Format("Type mismatch. Value is not a \"{0}\".",
                        typeof(T).GetPrettyName())
        );
      string name = TranslateName(Name, ExecutionContext);
      IVariable var = LookupVariable(name);
      if (var != null) var.Value = val;
      else ExecutionContext.Scope.Variables.Add(new Variable(name, val));
    }

    public event EventHandler ActualNameChanged;
    private void OnActualNameChanged() {
      if (ActualNameChanged != null)
        ActualNameChanged(this, new EventArgs());
      OnChanged();
    }

    public static string TranslateName(string name, ExecutionContext context) {
      string currentName = name;
      ExecutionContext currentContext = context;
      IParameter param;
      ILookupParameter<T> lookupParam;

      while (currentContext != null) {
        currentContext.Operator.Parameters.TryGetValue(currentName, out param);
        if (param != null) {
          lookupParam = param as ILookupParameter<T>;
          if (lookupParam == null)
            throw new InvalidOperationException(
              string.Format("Parameter look-up chain broken. Parameter \"{0}\" is not an \"{1}\".",
                            currentName,
                            typeof(ILookupParameter<T>).GetPrettyName())
            );
          currentName = lookupParam.ActualName;
        }
        currentContext = currentContext.Parent;
      }
      return currentName;
    }
  }
}

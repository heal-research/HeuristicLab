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

using System;
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;

namespace HeuristicLab.Parameters {
  /// <summary>
  /// A parameter whose value is retrieved from the scope.
  /// </summary>
  [Item("LookupParameter", "A parameter whose value is retrieved from or written to a scope.")]
  [StorableType("84FE5F33-94B8-4E30-B1CB-CD15314FB83B")]
  public class LookupParameter<T> : ContextParameter, ILookupParameter<T> where T : class, IItem {
    [Storable]
    private string actualName;
    public string ActualName {
      get { return actualName; }
      set {
        if (value == null) throw new ArgumentNullException();
        if (string.IsNullOrWhiteSpace(value)) {
          actualName = Name;
          OnActualNameChanged();
        } else if (!actualName.Equals(value)) {
          actualName = value;
          OnActualNameChanged();
        }
      }
    }
    public string TranslatedName {
      get {
        string translatedName = Name;
        GetValueParameterAndTranslateName(ExecutionContext, ref translatedName);
        return translatedName;
      }
    }
    public new T ActualValue {
      get { return (T)base.ActualValue; }
      set { base.ActualValue = value; }
    }

    [StorableConstructor]
    protected LookupParameter(StorableConstructorFlag _) : base(_) { }
    protected LookupParameter(LookupParameter<T> original, Cloner cloner)
      : base(original, cloner) {
      actualName = original.actualName;
    }
    public LookupParameter()
      : base("Anonymous", typeof(T)) {
      this.actualName = Name;
    }
    public LookupParameter(string name)
      : base(name, typeof(T)) {
      this.actualName = Name;
    }
    public LookupParameter(string name, string description)
      : base(name, description, typeof(T)) {
      this.actualName = Name;
    }
    public LookupParameter(string name, string description, string actualName)
      : base(name, description, typeof(T)) {
      this.actualName = string.IsNullOrWhiteSpace(actualName) ? Name : actualName;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new LookupParameter<T>(this, cloner);
    }

    public override string ToString() {
      if (Name.Equals(ActualName))
        return Name;
      else
        return Name + ": " + ActualName;
    }

    protected static IValueParameter GetValueParameterAndTranslateName(IExecutionContext executionContext, ref string translatedName) {
      IValueParameter valueParam;
      ILookupParameter lookupParam;
      IExecutionContext currentExecutionContext = executionContext;

      while (currentExecutionContext != null) {
        IParameter param = null;
        while (currentExecutionContext != null && !currentExecutionContext.Parameters.TryGetValue(translatedName, out param))
          currentExecutionContext = currentExecutionContext.Parent;
        if (currentExecutionContext == null) break;

        valueParam = param as IValueParameter;
        lookupParam = param as ILookupParameter;

        if (valueParam == null && lookupParam == null)
          throw new InvalidOperationException(
            string.Format("Parameter look-up chain broken. Parameter \"{0}\" is neither an \"{1}\" nor an \"{2}\".",
                          translatedName, typeof(IValueParameter).GetPrettyName(), typeof(ILookupParameter).GetPrettyName())
          );

        if (valueParam != null) {
          if (valueParam.Value != null) return valueParam;
          else if (lookupParam == null) return valueParam;
        }
        translatedName = lookupParam.ActualName;

        currentExecutionContext = currentExecutionContext.Parent;
      }
      return null;
    }
    protected static IVariable LookupVariable(IScope scope, string name) {
      IVariable variable = null;
      while (scope != null && !scope.Variables.TryGetValue(name, out variable))
        scope = scope.Parent;
      return scope != null ? variable : null;
    }

    protected override IItem GetActualValueFromContext() {
      string translatedName = Name;
      var value = GetValue(ExecutionContext, ref translatedName);
      if (value != null && !(value is T))
        throw new InvalidOperationException(
          string.Format("Type mismatch. Variable \"{0}\" does not contain a \"{1}\".",
                        translatedName,
                        typeof(T).GetPrettyName())
        );
      return value;
    }

    protected static IItem GetValue(IExecutionContext executionContext, ref string name) {
      // try to get value from context stack
      IValueParameter param = GetValueParameterAndTranslateName(executionContext, ref name);
      if (param != null) return param.Value;

      // try to get variable from scope
      IVariable var = LookupVariable(executionContext.Scope, name);
      return var != null ? var.Value : null;
    }

    protected override void SetActualValue(IItem value) {
      if (!(value is T))
        throw new InvalidOperationException(
          string.Format("Type mismatch. Value is not a \"{0}\".",
                        typeof(T).GetPrettyName())
        );
      CachedActualValue = value;

      string translatedName = Name;
      SetValue(ExecutionContext, ref translatedName, value);
    }

    protected static void SetValue(IExecutionContext executionContext, ref string name, IItem value) {
      // try to set value in context stack
      IValueParameter param = GetValueParameterAndTranslateName(executionContext, ref name);
      if (param != null) {
        param.Value = value;
        return;
      }

      // try to set value in scope
      IVariable var = LookupVariable(executionContext.Scope, name);
      if (var != null) {
        var.Value = value;
        return;
      }

      // create new variable
      executionContext.Scope.Variables.Add(new Variable(name, value));
    }

    public event EventHandler ActualNameChanged;
    protected virtual void OnActualNameChanged() {
      EventHandler handler = ActualNameChanged;
      if (handler != null) handler(this, EventArgs.Empty);
      OnToStringChanged();
    }
  }
}

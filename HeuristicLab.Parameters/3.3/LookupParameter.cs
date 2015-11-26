#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Threading;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Parameters {
  /// <summary>
  /// A parameter whose value is retrieved from the scope.
  /// </summary>
  [Item("LookupParameter", "A parameter whose value is retrieved from or written to a scope.")]
  [StorableClass]
  public class LookupParameter<T> : Parameter, IStatefulItem, ILookupParameter<T> where T : class, IItem {
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
        string translatedName;
        GetValueParameterAndTranslateName(out translatedName);
        return translatedName;
      }
    }
    public new T ActualValue {
      get { return (T)base.ActualValue; }
      set { base.ActualValue = value; }
    }

    private Lazy<ThreadLocal<IItem>> cachedActualValues;
    private IItem CachedActualValue {
      get { return cachedActualValues.Value.Value; }
    }

    private Lazy<ThreadLocal<IExecutionContext>> executionContexts;
    public IExecutionContext ExecutionContext {
      get { return executionContexts.Value.Value; }
      set {
        if (value != executionContexts.Value.Value) {
          executionContexts.Value.Value = value;
          cachedActualValues.Value.Value = null;
        }
      }
    }

    [StorableConstructor]
    protected LookupParameter(bool deserializing)
      : base(deserializing) {
      cachedActualValues = new Lazy<ThreadLocal<IItem>>(() => { return new ThreadLocal<IItem>(); }, LazyThreadSafetyMode.ExecutionAndPublication);
      executionContexts = new Lazy<ThreadLocal<IExecutionContext>>(() => { return new ThreadLocal<IExecutionContext>(); }, LazyThreadSafetyMode.ExecutionAndPublication);
    }
    protected LookupParameter(LookupParameter<T> original, Cloner cloner)
      : base(original, cloner) {
      actualName = original.actualName;
      this.Hidden = original.Hidden;
      cachedActualValues = new Lazy<ThreadLocal<IItem>>(() => { return new ThreadLocal<IItem>(); }, LazyThreadSafetyMode.ExecutionAndPublication);
      executionContexts = new Lazy<ThreadLocal<IExecutionContext>>(() => { return new ThreadLocal<IExecutionContext>(); }, LazyThreadSafetyMode.ExecutionAndPublication);
    }
    public LookupParameter()
      : base("Anonymous", typeof(T)) {
      this.actualName = Name;
      this.Hidden = false;
      cachedActualValues = new Lazy<ThreadLocal<IItem>>(() => { return new ThreadLocal<IItem>(); }, LazyThreadSafetyMode.ExecutionAndPublication);
      executionContexts = new Lazy<ThreadLocal<IExecutionContext>>(() => { return new ThreadLocal<IExecutionContext>(); }, LazyThreadSafetyMode.ExecutionAndPublication);
    }
    public LookupParameter(string name)
      : base(name, typeof(T)) {
      this.actualName = Name;
      this.Hidden = false;
      cachedActualValues = new Lazy<ThreadLocal<IItem>>(() => { return new ThreadLocal<IItem>(); }, LazyThreadSafetyMode.ExecutionAndPublication);
      executionContexts = new Lazy<ThreadLocal<IExecutionContext>>(() => { return new ThreadLocal<IExecutionContext>(); }, LazyThreadSafetyMode.ExecutionAndPublication);
    }
    public LookupParameter(string name, string description)
      : base(name, description, typeof(T)) {
      this.actualName = Name;
      this.Hidden = false;
      cachedActualValues = new Lazy<ThreadLocal<IItem>>(() => { return new ThreadLocal<IItem>(); }, LazyThreadSafetyMode.ExecutionAndPublication);
      executionContexts = new Lazy<ThreadLocal<IExecutionContext>>(() => { return new ThreadLocal<IExecutionContext>(); }, LazyThreadSafetyMode.ExecutionAndPublication);
    }
    public LookupParameter(string name, string description, string actualName)
      : base(name, description, typeof(T)) {
      this.actualName = string.IsNullOrWhiteSpace(actualName) ? Name : actualName;
      this.Hidden = false;
      cachedActualValues = new Lazy<ThreadLocal<IItem>>(() => { return new ThreadLocal<IItem>(); }, LazyThreadSafetyMode.ExecutionAndPublication);
      executionContexts = new Lazy<ThreadLocal<IExecutionContext>>(() => { return new ThreadLocal<IExecutionContext>(); }, LazyThreadSafetyMode.ExecutionAndPublication);
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

    private IValueParameter GetValueParameterAndTranslateName(out string actualName) {
      IValueParameter valueParam;
      ILookupParameter lookupParam;
      IExecutionContext currentExecutionContext = ExecutionContext;

      actualName = Name;
      while (currentExecutionContext != null) {
        valueParam = currentExecutionContext.Parameters[actualName] as IValueParameter;
        lookupParam = currentExecutionContext.Parameters[actualName] as ILookupParameter;

        if ((valueParam == null) && (lookupParam == null))
          throw new InvalidOperationException(
            string.Format("Parameter look-up chain broken. Parameter \"{0}\" is not an \"{1}\" or an \"{2}\".",
                          actualName, typeof(IValueParameter).GetPrettyName(), typeof(ILookupParameter).GetPrettyName())
          );

        if (valueParam != null) {
          if (valueParam.Value != null) return valueParam;
          else if (lookupParam == null) return valueParam;
        }
        if (lookupParam != null) actualName = lookupParam.ActualName;

        currentExecutionContext = currentExecutionContext.Parent;
        while ((currentExecutionContext != null) && !currentExecutionContext.Parameters.ContainsKey(actualName))
          currentExecutionContext = currentExecutionContext.Parent;
      }
      return null;
    }
    private IVariable LookupVariable(string name) {
      IScope scope = ExecutionContext.Scope;
      while ((scope != null) && !scope.Variables.ContainsKey(name))
        scope = scope.Parent;
      return scope != null ? scope.Variables[name] : null;
    }
    protected override IItem GetActualValue() {
      if (CachedActualValue != null) return CachedActualValue;
      string name;
      // try to get value from context stack
      IValueParameter param = GetValueParameterAndTranslateName(out name);
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
        cachedActualValues.Value.Value = var.Value;
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
      cachedActualValues.Value.Value = value;

      // try to set value in context stack
      string name;
      IValueParameter param = GetValueParameterAndTranslateName(out name);
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

    public virtual void InitializeState() {
    }
    public virtual void ClearState() {
      if (cachedActualValues.IsValueCreated) {
        cachedActualValues.Value.Dispose();
        cachedActualValues = new Lazy<ThreadLocal<IItem>>(() => { return new ThreadLocal<IItem>(); }, LazyThreadSafetyMode.ExecutionAndPublication);
      }
      if (executionContexts.IsValueCreated) {
        executionContexts.Value.Dispose();
        executionContexts = new Lazy<ThreadLocal<IExecutionContext>>(() => { return new ThreadLocal<IExecutionContext>(); }, LazyThreadSafetyMode.ExecutionAndPublication);
      }
    }

    public event EventHandler ActualNameChanged;
    protected virtual void OnActualNameChanged() {
      EventHandler handler = ActualNameChanged;
      if (handler != null) handler(this, EventArgs.Empty);
      OnToStringChanged();
    }
  }
}

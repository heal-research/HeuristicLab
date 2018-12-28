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

using System;
using System.Drawing;
using HeuristicLab.Common;
using HeuristicLab.Common.Resources;
using HeuristicLab.Core;
using HeuristicLab.Parameters;
using HEAL.Fossil;

namespace HeuristicLab.Optimization {
  [Item("ResultParameter", "A parameter whose value is written to a result collection.")]
  [StorableType("CF10EF50-82B6-4A98-82C0-3C5ECED48904")]
  public sealed class ResultParameter<T> : LookupParameter<T>, IResultParameter<T> where T : class, IItem {
    public override Image ItemImage { get { return VSImageLibrary.Exception; } }
    public override bool CanChangeDescription { get { return true; } }

    [Storable]
    private string resultCollectionName;
    public string ResultCollectionName {
      get { return resultCollectionName; }
      set {
        if (string.IsNullOrWhiteSpace(value)) throw new ArgumentNullException();
        else if (!value.Equals(resultCollectionName)) {
          resultCollectionName = value;
          OnResultCollectionNameChanged();
        }
      }
    }

    [Storable]
    private T defaultValue;
    public T DefaultValue {
      get { return defaultValue; }
      set {
        if (value != defaultValue) {
          defaultValue = value;
          OnDefaultValueChanged();
        }
      }
    }

    [StorableConstructor]
    private ResultParameter(StorableConstructorFlag _) : base(_) { }
    private ResultParameter(ResultParameter<T> original, Cloner cloner)
      : base(original, cloner) {
      resultCollectionName = original.resultCollectionName;
      defaultValue = cloner.Clone(original.defaultValue);
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new ResultParameter<T>(this, cloner);
    }
    public ResultParameter() : this("Anonymous", string.Empty, "Results") { }
    public ResultParameter(string name, string description) : this(name, description, "Results") { }

    public ResultParameter(string name, string description, string resultCollectionName)
      : base(name, description, string.Empty) {
      if (string.IsNullOrEmpty(resultCollectionName)) throw new ArgumentException("resultCollectionName");
      this.resultCollectionName = resultCollectionName;
      Hidden = false;
    }
    public ResultParameter(string name, string description, string resultCollectionName, T defaultValue)
      : base(name, description, string.Empty) {
      if (string.IsNullOrEmpty(resultCollectionName)) throw new ArgumentException("resultCollectionName");
      if (defaultValue == null) throw new ArgumentNullException("defaultValue");
      this.resultCollectionName = resultCollectionName;
      this.defaultValue = defaultValue;
      Hidden = false;
    }

    protected override IItem GetActualValue() {
      ResultCollection results;
      if (CachedActualValue != null) {
        results = CachedActualValue as ResultCollection;
        if (results == null) throw new InvalidOperationException("ResultParameter (" + ActualName + "): ResultCollection not found.");
      } else {
        var tmp = ResultCollectionName;
        // verifyType has to be disabled, because the ResultCollection may not be identical to the generic type of the parameter
        results = GetValue(ExecutionContext, ref tmp) as ResultCollection;
        if (results == null) throw new InvalidOperationException("ResultParameter (" + ActualName + "): ResultCollection with name " + tmp + " not found.");
        CachedActualValue = results;
      }

      IResult result;
      if (!results.TryGetValue(ActualName, out result)) {
        if (DefaultValue == null) throw new InvalidOperationException("ResultParameter (" + ActualName + "): Result not found and no default value specified.");
        result = ItemDescription == Description ? new Result(ActualName, (T)DefaultValue.Clone()) : new Result(ActualName, Description, (T)DefaultValue.Clone());
        results.Add(result);
      }

      var resultValue = result.Value as T;
      if (resultValue == null)
        throw new InvalidOperationException(string.Format("Type mismatch. Result \"{0}\" does not contain a \"{1}\".", ActualName, typeof(T).GetPrettyName()));

      return resultValue;
    }

    protected override void SetActualValue(IItem value) {
      ResultCollection results;
      if (CachedActualValue != null) {
        results = CachedActualValue as ResultCollection;
        if (results == null) throw new InvalidOperationException("ResultParameter (" + ActualName + "): ResultCollection not found.");
      } else {
        var tmp = ResultCollectionName;
        results = GetValue(ExecutionContext, ref tmp) as ResultCollection;
        if (results == null) throw new InvalidOperationException("ResultParameter (" + ActualName + "): ResultCollection with name " + tmp + " not found.");
        CachedActualValue = results;
      }

      IResult result;
      if (!results.TryGetValue(ActualName, out result)) {
        result = ItemDescription == Description ? new Result(ActualName, value) : new Result(ActualName, Description, value);
        results.Add(result);
      } else result.Value = value;
    }


    public event EventHandler ResultCollectionNameChanged;
    private void OnResultCollectionNameChanged() {
      var handler = ResultCollectionNameChanged;
      if (handler != null) handler(this, EventArgs.Empty);
      OnToStringChanged();
    }

    public event EventHandler DefaultValueChanged;
    private void OnDefaultValueChanged() {
      EventHandler handler = DefaultValueChanged;
      if (handler != null) handler(this, EventArgs.Empty);
      OnItemImageChanged();
    }
  }
}

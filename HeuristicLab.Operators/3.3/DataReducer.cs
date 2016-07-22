#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Operators {
  [Item("DataReducer", "An operator to reduce values of sub scopes.")]
  [StorableClass]
  public sealed class DataReducer : SingleSuccessorOperator {
    #region Parameter Properties
    public ScopeTreeLookupParameter<IItem> ParameterToReduce {
      get { return (ScopeTreeLookupParameter<IItem>)Parameters["ParameterToReduce"]; }
    }
    public LookupParameter<IItem> TargetParameter {
      get { return (LookupParameter<IItem>)Parameters["TargetParameter"]; }
    }
    public ValueLookupParameter<ReductionOperation> ReductionOperation {
      get { return (ValueLookupParameter<ReductionOperation>)Parameters["ReductionOperation"]; }
    }
    public ValueLookupParameter<ReductionOperation> TargetOperation {
      get { return (ValueLookupParameter<ReductionOperation>)Parameters["TargetOperation"]; }
    }
    #endregion

    [StorableConstructor]
    private DataReducer(bool deserializing) : base(deserializing) { }
    private DataReducer(DataReducer original, Cloner cloner)
      : base(original, cloner) {
    }
    public DataReducer()
      : base() {
      #region Create parameters
      Parameters.Add(new ScopeTreeLookupParameter<IItem>("ParameterToReduce", "The parameter on which the reduction operation should be applied."));
      Parameters.Add(new LookupParameter<IItem>("TargetParameter", "The target variable in which the reduced value should be stored."));
      Parameters.Add(new ValueLookupParameter<ReductionOperation>("ReductionOperation", "The operation which is applied on the parameters to reduce.", new ReductionOperation()));
      Parameters.Add(new ValueLookupParameter<ReductionOperation>("TargetOperation", "The operation used to apply the reduced value to the target variable.", new ReductionOperation()));
      #endregion
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new DataReducer(this, cloner);
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      // BackwardsCompatibility3.3
      var oldReductionOperation = Parameters["ReductionOperation"] as ValueParameter<ReductionOperation>;
      if (oldReductionOperation != null) {
        Parameters.Remove("ReductionOperation");
        Parameters.Add(new ValueLookupParameter<ReductionOperation>("ReductionOperation", "The operation which is applied on the parameters to reduce.", oldReductionOperation.Value));
      }
      var oldTargetOperation = Parameters["TargetOperation"] as ValueParameter<ReductionOperation>;
      if (oldTargetOperation != null) {
        Parameters.Remove("TargetOperation");
        Parameters.Add(new ValueLookupParameter<ReductionOperation>("TargetOperation", "The operation used to apply the reduced value to the target variable.", oldTargetOperation.Value));
      }
    }

    public override IOperation Apply() {
      var values = ParameterToReduce.ActualValue;
      if (!values.Any()) return base.Apply();

      if (values.All(x => x is IntValue)) {
        CalculateResult(values.OfType<IntValue>().Select(x => x.Value), values.First().GetType());
      } else if (values.All(x => x is DoubleValue)) {
        CalculateResult(values.OfType<DoubleValue>().Select(x => x.Value), values.First().GetType());
      } else if (values.All(x => x is TimeSpanValue)) {
        CalculateResult(values.OfType<TimeSpanValue>().Select(x => x.Value), values.First().GetType());
      } else if (values.All(x => x is BoolValue)) {
        CalculateResult(values.OfType<BoolValue>().Select(x => x.Value), values.First().GetType());
      } else {
        throw new ArgumentException(string.Format("Type {0} is not supported by the DataReducer.", values.First().GetType()));
      }

      return base.Apply();
    }

    #region integer reduction
    private void CalculateResult(IEnumerable<int> values, Type targetType) {
      int result;
      switch (ReductionOperation.ActualValue.Value) {
        case ReductionOperations.Sum:
          result = values.Sum();
          break;
        case ReductionOperations.Product:
          result = values.Aggregate(1, (x, y) => x * y);
          break;
        case ReductionOperations.Count:
          result = values.Count();
          break;
        case ReductionOperations.Min:
          result = values.Min();
          break;
        case ReductionOperations.Max:
          result = values.Max();
          break;
        case ReductionOperations.Avg:
          result = (int)Math.Round(values.Average());
          break;
        case ReductionOperations.Assign:
          result = values.Last();
          break;
        default:
          throw new InvalidOperationException(string.Format("Operation {0} is not supported as ReductionOperation for type: {1}.", ReductionOperation.ActualValue.Value, targetType));
      }

      IntValue target;
      switch (TargetOperation.ActualValue.Value) {
        case ReductionOperations.Sum:
          target = InitializeTarget<IntValue, int>(targetType, 0);
          target.Value += result;
          break;
        case ReductionOperations.Product:
          target = InitializeTarget<IntValue, int>(targetType, 1);
          target.Value = target.Value * result;
          break;
        case ReductionOperations.Min:
          target = InitializeTarget<IntValue, int>(targetType, int.MaxValue);
          target.Value = Math.Min(target.Value, result);
          break;
        case ReductionOperations.Max:
          target = InitializeTarget<IntValue, int>(targetType, int.MinValue);
          target.Value = Math.Max(target.Value, result);
          break;
        case ReductionOperations.Avg:
          target = InitializeTarget<IntValue, int>(targetType, result);
          target.Value = (int)Math.Round((target.Value + result) / 2.0);
          break;
        case ReductionOperations.Assign:
          target = InitializeTarget<IntValue, int>(targetType, 0);
          target.Value = result;
          break;
        default:
          throw new InvalidOperationException(string.Format("Operation {0} is not supported as TargetOperation for type: {1}.", TargetOperation.ActualValue.Value, targetType));
      }
    }
    #endregion
    #region double reduction
    private void CalculateResult(IEnumerable<double> values, Type targetType) {
      double result;
      switch (ReductionOperation.ActualValue.Value) {
        case ReductionOperations.Sum:
          result = values.Sum();
          break;
        case ReductionOperations.Product:
          result = values.Aggregate(1.0, (x, y) => x * y);
          break;
        case ReductionOperations.Count:
          result = values.Count();
          break;
        case ReductionOperations.Min:
          result = values.Min();
          break;
        case ReductionOperations.Max:
          result = values.Max();
          break;
        case ReductionOperations.Avg:
          result = values.Average();
          break;
        case ReductionOperations.Assign:
          result = values.Last();
          break;
        default:
          throw new InvalidOperationException(string.Format("Operation {0} is not supported as ReductionOperation for type: {1}.", ReductionOperation.ActualValue.Value, targetType));
      }

      DoubleValue target;
      switch (TargetOperation.ActualValue.Value) {
        case ReductionOperations.Sum:
          target = InitializeTarget<DoubleValue, double>(targetType, 0.0);
          target.Value += result;
          break;
        case ReductionOperations.Product:
          target = InitializeTarget<DoubleValue, double>(targetType, 1.0);
          target.Value = target.Value * result;
          break;
        case ReductionOperations.Min:
          target = InitializeTarget<DoubleValue, double>(targetType, double.MaxValue);
          target.Value = Math.Min(target.Value, result);
          break;
        case ReductionOperations.Max:
          target = InitializeTarget<DoubleValue, double>(targetType, double.MinValue);
          target.Value = Math.Max(target.Value, result);
          break;
        case ReductionOperations.Avg:
          target = InitializeTarget<DoubleValue, double>(targetType, result);
          target.Value = (target.Value + result) / 2.0;
          break;
        case ReductionOperations.Assign:
          target = InitializeTarget<DoubleValue, double>(targetType, 0.0);
          target.Value = result;
          break;
        default:
          throw new InvalidOperationException(string.Format("Operation {0} is not supported as TargetOperation for type: {1}.", TargetOperation.ActualValue.Value, targetType));
      }
    }
    #endregion
    #region TimeSpan reduction
    private void CalculateResult(IEnumerable<TimeSpan> values, Type targetType) {
      TimeSpan result;
      switch (ReductionOperation.ActualValue.Value) {
        case ReductionOperations.Sum:
          result = values.Aggregate(new TimeSpan(), (x, y) => x + y);
          break;
        case ReductionOperations.Min:
          result = values.Min();
          break;
        case ReductionOperations.Max:
          result = values.Max();
          break;
        case ReductionOperations.Avg:
          result = TimeSpan.FromMilliseconds(values.Average(x => x.TotalMilliseconds));
          break;
        case ReductionOperations.Assign:
          result = values.Last();
          break;
        default:
          throw new InvalidOperationException(string.Format("Operation {0} is not supported as ReductionOperation for type: {1}.", ReductionOperation.ActualValue.Value, targetType));
      }

      TimeSpanValue target;
      switch (TargetOperation.ActualValue.Value) {
        case ReductionOperations.Sum:
          target = InitializeTarget<TimeSpanValue, TimeSpan>(targetType, new TimeSpan());
          target.Value += result;
          break;
        case ReductionOperations.Min:
          target = InitializeTarget<TimeSpanValue, TimeSpan>(targetType, TimeSpan.MaxValue);
          target.Value = target.Value < result ? target.Value : result;
          break;
        case ReductionOperations.Max:
          target = InitializeTarget<TimeSpanValue, TimeSpan>(targetType, TimeSpan.MinValue);
          target.Value = target.Value > result ? target.Value : result;
          break;
        case ReductionOperations.Avg:
          target = InitializeTarget<TimeSpanValue, TimeSpan>(targetType, result);
          target.Value = TimeSpan.FromMilliseconds((target.Value.TotalMilliseconds + result.TotalMilliseconds) / 2);
          break;
        case ReductionOperations.Assign:
          target = InitializeTarget<TimeSpanValue, TimeSpan>(targetType, new TimeSpan());
          target.Value = result;
          break;
        default:
          throw new InvalidOperationException(string.Format("Operation {0} is not supported as TargetOperation for type: {1}.", TargetOperation.ActualValue.Value, targetType));
      }
    }
    #endregion
    #region bool reduction
    private void CalculateResult(IEnumerable<bool> values, Type targetType) {
      bool result;
      switch (ReductionOperation.ActualValue.Value) {
        case ReductionOperations.All:
          result = values.All(x => x);
          break;
        case ReductionOperations.Any:
          result = values.Any(x => x);
          break;
        default:
          throw new InvalidOperationException(string.Format("Operation {0} is not supported as ReductionOperation for type: {1}.", ReductionOperation.ActualValue.Value, targetType));
      }

      BoolValue target;
      switch (TargetOperation.ActualValue.Value) {
        case ReductionOperations.Assign:
          target = InitializeTarget<BoolValue, bool>(targetType, true);
          target.Value = result;
          break;
        default:
          throw new InvalidOperationException(string.Format("Operation {0} is not supported as TargetOperation for type: {1}.", TargetOperation.ActualValue.Value, targetType));
      }
    }
    #endregion

    #region helpers
    private T1 InitializeTarget<T1, T2>(Type targetType, T2 initialValue)
      where T1 : ValueTypeValue<T2>
      where T2 : struct {
      T1 target = (T1)TargetParameter.ActualValue;
      if (target == null) {
        target = (T1)Activator.CreateInstance(targetType);
        TargetParameter.ActualValue = target;
        target.Value = initialValue;
      }
      return target;
    }
    #endregion
  }
}

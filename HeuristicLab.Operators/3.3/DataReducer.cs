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
    public ValueParameter<ReductionOperation> ReductionOperation {
      get { return (ValueParameter<ReductionOperation>)Parameters["ReductionOperation"]; }
    }
    public ValueParameter<ReductionOperation> TargetOperation {
      get { return (ValueParameter<ReductionOperation>)Parameters["TargetOperation"]; }
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
      Parameters.Add(new ValueParameter<ReductionOperation>("ReductionOperation", "The operation which is applied on the parameters to reduce."));
      Parameters.Add(new ValueParameter<ReductionOperation>("TargetOperation", "The operation used to apply the reduced value to the target variable."));
      #endregion
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new DataReducer(this, cloner);
    }

    public override IOperation Apply() {
      var values = ParameterToReduce.ActualValue;
      if (values.Count() > 0) {
        if (values.All(x => x.GetType() == typeof(IntValue))) {
          List<IntValue> intValues = new List<IntValue>();
          values.ForEach(x => intValues.Add((IntValue)x));
          CalculateResult(intValues);
        } else if (values.All(x => x.GetType() == typeof(DoubleValue))) {
          List<DoubleValue> doubleValues = new List<DoubleValue>();
          values.ForEach(x => doubleValues.Add((DoubleValue)x));
          CalculateResult(doubleValues);
        } else if (values.All(x => x.GetType() == typeof(TimeSpanValue))) {
          List<TimeSpanValue> timeSpanValues = new List<TimeSpanValue>();
          values.ForEach(x => timeSpanValues.Add((TimeSpanValue)x));
          CalculateResult(timeSpanValues);
        } else {
          throw new ArgumentException(string.Format("Type {0} is not supported by the DataReducer.", values.First().GetType()));
        }
      }
      return base.Apply();
    }

    private void CalculateResult(List<IntValue> values) {
      int result = 1;
      if (TargetParameter.ActualValue == null) TargetParameter.ActualValue = new IntValue();
      IntValue target = (IntValue)TargetParameter.ActualValue;

      switch (ReductionOperation.Value.Value) {
        case ReductionOperations.Sum:
          result = values.Sum(x => x.Value);
          break;
        case ReductionOperations.Prod:
          values.ForEach(x => result *= x.Value);
          break;
        case ReductionOperations.Avg:
          result = (int)Math.Round(values.Average(x => x.Value));
          break;
        case ReductionOperations.Min:
          result = values.Min(x => x.Value);
          break;
        case ReductionOperations.Max:
          result = values.Max(x => x.Value);
          break;
        default:
          throw new InvalidOperationException(string.Format("Operation {0} is not supported as ReductionOperation for type: {1}.", ReductionOperation.Value.Value, result.GetType()));
      }

      switch (TargetOperation.Value.Value) {
        case ReductionOperations.Assign:
          target.Value = result;
          break;
        case ReductionOperations.Sum:
          target.Value += result;
          break;
        case ReductionOperations.Prod:
          if (target.Value == 0) target.Value = 1;
          target.Value *= result;
          break;
        default:
          throw new InvalidOperationException(string.Format("Operation {0} is not supported as TargetOperation for type: {1}.", TargetOperation.Value.Value, result.GetType()));
      }
    }

    private void CalculateResult(List<DoubleValue> values) {
      double result = 1.0;
      if (TargetParameter.ActualValue == null) TargetParameter.ActualValue = new DoubleValue();
      DoubleValue target = (DoubleValue)TargetParameter.ActualValue;

      switch (ReductionOperation.Value.Value) {
        case ReductionOperations.Sum:
          result = values.Sum(x => x.Value);
          break;
        case ReductionOperations.Prod:
          values.ForEach(x => result *= x.Value);
          break;
        case ReductionOperations.Avg:
          result = values.Average(x => x.Value);
          break;
        case ReductionOperations.Min:
          result = values.Min(x => x.Value);
          break;
        case ReductionOperations.Max:
          result = values.Max(x => x.Value);
          break;
        default:
          throw new InvalidOperationException(string.Format("Operation {0} is not supported as ReductionOperation for type: {1}.", ReductionOperation.Value.Value, result.GetType()));
      }

      switch (TargetOperation.Value.Value) {
        case ReductionOperations.Assign:
          target.Value = result;
          break;
        case ReductionOperations.Sum:
          target.Value += result;
          break;
        case ReductionOperations.Prod:
          if (target.Value == 0.0) target.Value = 1.0;
          target.Value *= result;
          break;
        default:
          throw new InvalidOperationException(string.Format("Operation {0} is not supported as TargetOperation for type: {1}.", TargetOperation.Value.Value, result.GetType()));
      }
    }

    private void CalculateResult(List<TimeSpanValue> values) {
      TimeSpan result = TimeSpan.Zero;
      if (TargetParameter.ActualValue == null) TargetParameter.ActualValue = new TimeSpanValue();
      TimeSpanValue target = (TimeSpanValue)TargetParameter.ActualValue;

      switch (ReductionOperation.Value.Value) {
        case ReductionOperations.Sum:
          values.ForEach(x => result = result.Add(x.Value));
          break;
        case ReductionOperations.Avg:
          double avg = values.Average(x => x.Value.TotalMilliseconds);
          result = TimeSpan.FromMilliseconds(avg);
          break;
        case ReductionOperations.Min:
          result = values.Min(x => x.Value);
          break;
        case ReductionOperations.Max:
          result = values.Max(x => x.Value);
          break;
        default:
          throw new InvalidOperationException(string.Format("Operation {0} is not supported as ReductionOperation for type: {1}.", ReductionOperation.Value.Value, result.GetType()));
      }

      switch (TargetOperation.Value.Value) {
        case ReductionOperations.Assign:
          target.Value = result;
          break;
        case ReductionOperations.Sum:
          target.Value += result;
          break;
        default:
          throw new InvalidOperationException(string.Format("Operation {0} is not supported as TargetOperation for type: {1}.", TargetOperation.Value.Value, result.GetType()));
      }
    }
  }
}

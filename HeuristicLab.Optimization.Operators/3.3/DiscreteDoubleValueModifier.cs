#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Optimization.Operators {
  /// <summary>
  /// Base class for modifying a double value according to a certain function in discrete intervalls.
  /// </summary>
  [Item("DiscreteDoubleValueModifier", "Base class for modifying a double value according to a certain function in discrete intervalls.")]
  [StorableClass]
  public abstract class DiscreteDoubleValueModifier : SingleSuccessorOperator, IDiscreteDoubleValueModifier {
    #region parameter properties
    /// <summary>
    /// The parameter that should be modified.
    /// </summary>
    public ILookupParameter<DoubleValue> ValueParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["Value"]; }
    }
    /// <summary>
    /// The start value of the parameter, will be assigned to <see cref="ValueParameter"/> as soon as <see cref="IndexParamter"/> equals <see cref="StartIndexParameter"/>.
    /// </summary>
    public IValueLookupParameter<DoubleValue> StartValueParameter {
      get { return (IValueLookupParameter<DoubleValue>)Parameters["StartValue"]; }
    }
    /// <summary>
    /// The end value of the parameter, will be assigned to <see cref="ValueParameter"/> as soon as <see cref="IndexParamter"/> equals <see cref="EndIndexParameter"/>.
    /// </summary>
    public IValueLookupParameter<DoubleValue> EndValueParameter {
      get { return (IValueLookupParameter<DoubleValue>)Parameters["EndValue"]; }
    }
    /// <summary>
    /// The index that denotes from which point in the function (relative to <see cref="StartIndexParameter"/> and <see cref="EndIndexParameter"/> the value should be assigned.
    /// </summary>
    public ILookupParameter<IntValue> IndexParameter {
      get { return (ILookupParameter<IntValue>)Parameters["Index"]; }
    }
    /// <summary>
    /// As soon as <see cref="IndexParameter"/> is &gt;= this parameter the value will start to be modified.
    /// </summary>
    public IValueLookupParameter<IntValue> StartIndexParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters["StartIndex"]; }
    }
    /// <summary>
    /// As long as <see cref="IndexParameter"/> is &lt;= this parameter the value will start to be modified.
    /// </summary>
    public IValueLookupParameter<IntValue> EndIndexParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters["EndIndex"]; }
    }
    #endregion
    [StorableConstructor]
    protected DiscreteDoubleValueModifier(bool deserializing) : base(deserializing) { }
    protected DiscreteDoubleValueModifier(DiscreteDoubleValueModifier original, Cloner cloner) : base(original, cloner) { }
    /// <summary>
    /// Initializes a new instance of <see cref="DiscreteDoubleValueModifier"/> with 6 parameters
    /// (<c>Value</c>, <c>StartValue</c>, <c>EndValue</c>, <c>Index</c>, <c>StartIndex</c>, <c>EndIndex</c>).
    /// </summary>
    protected DiscreteDoubleValueModifier()
      : base() {
      Parameters.Add(new LookupParameter<DoubleValue>("Value", "The double value to modify."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("StartValue", "The start value of 'Value'."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("EndValue", "The end value of 'Value'."));
      Parameters.Add(new LookupParameter<IntValue>("Index", "The current index."));
      Parameters.Add(new ValueLookupParameter<IntValue>("StartIndex", "The start index at which to start modifying 'Value'."));
      Parameters.Add(new ValueLookupParameter<IntValue>("EndIndex", "The end index by which 'Value' should have reached 'EndValue'."));
    }

    /// <summary>
    /// Checks whether index is between start and end and forwards the call to <see cref="Modify"/> if startIndex &lt; index &lt; endIndex.
    /// </summary>
    /// <remarks>
    /// If index = startIndex the call will not be forwarded and startValue will be used. The same with endIndex and endValue.
    /// </remarks>
    /// <returns>What the base class returns.</returns>
    public override IOperation Apply() {
      int index = IndexParameter.ActualValue.Value, startIndex = StartIndexParameter.ActualValue.Value;
      if (index >= startIndex) {
        int endIndex = EndIndexParameter.ActualValue.Value;
        DoubleValue value = ValueParameter.ActualValue;
        if (value == null) {
          value = new DoubleValue();
          ValueParameter.ActualValue = value;
        }
        double newValue = value.Value;
        if (index == startIndex) {
          newValue = StartValueParameter.ActualValue.Value;
        } else if (index == endIndex) {
          newValue = EndValueParameter.ActualValue.Value;
        } else if (index < endIndex) {
          double start = StartValueParameter.ActualValue.Value, end = EndValueParameter.ActualValue.Value;
          newValue = Modify(value.Value, start, end, index, startIndex, endIndex);
        }
        value.Value = newValue;
      }
      return base.Apply();
    }

    /// <summary>
    /// Modifies a given value according to two support points denoted by (startIndex; startValue) and (endIndex; endValue).
    /// The current 'index' and the last value of 'value' is also given.
    /// </summary>
    /// <param name="value">The last value.</param>
    /// <param name="startValue">The start value.</param>
    /// <param name="endValue">The end value.</param>
    /// <param name="index">The current index.</param>
    /// <param name="startIndex">The start index.</param>
    /// <param name="endIndex">The end index.</param>
    /// <returns>The new value.</returns>
    protected abstract double Modify(double value, double startValue, double endValue, int index, int startIndex, int endIndex);
  }
}

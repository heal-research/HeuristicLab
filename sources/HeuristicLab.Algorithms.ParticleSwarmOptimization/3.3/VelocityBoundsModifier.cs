#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Algorithms.ParticleSwarmOptimization {
  [Item("VelocityBoundsModifier", "Modifies the velocity bounds.")]
  [StorableClass]
  public sealed class VelocityBoundsModifier : SingleSuccessorOperator, IDiscreteDoubleMatrixModifier {
    #region Parameters
    public ILookupParameter<DoubleMatrix> ValueParameter {
      get { return (ILookupParameter<DoubleMatrix>)Parameters["Matrix"]; }
    }
    public IValueLookupParameter<DoubleValue> ScaleParameter {
      get { return (IValueLookupParameter<DoubleValue>)Parameters["Scale"]; }
    }
    public ConstrainedValueParameter<IDiscreteDoubleValueModifier> ScalingOperatorParameter {
      get { return (ConstrainedValueParameter<IDiscreteDoubleValueModifier>)Parameters["ScalingOperator"]; }
    }
    public IValueLookupParameter<DoubleValue> StartValueParameter {
      get { return (IValueLookupParameter<DoubleValue>)Parameters["StartValue"]; }
    }
    public IValueLookupParameter<DoubleValue> EndValueParameter {
      get { return (IValueLookupParameter<DoubleValue>)Parameters["EndValue"]; }
    }
    public ILookupParameter<IntValue> IndexParameter {
      get { return (ILookupParameter<IntValue>)Parameters["Index"]; }
    }
    public IValueLookupParameter<IntValue> StartIndexParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters["StartIndex"]; }
    }
    public IValueLookupParameter<IntValue> EndIndexParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters["EndIndex"]; }
    }
    #endregion

    #region Construction & Cloning

    [StorableConstructor]
    private VelocityBoundsModifier(bool deserializing) : base(deserializing) { }
    private VelocityBoundsModifier(VelocityBoundsModifier original, Cloner cloner)
      : base(original, cloner) {
      ParameterizeModifiers();
    }
    public VelocityBoundsModifier() {
      Parameters.Add(new LookupParameter<DoubleMatrix>("Matrix", "The double matrix to modify."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("Scale", "Scale parameter."));
      Parameters.Add(new ConstrainedValueParameter<IDiscreteDoubleValueModifier>("ScalingOperator", "Modifies the value"));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("StartValue", "The start value of 'Value'.", new DoubleValue(1)));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("EndValue", "The end value of 'Value'."));
      Parameters.Add(new LookupParameter<IntValue>("Index", "The current index."));
      Parameters.Add(new ValueLookupParameter<IntValue>("StartIndex", "The start index at which to start modifying 'Value'."));
      Parameters.Add(new ValueLookupParameter<IntValue>("EndIndex", "The end index by which 'Value' should have reached 'EndValue'."));

      Initialize();
      ParameterizeModifiers();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new VelocityBoundsModifier(this, cloner);
    }
    #endregion

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      ParameterizeModifiers();
    }

    private void Initialize() {
      foreach (IDiscreteDoubleValueModifier op in ApplicationManager.Manager.GetInstances<IDiscreteDoubleValueModifier>()) {
        ScalingOperatorParameter.ValidValues.Add(op);
      }
    }

    private void ParameterizeModifiers() {
      foreach (IDiscreteDoubleValueModifier op in ScalingOperatorParameter.ValidValues) {
        op.ValueParameter.ActualName = ScaleParameter.Name;
        op.StartValueParameter.ActualName = StartValueParameter.Name;
        op.EndValueParameter.ActualName = EndValueParameter.Name;
        op.IndexParameter.ActualName = IndexParameter.Name;
        op.StartIndexParameter.ActualName = StartIndexParameter.Name;
        op.EndIndexParameter.ActualName = EndIndexParameter.Name;
      }
    }

    public override IOperation Apply() {
      OperationCollection next = new OperationCollection();
      DoubleMatrix matrix = ValueParameter.ActualValue;
      if (this.ScaleParameter.ActualValue == null && this.StartValueParameter.ActualValue != null) {
        this.ScaleParameter.ActualValue = new DoubleValue(StartValueParameter.ActualValue.Value);
      }
      for (int i = 0; i < matrix.Rows; i++) {
        for (int j = 0; j < matrix.Columns; j++) {
          if (matrix[i, j] >= 0) {
            matrix[i, j] = ScaleParameter.ActualValue.Value;
          } else {
            matrix[i, j] = (-1) * ScaleParameter.ActualValue.Value;
          }
        }
      }
      next.Add(ExecutionContext.CreateChildOperation(this.ScalingOperatorParameter.Value));
      next.Add(base.Apply());
      return next;
    }
  }
}

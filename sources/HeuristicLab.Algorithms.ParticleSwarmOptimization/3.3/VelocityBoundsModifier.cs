using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Operators;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Algorithms.ParticleSwarmOptimization {

  [StorableClass]
  public class VelocityBoundsModifier : SingleSuccessorOperator, IDiscreteDoubleMatrixModifier {

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
    protected VelocityBoundsModifier(bool deserializing) : base(deserializing) { }
    protected VelocityBoundsModifier(VelocityBoundsModifier original, Cloner cloner) : base(original, cloner) {
      Initialize();
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
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new VelocityBoundsModifier(this, cloner);
    }

    private void Initialize() {
      foreach (IDiscreteDoubleValueModifier op in ApplicationManager.Manager.GetInstances<IDiscreteDoubleValueModifier>()) {
        ScalingOperatorParameter.ValidValues.Add(op);
        op.ValueParameter.ActualName = ScaleParameter.ActualName;
      }
      ScaleParameter.ActualNameChanged += new EventHandler(ScaleParameter_ActualNameChanged);
      StartValueParameter.ActualNameChanged += new EventHandler(StartValueParameter_ActualNameChanged);
      EndValueParameter.ActualNameChanged += new EventHandler(EndValueParameter_ActualNameChanged);
      IndexParameter.ActualNameChanged += new EventHandler(IndexParameter_ActualNameChanged);
      EndIndexParameter.ActualNameChanged += new EventHandler(EndIndexParameter_ActualNameChanged);
      StartIndexParameter.ActualNameChanged += new EventHandler(StartIndexParameter_ActualNameChanged);
    }
    #endregion

    #region Events
    private void ScaleParameter_ActualNameChanged(object sender, EventArgs e) {
      foreach (IDiscreteDoubleValueModifier modifier in ScalingOperatorParameter.ValidValues) {
        modifier.ValueParameter.ActualName = ScaleParameter.ActualName;
      }
    }

    private void StartValueParameter_ActualNameChanged(object sender, EventArgs e) {
      foreach (IDiscreteDoubleValueModifier modifier in ScalingOperatorParameter.ValidValues) {
        modifier.StartValueParameter.ActualName = StartValueParameter.ActualName;
      }
    }

    private void EndValueParameter_ActualNameChanged(object sender, EventArgs e) {
      foreach (IDiscreteDoubleValueModifier modifier in ScalingOperatorParameter.ValidValues) {
        modifier.EndValueParameter.ActualName = EndValueParameter.ActualName;
      }
    }

    private void IndexParameter_ActualNameChanged(object sender, EventArgs e) {
      foreach (IDiscreteDoubleValueModifier modifier in ScalingOperatorParameter.ValidValues) {
        modifier.IndexParameter.ActualName = IndexParameter.ActualName;
      }
    }

    private void StartIndexParameter_ActualNameChanged(object sender, EventArgs e) {
      foreach (IDiscreteDoubleValueModifier modifier in ScalingOperatorParameter.ValidValues) {
        modifier.StartIndexParameter.ActualName = StartIndexParameter.ActualName;
      }
    }

    private void EndIndexParameter_ActualNameChanged(object sender, EventArgs e) {
      foreach (IDiscreteDoubleValueModifier modifier in ScalingOperatorParameter.ValidValues) {
        modifier.EndIndexParameter.ActualName = EndIndexParameter.ActualName;
      }
    }
    #endregion

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

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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Algorithms.SimulatedAnnealing {
  [Item("TemperatureController", "Decides whether to use cooling or heating and calls the appropriate operator.")]
  [StorableClass]
  public class TemperatureController : SingleSuccessorOperator {
    #region Strings
    private const string AnnealingOperatorName = "AnnealingOperator";
    private const string HeatingOperatorName = "HeatingOperator";
    private const string MaximumIterationsName = "MaximumIterations";
    private const string UpperTemperatureName = "UpperTemperature";
    private const string LowerTemperatureName = "LowerTemperature";
    private const string IterationsName = "Iterations";
    private const string TemperatureStartIndexName = "TemperatureStartIndex";
    private const string CoolingName = "Cooling";
    private const string StartTemperatureName = "StartTemperature";
    private const string EndTemperatureName = "EndTemperature";
    private const string TemperatureName = "Temperature";
    private const string IsAcceptedName = "IsAccepted";
    private const string ChangeInertiaName = "ChangeInertia";
    #endregion

    #region Parameter Properties
    public ILookupParameter<DoubleValue> TemperatureParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters[TemperatureName]; }
    }
    public IValueLookupParameter<DoubleValue> UpperTemperatureParameter {
      get { return (IValueLookupParameter<DoubleValue>)Parameters[UpperTemperatureName]; }
    }
    public IValueLookupParameter<DoubleValue> LowerTemperatureParameter {
      get { return (IValueLookupParameter<DoubleValue>)Parameters[LowerTemperatureName]; }
    }
    public ILookupParameter<DoubleValue> StartTemperatureParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters[StartTemperatureName]; }
    }
    public ILookupParameter<DoubleValue> EndTemperatureParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters[EndTemperatureName]; }
    }
    public ILookupParameter<IntValue> TemperatureStartIndexParameter {
      get { return (ILookupParameter<IntValue>)Parameters[TemperatureStartIndexName]; }
    }
    public ILookupParameter<BoolValue> CoolingParameter {
      get { return (ILookupParameter<BoolValue>)Parameters[CoolingName]; }
    }
    public ILookupParameter<IntValue> IterationsParameter {
      get { return (ILookupParameter<IntValue>)Parameters[IterationsName]; }
    }
    public IValueLookupParameter<IntValue> MaximumIterationsParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters[MaximumIterationsName]; }
    }
    public IValueLookupParameter<IOperator> AnnealingOperatorParameter {
      get { return (IValueLookupParameter<IOperator>)Parameters[AnnealingOperatorName]; }
    }
    public IValueLookupParameter<IOperator> HeatingOperatorParameter {
      get { return (IValueLookupParameter<IOperator>)Parameters[HeatingOperatorName]; }
    }
    public ILookupParameter<BoolValue> IsAcceptedParameter {
      get { return (ILookupParameter<BoolValue>)Parameters[IsAcceptedName]; }
    }
    public IValueLookupParameter<IntValue> ChangeInertiaParameter {
      get { return (IValueLookupParameter<IntValue>) Parameters[ChangeInertiaName]; }
    }
    #endregion

    [StorableConstructor]
    protected TemperatureController(bool deserializing) : base(deserializing) {}
    protected TemperatureController(TemperatureController original, Cloner cloner) : base(original, cloner) {}
    public TemperatureController()
      : base() {
      Parameters.Add(new LookupParameter<DoubleValue>(TemperatureName, "The current temperature."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>(UpperTemperatureName, "The upper bound of the temperature."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>(LowerTemperatureName, "The lower bound of the temperature."));
      Parameters.Add(new LookupParameter<IntValue>(IterationsName, "The number of iterations."));
      Parameters.Add(new ValueLookupParameter<IntValue>(MaximumIterationsName, "The maximum number of iterations which should be processed."));
      Parameters.Add(new ValueLookupParameter<IOperator>(AnnealingOperatorName, "The operator that cools the temperature."));
      Parameters.Add(new ValueLookupParameter<IOperator>(HeatingOperatorName, "The operator that heats the temperature."));
      Parameters.Add(new LookupParameter<IntValue>(TemperatureStartIndexName, "The index where the annealing or heating was last changed."));
      Parameters.Add(new LookupParameter<BoolValue>(CoolingName, "True when the temperature should be cooled, false otherwise."));
      Parameters.Add(new LookupParameter<DoubleValue>(StartTemperatureName, "The temperature from which cooling or reheating should occur."));
      Parameters.Add(new LookupParameter<DoubleValue>(EndTemperatureName, "The temperature to which should be cooled or heated."));
      Parameters.Add(new LookupParameter<BoolValue>(IsAcceptedName, "Whether the move was accepted or not."));
      Parameters.Add(new ValueLookupParameter<IntValue>(ChangeInertiaName, "The minimum iterations that need to be passed, before the process can change between heating and cooling."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new TemperatureController(this, cloner);
    }

    public override IOperation Apply() {
      var accepted = IsAcceptedParameter.ActualValue;
      var heatingOperator = HeatingOperatorParameter.ActualValue;
      if (accepted == null || heatingOperator == null) { // annealing in case no heating operator is given
        return new OperationCollection {
          ExecutionContext.CreateOperation(AnnealingOperatorParameter.ActualValue),
          base.Apply()
        };
      }

      var cooling = CoolingParameter.ActualValue.Value;
      var lastChange = TemperatureStartIndexParameter.ActualValue.Value;
      var iterations = IterationsParameter.ActualValue.Value;
      var inertia = ChangeInertiaParameter.ActualValue.Value;

      if (accepted.Value && !cooling && (iterations - (lastChange+1)) > inertia) { // temperature is heated, but should be cooled
        cooling = true;
        TemperatureStartIndexParameter.ActualValue.Value = Math.Max(0, iterations - 1);
        StartTemperatureParameter.ActualValue.Value = TemperatureParameter.ActualValue.Value;
        EndTemperatureParameter.ActualValue.Value = LowerTemperatureParameter.ActualValue.Value;
      } else if (!accepted.Value && cooling && (iterations - (lastChange+1)) > inertia) {  // temperature is cooled, but should be heated
        cooling = false;
        TemperatureStartIndexParameter.ActualValue.Value = Math.Max(0, iterations - 1);
        StartTemperatureParameter.ActualValue.Value = TemperatureParameter.ActualValue.Value;
        EndTemperatureParameter.ActualValue.Value = UpperTemperatureParameter.ActualValue.Value;
      }

      CoolingParameter.ActualValue.Value = cooling;

      if (cooling) {
        return new OperationCollection {
          ExecutionContext.CreateOperation(AnnealingOperatorParameter.ActualValue),
          base.Apply()
        };
      }
      // heating
      return new OperationCollection {
        ExecutionContext.CreateOperation(HeatingOperatorParameter.ActualValue),
        base.Apply()
      };
    }
  }
}
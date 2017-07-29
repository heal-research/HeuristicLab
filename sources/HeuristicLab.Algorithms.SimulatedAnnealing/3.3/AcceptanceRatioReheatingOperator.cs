using HeuristicLab.Operators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Common;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Optimization;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.Analysis;

namespace HeuristicLab.Algorithms.SimulatedAnnealing
{
    [Item("AcceptanceRatioReheatingOperator", "Reheats the temperature if the acceptance is below a threshold until it is above another one.")]
    [StorableClass]
    public class AcceptanceRatioReheatingOperator : SingleSuccessorOperator, IReheatingOperator
    {
        #region Strings
        private const string AnnealingOperatorName = "AnnealingOperator";
        private const string LowerTemperatureName = "LowerTemperature";
        private const string IterationsName = "Iterations";
        private const string TemperatureStartIndexName = "TemperatureStartIndex";
        private const string StartTemperatureName = "StartTemperature";
        private const string EndTemperatureName = "EndTemperature";
        private const string TemperatureName = "Temperature";
        private const string MaximumIterationsName = "MaximumIterations";

        private const string UpperTemperatureName = "UpperTemperature";
        private const string ReheatingOperatorName = "ReheatingOperator";
        private const string ThresholdName = "Threshold";
        private const string MemorySizeName = "MemorySize";
        private const string CoolingName = "Cooling";
        private const string IsAcceptedName = "IsAccepted";
        private const string AcceptanceMemoryName = "AcceptanceMemory";
        private const string AverageAcceptanceRatioName = "AverageAcceptanceRatio";


        #endregion
        #region Parameters
        public ILookupParameter<DoubleValue> TemperatureParameter
        {
            get { return (ILookupParameter<DoubleValue>)Parameters[TemperatureName]; }
        }
        public ILookupParameter<DoubleValue> LowerTemperatureParameter
        {
            get { return (ILookupParameter<DoubleValue>)Parameters[LowerTemperatureName]; }
        }
        public ILookupParameter<DoubleValue> StartTemperatureParameter
        {
            get { return (ILookupParameter<DoubleValue>)Parameters[StartTemperatureName]; }
        }
        public ILookupParameter<DoubleValue> EndTemperatureParameter
        {
            get { return (ILookupParameter<DoubleValue>)Parameters[EndTemperatureName]; }
        }
        public ILookupParameter<IntValue> TemperatureStartIndexParameter
        {
            get { return (ILookupParameter<IntValue>)Parameters[TemperatureStartIndexName]; }
        }
        public ILookupParameter<IntValue> IterationsParameter
        {
            get { return (ILookupParameter<IntValue>)Parameters[IterationsName]; }
        }
        public ILookupParameter<IOperator> AnnealingOperatorParameter
        {
            get { return (ILookupParameter<IOperator>)Parameters[AnnealingOperatorName]; }
        }
        public ILookupParameter<IntValue> MaximumIterationsParameter
        {
            get { return (ILookupParameter<IntValue>)Parameters[MaximumIterationsName]; }
        }
        private ValueLookupParameter<DoubleValue> UpperTemperatureParameter
        {
            get { return (ValueLookupParameter<DoubleValue>)Parameters[UpperTemperatureName]; }
        }
        private IConstrainedValueParameter<IDiscreteDoubleValueModifier> ReheatingOperatorParameter
        {
            get { return (IConstrainedValueParameter<IDiscreteDoubleValueModifier>)Parameters[ReheatingOperatorName]; }
        }
        private ValueParameter<DoubleRange> ThresholdParameter
        {
            get { return (ValueParameter<DoubleRange>)Parameters[ThresholdName]; }
        }
        private ValueParameter<IntValue> MemorySizeParameter
        {
            get { return (ValueParameter<IntValue>)Parameters[MemorySizeName]; }
        }
        public ILookupParameter<BoolValue> CoolingParameter
        {
            get { return (ILookupParameter<BoolValue>)Parameters[CoolingName]; }
        }
        public ILookupParameter<BoolValue> IsAcceptedParameter
        {
            get { return (ILookupParameter<BoolValue>)Parameters[IsAcceptedName]; }
        }
        public ILookupParameter<ItemList<BoolValue>> AcceptanceMemoryParameter
        {
            get { return (ILookupParameter<ItemList<BoolValue>>)Parameters[AcceptanceMemoryName]; }
        }
        public ILookupParameter<DoubleValue> AverageAcceptanceRatioParameter
        {
            get { return (ILookupParameter<DoubleValue>)Parameters[AverageAcceptanceRatioName]; }
        }

        #endregion

        public AcceptanceRatioReheatingOperator() : base()
        {
            #region Create parameters
            Parameters.Add(new LookupParameter<DoubleValue>(TemperatureName, "The current temperature."));
            Parameters.Add(new LookupParameter<DoubleValue>(LowerTemperatureName, "The lower bound of the temperature."));
            Parameters.Add(new LookupParameter<IntValue>(IterationsName, "The number of iterations."));
            Parameters.Add(new LookupParameter<IOperator>(AnnealingOperatorName, "The operator that cools the temperature."));
            Parameters.Add(new LookupParameter<IntValue>(TemperatureStartIndexName, "The index where the annealing or heating was last changed."));
            Parameters.Add(new LookupParameter<DoubleValue>(StartTemperatureName, "The temperature from which cooling or reheating should occur."));
            Parameters.Add(new LookupParameter<DoubleValue>(EndTemperatureName, "The temperature to which should be cooled or heated."));
            Parameters.Add(new LookupParameter<IntValue>(MaximumIterationsName, "The maximum number of iterations which should be processed."));
            Parameters.Add(new LookupParameter<BoolValue>(IsAcceptedName, "Whether the move was accepted or not."));
            Parameters.Add(new LookupParameter<ItemList<BoolValue>>(AcceptanceMemoryName, "Memorizes the last N acceptance decisions."));
            Parameters.Add(new LookupParameter<BoolValue>(CoolingName, "True when the temperature should be cooled, false otherwise."));
            Parameters.Add(new ValueLookupParameter<DoubleValue>(UpperTemperatureName, "The upper bound of the temperature.", "InitialTemperature"));
            Parameters.Add(new ConstrainedValueParameter<IDiscreteDoubleValueModifier>(ReheatingOperatorName, "The operator that reheats the temperature."));
            Parameters.Add(new ValueParameter<DoubleRange>(ThresholdName, "The threshold controls the temperature. If the average ratio of accepted moves goes below the start of the range the temperature is heated. If the the average ratio of accepted moves goes beyond the end of the range the temperature is cooled again.", new DoubleRange(0.01, 0.1)));
            Parameters.Add(new ValueParameter<IntValue>(MemorySizeName, "The maximum size of the acceptance memory.", new IntValue(100)));
            Parameters.Add(new LookupParameter<DoubleValue>(AverageAcceptanceRatioName, "Average acceptance over full acceptance memory."));
            #endregion

            foreach (var op in ApplicationManager.Manager.GetInstances<IDiscreteDoubleValueModifier>().OrderBy(x => x.Name))
            {
                ReheatingOperatorParameter.ValidValues.Add((IDiscreteDoubleValueModifier) op);
            }
            Parameterize();
        }

        [StorableConstructor]
        protected AcceptanceRatioReheatingOperator(bool deserializing) : base(deserializing) { }
        protected AcceptanceRatioReheatingOperator(AcceptanceRatioReheatingOperator original, Cloner cloner) : base(original, cloner) { }


        public override IDeepCloneable Clone(Cloner cloner)
        {
            return new AcceptanceRatioReheatingOperator(this, cloner);
        }

        public void Parameterize()
        {
            foreach (var op in ReheatingOperatorParameter.ValidValues)
            {
                op.IndexParameter.ActualName = IterationsName;
                op.IndexParameter.Hidden = true;
                op.StartIndexParameter.Value = null;
                op.StartIndexParameter.ActualName = TemperatureStartIndexName;
                op.EndIndexParameter.ActualName = MaximumIterationsParameter.Name;
                op.ValueParameter.ActualName = TemperatureName;
                op.ValueParameter.Hidden = true;
                op.StartValueParameter.ActualName = StartTemperatureName;
                op.StartValueParameter.Hidden = true;
                op.EndValueParameter.ActualName = EndTemperatureName;
                op.EndValueParameter.Hidden = true;
            }
        }

        public override IOperation Apply()
        {
            var isAccepted = IsAcceptedParameter.ActualValue;
            var acceptances = AcceptanceMemoryParameter.ActualValue;
            var cooling = CoolingParameter.ActualValue.Value;
            var ratioAmount = AverageAcceptanceRatioParameter.ActualValue;

            acceptances.Add(isAccepted);

            ratioAmount.Value += isAccepted.Value ? 1 : 0;


            if (acceptances.Count > MemorySizeParameter.Value.Value) {
                ratioAmount.Value -= acceptances.ElementAt(0).Value ? 1 : 0;
                acceptances.RemoveAt(0);
            }

            // only reheat when at least MemorySizeParameter.Value iterations have passed
            if (acceptances.Count == MemorySizeParameter.Value.Value)
            {
                var ratio = ratioAmount.Value / MemorySizeParameter.Value.Value;
                var ratioStart = ThresholdParameter.Value.Start;
                var ratioEnd = ThresholdParameter.Value.End;

                if (!cooling && ratio >= ratioEnd)
                {
                    // if we are heating, but should be cooling
                    cooling = true;
                    TemperatureStartIndexParameter.ActualValue.Value = Math.Max(0, IterationsParameter.ActualValue.Value - 1);
                    StartTemperatureParameter.ActualValue.Value = TemperatureParameter.ActualValue.Value;
                    EndTemperatureParameter.ActualValue.Value = LowerTemperatureParameter.ActualValue.Value;
                }
                else if (cooling && ratio <= ratioStart)
                {
                    // if we are cooling but should be heating
                    cooling = false;
                    TemperatureStartIndexParameter.ActualValue.Value = Math.Max(0, IterationsParameter.ActualValue.Value - 1);
                    StartTemperatureParameter.ActualValue.Value = TemperatureParameter.ActualValue.Value;
                    EndTemperatureParameter.ActualValue.Value = UpperTemperatureParameter.ActualValue.Value;
                }

                CoolingParameter.ActualValue.Value = cooling;
            }
            return cooling ? Cool() : Heat();
        }

        private IOperation Heat()
        {
            return new OperationCollection {
                    ExecutionContext.CreateOperation(ReheatingOperatorParameter.Value),
                    base.Apply()
                };
        }

        private IOperation Cool()
        {
            return new OperationCollection {
                    ExecutionContext.CreateOperation(AnnealingOperatorParameter.ActualValue),
                    base.Apply()
                };
        }
    }
}

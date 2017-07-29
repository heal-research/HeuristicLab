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
    [Item("ConsecutiveRejectionTemperatureResetOperator", "The operator resets the temperature to the ResetTemperature when N consecutive solutions are rejected.")]
    [StorableClass]
    public class ConsecutiveRejectionTemperatureResetOperator : SingleSuccessorOperator, IReheatingOperator
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
        private const string InitialTemperatureName = "InitialTemperature";

        private const string ThresholdName = "Threshold";
        private const string IsAcceptedName = "IsAccepted";
        private const string ConsecutiveRejectedSolutionsCountName = "ConsecutiveRejectedSolutions";
        private const string ResetTemperatureName = "ResetTemperature";
        #endregion

        #region Parameters
        private ValueLookupParameter<DoubleValue> ResetTemperatureParameter
        {
            get { return (ValueLookupParameter<DoubleValue>)Parameters[ResetTemperatureName]; }
        }
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
        private ValueParameter<IntValue> ThresholdParameter
        {
            get { return (ValueParameter<IntValue>)Parameters[ThresholdName]; }
        }
        public ILookupParameter<BoolValue> IsAcceptedParameter
        {
            get { return (ILookupParameter<BoolValue>)Parameters[IsAcceptedName]; }
        }
        public ILookupParameter<IntValue> ConsecutiveRejectedSolutionsCountParameter
        {
            get { return (ILookupParameter<IntValue>)Parameters[ConsecutiveRejectedSolutionsCountName]; }
        }
        #endregion

        public ConsecutiveRejectionTemperatureResetOperator() : base()
        {
            #region Create parameters
            Parameters.Add(new LookupParameter<DoubleValue>(TemperatureName, "The current temperature."));
            Parameters.Add(new LookupParameter<DoubleValue>(LowerTemperatureName, "The lower bound of the temperature."));
            Parameters.Add(new LookupParameter<IntValue>(IterationsName, "The number of iterations."));
            Parameters.Add(new LookupParameter<IOperator>(AnnealingOperatorName, "The operator that cools the temperature."));
            Parameters.Add(new LookupParameter<IntValue>(TemperatureStartIndexName, "The index where the annealing or heating was last changed."));
            Parameters.Add(new LookupParameter<DoubleValue>(StartTemperatureName, "The temperature from which cooling should occur."));
            Parameters.Add(new LookupParameter<DoubleValue>(EndTemperatureName, "The temperature to which should be cooled."));
            Parameters.Add(new LookupParameter<IntValue>(MaximumIterationsName, "The maximum number of iterations which should be processed."));
            Parameters.Add(new LookupParameter<BoolValue>(IsAcceptedName, "Whether the move was accepted or not."));
            Parameters.Add(new LookupParameter<IntValue>(ConsecutiveRejectedSolutionsCountName, "Amount of consecutive rejected solutions."));
            Parameters.Add(new ValueParameter<IntValue>(ThresholdName, "How many consecutive rejected solutions must occur to trigger a reheat.", new IntValue(10)));
            Parameters.Add(new ValueLookupParameter<DoubleValue>(ResetTemperatureName, "The base reset temperature.", InitialTemperatureName));
          
            #endregion

            Parameterize();
        }

        [StorableConstructor]
        protected ConsecutiveRejectionTemperatureResetOperator(bool deserializing) : base(deserializing) { }
        protected ConsecutiveRejectionTemperatureResetOperator(ConsecutiveRejectionTemperatureResetOperator original, Cloner cloner) : base(original, cloner) { }


        public override IDeepCloneable Clone(Cloner cloner)
        {
            return new ConsecutiveRejectionTemperatureResetOperator(this, cloner);
        }

        public void Parameterize()
        {
        }

        public override IOperation Apply()
        {
            var count = ConsecutiveRejectedSolutionsCountParameter.ActualValue;

            count.Value = !IsAcceptedParameter.ActualValue.Value ? count.Value + 1 : 0;

            var heating = count.Value == ThresholdParameter.Value.Value;

            if(heating) count.Value = 0;

            return heating ? Heat() : Cool();
        }

        private IOperation Heat()
        {
            var temperature = Math.Max(ResetTemperatureParameter.ActualValue.Value, TemperatureParameter.ActualValue.Value);

            TemperatureParameter.ActualValue.Value = temperature;
            TemperatureStartIndexParameter.ActualValue.Value = Math.Max(0, IterationsParameter.ActualValue.Value - 1);
            StartTemperatureParameter.ActualValue.Value = TemperatureParameter.ActualValue.Value;
            return base.Apply();
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

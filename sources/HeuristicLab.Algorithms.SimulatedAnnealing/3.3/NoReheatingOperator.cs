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
    [Item("-", "No reheats are performed.")]
    [StorableClass]
    public class NoReheatingOperator : SingleSuccessorOperator, IReheatingOperator
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

        #endregion

        public NoReheatingOperator() : base()
        {
            #region Create parameters
            Parameters.Add(new LookupParameter<DoubleValue>(TemperatureName, "The current temperature."));
            Parameters.Add(new LookupParameter<DoubleValue>(LowerTemperatureName, "The lower bound of the temperature."));
            Parameters.Add(new LookupParameter<IntValue>(IterationsName, "The number of iterations."));
            Parameters.Add(new LookupParameter<IntValue>(TemperatureStartIndexName, "The index where the annealing or heating was last changed."));
            Parameters.Add(new LookupParameter<DoubleValue>(StartTemperatureName, "The temperature from which cooling or reheating should occur."));
            Parameters.Add(new LookupParameter<DoubleValue>(EndTemperatureName, "The temperature to which should be cooled or heated."));
            Parameters.Add(new LookupParameter<IntValue>(MaximumIterationsName, "The maximum number of iterations which should be processed."));
            Parameters.Add(new LookupParameter<IOperator>(AnnealingOperatorName, "The operator that cools the temperature."));
            #endregion

            Parameterize();
        }

        [StorableConstructor]
        protected NoReheatingOperator(bool deserializing) : base(deserializing) { }
        protected NoReheatingOperator(NoReheatingOperator original, Cloner cloner) : base(original, cloner) { }


        public override IDeepCloneable Clone(Cloner cloner)
        {
            return new NoReheatingOperator(this, cloner);
        }

        public void Parameterize()
        {
        }

        public override IOperation Apply()
        {
            TemperatureStartIndexParameter.ActualValue.Value = Math.Max(0, IterationsParameter.ActualValue.Value - 1);
            StartTemperatureParameter.ActualValue.Value = TemperatureParameter.ActualValue.Value;
            EndTemperatureParameter.ActualValue.Value = LowerTemperatureParameter.ActualValue.Value;
            return new OperationCollection {
                    ExecutionContext.CreateOperation(AnnealingOperatorParameter.ActualValue),
                    base.Apply()
                };
        }
    }
}

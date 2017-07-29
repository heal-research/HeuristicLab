using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicLab.Algorithms.SimulatedAnnealing
{
    [Item("BasicTemperatureInitializer", "Sets the initial temperature to a fixed value.")]
    [StorableClass]
    public class BasicTemperatureInitializer : SingleSuccessorOperator, ITemperatureInitializer
    {
        #region Strings
        private const string InitialTemperatureName = "InitialTemperature";
        private const string TemperatureInitializedName = "TemperatureInitialized";
        private const string InitialTemperatureSetterName = "InitialTemperatureValue";
        private const string StartTemperatureName = "StartTemperature";
        private const string TemperatureStartIndexName = "TemperatureStartIndex";
        private const string TemperatureName = "Temperature";
        private const string IterationsName = "Iterations";
        #endregion

        #region Properties
        public ILookupParameter<DoubleValue> InitialTemperatureParameter
        {
            get { return (ILookupParameter<DoubleValue>)Parameters[InitialTemperatureName]; }
        }
        public ILookupParameter<BoolValue> TemperatureInitializedParameter
        {
            get { return (ILookupParameter<BoolValue>)Parameters[TemperatureInitializedName]; }
        }
        public IValueParameter<DoubleValue> InitialTemperatureSetterParameter
        {
            get { return (IValueParameter<DoubleValue>)Parameters[InitialTemperatureSetterName]; }
        }
        public ILookupParameter<DoubleValue> StartTemperatureParameter
        {
            get { return (ILookupParameter<DoubleValue>)Parameters[StartTemperatureName]; }
        }
        public ILookupParameter<IntValue> TemperatureStartIndexParameter
        {
            get { return (ILookupParameter<IntValue>)Parameters[TemperatureStartIndexName]; }
        }
        public ILookupParameter<DoubleValue> TemperatureParameter
        {
            get { return (ILookupParameter<DoubleValue>)Parameters[TemperatureName]; }
        }
        public ILookupParameter<IntValue> IterationsParameter
        {
            get { return (ILookupParameter<IntValue>)Parameters[IterationsName]; }
        }

        #endregion

        public BasicTemperatureInitializer() : base()
        {
            Parameters.Add(new LookupParameter<DoubleValue>(InitialTemperatureName, "Temporary initial temperature field."));
            Parameters.Add(new ValueParameter<DoubleValue>(InitialTemperatureSetterName, "Sets the initial temperature to this value", new DoubleValue(100)));
            Parameters.Add(new LookupParameter<BoolValue>(TemperatureInitializedName, "True, if the temperature has already been initialized."));
            Parameters.Add(new LookupParameter<DoubleValue>(StartTemperatureName, "The temperature from which cooling or reheating should occur."));
            Parameters.Add(new LookupParameter<IntValue>(TemperatureStartIndexName, "The index where the annealing or heating was last changed."));
            Parameters.Add(new LookupParameter<DoubleValue>(TemperatureName, "The current temperature."));
            Parameters.Add(new LookupParameter<IntValue>(IterationsName, "The number of iterations."));
        }

        [StorableConstructor]
        protected BasicTemperatureInitializer(bool deserializing) : base(deserializing) { }
        protected BasicTemperatureInitializer(BasicTemperatureInitializer original, Cloner cloner) : base(original, cloner) { }


        public override IDeepCloneable Clone(Cloner cloner)
        {
            return new BasicTemperatureInitializer(this, cloner);
        }

        public override IOperation Apply()
        {
            // set temperature to selected value
            InitialTemperatureParameter.ActualValue.Value = InitialTemperatureSetterParameter.Value.Value;
            TemperatureParameter.ActualValue.Value = InitialTemperatureSetterParameter.Value.Value;
            TemperatureStartIndexParameter.ActualValue.Value = Math.Max(0, IterationsParameter.ActualValue.Value - 1);
            StartTemperatureParameter.ActualValue.Value = TemperatureParameter.ActualValue.Value;

            // indicate that initial temperature is now set
            TemperatureInitializedParameter.ActualValue.Value = true;

            return base.Apply();
        }
    }
}

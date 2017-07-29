using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicLab.Algorithms.SimulatedAnnealing
{
    /*  Temperature initializer based on Johnson, David S., et al. "Optimization by simulated annealing: an experimental evaluation; part I, graph partitioning." Operations research 37.6 (1989): 865-892.
     *  http://faculty.washington.edu/aragon/pubs/annealing-pt1.pdf (see Section 5.4, Paragraph 3, Parameter INITPROB)
     *  
     *  The initial temperature gets calculated by
     *      1. Performing a random walk and collection of (UphillMovesMemorySize) amount of hills
     *      2. Calculating the average hillsize
     *      3. Define initial temperature as the temperature that lets the algorithm overcome a hill of average size with a percentage of INITIALACCEPTANCERATE
     */

    [Item("TemperatureInitializerBasedOnAverageHillsize", "Sets the initial temperature to the temperature that initialy lets the algorithm accept an averaged sized hill with a certain probability.")]
    [StorableClass]
    public class TemperatureInitializerBasedOnAverageHillsize : SingleSuccessorOperator, ITemperatureInitializer
    {
        #region Strings
        private const string InitialTemperatureName = "InitialTemperature";
        private const string TemperatureInitializedName = "TemperatureInitialized";
        private const string StartTemperatureName = "StartTemperature";
        private const string TemperatureStartIndexName = "TemperatureStartIndex";
        private const string TemperatureName = "Temperature";
        private const string IterationsName = "Iterations";

        private const string LastQualityName = "LastQuality";
        private const string UphillMovesMemoryName = "UphillMovesMemory";
        private const string UphillMovesMemorySizeName = "UphillMovesMemorySize";
        private const string MaximizationName = "Maximization";
        private const string MoveQualityName = "MoveQuality";
        private const string InitialAcceptanceRateName = "InitialAcceptanceRate";

        private const string ResultsName = "Results";


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
        public ILookupParameter<ItemList<DoubleValue>> UphillMovesMemoryParameter
        {
            get { return (ILookupParameter<ItemList<DoubleValue>>)Parameters[UphillMovesMemoryName]; }
        }
        public ILookupParameter<DoubleValue> MoveQualityParameter
        {
            get { return (ILookupParameter<DoubleValue>)Parameters[MoveQualityName]; }
        }
        public ILookupParameter<DoubleValue> LastQualityParameter
        {
            get { return (ILookupParameter<DoubleValue>)Parameters[LastQualityName]; }
        }
        public IValueLookupParameter<BoolValue> MaximizationParameter
        {
            get { return (IValueLookupParameter<BoolValue>)Parameters[MaximizationName]; }
        }
        public IValueParameter<DoubleValue> InitialAcceptanceRateParameter
        {
            get { return (IValueParameter<DoubleValue>)Parameters[InitialAcceptanceRateName]; }
        }
        public IValueParameter<IntValue> UphillMovesMemorySizeParameter
        {
            get { return (IValueParameter<IntValue>)Parameters[UphillMovesMemorySizeName]; }
        }

        #endregion

        public TemperatureInitializerBasedOnAverageHillsize() : base()
        {
            Parameters.Add(new LookupParameter<DoubleValue>(InitialTemperatureName, "Temporary initial temperature field."));
            Parameters.Add(new LookupParameter<BoolValue>(TemperatureInitializedName, "True, if the temperature has already been initialized."));
            Parameters.Add(new LookupParameter<DoubleValue>(StartTemperatureName, "The temperature from which cooling or reheating should occur."));
            Parameters.Add(new LookupParameter<IntValue>(TemperatureStartIndexName, "The index where the annealing or heating was last changed."));
            Parameters.Add(new LookupParameter<DoubleValue>(MoveQualityName, "The value which represents the quality of a move."));
            Parameters.Add(new LookupParameter<DoubleValue>(LastQualityName, "The value which represents the quality of the last move."));
            Parameters.Add(new LookupParameter<DoubleValue>(TemperatureName, "The current temperature."));
            Parameters.Add(new ValueLookupParameter<BoolValue>(MaximizationName, "True if the problem is a maximization problem, otherwise false."));
            Parameters.Add(new LookupParameter<IntValue>(IterationsName, "The number of iterations."));
            Parameters.Add(new ValueParameter<DoubleValue>(InitialAcceptanceRateName, "The initial acceptance rate of average-sized hills used to calculate the initial temperature.", new DoubleValue(0.001)));
            Parameters.Add(new ValueParameter<IntValue>(UphillMovesMemorySizeName, "The amount of uphill moves necessary to compute the initial temperature.", new IntValue(100)));
            Parameters.Add(new LookupParameter<ItemList<DoubleValue>>(UphillMovesMemoryName, "The field that stores the uphill moves."));
        }

        [StorableConstructor]
        protected TemperatureInitializerBasedOnAverageHillsize(bool deserializing) : base(deserializing) { }
        protected TemperatureInitializerBasedOnAverageHillsize(TemperatureInitializerBasedOnAverageHillsize original, Cloner cloner) : base(original, cloner) { }

        public override IDeepCloneable Clone(Cloner cloner)
        {
            return new TemperatureInitializerBasedOnAverageHillsize(this, cloner);
        }

        public override IOperation Apply()
        {
            var currentQuality = MoveQualityParameter.ActualValue.Value;
            var lastQuality = LastQualityParameter.ActualValue.Value;
            var isMaximation = MaximizationParameter.ActualValue.Value;
            var hillMemory = UphillMovesMemoryParameter.ActualValue;

            if (lastQuality != -1)
            {
                double hillSize = CalculateHillSize(lastQuality, currentQuality, isMaximation);
                
                // if there was an uphill move
                if (hillSize > 0)
                {
                    // add it to the list
                    hillMemory.Add(new DoubleValue(hillSize));
                    
                    // if enough hills have been collected
                    if (hillMemory.Count >= UphillMovesMemorySizeParameter.Value.Value)
                    {
                        // calculate and set initial temperature
                        var initialTemperature = CalculateInitialTemperatureBasedOnAverageHillSize(hillMemory);
                        InitialTemperatureParameter.ActualValue.Value = initialTemperature;
                        TemperatureParameter.ActualValue.Value = initialTemperature;
                        TemperatureStartIndexParameter.ActualValue.Value = Math.Max(0, IterationsParameter.ActualValue.Value - 1);
                        StartTemperatureParameter.ActualValue.Value = TemperatureParameter.ActualValue.Value;

                        var results = (ResultCollection)ExecutionContext.Parent.Scope.Variables[ResultsName].Value;
                        results.Add(new Result(InitialTemperatureName, new DoubleValue(initialTemperature)));

                        // indicate that initial temperature is now set
                        TemperatureInitializedParameter.ActualValue.Value = true;
                    }
                }
            }

            LastQualityParameter.ActualValue.Value = currentQuality;
            return base.Apply();
        }

        private double CalculateInitialTemperatureBasedOnAverageHillSize(ItemList<DoubleValue> hillMemory)
        {
            var averageHillSize = hillMemory.Average(x => x.Value);
            return -(averageHillSize / Math.Log(InitialAcceptanceRateParameter.Value.Value));

        }

        private double CalculateHillSize(double lastQuality, double currentQuality, bool isMaximation)
        {
            return isMaximation ? lastQuality - currentQuality : currentQuality - lastQuality;
        }
    }
}

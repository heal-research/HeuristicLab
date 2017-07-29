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
    [Item("RandomWalkReheatingOperator", "Performs a randomwalk with the lenght of n after x consecutive solutions were rejected.")]
    [StorableClass]
    public class RandomWalkReheatingOperator : SingleSuccessorOperator, IReheatingOperator
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

        private const string TemperatureBeforeReheatName = "TemperatureBeforeReheat";
        private const string CurrentRandomWalkStepName = "CurrentRandomWalkStep";
        private const string RandomWalkLengthName = "RandomWalkLength";
        private const string ThresholdName = "Threshold";
        private const string IsAcceptedName = "IsAccepted";
        private const string ConsecutiveRejectedSolutionsCountName = "ConsecutiveRejectedSolutions";
        private const string QualitiesBeforeReheatingName = "QualitiesBeforeReheating";
        private const string AmountOfReheatsName = "AmountOfReheats";
        private const string RetrappedName = "Retrapped";
        private const string LastAcceptedQualityName = "LastAcceptedQuality";
        private const string ResultsName = "Results";
        private const string MoveQualityName = "MoveQuality";
        private const string CoolingName = "Cooling";
        private const string TabuListActiveName = "TabuListActive";


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
        private ValueParameter<IntValue> ThresholdParameter
        {
            get { return (ValueParameter<IntValue>)Parameters[ThresholdName]; }
        }
        private ValueParameter<IntValue> RandomWalkLengthParameter
        {
            get { return (ValueParameter<IntValue>)Parameters[RandomWalkLengthName]; }
        }
        public ILookupParameter<BoolValue> IsAcceptedParameter
        {
            get { return (ILookupParameter<BoolValue>)Parameters[IsAcceptedName]; }
        }
        public ILookupParameter<IntValue> ConsecutiveRejectedSolutionsCountParameter
        {
            get { return (ILookupParameter<IntValue>)Parameters[ConsecutiveRejectedSolutionsCountName]; }
        }
        public ILookupParameter<IntValue> RandomWalkLengthNameParameter
        {
            get { return (ILookupParameter<IntValue>)Parameters[RandomWalkLengthName]; }
        }
        public ILookupParameter<IntValue> CurrentRandomWalkStepParameter
        {
            get { return (ILookupParameter<IntValue>)Parameters[CurrentRandomWalkStepName]; }
        }
        public ILookupParameter<DoubleValue> TemperatureBeforeReheatParameter
        {
            get { return (ILookupParameter<DoubleValue>)Parameters[TemperatureBeforeReheatName]; }
        }
        private LookupParameter<ItemList<DoubleValue>> QualitiesBeforeReheatingParameter
        {
            get { return (LookupParameter<ItemList<DoubleValue>>)Parameters[QualitiesBeforeReheatingName]; }
        }
        public ILookupParameter<BoolValue> CoolingParameter
        {
            get { return (ILookupParameter<BoolValue>)Parameters[CoolingName]; }
        }
        public ILookupParameter<DoubleValue> MoveQualityParameter
        {
            get { return (ILookupParameter<DoubleValue>)Parameters[MoveQualityName]; }
        }
        public ILookupParameter<DoubleValue> LastAcceptedQualityParameter
        {
            get { return (ILookupParameter<DoubleValue>)Parameters[LastAcceptedQualityName]; }
        }
        public IValueParameter<BoolValue> TabuListActiveParameter
        {
            get { return (IValueParameter<BoolValue>)Parameters[TabuListActiveName];  }
        }

        #endregion

        public RandomWalkReheatingOperator() : base()
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
            Parameters.Add(new LookupParameter<IntValue>(ConsecutiveRejectedSolutionsCountName, "Amount of consecutive rejected solutions."));
            Parameters.Add(new ValueParameter<IntValue>(ThresholdName, "How many consecutive rejected solutions must occur to trigger a reheat.", new IntValue(10)));
            Parameters.Add(new ValueParameter<IntValue>(RandomWalkLengthName, "Amount of randomly accepted moves upon reheat.", new IntValue(2)));
            Parameters.Add(new LookupParameter<IntValue>(CurrentRandomWalkStepName, "Current random walk step."));
            Parameters.Add(new LookupParameter<DoubleValue>(TemperatureBeforeReheatName, "Temperature before the reheat occured."));
            Parameters.Add(new LookupParameter<ItemList<DoubleValue>>(QualitiesBeforeReheatingName, "Quality of last optimum."));
            Parameters.Add(new LookupParameter<DoubleValue>(MoveQualityName, "The value which represents the quality of a move."));
            Parameters.Add(new LookupParameter<BoolValue>(CoolingName, "True when the temperature should be cooled, false otherwise."));
            Parameters.Add(new LookupParameter<DoubleValue>(LastAcceptedQualityName, "Quality of last accepted solution."));
            Parameters.Add(new ValueParameter<BoolValue>(TabuListActiveName, "Determines whether visiting a 'frozen' quality instantly triggers a reheat or not", new BoolValue(true)));

            #endregion

            Parameterize();
        }

        [StorableConstructor]
        protected RandomWalkReheatingOperator(bool deserializing) : base(deserializing) { }
        protected RandomWalkReheatingOperator(RandomWalkReheatingOperator original, Cloner cloner) : base(original, cloner) { }


        public override IDeepCloneable Clone(Cloner cloner)
        {
            return new RandomWalkReheatingOperator(this, cloner);
        }

        public void Parameterize()
        {
        }

        public override IOperation Apply()
        {
            var isAccepted = IsAcceptedParameter.ActualValue.Value;
            var consecutiveRejectedCount = ConsecutiveRejectedSolutionsCountParameter.ActualValue;
            var cooling = CoolingParameter.ActualValue.Value;
            var frozenSolutions = QualitiesBeforeReheatingParameter.ActualValue;
            var quality = MoveQualityParameter.ActualValue;
            var lastAcceptedQuality = LastAcceptedQualityParameter.ActualValue;
            var tabuListActive = TabuListActiveParameter.Value.Value;

            if(isAccepted)
            {
                LastAcceptedQualityParameter.ActualValue.Value = quality.Value;
            }

            if (cooling)
            { 
                // check if we are re-trapped
                if (isAccepted && frozenSolutions.Any(frozen => frozen.Value.Equals(quality.Value)) && tabuListActive)
                {
                   cooling = false;

                    IntValue retrapped = new IntValue(0);
                    IResult retrappedAmount;
                    var results = (ResultCollection)ExecutionContext.Parent.Parent.Scope.Variables[ResultsName].Value;
                    if (results.TryGetValue(RetrappedName, out retrappedAmount))
                    {
                        retrapped = (IntValue)retrappedAmount.Value;
                        retrapped.Value += 1;
                    }
                    else
                    {
                        retrapped.Value += 1;
                        results.Add(new Result(RetrappedName, retrapped));
                    }
                    // remember local optimum
                    frozenSolutions.Add(new DoubleValue(lastAcceptedQuality.Value));
                }
                else
                {
                    // add acceptance value to consecutive rejected solutions count
                    consecutiveRejectedCount.Value = !IsAcceptedParameter.ActualValue.Value ? consecutiveRejectedCount.Value + 1 : 0;

                    // check if we are trapped in a new local optimum
                    if (consecutiveRejectedCount.Value == ThresholdParameter.Value.Value)
                    {
                        cooling = false;
                        consecutiveRejectedCount.Value = 0;
                        // remember local optimum
                        frozenSolutions.Add(new DoubleValue(lastAcceptedQuality.Value));
                    }
                }
            }
            else
            {
                // random walk not yet finished
                if (RandomWalkLengthParameter.Value.Value != CurrentRandomWalkStepParameter.ActualValue.Value)
                {
                    if (CurrentRandomWalkStepParameter.ActualValue.Value == 0)
                    {
                        DebugIncreaseReheatCounter();
                    }
                    CurrentRandomWalkStepParameter.ActualValue.Value++;
                }
                else
                {
                    // random walk finished start cooling again
                    cooling = true;
                    TemperatureStartIndexParameter.ActualValue.Value = Math.Max(0, IterationsParameter.ActualValue.Value - 1);
                    StartTemperatureParameter.ActualValue.Value = TemperatureBeforeReheatParameter.ActualValue.Value;
                    EndTemperatureParameter.ActualValue.Value = LowerTemperatureParameter.ActualValue.Value;
                    consecutiveRejectedCount.Value = isAccepted ? 0 : 1;
                    CurrentRandomWalkStepParameter.ActualValue.Value = 0;

                }
            }
            CoolingParameter.ActualValue.Value = cooling;
            return cooling ? Cool() : Heat();
        }

        private void DebugIncreaseReheatCounter()
        {
            var reheats = new IntValue(0);
            var frozen = new ItemList<DoubleValue>();
            var results = (ResultCollection)ExecutionContext.Parent.Parent.Scope.Variables[ResultsName].Value;
            IResult amountOfReheats;
            IResult frozenSolutions;
            if (results.TryGetValue(AmountOfReheatsName, out amountOfReheats))
            {
                reheats = (IntValue)amountOfReheats.Value;
                reheats.Value += 1;
            }
            else
            {
                reheats.Value += 1;
                results.Add(new Result(AmountOfReheatsName, reheats));
            }

            if (results.TryGetValue(QualitiesBeforeReheatingName, out frozenSolutions))
            {
                
                frozen = (ItemList<DoubleValue>) frozenSolutions.Value;
                frozen.Add(new DoubleValue(LastAcceptedQualityParameter.ActualValue.Value));
            }
            else
            {
                frozen.Add(new DoubleValue(LastAcceptedQualityParameter.ActualValue.Value));
                results.Add(new Result(QualitiesBeforeReheatingName, frozen));
            }
        }

        private IOperation Heat()
        {
            TemperatureParameter.ActualValue.Value = Double.PositiveInfinity;
            return base.Apply();
        }

        private IOperation Cool()
        {
            TemperatureBeforeReheatParameter.ActualValue.Value = TemperatureParameter.ActualValue.Value;
            return new OperationCollection {
                    ExecutionContext.CreateOperation(AnnealingOperatorParameter.ActualValue),
                    base.Apply()
                };
        }
    }
}

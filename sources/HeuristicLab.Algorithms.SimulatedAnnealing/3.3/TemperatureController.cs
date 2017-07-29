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
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Optimization;

namespace HeuristicLab.Algorithms.SimulatedAnnealing
{
    [Item("TemperatureController", "Responsible to first initialize the temperature and then control the temperature.")]
    [StorableClass]
    public class TemperatureController : SingleSuccessorOperator
    {
        #region Strings
        private const string ReheatingOperatorName = "ReheatingOperator";
        private const string AnnealingOperatorName = "AnnealingOperator";
        private const string MaximumIterationsName = "MaximumIterations";
        private const string InitialTemperatureName = "InitialTemperature";
        private const string LowerTemperatureName = "LowerTemperature";
        private const string IterationsName = "Iterations";
        private const string TemperatureStartIndexName = "TemperatureStartIndex";
        private const string CoolingName = "Cooling";
        private const string StartTemperatureName = "StartTemperature";
        private const string EndTemperatureName = "EndTemperature";
        private const string TemperatureName = "Temperature";
        private const string IsAcceptedName = "IsAccepted";
        private const string ConsecutiveRejectedSolutionsCountName = "ConsecutiveRejectedSolutions";
        private const string AverageAcceptanceRatioName = "AverageAcceptanceRatio";
        private const string AcceptanceMemoryName = "AcceptanceMemory";
        private const string LastQualityName = "LastQuality";
        private const string UphillMovesMemoryName = "UphillMovesMemory";
        private const string MaximizationName = "Maximization";
        private const string MoveQualityName = "MoveQuality";
        private const string TemperatureBeforeReheatName = "TemperatureBeforeReheat";
        private const string CurrentRandomWalkStepName = "CurrentRandomWalkStep";
        private const string QualitiesBeforeReheatingName = "QualitiesBeforeReheating";
        private const string LastAcceptedQualityName = "LastAcceptedQuality";
        private const string TemperatureInitializerName = "TemperatureInitializer";
        private const string TemperatureInitializedName = "TemperatureInitialized";
        #endregion

        #region Parameter Properties
        public IValueLookupParameter<IReheatingOperator> ReheatingOperatorParameter
        {
            get { return (IValueLookupParameter<IReheatingOperator>)Parameters[ReheatingOperatorName]; }
        }
        public IValueLookupParameter<ITemperatureInitializer> TemperatureInitializerParameter
        {
            get { return (IValueLookupParameter<ITemperatureInitializer>)Parameters[TemperatureInitializerName]; }
        }
        public ILookupParameter<BoolValue> TemperatureInitializedParameter
        {
            get { return (ILookupParameter<BoolValue>)Parameters[TemperatureInitializedName]; }
        }
        #endregion

        [StorableConstructor]
        protected TemperatureController(bool deserializing) : base(deserializing) { }
        protected TemperatureController(TemperatureController original, Cloner cloner) : base(original, cloner) { }
        public TemperatureController()
          : base()
        {
            Parameters.Add(new LookupParameter<DoubleValue>(TemperatureName, "The current temperature."));
            Parameters.Add(new ValueLookupParameter<DoubleValue>(LowerTemperatureName, "The lower bound of the temperature."));
            Parameters.Add(new LookupParameter<IntValue>(IterationsName, "The number of iterations."));
            Parameters.Add(new ValueLookupParameter<IntValue>(MaximumIterationsName, "The maximum number of iterations which should be processed."));
            Parameters.Add(new ValueLookupParameter<BoolValue>(MaximizationName, "True if the problem is a maximization problem, otherwise false."));
            Parameters.Add(new ValueLookupParameter<IOperator>(AnnealingOperatorName, "The operator that cools the temperature."));
            Parameters.Add(new ValueLookupParameter<IReheatingOperator>(ReheatingOperatorName, "The operator that reheats the temperature if necessary."));
            Parameters.Add(new LookupParameter<IntValue>(TemperatureStartIndexName, "The index where the annealing or heating was last changed."));
            Parameters.Add(new LookupParameter<BoolValue>(CoolingName, "True when the temperature should be cooled, false otherwise."));
            Parameters.Add(new LookupParameter<DoubleValue>(StartTemperatureName, "The temperature from which cooling or reheating should occur."));
            Parameters.Add(new LookupParameter<DoubleValue>(EndTemperatureName, "The temperature to which should be cooled or heated."));
            Parameters.Add(new LookupParameter<BoolValue>(IsAcceptedName, "Whether the move was accepted or not."));
            Parameters.Add(new LookupParameter<ItemList<BoolValue>>(AcceptanceMemoryName, "Memorizes the last N acceptance decisions."));
            Parameters.Add(new LookupParameter<DoubleValue>(InitialTemperatureName, "The initial temperature."));

            Parameters.Add(new LookupParameter<ItemList<DoubleValue>>(UphillMovesMemoryName, "Memorizes the last 100 uphill moves."));
            Parameters.Add(new LookupParameter<DoubleValue>(MoveQualityName, "The value which represents the quality of a move."));
            Parameters.Add(new LookupParameter<DoubleValue>(LastQualityName, "The value which represents the quality of the last move."));
            Parameters.Add(new LookupParameter<IntValue>(ConsecutiveRejectedSolutionsCountName, "Amount of consecutive rejected solutions."));
            Parameters.Add(new LookupParameter<DoubleValue>(AverageAcceptanceRatioName, "Average acceptance over full acceptance memory."));
            Parameters.Add(new LookupParameter<IntValue>(CurrentRandomWalkStepName, "Current random walk step."));
            Parameters.Add(new LookupParameter<DoubleValue>(TemperatureBeforeReheatName, "Temperature before the reheat occured."));
            Parameters.Add(new LookupParameter<ItemList<DoubleValue>>(QualitiesBeforeReheatingName, "List of qualities where the algorithm has been stuck."));
            Parameters.Add(new LookupParameter<DoubleValue>(LastAcceptedQualityName, "Quality of last accepted solution."));

            Parameters.Add(new ValueLookupParameter<ITemperatureInitializer>(TemperatureInitializerName, "The operator that initilized the temperature."));
            Parameters.Add(new LookupParameter<BoolValue>(TemperatureInitializedName, "True, if the temperature has already been initialized."));

        }

        public override IDeepCloneable Clone(Cloner cloner)
        {
            return new TemperatureController(this, cloner);
        }

        public override IOperation Apply()
        {

            if (!TemperatureInitializedParameter.ActualValue.Value)
            {
                return new OperationCollection
                {
                    ExecutionContext.CreateOperation(TemperatureInitializerParameter.ActualValue),
                    base.Apply()
                };
            }
            else
            {
                return new OperationCollection {
                    ExecutionContext.CreateOperation(ReheatingOperatorParameter.ActualValue),
                    base.Apply()
                };
            }
        }
    }
}
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
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Optimization.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.VehicleRouting.Interfaces;
using HeuristicLab.Problems.VehicleRouting.Variants;

namespace HeuristicLab.Problems.VehicleRouting {
  /// <summary>
  /// An operator which analyzes the best, average and worst quality of solutions in the scope tree.
  /// </summary>
  [Item("BestAverageWorstPickupAndDeliveryVRPToursAnalyzer", "An operator which analyzes the best, average and worst properties of the VRP tours in the scope tree.")]
  [StorableClass]
  public sealed class BestAverageWorstPickupAndDeliveryVRPToursAnalyzer : AlgorithmOperator, IAnalyzer, IPickupAndDeliveryOperator {
    #region Parameter properties
    public ILookupParameter<IVRPProblemInstance> ProblemInstanceParameter {
      get { return (ILookupParameter<IVRPProblemInstance>)Parameters["ProblemInstance"]; }
    }
    public ScopeTreeLookupParameter<IntValue> PickupViolationsParameter {
      get { return (ScopeTreeLookupParameter<IntValue>)Parameters["PickupViolations"]; }
    }
    public ValueLookupParameter<IntValue> BestPickupViolationsParameter {
      get { return (ValueLookupParameter<IntValue>)Parameters["BestPickupViolations"]; }
    }
    public ValueLookupParameter<IntValue> CurrentBestPickupViolationsParameter {
      get { return (ValueLookupParameter<IntValue>)Parameters["CurrentBestPickupViolations"]; }
    }
    public ValueLookupParameter<DoubleValue> CurrentAveragePickupViolationsParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["CurrentAveragePickupViolations"]; }
    }
    public ValueLookupParameter<IntValue> CurrentWorstPickupViolationsParameter {
      get { return (ValueLookupParameter<IntValue>)Parameters["CurrentWorstPickupViolations"]; }
    }
    public ValueLookupParameter<DataTable> PickupViolationsValuesParameter {
      get { return (ValueLookupParameter<DataTable>)Parameters["PickupViolationsValues"]; }
    }

    public ValueLookupParameter<VariableCollection> ResultsParameter {
      get { return (ValueLookupParameter<VariableCollection>)Parameters["Results"]; }
    }
    #endregion

    #region Properties
    public bool EnabledByDefault {
      get { return true; }
    }
    private BestPickupAndDeliveryVRPToursMemorizer BestMemorizer {
      get { return (BestPickupAndDeliveryVRPToursMemorizer)OperatorGraph.InitialOperator; }
    }
    private BestAverageWorstPickupAndDeliveryVRPToursCalculator BestAverageWorstCalculator {
      get { return (BestAverageWorstPickupAndDeliveryVRPToursCalculator)BestMemorizer.Successor; }
    }
    #endregion

    public BestAverageWorstPickupAndDeliveryVRPToursAnalyzer()
      : base() {
      #region Create parameters
      Parameters.Add(new LookupParameter<IVRPProblemInstance>("ProblemInstance", "The problem instance."));

      Parameters.Add(new ScopeTreeLookupParameter<IntValue>("PickupViolations", "The pickup violations of the VRP solutions which should be analyzed."));
      Parameters.Add(new ValueLookupParameter<IntValue>("BestPickupViolations", "The best pickup violations value."));
      Parameters.Add(new ValueLookupParameter<IntValue>("CurrentBestPickupViolations", "The current best pickup violations value."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("CurrentAveragePickupViolations", "The current average pickup violations value of all solutions."));
      Parameters.Add(new ValueLookupParameter<IntValue>("CurrentWorstPickupViolations", "The current worst pickup violations value of all solutions."));
      Parameters.Add(new ValueLookupParameter<DataTable>("PickupViolationsValues", "The data table to store the current best, current average, current worst, best and best known pickup violations value."));

      Parameters.Add(new ValueLookupParameter<VariableCollection>("Results", "The results collection where the analysis values should be stored."));
      #endregion

      #region Create operators
      BestPickupAndDeliveryVRPToursMemorizer bestMemorizer = new BestPickupAndDeliveryVRPToursMemorizer();
      BestAverageWorstPickupAndDeliveryVRPToursCalculator calculator = new BestAverageWorstPickupAndDeliveryVRPToursCalculator();
      ResultsCollector resultsCollector = new ResultsCollector();

      //pickup violations
      bestMemorizer.BestPickupViolationsParameter.ActualName = BestPickupViolationsParameter.Name;
      bestMemorizer.PickupViolationsParameter.ActualName = PickupViolationsParameter.Name;
      bestMemorizer.PickupViolationsParameter.Depth = PickupViolationsParameter.Depth;

      calculator.PickupViolationsParameter.ActualName = PickupViolationsParameter.Name;
      calculator.PickupViolationsParameter.Depth = PickupViolationsParameter.Depth;
      calculator.BestPickupViolationsParameter.ActualName = CurrentBestPickupViolationsParameter.Name;
      calculator.AveragePickupViolationsParameter.ActualName = CurrentAveragePickupViolationsParameter.Name;
      calculator.WorstPickupViolationsParameter.ActualName = CurrentWorstPickupViolationsParameter.Name;

      DataTableValuesCollector pickupViolationsDataTablesCollector = new DataTableValuesCollector();
      pickupViolationsDataTablesCollector.CollectedValues.Add(new LookupParameter<IntValue>("BestPickupViolations", null, BestPickupViolationsParameter.Name));
      pickupViolationsDataTablesCollector.CollectedValues.Add(new LookupParameter<IntValue>("CurrentBestPickupViolations", null, CurrentBestPickupViolationsParameter.Name));
      pickupViolationsDataTablesCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("CurrentAveragePickupViolations", null, CurrentAveragePickupViolationsParameter.Name));
      pickupViolationsDataTablesCollector.CollectedValues.Add(new LookupParameter<IntValue>("CurrentWorstPickupViolations", null, CurrentWorstPickupViolationsParameter.Name));
      pickupViolationsDataTablesCollector.DataTableParameter.ActualName = PickupViolationsValuesParameter.Name;

      resultsCollector.CollectedValues.Add(new LookupParameter<DataTable>(PickupViolationsValuesParameter.Name));
      #endregion

      #region Create operator graph
      OperatorGraph.InitialOperator = bestMemorizer;
      bestMemorizer.Successor = calculator;
      calculator.Successor = pickupViolationsDataTablesCollector;
      pickupViolationsDataTablesCollector.Successor = resultsCollector;
      resultsCollector.Successor = null;
      #endregion

      Initialize();
    }
    [StorableConstructor]
    private BestAverageWorstPickupAndDeliveryVRPToursAnalyzer(bool deserializing) : base() { }

    [StorableHook(HookType.AfterDeserialization)]
    private void Initialize() {
      PickupViolationsParameter.DepthChanged += new EventHandler(PickupViolationsParameter_DepthChanged);
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new BestAverageWorstPickupAndDeliveryVRPToursAnalyzer(this, cloner);
    }

    private BestAverageWorstPickupAndDeliveryVRPToursAnalyzer(BestAverageWorstPickupAndDeliveryVRPToursAnalyzer original, Cloner cloner)
      : base(original, cloner) {
      this.Initialize();
    }

    void PickupViolationsParameter_DepthChanged(object sender, EventArgs e) {
      BestAverageWorstCalculator.PickupViolationsParameter.Depth = PickupViolationsParameter.Depth;
      BestMemorizer.PickupViolationsParameter.Depth = PickupViolationsParameter.Depth;
    }
  }
}

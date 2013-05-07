#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  [Item("BestAverageWorstCapaciatatedVRPToursAnalyzer", "An operator which analyzes the best, average and worst properties of the VRP tours in the scope tree.")]
  [StorableClass]
  public sealed class BestAverageWorstCapaciatatedVRPToursAnalyzer : AlgorithmOperator, IAnalyzer, IHomogenousCapacitatedOperator {
    #region Parameter properties
    public ILookupParameter<IVRPProblemInstance> ProblemInstanceParameter {
      get { return (ILookupParameter<IVRPProblemInstance>)Parameters["ProblemInstance"]; }
    }
    public ScopeTreeLookupParameter<DoubleValue> OverloadParameter {
      get { return (ScopeTreeLookupParameter<DoubleValue>)Parameters["Overload"]; }
    }
    public ValueLookupParameter<DoubleValue> BestOverloadParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["BestOverload"]; }
    }
    public ValueLookupParameter<DoubleValue> CurrentBestOverloadParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["CurrentBestOverload"]; }
    }
    public ValueLookupParameter<DoubleValue> CurrentAverageOverloadParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["CurrentAverageOverload"]; }
    }
    public ValueLookupParameter<DoubleValue> CurrentWorstOverloadParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["CurrentWorstOverload"]; }
    }
    public ValueLookupParameter<DataTable> OverloadsParameter {
      get { return (ValueLookupParameter<DataTable>)Parameters["Overloads"]; }
    }

    public ValueLookupParameter<VariableCollection> ResultsParameter {
      get { return (ValueLookupParameter<VariableCollection>)Parameters["Results"]; }
    }
    #endregion

    #region Properties
    public bool EnabledByDefault {
      get { return true; }
    }
    private BestCapacitatedVRPToursMemorizer BestMemorizer {
      get { return (BestCapacitatedVRPToursMemorizer)OperatorGraph.InitialOperator; }
    }
    private BestAverageWorstCapacitatedVRPToursCalculator BestAverageWorstCalculator {
      get { return (BestAverageWorstCapacitatedVRPToursCalculator)BestMemorizer.Successor; }
    }
    #endregion

    public BestAverageWorstCapaciatatedVRPToursAnalyzer()
      : base() {
      #region Create parameters
      Parameters.Add(new LookupParameter<IVRPProblemInstance>("ProblemInstance", "The problem instance."));

      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("Overload", "The overloads of the VRP solutions which should be analyzed."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("BestOverload", "The best overload value."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("CurrentBestOverload", "The current best overload value."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("CurrentAverageOverload", "The current average overload value of all solutions."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("CurrentWorstOverload", "The current worst overload value of all solutions."));
      Parameters.Add(new ValueLookupParameter<DataTable>("Overloads", "The data table to store the current best, current average, current worst, best and best known overload value."));

      Parameters.Add(new ValueLookupParameter<VariableCollection>("Results", "The results collection where the analysis values should be stored."));
      #endregion

      #region Create operators
      BestCapacitatedVRPToursMemorizer bestMemorizer = new BestCapacitatedVRPToursMemorizer();
      BestAverageWorstCapacitatedVRPToursCalculator calculator = new BestAverageWorstCapacitatedVRPToursCalculator();
      ResultsCollector resultsCollector = new ResultsCollector();

      //overload
      bestMemorizer.BestOverloadParameter.ActualName = BestOverloadParameter.Name;
      bestMemorizer.OverloadParameter.ActualName = OverloadParameter.Name;
      bestMemorizer.OverloadParameter.Depth = OverloadParameter.Depth;

      calculator.OverloadParameter.ActualName = OverloadParameter.Name;
      calculator.OverloadParameter.Depth = OverloadParameter.Depth;
      calculator.BestOverloadParameter.ActualName = CurrentBestOverloadParameter.Name;
      calculator.AverageOverloadParameter.ActualName = CurrentAverageOverloadParameter.Name;
      calculator.WorstOverloadParameter.ActualName = CurrentWorstOverloadParameter.Name;

      DataTableValuesCollector overloadDataTablesCollector = new DataTableValuesCollector();
      overloadDataTablesCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("BestOverload", null, BestOverloadParameter.Name));
      overloadDataTablesCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("CurrentBestOverload", null, CurrentBestOverloadParameter.Name));
      overloadDataTablesCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("CurrentAverageOverload", null, CurrentAverageOverloadParameter.Name));
      overloadDataTablesCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("CurrentWorstOverload", null, CurrentWorstOverloadParameter.Name));
      overloadDataTablesCollector.DataTableParameter.ActualName = OverloadsParameter.Name;

      resultsCollector.CollectedValues.Add(new LookupParameter<DataTable>(OverloadsParameter.Name));
      #endregion

      #region Create operator graph
      OperatorGraph.InitialOperator = bestMemorizer;
      bestMemorizer.Successor = calculator;
      calculator.Successor = overloadDataTablesCollector;
      overloadDataTablesCollector.Successor = resultsCollector;
      resultsCollector.Successor = null;
      #endregion

      Initialize();
    }
    [StorableConstructor]
    private BestAverageWorstCapaciatatedVRPToursAnalyzer(bool deserializing) : base() { }

    [StorableHook(HookType.AfterDeserialization)]
    private void Initialize() {
      OverloadParameter.DepthChanged += new EventHandler(OverloadParameter_DepthChanged);
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new BestAverageWorstCapaciatatedVRPToursAnalyzer(this, cloner);
    }

    private BestAverageWorstCapaciatatedVRPToursAnalyzer(BestAverageWorstCapaciatatedVRPToursAnalyzer original, Cloner cloner)
      : base(original, cloner) {
      this.Initialize();
    }

    void OverloadParameter_DepthChanged(object sender, EventArgs e) {
      BestAverageWorstCalculator.OverloadParameter.Depth = OverloadParameter.Depth;
      BestMemorizer.OverloadParameter.Depth = OverloadParameter.Depth;
    }
  }
}

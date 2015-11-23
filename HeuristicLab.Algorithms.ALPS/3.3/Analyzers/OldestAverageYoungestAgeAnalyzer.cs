#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Algorithms.ALPS {
  [Item("OldestAverageYoungestAgeAnalyzer", "An operator which calculates the current oldest, average and youngest age in the scope tree.")]
  [StorableClass]
  public sealed class OldestAverageYoungestAgeAnalyzer : AlgorithmOperator, IAnalyzer {
    #region Parameter properties
    public IScopeTreeLookupParameter<DoubleValue> AgeParameter {
      get { return (IScopeTreeLookupParameter<DoubleValue>)Parameters["Age"]; }
    }
    public IValueLookupParameter<DoubleValue> CurrentOldestAgeParameter {
      get { return (IValueLookupParameter<DoubleValue>)Parameters["CurrentOldestAge"]; }
    }
    public IValueLookupParameter<DoubleValue> CurrentAverageAgeParameter {
      get { return (IValueLookupParameter<DoubleValue>)Parameters["CurrentAverageAge"]; }
    }
    public IValueLookupParameter<DoubleValue> CurrentYoungestAgeParameter {
      get { return (IValueLookupParameter<DoubleValue>)Parameters["CurrentYoungestAge"]; }
    }
    public IValueLookupParameter<DataTable> AgesParameter {
      get { return (IValueLookupParameter<DataTable>)Parameters["Ages"]; }
    }
    public IValueLookupParameter<ResultCollection> ResultsParameter {
      get { return (IValueLookupParameter<ResultCollection>)Parameters["Results"]; }
    }
    #endregion

    #region Properties
    public bool EnabledByDefault {
      get { return false; }
    }
    private OldestAverageYoungestAgeCalculator OldestAverageYoungestAgeCalculator {
      get { return (OldestAverageYoungestAgeCalculator)OperatorGraph.InitialOperator; }
    }
    #endregion

    #region Storing & Cloning
    [StorableConstructor]
    private OldestAverageYoungestAgeAnalyzer(bool deserializing) : base(deserializing) { }
    private OldestAverageYoungestAgeAnalyzer(OldestAverageYoungestAgeAnalyzer original, Cloner cloner)
      : base(original, cloner) {
      Initialize();
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new OldestAverageYoungestAgeAnalyzer(this, cloner);
    }
    #endregion
    public OldestAverageYoungestAgeAnalyzer()
      : base() {
      #region Create parameters
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("Age", "The value which represents the age of a solution."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("CurrentOldestAge", "The oldest age value found in the current population."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("CurrentAverageAge", "The average age value of all solutions in the current population."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("CurrentYoungestAge", "The youngest age value found in the current population."));
      Parameters.Add(new ValueLookupParameter<DataTable>("Ages", "The data table to store the current oldest, current average, current youngest age value."));
      Parameters.Add(new ValueLookupParameter<ResultCollection>("Results", "The results collection where the analysis values should be stored."));

      CurrentOldestAgeParameter.Hidden = true;
      CurrentAverageAgeParameter.Hidden = true;
      CurrentYoungestAgeParameter.Hidden = true;
      AgesParameter.Hidden = true;
      #endregion

      #region Create operators
      var oldestAverageYoungestAgeCalculator = new OldestAverageYoungestAgeCalculator();
      var dataTableValuesCollector = new DataTableValuesCollector();
      var resultsCollector = new ResultsCollector();

      oldestAverageYoungestAgeCalculator.AverageAgeParameter.ActualName = CurrentAverageAgeParameter.Name;
      oldestAverageYoungestAgeCalculator.OldestAgeParameter.ActualName = CurrentOldestAgeParameter.Name;
      oldestAverageYoungestAgeCalculator.AgeParameter.ActualName = AgeParameter.Name;
      oldestAverageYoungestAgeCalculator.AgeParameter.Depth = AgeParameter.Depth;
      oldestAverageYoungestAgeCalculator.YoungestAgeParameter.ActualName = CurrentYoungestAgeParameter.Name;

      dataTableValuesCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("CurrentOldestAge", null, CurrentOldestAgeParameter.Name));
      dataTableValuesCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("CurrentAverageAge", null, CurrentAverageAgeParameter.Name));
      dataTableValuesCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("CurrentYoungestAge", null, CurrentYoungestAgeParameter.Name));
      dataTableValuesCollector.DataTableParameter.ActualName = AgesParameter.Name;

      resultsCollector.CollectedValues.Add(new LookupParameter<DataTable>(AgesParameter.Name));
      resultsCollector.ResultsParameter.ActualName = ResultsParameter.Name;
      #endregion

      #region Create operator graph
      OperatorGraph.InitialOperator = oldestAverageYoungestAgeCalculator;
      oldestAverageYoungestAgeCalculator.Successor = dataTableValuesCollector;
      dataTableValuesCollector.Successor = resultsCollector;
      resultsCollector.Successor = null;
      #endregion

      Initialize();
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      Initialize();
    }

    private void Initialize() {
      AgeParameter.DepthChanged += new EventHandler(AgeParameter_DepthChanged);
    }

    private void AgeParameter_DepthChanged(object sender, EventArgs e) {
      OldestAverageYoungestAgeCalculator.AgeParameter.Depth = AgeParameter.Depth;
    }
  }
}
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
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Collections;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.DataAnalysis {
  [StorableClass]
  public abstract class DataAnalysisProblemData : ParameterizedNamedItem, IDataAnalysisProblemData {
    protected const string DatasetParameterName = "Dataset";
    protected const string InputVariablesParameterName = "InputVariables";
    protected const string TrainingPartitionParameterName = "TrainingPartition";
    protected const string TestPartitionParameterName = "TestPartition";
    protected const string DatasetCorrelationParameterName = "Dataset Correlation";

    #region parameter properites
    public IFixedValueParameter<Dataset> DatasetParameter {
      get { return (IFixedValueParameter<Dataset>)Parameters[DatasetParameterName]; }
    }
    public IFixedValueParameter<ReadOnlyCheckedItemList<StringValue>> InputVariablesParameter {
      get { return (IFixedValueParameter<ReadOnlyCheckedItemList<StringValue>>)Parameters[InputVariablesParameterName]; }
    }
    public IFixedValueParameter<IntRange> TrainingPartitionParameter {
      get { return (IFixedValueParameter<IntRange>)Parameters[TrainingPartitionParameterName]; }
    }
    public IFixedValueParameter<IntRange> TestPartitionParameter {
      get { return (IFixedValueParameter<IntRange>)Parameters[TestPartitionParameterName]; }
    }
    #endregion

    #region properties
    protected bool isEmpty = false;
    public bool IsEmpty {
      get { return isEmpty; }
    }
    public Dataset Dataset {
      get { return DatasetParameter.Value; }
    }
    public ICheckedItemList<StringValue> InputVariables {
      get { return InputVariablesParameter.Value; }
    }
    public IEnumerable<string> AllowedInputVariables {
      get { return InputVariables.CheckedItems.Select(x => x.Value.Value); }
    }

    public IntRange TrainingPartition {
      get { return TrainingPartitionParameter.Value; }
    }
    public IntRange TestPartition {
      get { return TestPartitionParameter.Value; }
    }

    public virtual IEnumerable<int> TrainingIndices {
      get {
        return Enumerable.Range(TrainingPartition.Start, Math.Max(0, TrainingPartition.End - TrainingPartition.Start))
                         .Where(IsTrainingSample);
      }
    }
    public virtual IEnumerable<int> TestIndices {
      get {
        return Enumerable.Range(TestPartition.Start, Math.Max(0, TestPartition.End - TestPartition.Start))
           .Where(IsTestSample);
      }
    }

    public virtual bool IsTrainingSample(int index) {
      return index >= 0 && index < Dataset.Rows &&
        TrainingPartition.Start <= index && index < TrainingPartition.End &&
        (index < TestPartition.Start || TestPartition.End <= index);
    }

    public virtual bool IsTestSample(int index) {
      return index >= 0 && index < Dataset.Rows &&
             TestPartition.Start <= index && index < TestPartition.End;
    }
    #endregion

    protected DataAnalysisProblemData(DataAnalysisProblemData original, Cloner cloner)
      : base(original, cloner) {
      isEmpty = original.isEmpty;
      RegisterEventHandlers();
    }
    [StorableConstructor]
    protected DataAnalysisProblemData(bool deserializing) : base(deserializing) { }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterEventHandlers();
    }

    protected DataAnalysisProblemData(Dataset dataset, IEnumerable<string> allowedInputVariables) {
      if (dataset == null) throw new ArgumentNullException("The dataset must not be null.");
      if (allowedInputVariables == null) throw new ArgumentNullException("The allowedInputVariables must not be null.");

      if (allowedInputVariables.Except(dataset.DoubleVariables).Any())
        throw new ArgumentException("All allowed input variables must be present in the dataset and of type double.");

      var inputVariables = new CheckedItemList<StringValue>(dataset.DoubleVariables.Select(x => new StringValue(x)));
      foreach (StringValue x in inputVariables)
        inputVariables.SetItemCheckedState(x, allowedInputVariables.Contains(x.Value));

      int trainingPartitionStart = 0;
      int trainingPartitionEnd = dataset.Rows / 2;
      int testPartitionStart = dataset.Rows / 2;
      int testPartitionEnd = dataset.Rows;

      Parameters.Add(new FixedValueParameter<Dataset>(DatasetParameterName, "", dataset));
      Parameters.Add(new FixedValueParameter<ReadOnlyCheckedItemList<StringValue>>(InputVariablesParameterName, "", inputVariables.AsReadOnly()));
      Parameters.Add(new FixedValueParameter<IntRange>(TrainingPartitionParameterName, "", new IntRange(trainingPartitionStart, trainingPartitionEnd)));
      Parameters.Add(new FixedValueParameter<IntRange>(TestPartitionParameterName, "", new IntRange(testPartitionStart, testPartitionEnd)));

      ((ValueParameter<Dataset>)DatasetParameter).ReactOnValueToStringChangedAndValueItemImageChanged = false;
      RegisterEventHandlers();
    }

    private void RegisterEventHandlers() {
      DatasetParameter.ValueChanged += new EventHandler(Parameter_ValueChanged);
      InputVariables.CheckedItemsChanged += new CollectionItemsChangedEventHandler<IndexedItem<StringValue>>(InputVariables_CheckedItemsChanged);
      TrainingPartition.ValueChanged += new EventHandler(Parameter_ValueChanged);
      TestPartition.ValueChanged += new EventHandler(Parameter_ValueChanged);
    }

    private void InputVariables_CheckedItemsChanged(object sender, CollectionItemsChangedEventArgs<IndexedItem<StringValue>> e) {
      OnChanged();
    }

    private void Parameter_ValueChanged(object sender, EventArgs e) {
      OnChanged();
    }

    public event EventHandler Changed;
    protected virtual void OnChanged() {
      var listeners = Changed;
      if (listeners != null) listeners(this, EventArgs.Empty);
    }
  }
}

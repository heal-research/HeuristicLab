#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Text;
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
    protected const string TransformationsParameterName = "Transformations";

    #region parameter properites
    //mkommend: inserted parameter caching due to performance reasons
    private IFixedValueParameter<Dataset> datasetParameter;
    public IFixedValueParameter<Dataset> DatasetParameter {
      get {
        if (datasetParameter == null) datasetParameter = (IFixedValueParameter<Dataset>)Parameters[DatasetParameterName];
        return datasetParameter;
      }
    }

    private IFixedValueParameter<ReadOnlyCheckedItemList<StringValue>> inputVariablesParameter;
    public IFixedValueParameter<ReadOnlyCheckedItemList<StringValue>> InputVariablesParameter {
      get {
        if (inputVariablesParameter == null) inputVariablesParameter = (IFixedValueParameter<ReadOnlyCheckedItemList<StringValue>>)Parameters[InputVariablesParameterName];
        return inputVariablesParameter;
      }
    }

    private IFixedValueParameter<IntRange> trainingPartitionParameter;
    public IFixedValueParameter<IntRange> TrainingPartitionParameter {
      get {
        if (trainingPartitionParameter == null) trainingPartitionParameter = (IFixedValueParameter<IntRange>)Parameters[TrainingPartitionParameterName];
        return trainingPartitionParameter;
      }
    }

    private IFixedValueParameter<IntRange> testPartitionParameter;
    public IFixedValueParameter<IntRange> TestPartitionParameter {
      get {
        if (testPartitionParameter == null) testPartitionParameter = (IFixedValueParameter<IntRange>)Parameters[TestPartitionParameterName];
        return testPartitionParameter;
      }
    }

    public IFixedValueParameter<ReadOnlyItemList<ITransformation>> TransformationsParameter {
      get { return (IFixedValueParameter<ReadOnlyItemList<ITransformation>>)Parameters[TransformationsParameterName]; }
    }
    #endregion

    #region properties
    protected bool isEmpty = false;
    public bool IsEmpty {
      get { return isEmpty; }
    }
    public IDataset Dataset {
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

    public virtual IEnumerable<int> AllIndices {
      get { return Enumerable.Range(0, Dataset.Rows); }
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

    public IEnumerable<ITransformation> Transformations {
      get { return TransformationsParameter.Value; }
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
      if (!Parameters.ContainsKey(TransformationsParameterName)) {
        Parameters.Add(new FixedValueParameter<ReadOnlyItemList<ITransformation>>(TransformationsParameterName, "", new ItemList<ITransformation>().AsReadOnly()));
        TransformationsParameter.Hidden = true;
      }
      RegisterEventHandlers();
    }

    protected DataAnalysisProblemData(IDataset dataset, IEnumerable<string> allowedInputVariables, IEnumerable<ITransformation> transformations = null) {
      if (dataset == null) throw new ArgumentNullException("The dataset must not be null.");
      if (allowedInputVariables == null) throw new ArgumentNullException("The allowed input variables must not be null.");

      if (allowedInputVariables.Except(dataset.DoubleVariables).Except(dataset.StringVariables).Any())
        throw new ArgumentException("All allowed input variables must be present in the dataset and of type double or string.");

      var variables = dataset.VariableNames.Where(variable => dataset.VariableHasType<double>(variable) || dataset.VariableHasType<string>(variable));
      var inputVariables = new CheckedItemList<StringValue>(variables.Select(x => new StringValue(x)));
      foreach (StringValue x in inputVariables)
        inputVariables.SetItemCheckedState(x, allowedInputVariables.Contains(x.Value));

      int trainingPartitionStart = 0;
      int trainingPartitionEnd = dataset.Rows / 2;
      int testPartitionStart = dataset.Rows / 2;
      int testPartitionEnd = dataset.Rows;

      var transformationsList = new ItemList<ITransformation>(transformations ?? Enumerable.Empty<ITransformation>());

      Parameters.Add(new FixedValueParameter<Dataset>(DatasetParameterName, "", (Dataset)dataset));
      Parameters.Add(new FixedValueParameter<ReadOnlyCheckedItemList<StringValue>>(InputVariablesParameterName, "", inputVariables.AsReadOnly()));
      Parameters.Add(new FixedValueParameter<IntRange>(TrainingPartitionParameterName, "", new IntRange(trainingPartitionStart, trainingPartitionEnd)));
      Parameters.Add(new FixedValueParameter<IntRange>(TestPartitionParameterName, "", new IntRange(testPartitionStart, testPartitionEnd)));
      Parameters.Add(new FixedValueParameter<ReadOnlyItemList<ITransformation>>(TransformationsParameterName, "", transformationsList.AsReadOnly()));

      TransformationsParameter.Hidden = true;

      ((ValueParameter<Dataset>)DatasetParameter).ReactOnValueToStringChangedAndValueItemImageChanged = false;
      RegisterEventHandlers();
    }

    private void RegisterEventHandlers() {
      DatasetParameter.ValueChanged += new EventHandler(Parameter_ValueChanged);
      InputVariables.CheckedItemsChanged += new CollectionItemsChangedEventHandler<IndexedItem<StringValue>>(InputVariables_CheckedItemsChanged);
      TrainingPartition.ValueChanged += new EventHandler(Parameter_ValueChanged);
      TestPartition.ValueChanged += new EventHandler(Parameter_ValueChanged);
      TransformationsParameter.ValueChanged += new EventHandler(Parameter_ValueChanged);
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

    protected virtual bool IsProblemDataCompatible(IDataAnalysisProblemData problemData, out string errorMessage) {
      errorMessage = string.Empty;
      if (problemData == null) throw new ArgumentNullException("problemData", "The provided problemData is null.");

      //check allowed input variables
      StringBuilder message = new StringBuilder();
      var variables = new HashSet<string>(problemData.InputVariables.Select(x => x.Value));
      foreach (var item in AllowedInputVariables) {
        if (!variables.Contains(item))
          message.AppendLine("Input variable '" + item + "' is not present in the new problem data.");
      }

      if (message.Length != 0) {
        errorMessage = message.ToString();
        return false;
      }
      return true;

    }

    public virtual void AdjustProblemDataProperties(IDataAnalysisProblemData problemData) {
      DataAnalysisProblemData data = problemData as DataAnalysisProblemData;
      if (data == null) throw new ArgumentException("The problem data is not a data analysis problem data. Instead a " + problemData.GetType().GetPrettyName() + " was provided.", "problemData");

      string errorMessage;
      if (!data.IsProblemDataCompatible(this, out errorMessage)) {
        throw new InvalidOperationException(errorMessage);
      }

      foreach (var inputVariable in InputVariables) {
        var variable = data.InputVariables.FirstOrDefault(i => i.Value == inputVariable.Value);
        InputVariables.SetItemCheckedState(inputVariable, variable != null && data.InputVariables.ItemChecked(variable));
      }
    }
  }
}

#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.IO;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Problems.DataAnalysis {
  [Item("DataAnalysisProblemData", "Represents an item containing all data defining a data analysis problem.")]
  [StorableClass]
  [NonDiscoverableType]
  public class DataAnalysisProblemData : ParameterizedNamedItem, IStorableContent {
    protected bool suppressEvents = false;
    #region IStorableContent Members
    public string Filename { get; set; }
    #endregion
    #region default data
    // y = x^4 + x^3 + x^2 + x
    private static double[,] kozaF1 = new double[,] {
{2.017885919,	-1.449165046},
{1.30060506,	-1.344523885},
{1.147134798,	-1.317989331},
{0.877182504,	-1.266142284},
{0.852562452,	-1.261020794},
{0.431095788,	-1.158793317},
{0.112586002,	-1.050908405},
{0.04594507,	-1.021989402},
{0.042572879,	-1.020438113},
{-0.074027291,	-0.959859562},
{-0.109178553,	-0.938094706},
{-0.259721109,	-0.803635355},
{-0.272991057,	-0.387519561},
{-0.161978191,	-0.193611001},
{-0.102489983,	-0.114215349},
{-0.01469968,	-0.014918985},
{-0.008863365,	-0.008942626},
{0.026751057,	0.026054094},
{0.166922436,	0.14309643},
{0.176953808,	0.1504144},
{0.190233418,	0.159916534},
{0.199800708,	0.166635331},
{0.261502822,	0.207600348},
{0.30182879,	0.232370249},
{0.83763905,	0.468046718}
    };
    #endregion
    #region parameter properties
    public ValueParameter<Dataset> DatasetParameter {
      get { return (ValueParameter<Dataset>)Parameters["Dataset"]; }
    }
    public IValueParameter<StringValue> TargetVariableParameter {
      get { return (IValueParameter<StringValue>)Parameters["TargetVariable"]; }
    }
    public IValueParameter<ICheckedItemList<StringValue>> InputVariablesParameter {
      get { return (IValueParameter<ICheckedItemList<StringValue>>)Parameters["InputVariables"]; }
    }
    public IValueParameter<IntValue> TrainingSamplesStartParameter {
      get { return (IValueParameter<IntValue>)Parameters["TrainingSamplesStart"]; }
    }
    public IValueParameter<IntValue> TrainingSamplesEndParameter {
      get { return (IValueParameter<IntValue>)Parameters["TrainingSamplesEnd"]; }
    }
    public IValueParameter<IntValue> TestSamplesStartParameter {
      get { return (IValueParameter<IntValue>)Parameters["TestSamplesStart"]; }
    }
    public IValueParameter<IntValue> TestSamplesEndParameter {
      get { return (IValueParameter<IntValue>)Parameters["TestSamplesEnd"]; }
    }
    public IValueParameter<PercentValue> ValidationPercentageParameter {
      get { return (IValueParameter<PercentValue>)Parameters["ValidationPercentage"]; }
    }
    #endregion

    #region properties
    public Dataset Dataset {
      get { return DatasetParameter.Value; }
      set {
        if (value != Dataset) {
          if (value == null) throw new ArgumentNullException();
          DatasetParameter.Value = value;
        }
      }
    }
    public StringValue TargetVariable {
      get { return TargetVariableParameter.Value; }
      set {
        if (value != TargetVariableParameter.Value) {
          if (value == null) throw new ArgumentNullException();
          if (TargetVariable != null) DeregisterStringValueEventHandlers(TargetVariable);
          TargetVariableParameter.Value = value;
        }
      }
    }
    public ICheckedItemList<StringValue> InputVariables {
      get { return InputVariablesParameter.Value; }
      set {
        if (value != InputVariables) {
          if (value == null) throw new ArgumentNullException();
          if (InputVariables != null) DeregisterInputVariablesEventHandlers();
          InputVariablesParameter.Value = value;
        }
      }
    }
    public IntValue TrainingSamplesStart {
      get { return TrainingSamplesStartParameter.Value; }
      set {
        if (value != TrainingSamplesStart) {
          if (value == null) throw new ArgumentNullException();
          if (TrainingSamplesStart != null) DeregisterValueTypeEventHandlers(TrainingSamplesStart);
          TrainingSamplesStartParameter.Value = value;
        }
      }
    }
    public IntValue TrainingSamplesEnd {
      get { return TrainingSamplesEndParameter.Value; }
      set {
        if (value != TrainingSamplesEnd) {
          if (value == null) throw new ArgumentNullException();
          if (TrainingSamplesEnd != null) DeregisterValueTypeEventHandlers(TrainingSamplesEnd);
          TrainingSamplesEndParameter.Value = value;
        }
      }
    }
    public IntValue TestSamplesStart {
      get { return TestSamplesStartParameter.Value; }
      set {
        if (value != TestSamplesStart) {
          if (value == null) throw new ArgumentNullException();
          if (TestSamplesStart != null) DeregisterValueTypeEventHandlers(TestSamplesStart);
          TestSamplesStartParameter.Value = value;
        }
      }
    }
    public IntValue TestSamplesEnd {
      get { return TestSamplesEndParameter.Value; }
      set {
        if (value != TestSamplesEnd) {
          if (value == null) throw new ArgumentNullException();
          if (TestSamplesEnd != null) DeregisterValueTypeEventHandlers(TestSamplesEnd);
          TestSamplesEndParameter.Value = value;
        }
      }
    }
    public PercentValue ValidationPercentage {
      get { return ValidationPercentageParameter.Value; }
      set {
        if (value != ValidationPercentage) {
          if (value == null) throw new ArgumentNullException();
          if (value.Value < 0 || value.Value > 1) throw new ArgumentException("ValidationPercentage must be between 0 and 1.");
          if (ValidationPercentage != null) DeregisterValueTypeEventHandlers(ValidationPercentage);
          ValidationPercentageParameter.Value = value;
        }
      }
    }

    public IEnumerable<int> TrainingIndizes {
      get {
        return Enumerable.Range(TrainingSamplesStart.Value, TrainingSamplesEnd.Value - TrainingSamplesStart.Value)
                         .Where(i => i >= 0 && i < Dataset.Rows && (i < TestSamplesStart.Value || TestSamplesEnd.Value <= i));
      }
    }
    public IEnumerable<int> TestIndizes {
      get {
        return Enumerable.Range(TestSamplesStart.Value, TestSamplesEnd.Value - TestSamplesStart.Value)
           .Where(i => i >= 0 && i < Dataset.Rows);
      }
    }
    #endregion

    [StorableConstructor]
    protected DataAnalysisProblemData(bool deserializing) : base(deserializing) { }
    protected DataAnalysisProblemData(DataAnalysisProblemData original, Cloner cloner)
      : base(original, cloner) {
      RegisterParameterEventHandlers();
      RegisterParameterValueEventHandlers();
    }
    public DataAnalysisProblemData()
      : base() {
      var inputVariables = new CheckedItemList<StringValue>();
      StringValue inputVariable = new StringValue("x");
      inputVariables.Add(inputVariable);
      StringValue targetVariable = new StringValue("y");
      var validTargetVariables = new ItemSet<StringValue>();
      validTargetVariables.Add(targetVariable);
      Parameters.Add(new ValueParameter<Dataset>("Dataset", new Dataset(new string[] { "y", "x" }, kozaF1)));
      Parameters.Add(new ValueParameter<ICheckedItemList<StringValue>>("InputVariables", inputVariables.AsReadOnly()));
      Parameters.Add(new ConstrainedValueParameter<StringValue>("TargetVariable", validTargetVariables, targetVariable));
      Parameters.Add(new ValueParameter<IntValue>("TrainingSamplesStart", new IntValue(0)));
      Parameters.Add(new ValueParameter<IntValue>("TrainingSamplesEnd", new IntValue(15)));
      Parameters.Add(new ValueParameter<IntValue>("TestSamplesStart", new IntValue(15)));
      Parameters.Add(new ValueParameter<IntValue>("TestSamplesEnd", new IntValue(25)));
      Parameters.Add(new ValueParameter<PercentValue>("ValidationPercentage", "The relative amount of the training samples that should be used as validation set.", new PercentValue(0.5)));

      DatasetParameter.ReactOnValueToStringChangedAndValueItemImageChanged = false;
      RegisterParameterEventHandlers();
      RegisterParameterValueEventHandlers();
    }

    public DataAnalysisProblemData(Dataset dataset, IEnumerable<string> inputVariables, string targetVariable,
      int trainingSamplesStart, int trainingSamplesEnd, int testSamplesStart, int testSamplesEnd) {
      var inputVariablesList = new CheckedItemList<StringValue>(inputVariables.Select(x => new StringValue(x)).ToList());
      StringValue targetVariableValue = new StringValue(targetVariable);
      var validTargetVariables = new ItemSet<StringValue>();
      foreach (var variable in dataset.VariableNames)
        if (variable != targetVariable)
          validTargetVariables.Add(new StringValue(variable));
      validTargetVariables.Add(targetVariableValue);
      Parameters.Add(new ValueParameter<Dataset>("Dataset", dataset));
      Parameters.Add(new ValueParameter<ICheckedItemList<StringValue>>("InputVariables", inputVariablesList.AsReadOnly()));
      Parameters.Add(new ConstrainedValueParameter<StringValue>("TargetVariable", validTargetVariables, targetVariableValue));
      Parameters.Add(new ValueParameter<IntValue>("TrainingSamplesStart", new IntValue(trainingSamplesStart)));
      Parameters.Add(new ValueParameter<IntValue>("TrainingSamplesEnd", new IntValue(trainingSamplesEnd)));
      Parameters.Add(new ValueParameter<IntValue>("TestSamplesStart", new IntValue(testSamplesStart)));
      Parameters.Add(new ValueParameter<IntValue>("TestSamplesEnd", new IntValue(testSamplesEnd)));
      Parameters.Add(new ValueParameter<PercentValue>("ValidationPercentage", "The relative amount of the training samples that should be used as validation set.", new PercentValue(0.5)));

      DatasetParameter.ReactOnValueToStringChangedAndValueItemImageChanged = false;
      RegisterParameterEventHandlers();
      RegisterParameterValueEventHandlers();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new DataAnalysisProblemData(this, cloner);
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      if (!Parameters.ContainsKey("ValidationPercentage"))
        Parameters.Add(new ValueParameter<PercentValue>("ValidationPercentage", "The relative amount of the training samples that should be used as validation set.", new PercentValue(0.5)));

      DatasetParameter.ReactOnValueToStringChangedAndValueItemImageChanged = false;
      RegisterParameterEventHandlers();
      RegisterParameterValueEventHandlers();
    }

    #region events
    public event EventHandler ProblemDataChanged;
    protected virtual void OnProblemDataChanged(EventArgs e) {
      if (TrainingSamplesStart.Value < 0) TrainingSamplesStart.Value = 0;
      else if (TestSamplesStart.Value < 0) TestSamplesStart.Value = 0;
      else if (TrainingSamplesEnd.Value > Dataset.Rows - 1) TrainingSamplesEnd.Value = Dataset.Rows - 1;
      else if (TestSamplesEnd.Value > Dataset.Rows - 1) TestSamplesEnd.Value = Dataset.Rows - 1;
      else if (TrainingSamplesStart.Value > TrainingSamplesEnd.Value) TrainingSamplesStart.Value = TestSamplesEnd.Value;
      else if (TestSamplesStart.Value > TestSamplesEnd.Value) TestSamplesStart.Value = TestSamplesEnd.Value;
      else if (ValidationPercentage.Value < 0) ValidationPercentage.Value = 0;
      else if (ValidationPercentage.Value > 1) ValidationPercentage.Value = 1;
      else if (!TrainingIndizes.Any()) throw new ArgumentException("No training samples are available.");
      else if (!suppressEvents) {
        var listeners = ProblemDataChanged;
        if (listeners != null) listeners(this, e);
      }
    }

    private void RegisterParameterEventHandlers() {
      DatasetParameter.ValueChanged += new EventHandler(DatasetParameter_ValueChanged);
      InputVariablesParameter.ValueChanged += new EventHandler(InputVariablesParameter_ValueChanged);
      TargetVariableParameter.ValueChanged += new EventHandler(TargetVariableParameter_ValueChanged);
      TrainingSamplesStartParameter.ValueChanged += new EventHandler(TrainingSamplesStartParameter_ValueChanged);
      TrainingSamplesEndParameter.ValueChanged += new EventHandler(TrainingSamplesEndParameter_ValueChanged);
      TestSamplesStartParameter.ValueChanged += new EventHandler(TestSamplesStartParameter_ValueChanged);
      TestSamplesEndParameter.ValueChanged += new EventHandler(TestSamplesEndParameter_ValueChanged);
      ValidationPercentageParameter.ValueChanged += new EventHandler(ValidationPercentageParameter_ValueChanged);
    }

    private void RegisterParameterValueEventHandlers() {
      RegisterInputVariablesEventHandlers();
      if (TargetVariable != null) RegisterStringValueEventHandlers(TargetVariable);
      RegisterValueTypeEventHandlers(TrainingSamplesStart);
      RegisterValueTypeEventHandlers(TrainingSamplesEnd);
      RegisterValueTypeEventHandlers(TestSamplesStart);
      RegisterValueTypeEventHandlers(TestSamplesEnd);
      RegisterValueTypeEventHandlers(ValidationPercentage);
    }


    #region parameter value changed event handlers
    private void DatasetParameter_ValueChanged(object sender, EventArgs e) {
      OnProblemDataChanged(EventArgs.Empty);
    }
    private void InputVariablesParameter_ValueChanged(object sender, EventArgs e) {
      RegisterInputVariablesEventHandlers();
      OnProblemDataChanged(EventArgs.Empty);
    }
    private void TargetVariableParameter_ValueChanged(object sender, EventArgs e) {
      if (TargetVariable != null) {
        RegisterStringValueEventHandlers(TargetVariable);
        OnProblemDataChanged(EventArgs.Empty);
      }
    }
    private void TrainingSamplesStartParameter_ValueChanged(object sender, EventArgs e) {
      RegisterValueTypeEventHandlers(TrainingSamplesStart);
      OnProblemDataChanged(EventArgs.Empty);
    }
    private void TrainingSamplesEndParameter_ValueChanged(object sender, EventArgs e) {
      RegisterValueTypeEventHandlers(TrainingSamplesEnd);
      OnProblemDataChanged(EventArgs.Empty);
    }
    private void TestSamplesStartParameter_ValueChanged(object sender, EventArgs e) {
      RegisterValueTypeEventHandlers(TestSamplesStart);
      OnProblemDataChanged(EventArgs.Empty);
    }
    private void TestSamplesEndParameter_ValueChanged(object sender, EventArgs e) {
      RegisterValueTypeEventHandlers(TestSamplesEnd);
      OnProblemDataChanged(EventArgs.Empty);
    }
    private void ValidationPercentageParameter_ValueChanged(object sender, EventArgs e) {
      RegisterValueTypeEventHandlers(ValidationPercentage);
      OnProblemDataChanged(EventArgs.Empty);
    }
    #endregion

    private void RegisterInputVariablesEventHandlers() {
      InputVariables.CollectionReset += new HeuristicLab.Collections.CollectionItemsChangedEventHandler<HeuristicLab.Collections.IndexedItem<StringValue>>(InputVariables_CollectionReset);
      InputVariables.ItemsAdded += new HeuristicLab.Collections.CollectionItemsChangedEventHandler<HeuristicLab.Collections.IndexedItem<StringValue>>(InputVariables_ItemsAdded);
      InputVariables.ItemsRemoved += new HeuristicLab.Collections.CollectionItemsChangedEventHandler<HeuristicLab.Collections.IndexedItem<StringValue>>(InputVariables_ItemsRemoved);
      InputVariables.CheckedItemsChanged += new HeuristicLab.Collections.CollectionItemsChangedEventHandler<HeuristicLab.Collections.IndexedItem<StringValue>>(InputVariables_CheckedItemsChanged);
      foreach (var item in InputVariables) {
        item.ValueChanged += new EventHandler(InputVariable_ValueChanged);
      }
    }

    private void DeregisterInputVariablesEventHandlers() {
      InputVariables.CollectionReset -= new HeuristicLab.Collections.CollectionItemsChangedEventHandler<HeuristicLab.Collections.IndexedItem<StringValue>>(InputVariables_CollectionReset);
      InputVariables.ItemsAdded -= new HeuristicLab.Collections.CollectionItemsChangedEventHandler<HeuristicLab.Collections.IndexedItem<StringValue>>(InputVariables_ItemsAdded);
      InputVariables.ItemsRemoved -= new HeuristicLab.Collections.CollectionItemsChangedEventHandler<HeuristicLab.Collections.IndexedItem<StringValue>>(InputVariables_ItemsRemoved);
      InputVariables.CheckedItemsChanged -= new HeuristicLab.Collections.CollectionItemsChangedEventHandler<HeuristicLab.Collections.IndexedItem<StringValue>>(InputVariables_CheckedItemsChanged);
      foreach (var item in InputVariables) {
        item.ValueChanged -= new EventHandler(InputVariable_ValueChanged);
      }
    }

    private void InputVariables_CheckedItemsChanged(object sender, HeuristicLab.Collections.CollectionItemsChangedEventArgs<HeuristicLab.Collections.IndexedItem<StringValue>> e) {
      OnProblemDataChanged(e);
    }
    private void InputVariables_ItemsRemoved(object sender, HeuristicLab.Collections.CollectionItemsChangedEventArgs<HeuristicLab.Collections.IndexedItem<StringValue>> e) {
      foreach (var indexedItem in e.Items)
        indexedItem.Value.ValueChanged -= new EventHandler(InputVariable_ValueChanged);
      OnProblemDataChanged(e);
    }
    private void InputVariables_ItemsAdded(object sender, HeuristicLab.Collections.CollectionItemsChangedEventArgs<HeuristicLab.Collections.IndexedItem<StringValue>> e) {
      foreach (var indexedItem in e.Items)
        indexedItem.Value.ValueChanged += new EventHandler(InputVariable_ValueChanged);
      OnProblemDataChanged(e);
    }
    private void InputVariables_CollectionReset(object sender, HeuristicLab.Collections.CollectionItemsChangedEventArgs<HeuristicLab.Collections.IndexedItem<StringValue>> e) {
      foreach (var indexedItem in e.OldItems)
        indexedItem.Value.ValueChanged -= new EventHandler(InputVariable_ValueChanged);
      OnProblemDataChanged(e);
    }
    private void InputVariable_ValueChanged(object sender, EventArgs e) {
      OnProblemDataChanged(e);
    }

    #region helper
    private void RegisterValueTypeEventHandlers<T>(ValueTypeValue<T> value) where T : struct {
      value.ValueChanged += new EventHandler(value_ValueChanged);
    }
    private void DeregisterValueTypeEventHandlers<T>(ValueTypeValue<T> value) where T : struct {
      value.ValueChanged -= new EventHandler(value_ValueChanged);
    }
    private void RegisterStringValueEventHandlers(StringValue value) {
      value.ValueChanged += new EventHandler(value_ValueChanged);
    }
    private void DeregisterStringValueEventHandlers(StringValue value) {
      value.ValueChanged -= new EventHandler(value_ValueChanged);
    }

    private void value_ValueChanged(object sender, EventArgs e) {
      OnProblemDataChanged(e);
    }
    #endregion
    #endregion

    public virtual void ImportFromFile(string fileName) {
      var csvFileParser = new TableFileParser();
      csvFileParser.Parse(fileName);
      suppressEvents = true;
      Name = "Data imported from " + Path.GetFileName(fileName);
      Dataset = new Dataset(csvFileParser.VariableNames, csvFileParser.Values);
      Dataset.Name = Path.GetFileName(fileName);
      var variableNames = Dataset.VariableNames.Select(x => new StringValue(x).AsReadOnly()).ToList();
      ((ConstrainedValueParameter<StringValue>)TargetVariableParameter).ValidValues.Clear();
      foreach (var variableName in variableNames)
        ((ConstrainedValueParameter<StringValue>)TargetVariableParameter).ValidValues.Add(variableName);
      TargetVariable = variableNames.First();
      InputVariables = new CheckedItemList<StringValue>(variableNames).AsReadOnly();
      InputVariables.SetItemCheckedState(variableNames.First(), false);
      int middle = (int)(csvFileParser.Rows * 0.5);
      TrainingSamplesEnd = new IntValue(middle);
      TrainingSamplesStart = new IntValue(0);
      TestSamplesEnd = new IntValue(csvFileParser.Rows);
      TestSamplesStart = new IntValue(middle);
      suppressEvents = false;
      OnProblemDataChanged(EventArgs.Empty);
    }
  }
}

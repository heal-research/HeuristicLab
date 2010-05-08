#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Parameters;
using HeuristicLab.Data;
using HeuristicLab.Problems.DataAnalysis;
using System.Drawing;
using System.IO;

namespace HeuristicLab.Problems.DataAnalysis {
  [Item("DataAnalysisProblemData", "Represents an item containing all data defining a data analysis problem.")]
  [StorableClass]
  public class DataAnalysisProblemData : ParameterizedNamedItem {
    private bool suppressEvents = false;
    #region default data
    // y = x^4 + x^3 + x^2 + x
    private readonly double[,] kozaF1 = new double[,] {
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
    public IValueParameter<Dataset> DatasetParameter {
      get { return (IValueParameter<Dataset>)Parameters["Dataset"]; }
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
    #endregion

    #region properties
    public Dataset Dataset {
      get { return (Dataset)DatasetParameter.Value; }
      set {
        if (value != Dataset) {
          if (value == null) throw new ArgumentNullException();
          if (Dataset != null) DeregisterDatasetEventHandlers();
          DatasetParameter.Value = value;
        }
      }
    }
    public StringValue TargetVariable {
      get { return (StringValue)TargetVariableParameter.Value; }
      set {
        if (value != TargetVariableParameter.Value) {
          if (value == null) throw new ArgumentNullException();
          if (TargetVariable != null) DeregisterStringValueEventHandlers(TargetVariable);
          TargetVariableParameter.Value = value;
        }
      }
    }
    public ICheckedItemList<StringValue> InputVariables {
      get { return (ICheckedItemList<StringValue>)InputVariablesParameter.Value; }
      set {
        if (value != InputVariables) {
          if (value == null) throw new ArgumentNullException();
          if (InputVariables != null) DeregisterInputVariablesEventHandlers();
          InputVariablesParameter.Value = value;
        }
      }
    }
    public IntValue TrainingSamplesStart {
      get { return (IntValue)TrainingSamplesStartParameter.Value; }
      set {
        if (value != TrainingSamplesStart) {
          if (value == null) throw new ArgumentNullException();
          if (TrainingSamplesStart != null) DeregisterValueTypeEventHandlers(TrainingSamplesStart);
          TrainingSamplesStartParameter.Value = value;
        }
      }
    }
    public IntValue TrainingSamplesEnd {
      get { return (IntValue)TrainingSamplesEndParameter.Value; }
      set {
        if (value != TrainingSamplesEnd) {
          if (value == null) throw new ArgumentNullException();
          if (TrainingSamplesEnd != null) DeregisterValueTypeEventHandlers(TrainingSamplesEnd);
          TrainingSamplesEndParameter.Value = value;
        }
      }
    }
    public IntValue TestSamplesStart {
      get { return (IntValue)TestSamplesStartParameter.Value; }
      set {
        if (value != TestSamplesStart) {
          if (value == null) throw new ArgumentNullException();
          if (TestSamplesStart != null) DeregisterValueTypeEventHandlers(TestSamplesStart);
          TestSamplesStartParameter.Value = value;
        }
      }
    }
    public IntValue TestSamplesEnd {
      get { return (IntValue)TestSamplesEndParameter.Value; }
      set {
        if (value != TestSamplesEnd) {
          if (value == null) throw new ArgumentNullException();
          if (TestSamplesEnd != null) DeregisterValueTypeEventHandlers(TestSamplesEnd);
          TestSamplesEndParameter.Value = value;
        }
      }
    }
    #endregion

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
      RegisterParameterEventHandlers();
      RegisterParameterValueEventHandlers();
    }


    [StorableConstructor]
    private DataAnalysisProblemData(bool deserializing) : base() { }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserializationHook() {
      RegisterParameterEventHandlers();
      RegisterParameterValueEventHandlers();
    }

    #region events
    public event EventHandler ProblemDataChanged;
    protected virtual void OnProblemDataChanged(EventArgs e) {
      if (!suppressEvents) {
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
    }

    private void RegisterParameterValueEventHandlers() {
      RegisterDatasetEventHandlers();
      RegisterInputVariablesEventHandlers();
      if (TargetVariable != null) RegisterStringValueEventHandlers(TargetVariable);
      RegisterValueTypeEventHandlers(TrainingSamplesStart);
      RegisterValueTypeEventHandlers(TrainingSamplesEnd);
      RegisterValueTypeEventHandlers(TestSamplesStart);
      RegisterValueTypeEventHandlers(TestSamplesEnd);
    }


    #region parameter value changed event handlers
    void DatasetParameter_ValueChanged(object sender, EventArgs e) {
      RegisterDatasetEventHandlers();
      OnProblemDataChanged(EventArgs.Empty);
    }
    void InputVariablesParameter_ValueChanged(object sender, EventArgs e) {
      RegisterInputVariablesEventHandlers();
      OnProblemDataChanged(EventArgs.Empty);
    }
    void TargetVariableParameter_ValueChanged(object sender, EventArgs e) {
      if (TargetVariable != null) {
        RegisterStringValueEventHandlers(TargetVariable);
        OnProblemDataChanged(EventArgs.Empty);
      }
    }
    void TrainingSamplesStartParameter_ValueChanged(object sender, EventArgs e) {
      RegisterValueTypeEventHandlers(TrainingSamplesStart);
      OnProblemDataChanged(EventArgs.Empty);
    }
    void TrainingSamplesEndParameter_ValueChanged(object sender, EventArgs e) {
      RegisterValueTypeEventHandlers(TrainingSamplesEnd);
      OnProblemDataChanged(EventArgs.Empty);
    }
    void TestSamplesStartParameter_ValueChanged(object sender, EventArgs e) {
      RegisterValueTypeEventHandlers(TestSamplesStart);
      OnProblemDataChanged(EventArgs.Empty);
    }
    void TestSamplesEndParameter_ValueChanged(object sender, EventArgs e) {
      RegisterValueTypeEventHandlers(TestSamplesEnd);
      OnProblemDataChanged(EventArgs.Empty);
    }
    #endregion


    private void RegisterDatasetEventHandlers() {
      Dataset.DataChanged += new EventHandler<EventArgs<int, int>>(Dataset_DataChanged);
      Dataset.Reset += new EventHandler(Dataset_Reset);
      Dataset.ColumnNamesChanged += new EventHandler(Dataset_ColumnNamesChanged);
    }

    private void DeregisterDatasetEventHandlers() {
      Dataset.DataChanged -= new EventHandler<EventArgs<int, int>>(Dataset_DataChanged);
      Dataset.Reset -= new EventHandler(Dataset_Reset);
      Dataset.ColumnNamesChanged -= new EventHandler(Dataset_ColumnNamesChanged);
    }

    void Dataset_ColumnNamesChanged(object sender, EventArgs e) {
      OnProblemDataChanged(e);
    }

    void Dataset_Reset(object sender, EventArgs e) {
      OnProblemDataChanged(e);
    }

    void Dataset_DataChanged(object sender, EventArgs<int, int> e) {
      OnProblemDataChanged(e);
    }

    private void RegisterInputVariablesEventHandlers() {
      InputVariables.CollectionReset += new HeuristicLab.Collections.CollectionItemsChangedEventHandler<HeuristicLab.Collections.IndexedItem<StringValue>>(InputVariables_CollectionReset);
      InputVariables.ItemsAdded += new HeuristicLab.Collections.CollectionItemsChangedEventHandler<HeuristicLab.Collections.IndexedItem<StringValue>>(InputVariables_ItemsAdded);
      InputVariables.ItemsRemoved += new HeuristicLab.Collections.CollectionItemsChangedEventHandler<HeuristicLab.Collections.IndexedItem<StringValue>>(InputVariables_ItemsRemoved);
      InputVariables.CheckedItemsChanged += new HeuristicLab.Collections.CollectionItemsChangedEventHandler<HeuristicLab.Collections.IndexedItem<StringValue>>(InputVariables_CheckedItemsChanged);
      foreach (var item in InputVariables)
        item.ValueChanged += new EventHandler(InputVariable_ValueChanged);
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

    void InputVariable_ValueChanged(object sender, EventArgs e) {
      OnProblemDataChanged(e);
    }
    #region helper

    private void RegisterValueTypeEventHandlers<T>(ValueTypeValue<T> value) where T : struct {
      value.ValueChanged += new EventHandler(value_ValueChanged);
    }

    private void DeregisterValueTypeEventHandlers<T>(ValueTypeValue<T> value) where T : struct {
      value.ValueChanged -= new EventHandler(value_ValueChanged);
    }

    void value_ValueChanged(object sender, EventArgs e) {
      OnProblemDataChanged(e);
    }

    private void RegisterStringValueEventHandlers(StringValue value) {
      value.ValueChanged += new EventHandler(value_ValueChanged);
    }

    private void DeregisterStringValueEventHandlers(StringValue value) {
      value.ValueChanged -= new EventHandler(value_ValueChanged);
    }

    #endregion
    #endregion

    public virtual void ImportFromFile(string fileName) {
      var csvFileParser = new CsvFileParser();
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
      TrainingSamplesStart = new IntValue(0);
      TrainingSamplesEnd = new IntValue(middle);
      TestSamplesStart = new IntValue(middle);
      TestSamplesEnd = new IntValue(csvFileParser.Rows);
      suppressEvents = false;
      OnProblemDataChanged(EventArgs.Empty);
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      DataAnalysisProblemData clone = (DataAnalysisProblemData)base.Clone(cloner);
      clone.RegisterParameterEventHandlers();
      clone.RegisterParameterValueEventHandlers();
      return clone;
    }
  }
}

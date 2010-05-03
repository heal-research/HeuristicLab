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
    #region parameter properties
    public IValueParameter<Dataset> DatasetParameter {
      get { return (IValueParameter<Dataset>)Parameters["Dataset"]; }
    }

    public IValueParameter<StringValue> TargetVariableParameter {
      get { return (IValueParameter<StringValue>)Parameters["TargetVariable"]; }
    }

    public IValueParameter<CheckedItemList<StringValue>> InputVariablesParameter {
      get { return (IValueParameter<CheckedItemList<StringValue>>)Parameters["InputVariables"]; }
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
    public CheckedItemList<StringValue> InputVariables {
      get { return (CheckedItemList<StringValue>)InputVariablesParameter.Value; }
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
      Parameters.Add(new ValueParameter<Dataset>("Dataset", new Dataset()));
      Parameters.Add(new ValueParameter<CheckedItemList<StringValue>>("InputVariables", new CheckedItemList<StringValue>()));
      Parameters.Add(new ConstrainedValueParameter<StringValue>("TargetVariable"));
      Parameters.Add(new ValueParameter<IntValue>("TrainingSamplesStart", new IntValue()));
      Parameters.Add(new ValueParameter<IntValue>("TrainingSamplesEnd", new IntValue()));
      Parameters.Add(new ValueParameter<IntValue>("TestSamplesStart", new IntValue()));
      Parameters.Add(new ValueParameter<IntValue>("TestSamplesEnd", new IntValue()));
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
      InputVariables = new CheckedItemList<StringValue>(variableNames);
      foreach (var variableName in variableNames.Skip(1))
        InputVariables.SetItemCheckedState(variableName, true);
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

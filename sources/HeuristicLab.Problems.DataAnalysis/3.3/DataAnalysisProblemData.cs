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
  public class DataAnalysisProblemData : NamedItem {
    [Storable]
    private VariableCollection variables;
    public VariableCollection Variables {
      get { return variables; }
    }

    #region variable properties
    public IVariable DatasetVariable {
      get { return variables["Dataset"]; }
    }

    public IVariable TargetVariableVariable {
      get { return variables["TargetVariable"]; }
    }

    public IVariable InputVariablesVariable {
      get { return variables["InputVariables"]; }
    }

    public IVariable TrainingSamplesStartVariable {
      get { return variables["TrainingSamplesStart"]; }
    }
    public IVariable TrainingSamplesEndVariable {
      get { return variables["TrainingSamplesEnd"]; }
    }

    public IVariable TestSamplesStartVariable {
      get { return variables["TestSamplesStart"]; }
    }
    public IVariable TestSamplesEndVariable {
      get { return variables["TestSamplesEnd"]; }
    }
    #endregion

    #region properties
    public Dataset Dataset {
      get { return (Dataset)DatasetVariable.Value; }
      set {
        if (value != Dataset) {
          if (value == null) throw new ArgumentNullException();
          if (Dataset != null) DeregisterDatasetEventHandlers();
          DatasetVariable.Value = value;
          RegisterDatasetEventHandlers();
          OnProblemDataChanged(EventArgs.Empty);
        }
      }
    }
    public StringValue TargetVariable {
      get { return (StringValue)TargetVariableVariable.Value; }
      set {
        if (value != TargetVariableVariable) {
          if (value == null) throw new ArgumentNullException();
          if (TargetVariable != null) DeregisterStringValueEventHandlers(TargetVariable);
          TargetVariableVariable.Value = value;
          RegisterStringValueEventHandlers(TargetVariable);
          OnProblemDataChanged(EventArgs.Empty);
        }
      }
    }
    public ItemList<StringValue> InputVariables {
      get { return (ItemList<StringValue>)InputVariablesVariable.Value; }
      set {
        if (value != InputVariables) {
          if (value == null) throw new ArgumentNullException();
          if (InputVariables != null) DeregisterInputVariablesEventHandlers();
          InputVariablesVariable.Value = value;
          RegisterInputVariablesEventHandlers();
          OnProblemDataChanged(EventArgs.Empty);
        }
      }
    }
    public IntValue TrainingSamplesStart {
      get { return (IntValue)TrainingSamplesStartVariable.Value; }
      set {
        if (value != TrainingSamplesStart) {
          if (value == null) throw new ArgumentNullException();
          if (TrainingSamplesStart != null) DeregisterValueTypeEventHandlers(TrainingSamplesStart);
          TrainingSamplesStartVariable.Value = value;
          RegisterValueTypeEventHandlers(TrainingSamplesStart);
          OnProblemDataChanged(EventArgs.Empty);
        }
      }
    }
    public IntValue TrainingSamplesEnd {
      get { return (IntValue)TrainingSamplesEndVariable.Value; }
      set {
        if (value != TrainingSamplesEnd) {
          if (value == null) throw new ArgumentNullException();
          if (TrainingSamplesEnd != null) DeregisterValueTypeEventHandlers(TrainingSamplesEnd);
          TrainingSamplesEndVariable.Value = value;
          RegisterValueTypeEventHandlers(TrainingSamplesEnd);
          OnProblemDataChanged(EventArgs.Empty);
        }
      }
    }
    public IntValue TestSamplesStart {
      get { return (IntValue)TestSamplesStartVariable.Value; }
      set {
        if (value != TestSamplesStart) {
          if (value == null) throw new ArgumentNullException();
          if (TestSamplesStart != null) DeregisterValueTypeEventHandlers(TestSamplesStart);
          TestSamplesStartVariable.Value = value;
          RegisterValueTypeEventHandlers(TestSamplesStart);
          OnProblemDataChanged(EventArgs.Empty);
        }
      }
    }
    public IntValue TestSamplesEnd {
      get { return (IntValue)TestSamplesEndVariable.Value; }
      set {
        if (value != TestSamplesEnd) {
          if (value == null) throw new ArgumentNullException();
          if (TestSamplesEnd != null) DeregisterValueTypeEventHandlers(TestSamplesEnd);
          TestSamplesEndVariable.Value = value;
          RegisterValueTypeEventHandlers(TestSamplesEnd);
          OnProblemDataChanged(EventArgs.Empty);
        }
      }
    }
    #endregion

    public DataAnalysisProblemData()
      : base() {
      variables = new VariableCollection();
      variables.Add(new Variable("Dataset", new Dataset()));
      variables.Add(new Variable("InputVariables", new ItemList<StringValue>()));
      variables.Add(new Variable("TargetVariable", new StringValue()));
      variables.Add(new Variable("TrainingSamplesStart", new IntValue()));
      variables.Add(new Variable("TrainingSamplesEnd", new IntValue()));
      variables.Add(new Variable("TestSamplesStart", new IntValue()));
      variables.Add(new Variable("TestSamplesEnd", new IntValue()));
      RegisterEventHandlers();
    }

    [StorableConstructor]
    private DataAnalysisProblemData(bool deserializing) : base() { }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserializationHook() {
      RegisterEventHandlers();
    }

    #region events
    private void RegisterEventHandlers() {
      RegisterDatasetEventHandlers();
      RegisterInputVariablesEventHandlers();
      RegisterStringValueEventHandlers(TargetVariable);
      RegisterValueTypeEventHandlers(TrainingSamplesStart);
      RegisterValueTypeEventHandlers(TrainingSamplesEnd);
      RegisterValueTypeEventHandlers(TestSamplesStart);
      RegisterValueTypeEventHandlers(TestSamplesEnd);
    }

    public event EventHandler ProblemDataChanged;
    protected virtual void OnProblemDataChanged(EventArgs e) {
      var listeners = ProblemDataChanged;
      if (listeners != null) listeners(this, e);
    }


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
      InputVariables.ItemsReplaced += new HeuristicLab.Collections.CollectionItemsChangedEventHandler<HeuristicLab.Collections.IndexedItem<StringValue>>(InputVariables_ItemsReplaced);
      foreach (var item in InputVariables)
        item.ValueChanged += new EventHandler(InputVariables_Value_ValueChanged);
    }

    private void DeregisterInputVariablesEventHandlers() {
      InputVariables.CollectionReset -= new HeuristicLab.Collections.CollectionItemsChangedEventHandler<HeuristicLab.Collections.IndexedItem<StringValue>>(InputVariables_CollectionReset);
      InputVariables.ItemsAdded -= new HeuristicLab.Collections.CollectionItemsChangedEventHandler<HeuristicLab.Collections.IndexedItem<StringValue>>(InputVariables_ItemsAdded);
      InputVariables.ItemsRemoved -= new HeuristicLab.Collections.CollectionItemsChangedEventHandler<HeuristicLab.Collections.IndexedItem<StringValue>>(InputVariables_ItemsRemoved);
      InputVariables.ItemsReplaced -= new HeuristicLab.Collections.CollectionItemsChangedEventHandler<HeuristicLab.Collections.IndexedItem<StringValue>>(InputVariables_ItemsReplaced);
      foreach (var item in InputVariables) {
        item.ValueChanged -= new EventHandler(InputVariables_Value_ValueChanged);
      }
    }

    void InputVariables_ItemsReplaced(object sender, HeuristicLab.Collections.CollectionItemsChangedEventArgs<HeuristicLab.Collections.IndexedItem<StringValue>> e) {
      foreach (var indexedItem in e.OldItems)
        indexedItem.Value.ValueChanged -= new EventHandler(InputVariables_Value_ValueChanged);
      foreach (var indexedItem in e.Items)
        indexedItem.Value.ValueChanged += new EventHandler(InputVariables_Value_ValueChanged);
      OnProblemDataChanged(e);
    }

    void InputVariables_ItemsRemoved(object sender, HeuristicLab.Collections.CollectionItemsChangedEventArgs<HeuristicLab.Collections.IndexedItem<StringValue>> e) {
      foreach (var indexedItem in e.Items)
        indexedItem.Value.ValueChanged -= new EventHandler(InputVariables_Value_ValueChanged);
      OnProblemDataChanged(e);
    }

    void InputVariables_ItemsAdded(object sender, HeuristicLab.Collections.CollectionItemsChangedEventArgs<HeuristicLab.Collections.IndexedItem<StringValue>> e) {
      foreach (var indexedItem in e.Items)
        indexedItem.Value.ValueChanged += new EventHandler(InputVariables_Value_ValueChanged);
      OnProblemDataChanged(e);
    }

    void InputVariables_CollectionReset(object sender, HeuristicLab.Collections.CollectionItemsChangedEventArgs<HeuristicLab.Collections.IndexedItem<StringValue>> e) {
      foreach (var indexedItem in e.OldItems)
        indexedItem.Value.ValueChanged -= new EventHandler(InputVariables_Value_ValueChanged);

      OnProblemDataChanged(e);
    }
    void InputVariables_Value_ValueChanged(object sender, EventArgs e) {
      OnProblemDataChanged(e);
    }

    #endregion

    public virtual void ImportFromFile(string fileName) {
      var csvFileParser = new CsvFileParser();
      csvFileParser.Parse(fileName);
      Name = "Data imported from " + Path.GetFileName(fileName);
      Dataset = new Dataset(csvFileParser.VariableNames, csvFileParser.Values);
      Dataset.Name = Path.GetFileName(fileName);
      TargetVariable = new StringValue(Dataset.VariableNames.First());
      InputVariables = new ItemList<StringValue>(Dataset.VariableNames.Skip(1).Select(s => new StringValue(s)));
      int middle = (int)(csvFileParser.Rows * 0.5);
      TrainingSamplesStart = new IntValue(0);
      TrainingSamplesEnd = new IntValue(middle);
      TestSamplesStart = new IntValue(middle);
      TestSamplesEnd = new IntValue(csvFileParser.Rows);
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      DataAnalysisProblemData clone = (DataAnalysisProblemData)base.Clone(cloner);
      clone.variables = (VariableCollection)variables.Clone(cloner);

      clone.RegisterEventHandlers();
      return clone;
    }
  }
}

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
    #region properties
    private Dataset dataset;
    [Storable]
    public Dataset Dataset {
      get { return dataset; }
      set {
        if (dataset != value) {
          if (value == null) throw new ArgumentNullException();
          else {
            dataset = value;
            OnDatasetChanged(EventArgs.Empty);
          }
        }
      }
    }
    private StringValue targetVariable;
    [Storable]
    public StringValue TargetVariable {
      get { return targetVariable; }
      set { targetVariable = value; }
    }
    private ItemList<StringValue> inputVariables;
    [Storable]
    public ItemList<StringValue> InputVariables {
      get { return inputVariables; }
      set {
        if (inputVariables != value) {
          if (value == null) throw new ArgumentNullException();
          else {
            inputVariables = value;
            OnInputVariablesChanged(EventArgs.Empty);
          }
        }
      }
    }
    private IntValue trainingSamplesStart;
    [Storable]
    public IntValue TrainingSamplesStart {
      get { return trainingSamplesStart; }
      set { trainingSamplesStart = value; }
    }
    private IntValue trainingSamplesEnd;
    [Storable]
    public IntValue TrainingSamplesEnd {
      get { return trainingSamplesEnd; }
      set { trainingSamplesEnd = value; }
    }
    private IntValue validationSamplesStart;
    [Storable]
    public IntValue ValidationSamplesStart {
      get { return validationSamplesStart; }
      set { validationSamplesStart = value; }
    }
    private IntValue validationSamplesEnd;
    [Storable]
    public IntValue ValidationSamplesEnd {
      get { return validationSamplesEnd; }
      set { validationSamplesEnd = value; }
    }
    private IntValue testSamplesStart;
    [Storable]
    public IntValue TestSamplesStart {
      get { return testSamplesStart; }
      set { testSamplesStart = value; }
    }
    private IntValue testSamplesEnd;
    [Storable]
    public IntValue TestSamplesEnd {
      get { return testSamplesEnd; }
      set { testSamplesEnd = value; }
    }
    #endregion

    public DataAnalysisProblemData()
      : base() {
      dataset = new Dataset();
      targetVariable = new StringValue();
      inputVariables = new ItemList<StringValue>();
      trainingSamplesStart = new IntValue();
      trainingSamplesEnd = new IntValue();
      validationSamplesStart = new IntValue();
      validationSamplesEnd = new IntValue();
      testSamplesStart = new IntValue();
      testSamplesEnd = new IntValue();
    }

    [StorableConstructor]
    private DataAnalysisProblemData(bool deserializing) : base() { }

    #region events
    public event EventHandler InputVariablesChanged;
    protected virtual void OnInputVariablesChanged(EventArgs e) {
      var listeners = InputVariablesChanged;
      if (listeners != null) listeners(this, e);
    }

    public event EventHandler DatasetChanged;
    protected virtual void OnDatasetChanged(EventArgs e) {
      EventHandler handler = DatasetChanged;
      if (handler != null) handler(this, e);
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
      TrainingSamplesStart = new IntValue(0);
      TrainingSamplesEnd = new IntValue(csvFileParser.Rows);
      TestSamplesStart = new IntValue(0);
      TestSamplesEnd = new IntValue(csvFileParser.Rows);
    }
  }
}

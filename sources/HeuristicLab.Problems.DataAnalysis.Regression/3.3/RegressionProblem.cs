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
using HeuristicLab.Optimization;
using HeuristicLab.Problems.DataAnalysis;
using System.Drawing;
using System.IO;

namespace HeuristicLab.Problems.DataAnalysis.Regression {
  [Item("RegressionProblem", "Represents a regression problem.")]
  [Creatable("Problems")]
  [StorableClass]
  public class RegressionProblem : ParameterizedNamedItem {
    public override Image ItemImage {
      get { return HeuristicLab.Common.Resources.VS2008ImageLibrary.Type; }
    }

    #region Parameter Properties
    public ValueParameter<Dataset> DatasetParameter {
      get { return (ValueParameter<Dataset>)Parameters["Dataset"]; }
    }
    public ValueParameter<StringValue> TargetVariableParameter {
      get { return (ValueParameter<StringValue>)Parameters["TargetVariable"]; }
    }
    public ValueParameter<ItemList<StringValue>> InputVariablesParameter {
      get { return (ValueParameter<ItemList<StringValue>>)Parameters["InputVariables"]; }
    }
    public ValueParameter<IntValue> TrainingSamplesStartParameter {
      get { return (ValueParameter<IntValue>)Parameters["TrainingSamplesStart"]; }
    }
    public ValueParameter<IntValue> TrainingSamplesEndParameter {
      get { return (ValueParameter<IntValue>)Parameters["TrainingSamplesEnd"]; }
    }
    public OptionalValueParameter<IntValue> ValidationSamplesStartParameter {
      get { return (OptionalValueParameter<IntValue>)Parameters["ValidationSamplesStart"]; }
    }
    public OptionalValueParameter<IntValue> ValidationSamplesEndParameter {
      get { return (OptionalValueParameter<IntValue>)Parameters["ValidationSamplesEnd"]; }
    }
    public ValueParameter<IntValue> TestSamplesStartParameter {
      get { return (ValueParameter<IntValue>)Parameters["TestSamplesStart"]; }
    }
    public ValueParameter<IntValue> TestSamplesEndParameter {
      get { return (ValueParameter<IntValue>)Parameters["TestSamplesEnd"]; }
    }
    #endregion
    #region properties
    public Dataset Dataset {
      get { return DatasetParameter.Value; }
      set { DatasetParameter.Value = value; }
    }
    public StringValue TargetVariable {
      get { return TargetVariableParameter.Value; }
      set { TargetVariableParameter.Value = value; }
    }
    public ItemList<StringValue> InputVariables {
      get { return InputVariablesParameter.Value; }
      set { InputVariablesParameter.Value = value; }
    }
    public IntValue TrainingSamplesStart {
      get { return TrainingSamplesStartParameter.Value; }
      set { TrainingSamplesStartParameter.Value = value; }
    }
    public IntValue TrainingSamplesEnd {
      get { return TrainingSamplesEndParameter.Value; }
      set { TrainingSamplesEndParameter.Value = value; }
    }
    public IntValue ValidationSamplesStart {
      get { return ValidationSamplesStartParameter.Value; }
      set { ValidationSamplesStartParameter.Value = value; }
    }
    public IntValue ValidationSamplesEnd {
      get { return ValidationSamplesEndParameter.Value; }
      set { ValidationSamplesEndParameter.Value = value; }
    }
    public IntValue TestSamplesStart {
      get { return TestSamplesStartParameter.Value; }
      set { TestSamplesStartParameter.Value = value; }
    }
    public IntValue TestSamplesEnd {
      get { return TestSamplesEndParameter.Value; }
      set { TestSamplesEndParameter.Value = value; }
    }
    #endregion

    public RegressionProblem()
      : base() {
      var dataset = new Dataset();
      // TODO: wiring for sanity checks of parameter values based on dataset (target & input variables available?, training and test partition correct?...)
      Parameters.Add(new ValueParameter<Dataset>("Dataset", "The data set containing data to be analyzer.", dataset));
      Parameters.Add(new ValueParameter<StringValue>("TargetVariable", "The target variable for which a regression model should be created.", new StringValue()));
      Parameters.Add(new ValueParameter<ItemList<StringValue>>("InputVariables", "The input variables (regressors) that are available for the regression model.", new ItemList<StringValue>()));
      Parameters.Add(new ValueParameter<IntValue>("TrainingSamplesStart", "The start index of the training partition.", new IntValue()));
      Parameters.Add(new ValueParameter<IntValue>("TrainingSamplesEnd", "The end index of the training partition.", new IntValue()));
      Parameters.Add(new OptionalValueParameter<IntValue>("ValidationSamplesStart", "The start index of the validation partition."));
      Parameters.Add(new OptionalValueParameter<IntValue>("ValidationSamplesEnd", "The end index of the validation partition."));
      Parameters.Add(new ValueParameter<IntValue>("TestSamplesStart", "The start index of the test partition.", new IntValue()));
      Parameters.Add(new ValueParameter<IntValue>("TestSamplesEnd", "The end index of the test partition.", new IntValue()));
    }

    [StorableConstructor]
    private RegressionProblem(bool deserializing) : base() { }

    public virtual void ImportFromFile(string fileName) {
      var csvFileParser = new CsvFileParser();
      csvFileParser.Parse(fileName);
      Name = "Regression Problem (imported from " + Path.GetFileName(fileName);
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

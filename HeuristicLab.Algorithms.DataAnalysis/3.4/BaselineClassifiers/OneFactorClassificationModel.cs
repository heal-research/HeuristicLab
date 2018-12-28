#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HEAL.Fossil;
using HeuristicLab.Problems.DataAnalysis;

namespace HeuristicLab.Algorithms.DataAnalysis {
  [StorableType("1FD28BA4-E30E-48E8-B868-24A5F2363DD0")]
  [Item("OneFactor Classification Model", "A model that uses only one categorical feature (factor) to determine the class.")]
  public sealed class OneFactorClassificationModel : ClassificationModel {
    public override IEnumerable<string> VariablesUsedForPrediction {
      get { return new[] { Variable }; }
    }

    [Storable]
    private string variable;
    public string Variable {
      get { return variable; }
    }

    [Storable]
    private string[] variableValues;
    public string[] VariableValues {
      get { return variableValues; }
    }

    [Storable]
    private double[] classes;
    public double[] Classes {
      get { return classes; }
    }

    [Storable]
    private double defaultClass;
    public double DefaultClass {
      get { return defaultClass; }
    }

    [StorableConstructor]
    private OneFactorClassificationModel(StorableConstructorFlag _) : base(_) { }
    private OneFactorClassificationModel(OneFactorClassificationModel original, Cloner cloner)
      : base(original, cloner) {
      this.variable = (string)original.variable;
      this.variableValues = (string[])original.variableValues.Clone();
      this.classes = (double[])original.classes.Clone();
      this.defaultClass = original.defaultClass;
    }
    public override IDeepCloneable Clone(Cloner cloner) { return new OneFactorClassificationModel(this, cloner); }

    public OneFactorClassificationModel(string targetVariable, string variable, string[] variableValues, double[] classes, double defaultClass = double.NaN)
      : base(targetVariable) {
      if (variableValues.Length != classes.Length) {
        throw new ArgumentException("Number of variable values and classes has to be equal.");
      }
      this.name = ItemName;
      this.description = ItemDescription;
      this.variable = variable;
      this.variableValues = variableValues;
      this.classes = classes;
      this.defaultClass = double.IsNaN(defaultClass) ? classes.First() : defaultClass;
      Array.Sort(variableValues, classes);
    }

    public override IEnumerable<double> GetEstimatedClassValues(IDataset dataset, IEnumerable<int> rows) {
      return dataset.GetStringValues(Variable, rows)
        .Select(GetPredictedValueForInput);
    }

    private double GetPredictedValueForInput(string val) {
      var matchingIdx = Array.BinarySearch(variableValues, val);
      if (matchingIdx >= 0) return classes[matchingIdx];
      else return DefaultClass;
    }

    public override IClassificationSolution CreateClassificationSolution(IClassificationProblemData problemData) {
      return new OneFactorClassificationSolution(this, new ClassificationProblemData(problemData));
    }

  }
}

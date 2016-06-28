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
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.DataAnalysis;

namespace HeuristicLab.Algorithms.DataAnalysis {
  [StorableClass]
  [Item("OneR Classification Model", "A model that uses intervals for one variable to determine the class.")]
  public class OneRClassificationModel : ClassificationModel {
    public override IEnumerable<string> VariablesUsedForPrediction {
      get { return new[] { Variable }; }
    }

    [Storable]
    protected string variable;
    public string Variable {
      get { return variable; }
    }

    [Storable]
    protected double[] splits;
    public double[] Splits {
      get { return splits; }
    }

    [Storable]
    protected double[] classes;
    public double[] Classes {
      get { return classes; }
    }

    [Storable]
    protected double missingValuesClass;
    public double MissingValuesClass {
      get { return missingValuesClass; }
    }

    [StorableConstructor]
    protected OneRClassificationModel(bool deserializing) : base(deserializing) { }
    protected OneRClassificationModel(OneRClassificationModel original, Cloner cloner)
      : base(original, cloner) {
      this.variable = (string)original.variable;
      this.splits = (double[])original.splits.Clone();
      this.classes = (double[])original.classes.Clone();
    }
    public override IDeepCloneable Clone(Cloner cloner) { return new OneRClassificationModel(this, cloner); }

    public OneRClassificationModel(string targetVariable, string variable, double[] splits, double[] classes, double missingValuesClass = double.NaN)
      : base(targetVariable) {
      if (splits.Length != classes.Length) {
        throw new ArgumentException("Number of splits and classes has to be equal.");
      }
      if (!Double.IsPositiveInfinity(splits[splits.Length - 1])) {
        throw new ArgumentException("Last split has to be double.PositiveInfinity, so that all values are covered.");
      }
      this.name = ItemName;
      this.description = ItemDescription;
      this.variable = variable;
      this.splits = splits;
      this.classes = classes;
      this.missingValuesClass = missingValuesClass;
    }

    // uses sorting to return the values in the order of rows, instead of using nested for loops
    // to avoid O(n²) runtime
    public override IEnumerable<double> GetEstimatedClassValues(IDataset dataset, IEnumerable<int> rows) {
      var values = dataset.GetDoubleValues(Variable, rows).ToArray();
      var rowsArray = rows.ToArray();
      var order = Enumerable.Range(0, rowsArray.Length).ToArray();
      double[] estimated = new double[rowsArray.Length];
      Array.Sort(rowsArray, order);
      Array.Sort(values, rowsArray);
      int curSplit = 0, curIndex = 0;
      while (curIndex < values.Length && Double.IsNaN(values[curIndex])) {
        estimated[curIndex] = MissingValuesClass;
        curIndex++;
      }
      while (curSplit < Splits.Length) {
        while (curIndex < values.Length && Splits[curSplit] > values[curIndex]) {
          estimated[curIndex] = classes[curSplit];
          curIndex++;
        }
        curSplit++;
      }
      Array.Sort(rowsArray, estimated);
      Array.Sort(order, estimated);
      return estimated;
    }

    public override IClassificationSolution CreateClassificationSolution(IClassificationProblemData problemData) {
      return new OneRClassificationSolution(this, new ClassificationProblemData(problemData));
    }

  }
}

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
using System.Text;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.DataAnalysis;
using SVM;

namespace HeuristicLab.Algorithms.DataAnalysis {
  /// <summary>
  /// Represents a random forest regression model.
  /// </summary>
  [StorableClass]
  [Item("RandomForestRegressionModel", "Represents a random forest regression model.")]
  public sealed class RandomForestRegressionModel : NamedItem, IRandomForestRegressionModel {

    private alglib.decisionforest randomForest;
    /// <summary>
    /// Gets or sets the SVM model.
    /// </summary>
    public alglib.decisionforest RandomForest {
      get { return randomForest; }
      set {
        if (value != randomForest) {
          if (value == null) throw new ArgumentNullException();
          randomForest = value;
          OnChanged(EventArgs.Empty);
        }
      }
    }

    [Storable]
    private string targetVariable;
    [Storable]
    private string[] allowedInputVariables;

    [StorableConstructor]
    private RandomForestRegressionModel(bool deserializing)
      : base(deserializing) {
      if (deserializing)
        randomForest = new alglib.decisionforest();
    }
    private RandomForestRegressionModel(RandomForestRegressionModel original, Cloner cloner)
      : base(original, cloner) {
      randomForest = new alglib.decisionforest();
      randomForest.innerobj.bufsize = original.randomForest.innerobj.bufsize;
      randomForest.innerobj.nclasses = original.randomForest.innerobj.nclasses;
      randomForest.innerobj.ntrees = original.randomForest.innerobj.ntrees;
      randomForest.innerobj.nvars = original.randomForest.innerobj.nvars;
      randomForest.innerobj.trees = (double[])original.randomForest.innerobj.trees.Clone();
      targetVariable = original.targetVariable;
      allowedInputVariables = (string[])original.allowedInputVariables.Clone();
    }
    public RandomForestRegressionModel(alglib.decisionforest randomForest, string targetVariable, IEnumerable<string> allowedInputVariables)
      : base() {
      this.name = ItemName;
      this.description = ItemDescription;
      this.randomForest = randomForest;
      this.targetVariable = targetVariable;
      this.allowedInputVariables = allowedInputVariables.ToArray();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new RandomForestRegressionModel(this, cloner);
    }

    public IEnumerable<double> GetEstimatedValues(Dataset dataset, IEnumerable<int> rows) {
      double[,] inputData = AlglibUtil.PrepareInputMatrix(dataset, allowedInputVariables, rows);

      int n = inputData.GetLength(0);
      int columns = inputData.GetLength(1);
      double[] x = new double[columns];
      double[] y = new double[1];

      for (int row = 0; row < n; row++) {
        for (int column = 0; column < columns; column++) {
          x[column] = inputData[row, column];
        }
        alglib.dfprocess(randomForest, x, ref y);
        yield return y[0];
      }
    }

    #region events
    public event EventHandler Changed;
    private void OnChanged(EventArgs e) {
      var handlers = Changed;
      if (handlers != null)
        handlers(this, e);
    }
    #endregion

    #region persistence
    [Storable]
    private int RandomForestBufSize {
      get {
        return randomForest.innerobj.bufsize;
      }
      set {
        randomForest.innerobj.bufsize = value;
      }
    }
    [Storable]
    private int RandomForestNClasses {
      get {
        return randomForest.innerobj.nclasses;
      }
      set {
        randomForest.innerobj.nclasses = value;
      }
    }
    [Storable]
    private int RandomForestNTrees {
      get {
        return randomForest.innerobj.ntrees;
      }
      set {
        randomForest.innerobj.ntrees = value;
      }
    }
    [Storable]
    private int RandomForestNVars {
      get {
        return randomForest.innerobj.nvars;
      }
      set {
        randomForest.innerobj.nvars = value;
      }
    }
    [Storable]
    private double[] RandomForestTrees {
      get {
        return randomForest.innerobj.trees;
      }
      set {
        randomForest.innerobj.trees = value;
      }
    }
    #endregion
  }
}

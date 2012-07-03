#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  /// <summary>
  /// Represents a multinomial logit model for classification
  /// </summary>
  [StorableClass]
  [Item("Multinomial Logit Model", "Represents a multinomial logit model for classification.")]
  public sealed class MultinomialLogitModel : NamedItem, IClassificationModel {

    private alglib.logitmodel logitModel;
    public alglib.logitmodel Model {
      get { return logitModel; }
      set {
        if (value != logitModel) {
          if (value == null) throw new ArgumentNullException();
          logitModel = value;
          OnChanged(EventArgs.Empty);
        }
      }
    }

    [Storable]
    private string targetVariable;
    [Storable]
    private string[] allowedInputVariables;
    [Storable]
    private double[] classValues;
    [StorableConstructor]
    private MultinomialLogitModel(bool deserializing)
      : base(deserializing) {
      if (deserializing)
        logitModel = new alglib.logitmodel();
    }
    private MultinomialLogitModel(MultinomialLogitModel original, Cloner cloner)
      : base(original, cloner) {
      logitModel = new alglib.logitmodel();
      logitModel.innerobj.w = (double[])original.logitModel.innerobj.w.Clone();
      targetVariable = original.targetVariable;
      allowedInputVariables = (string[])original.allowedInputVariables.Clone();
      classValues = (double[])original.classValues.Clone();
    }
    public MultinomialLogitModel(alglib.logitmodel logitModel, string targetVariable, IEnumerable<string> allowedInputVariables, double[] classValues)
      : base() {
      this.name = ItemName;
      this.description = ItemDescription;
      this.logitModel = logitModel;
      this.targetVariable = targetVariable;
      this.allowedInputVariables = allowedInputVariables.ToArray();
      this.classValues = (double[])classValues.Clone();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new MultinomialLogitModel(this, cloner);
    }

    public IEnumerable<double> GetEstimatedClassValues(Dataset dataset, IEnumerable<int> rows) {
      double[,] inputData = AlglibUtil.PrepareInputMatrix(dataset, allowedInputVariables, rows);

      int n = inputData.GetLength(0);
      int columns = inputData.GetLength(1);
      double[] x = new double[columns];
      double[] y = new double[classValues.Length];

      for (int row = 0; row < n; row++) {
        for (int column = 0; column < columns; column++) {
          x[column] = inputData[row, column];
        }
        alglib.mnlprocess(logitModel, x, ref y);
        // find class for with the largest probability value
        int maxProbClassIndex = 0;
        double maxProb = y[0];
        for (int i = 1; i < y.Length; i++) {
          if (maxProb < y[i]) {
            maxProb = y[i];
            maxProbClassIndex = i;
          }
        }
        yield return classValues[maxProbClassIndex];
      }
    }

    public MultinomialLogitClassificationSolution CreateClassificationSolution(IClassificationProblemData problemData) {
      return new MultinomialLogitClassificationSolution(problemData, this);
    }
    IClassificationSolution IClassificationModel.CreateClassificationSolution(IClassificationProblemData problemData) {
      return CreateClassificationSolution(problemData);
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
    private double[] LogitModelW {
      get {
        return logitModel.innerobj.w;
      }
      set {
        logitModel.innerobj.w = value;
      }
    }
    #endregion
  }
}

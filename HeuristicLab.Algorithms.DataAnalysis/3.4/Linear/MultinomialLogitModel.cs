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
using HEAL.Attic;
using HeuristicLab.Problems.DataAnalysis;

namespace HeuristicLab.Algorithms.DataAnalysis {
  /// <summary>
  /// Represents a multinomial logit model for classification
  /// </summary>
  [StorableType("AC4174A4-9FBC-4B07-9239-1E0E6F86034D")]
  [Item("Multinomial Logit Model", "Represents a multinomial logit model for classification.")]
  public sealed class MultinomialLogitModel : ClassificationModel {

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

    public override IEnumerable<string> VariablesUsedForPrediction {
      get { return allowedInputVariables; }
    }

    [Storable]
    private string[] allowedInputVariables;
    [Storable]
    private double[] classValues;
    [Storable]
    private List<KeyValuePair<string, IEnumerable<string>>> factorVariables;

    [StorableConstructor]
    private MultinomialLogitModel(StorableConstructorFlag _) : base(_) {
      logitModel = new alglib.logitmodel();
    }
    private MultinomialLogitModel(MultinomialLogitModel original, Cloner cloner)
      : base(original, cloner) {
      logitModel = new alglib.logitmodel();
      logitModel.innerobj.w = (double[])original.logitModel.innerobj.w.Clone();
      allowedInputVariables = (string[])original.allowedInputVariables.Clone();
      classValues = (double[])original.classValues.Clone();
      this.factorVariables = original.factorVariables.Select(kvp => new KeyValuePair<string, IEnumerable<string>>(kvp.Key, new List<string>(kvp.Value))).ToList();
    }
    public MultinomialLogitModel(alglib.logitmodel logitModel, string targetVariable, IEnumerable<string> doubleInputVariables, IEnumerable<KeyValuePair<string, IEnumerable<string>>> factorVariables, double[] classValues)
      : base(targetVariable) {
      this.name = ItemName;
      this.description = ItemDescription;
      this.logitModel = logitModel;
      this.allowedInputVariables = doubleInputVariables.ToArray();
      this.factorVariables = factorVariables.Select(kvp => new KeyValuePair<string, IEnumerable<string>>(kvp.Key, new List<string>(kvp.Value))).ToList();
      this.classValues = (double[])classValues.Clone();
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      // BackwardsCompatibility3.3
      #region Backwards compatible code, remove with 3.4
      factorVariables = new List<KeyValuePair<string, IEnumerable<string>>>();
      #endregion
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new MultinomialLogitModel(this, cloner);
    }

    public override IEnumerable<double> GetEstimatedClassValues(IDataset dataset, IEnumerable<int> rows) {

      double[,] inputData = dataset.ToArray(allowedInputVariables, rows);
      double[,] factorData = dataset.ToArray(factorVariables, rows);

      inputData = factorData.HorzCat(inputData);

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

    public override IClassificationSolution CreateClassificationSolution(IClassificationProblemData problemData) {
      return new MultinomialLogitClassificationSolution(this, new ClassificationProblemData(problemData));
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

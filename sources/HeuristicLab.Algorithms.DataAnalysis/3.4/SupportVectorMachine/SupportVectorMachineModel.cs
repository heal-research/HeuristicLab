#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using LibSVM;

namespace HeuristicLab.Algorithms.DataAnalysis {
  /// <summary>
  /// Represents a support vector machine model.
  /// </summary>
  [StorableClass]
  [Item("SupportVectorMachineModel", "Represents a support vector machine model.")]
  public sealed class SupportVectorMachineModel : NamedItem, ISupportVectorMachineModel {

    private svm_model model;
    /// <summary>
    /// Gets or sets the SVM model.
    /// </summary>
    public svm_model Model {
      get { return model; }
      set {
        if (value != model) {
          if (value == null) throw new ArgumentNullException();
          model = value;
          OnChanged(EventArgs.Empty);
        }
      }
    }

    /// <summary>
    /// Gets or sets the range transformation for the model.
    /// </summary>
    private RangeTransform rangeTransform;
    public RangeTransform RangeTransform {
      get { return rangeTransform; }
      set {
        if (value != rangeTransform) {
          if (value == null) throw new ArgumentNullException();
          rangeTransform = value;
          OnChanged(EventArgs.Empty);
        }
      }
    }

    public Dataset SupportVectors {
      get {
        var data = new double[Model.sv_coef.Length, allowedInputVariables.Count()];
        for (int i = 0; i < Model.sv_coef.Length; i++) {
          var sv = Model.SV[i];
          for (int j = 0; j < sv.Length; j++) {
            data[i, j] = sv[j].value;
          }
        }
        return new Dataset(allowedInputVariables, data);
      }
    }

    [Storable]
    private string targetVariable;
    [Storable]
    private string[] allowedInputVariables;
    [Storable]
    private double[] classValues; // only for SVM classification models

    [StorableConstructor]
    private SupportVectorMachineModel(bool deserializing) : base(deserializing) { }
    private SupportVectorMachineModel(SupportVectorMachineModel original, Cloner cloner)
      : base(original, cloner) {
      // only using a shallow copy here! (gkronber)
      this.model = original.model;
      this.rangeTransform = original.rangeTransform;
      this.targetVariable = original.targetVariable;
      this.allowedInputVariables = (string[])original.allowedInputVariables.Clone();
      if (original.classValues != null)
        this.classValues = (double[])original.classValues.Clone();
    }
    public SupportVectorMachineModel(svm_model model, RangeTransform rangeTransform, string targetVariable, IEnumerable<string> allowedInputVariables, IEnumerable<double> classValues)
      : this(model, rangeTransform, targetVariable, allowedInputVariables) {
      this.classValues = classValues.ToArray();
    }
    public SupportVectorMachineModel(svm_model model, RangeTransform rangeTransform, string targetVariable, IEnumerable<string> allowedInputVariables)
      : base() {
      this.name = ItemName;
      this.description = ItemDescription;
      this.model = model;
      this.rangeTransform = rangeTransform;
      this.targetVariable = targetVariable;
      this.allowedInputVariables = allowedInputVariables.ToArray();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SupportVectorMachineModel(this, cloner);
    }

    #region IRegressionModel Members
    public IEnumerable<double> GetEstimatedValues(Dataset dataset, IEnumerable<int> rows) {
      return GetEstimatedValuesHelper(dataset, rows);
    }
    public SupportVectorRegressionSolution CreateRegressionSolution(IRegressionProblemData problemData) {
      return new SupportVectorRegressionSolution(this, new RegressionProblemData(problemData));
    }
    IRegressionSolution IRegressionModel.CreateRegressionSolution(IRegressionProblemData problemData) {
      return CreateRegressionSolution(problemData);
    }
    #endregion

    #region IClassificationModel Members
    public IEnumerable<double> GetEstimatedClassValues(Dataset dataset, IEnumerable<int> rows) {
      if (classValues == null) throw new NotSupportedException();
      // return the original class value instead of the predicted value of the model
      // svm classification only works for integer classes
      foreach (var estimated in GetEstimatedValuesHelper(dataset, rows)) {
        // find closest class
        double bestDist = double.MaxValue;
        double bestClass = -1;
        for (int i = 0; i < classValues.Length; i++) {
          double d = Math.Abs(estimated - classValues[i]);
          if (d < bestDist) {
            bestDist = d;
            bestClass = classValues[i];
            if (d.IsAlmost(0.0)) break; // exact match no need to look further
          }
        }
        yield return bestClass;
      }
    }

    public SupportVectorClassificationSolution CreateClassificationSolution(IClassificationProblemData problemData) {
      return new SupportVectorClassificationSolution(this, new ClassificationProblemData(problemData));
    }
    IClassificationSolution IClassificationModel.CreateClassificationSolution(IClassificationProblemData problemData) {
      return CreateClassificationSolution(problemData);
    }
    #endregion
    private IEnumerable<double> GetEstimatedValuesHelper(Dataset dataset, IEnumerable<int> rows) {
      // calculate predictions for the currently requested rows
      svm_problem problem = SupportVectorMachineUtil.CreateSvmProblem(dataset, targetVariable, allowedInputVariables, rows);
      svm_problem scaledProblem = rangeTransform.Scale(problem);

      for (int i = 0; i < problem.l; i++) {
        yield return svm.svm_predict(Model, scaledProblem.x[i]);
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
    private string ModelAsString {
      get {
        using (MemoryStream stream = new MemoryStream()) {
          svm.svm_save_model(new StreamWriter(stream), Model);
          stream.Seek(0, System.IO.SeekOrigin.Begin);
          StreamReader reader = new StreamReader(stream);
          return reader.ReadToEnd();
        }
      }
      set {
        using (MemoryStream stream = new MemoryStream(Encoding.ASCII.GetBytes(value))) {
          model = svm.svm_load_model(new StreamReader(stream));
        }
      }
    }
    [Storable]
    private string RangeTransformAsString {
      get {
        using (MemoryStream stream = new MemoryStream()) {
          RangeTransform.Write(stream, RangeTransform);
          stream.Seek(0, System.IO.SeekOrigin.Begin);
          StreamReader reader = new StreamReader(stream);
          return reader.ReadToEnd();
        }
      }
      set {
        using (MemoryStream stream = new MemoryStream(Encoding.ASCII.GetBytes(value))) {
          RangeTransform = RangeTransform.Read(stream);
        }
      }
    }
    #endregion
  }
}

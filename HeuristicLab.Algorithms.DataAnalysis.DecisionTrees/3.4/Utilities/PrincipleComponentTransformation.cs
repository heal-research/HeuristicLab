#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Problems.DataAnalysis;
using HEAL.Attic;

namespace HeuristicLab.Algorithms.DataAnalysis {
  [StorableType("A2DDC528-BAA7-445F-98E1-5F895CE2FD5C")]
  public class PrincipleComponentTransformation : IDeepCloneable {
    #region Properties
    [Storable]
    private double[,] Matrix { get; set; }
    [Storable]
    public double[] Variances { get; private set; }
    [Storable]
    public string[] VariableNames { get; private set; }
    [Storable]
    private double[] Deviations { get; set; }
    [Storable]
    private double[] Means { get; set; }
    public string[] ComponentNames {
      get { return VariableNames.Select((_, x) => "pc" + x).ToArray(); }
    }
    #endregion

    #region HLConstructors
    [StorableConstructor]
    protected PrincipleComponentTransformation(StorableConstructorFlag _) { }
    protected PrincipleComponentTransformation(PrincipleComponentTransformation original, Cloner cloner) {
      if (original.Variances != null) Variances = original.Variances.ToArray();
      if (original.VariableNames != null) VariableNames = original.VariableNames.ToArray();
      if (original.Deviations != null) Deviations = original.Deviations.ToArray();
      if (original.Means != null) Means = original.Means.ToArray();
      if (original.Matrix == null) return;
      Matrix = new double[original.Matrix.GetLength(0), original.Matrix.GetLength(1)];
      for (var i = 0; i < original.Matrix.GetLength(0); i++)
      for (var j = 0; j < original.Matrix.GetLength(1); j++)
        Matrix[i, j] = original.Matrix[i, j];
    }
    private PrincipleComponentTransformation() { }
    public IDeepCloneable Clone(Cloner cloner) {
      return new PrincipleComponentTransformation(this, cloner);
    }
    public object Clone() {
      return new Cloner().Clone(this);
    }
    #endregion

    #region Static Interface
    public static PrincipleComponentTransformation CreateProjection(IDataset dataset, IEnumerable<int> rows, IEnumerable<string> variables, bool normalize = false) {
      var res = new PrincipleComponentTransformation();
      res.BuildPca(dataset, rows, variables, normalize);
      return res;
    }
    #endregion

    #region Projection
    public IRegressionProblemData TransformProblemData(IRegressionProblemData pd) {
      return CreateProblemData(pd, TransformDataset(pd.Dataset), ComponentNames);
    }

    public IDataset TransformDataset(IDataset data) {
      return CreateDataset(data, TransformData(data, Enumerable.Range(0, data.Rows)));
    }

    public double[,] TransformData(IDataset dataset, IEnumerable<int> rows) {
      var instances = rows.ToArray();
      var result = new double[instances.Length, VariableNames.Length];
      for (var r = 0; r < instances.Length; r++)
      for (var i = 0; i < VariableNames.Length; i++) {
        var val = (dataset.GetDoubleValue(VariableNames[i], instances[r]) - Means[i]) / Deviations[i];
        for (var j = 0; j < VariableNames.Length; j++)
          result[r, j] += val * Matrix[i, j];
      }
      return result;
    }
    #endregion

    #region Reversion
    public IRegressionProblemData RevertProblemData(IRegressionProblemData pd) {
      return CreateProblemData(pd, RevertDataset(pd.Dataset), VariableNames);
    }

    public IDataset RevertDataset(IDataset data) {
      return CreateRevertedDataset(data, RevertData(data, Enumerable.Range(0, data.Rows)));
    }

    public double[,] RevertData(IDataset dataset, IEnumerable<int> rows) {
      var instances = rows.ToArray();
      var components = ComponentNames;
      var result = new double[instances.Length, VariableNames.Length];
      for (var r = 0; r < instances.Length; r++)
      for (var i = 0; i < components.Length; i++) {
        var val = dataset.GetDoubleValue(components[i], instances[r]);
        for (var j = 0; j < VariableNames.Length; j++)
          result[r, j] += val * Matrix[j, i];
      }
      for (var r = 0; r < instances.Length; r++) {
        for (var j = 0; j < VariableNames.Length; j++) {
          result[r, j] *= Deviations[j];
          result[r, j] += Means[j];
        }
      }

      return result;
    }
    #endregion

    #region Helpers
    private static IRegressionProblemData CreateProblemData(IRegressionProblemData pd, IDataset data, IReadOnlyList<string> allowedNames) {
      var res = new RegressionProblemData(data, allowedNames, pd.TargetVariable);
      res.TestPartition.Start = pd.TestPartition.Start;
      res.TestPartition.End = pd.TestPartition.End;
      res.TrainingPartition.Start = pd.TrainingPartition.Start;
      res.TrainingPartition.End = pd.TrainingPartition.End;
      res.Name = pd.Name;
      return res;
    }

    private IDataset CreateDataset(IDataset data, double[,] pcs) {
      var n = ComponentNames;
      var nDouble = data.DoubleVariables.Where(x => !VariableNames.Contains(x)).ToArray();
      var nDateTime = data.DateTimeVariables.ToArray();
      var nString = data.StringVariables.ToArray();

      IEnumerable<IList> nData = n.Select((_, x) => Enumerable.Range(0, pcs.GetLength(0)).Select(r => pcs[r, x]).ToList());
      IEnumerable<IList> nDoubleData = nDouble.Select(x => data.GetDoubleValues(x).ToList());
      IEnumerable<IList> nDateTimeData = nDateTime.Select(x => data.GetDateTimeValues(x).ToList());
      IEnumerable<IList> nStringData = nString.Select(x => data.GetStringValues(x).ToList());

      return new Dataset(n.Concat(nDouble).Concat(nDateTime).Concat(nString), nData.Concat(nDoubleData).Concat(nDateTimeData).Concat(nStringData).ToArray());
    }

    private IDataset CreateRevertedDataset(IDataset data, double[,] pcs) {
      var n = VariableNames;
      var nDouble = data.DoubleVariables.Where(x => !ComponentNames.Contains(x)).ToArray();
      var nDateTime = data.DateTimeVariables.ToArray();
      var nString = data.StringVariables.ToArray();

      IEnumerable<IList> nData = n.Select((_, x) => Enumerable.Range(0, pcs.GetLength(0)).Select(r => pcs[r, x]).ToList());
      IEnumerable<IList> nDoubleData = nDouble.Select(x => data.GetDoubleValues(x).ToList());
      IEnumerable<IList> nDateTimeData = nDateTime.Select(x => data.GetDateTimeValues(x).ToList());
      IEnumerable<IList> nStringData = nString.Select(x => data.GetStringValues(x).ToList());

      return new Dataset(n.Concat(nDouble).Concat(nDateTime).Concat(nString), nData.Concat(nDoubleData).Concat(nDateTimeData).Concat(nStringData).ToArray());
    }

    private void BuildPca(IDataset dataset, IEnumerable<int> rows, IEnumerable<string> variables, bool normalize) {
      var instances = rows.ToArray();
      var attributes = variables.ToArray();
      Means = normalize
        ? attributes.Select(v => dataset.GetDoubleValues(v, instances).Average()).ToArray()
        : attributes.Select(x => 0.0).ToArray();
      Deviations = normalize
        ? attributes.Select(v => dataset.GetDoubleValues(v, instances).StandardDeviationPop()).Select(x => x.IsAlmost(0.0) ? 1 : x).ToArray()
        : attributes.Select(x => 1.0).ToArray();

      var data = new double[instances.Length, attributes.Length];

      for (var j = 0; j < attributes.Length; j++) {
        var i = 0;
        foreach (var v in dataset.GetDoubleValues(attributes[j], instances)) {
          data[i, j] = (v - Means[j]) / Deviations[j];
          i++;
        }
      }

      int info;
      double[] variances;
      double[,] matrix;
      alglib.pcabuildbasis(data, instances.Length, attributes.Length, out info, out variances, out matrix);
      Matrix = matrix;
      Variances = variances;
      VariableNames = attributes;
    }
    #endregion
  }
}
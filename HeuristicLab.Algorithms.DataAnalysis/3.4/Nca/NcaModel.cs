#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.DataAnalysis;

namespace HeuristicLab.Algorithms.DataAnalysis {
  [Item("NCA Model", "")]
  [StorableClass]
  public class NcaModel : NamedItem, INcaModel {

    [Storable]
    private double[,] transformationMatrix;
    public double[,] TransformationMatrix {
      get { return (double[,])transformationMatrix.Clone(); }
    }
    [Storable]
    private string[] allowedInputVariables;
    [Storable]
    private string targetVariable;
    [Storable]
    private INearestNeighbourModel nnModel;
    [Storable]
    private double[] classValues;

    [StorableConstructor]
    protected NcaModel(bool deserializing) : base(deserializing) { }
    protected NcaModel(NcaModel original, Cloner cloner)
      : base(original, cloner) {
      this.transformationMatrix = (double[,])original.transformationMatrix.Clone();
      this.allowedInputVariables = (string[])original.allowedInputVariables.Clone();
      this.targetVariable = original.targetVariable;
      this.nnModel = cloner.Clone(original.nnModel);
      this.classValues = (double[])original.classValues.Clone();
    }
    public NcaModel(int k, double[,] transformationMatrix, Dataset dataset, IEnumerable<int> rows, string targetVariable, IEnumerable<string> allowedInputVariables, double[] classValues) {
      Name = ItemName;
      Description = ItemDescription;
      this.transformationMatrix = (double[,])transformationMatrix.Clone();
      this.allowedInputVariables = allowedInputVariables.ToArray();
      this.targetVariable = targetVariable;
      this.classValues = (double[])classValues.Clone();

      var ds = ReduceDataset(dataset, rows);
      nnModel = new NearestNeighbourModel(ds, Enumerable.Range(0, ds.Rows), k, ds.VariableNames.Last(), ds.VariableNames.Take(transformationMatrix.GetLength(1)), classValues);
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new NcaModel(this, cloner);
    }

    public IEnumerable<double> GetEstimatedClassValues(Dataset dataset, IEnumerable<int> rows) {
      var ds = ReduceDataset(dataset, rows);
      return nnModel.GetEstimatedClassValues(ds, Enumerable.Range(0, ds.Rows));
    }

    public INcaClassificationSolution CreateClassificationSolution(IClassificationProblemData problemData) {
      return new NcaClassificationSolution(new ClassificationProblemData(problemData), this);
    }

    IClassificationSolution IClassificationModel.CreateClassificationSolution(IClassificationProblemData problemData) {
      return CreateClassificationSolution(problemData);
    }

    public double[,] Reduce(Dataset dataset, IEnumerable<int> rows) {
      var data = AlglibUtil.PrepareInputMatrix(dataset, allowedInputVariables, rows);

      var targets = dataset.GetDoubleValues(targetVariable, rows).ToArray();
      var result = new double[data.GetLength(0), transformationMatrix.GetLength(1) + 1];
      for (int i = 0; i < data.GetLength(0); i++)
        for (int j = 0; j < data.GetLength(1); j++) {
          for (int x = 0; x < transformationMatrix.GetLength(1); x++) {
            result[i, x] += data[i, j] * transformationMatrix[j, x];
          }
          result[i, transformationMatrix.GetLength(1)] = targets[i];
        }
      return result;
    }

    public Dataset ReduceDataset(Dataset dataset, IEnumerable<int> rows) {
      return new Dataset(Enumerable
          .Range(0, transformationMatrix.GetLength(1))
          .Select(x => "X" + x.ToString())
          .Concat(targetVariable.ToEnumerable()),
        Reduce(dataset, rows));
    }
  }
}

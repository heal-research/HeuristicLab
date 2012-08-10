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

using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.DataAnalysis;

namespace HeuristicLab.Algorithms.DataAnalysis {
  [Item("LDA", "Initializes the matrix by performing a linear discriminant analysis.")]
  [StorableClass]
  public class LDAInitializer : Item, INCAInitializer {

    [StorableConstructor]
    protected LDAInitializer(bool deserializing) : base(deserializing) { }
    protected LDAInitializer(LDAInitializer original, Cloner cloner) : base(original, cloner) { }
    public LDAInitializer() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new LDAInitializer(this, cloner);
    }

    public double[] Initialize(IClassificationProblemData data, int dimensions) {
      var instances = data.TrainingIndices.Count();
      var attributes = data.AllowedInputVariables.Count();

      var ldaDs = new double[instances, attributes + 1];
      int row, col = 0;
      foreach (var variable in data.AllowedInputVariables) {
        row = 0;
        foreach (var value in data.Dataset.GetDoubleValues(variable, data.TrainingIndices)) {
          ldaDs[row, col] = value;
          row++;
        }
        col++;
      }
      row = 0;
      var uniqueClasses = new Dictionary<double, int>();
      foreach (var label in data.Dataset.GetDoubleValues(data.TargetVariable, data.TrainingIndices)) {
        if (!uniqueClasses.ContainsKey(label))
          uniqueClasses[label] = uniqueClasses.Count;
        ldaDs[row++, attributes] = label;
      }
      for (row = 0; row < instances; row++)
        ldaDs[row, attributes] = uniqueClasses[ldaDs[row, attributes]];

      int info;
      double[,] matrix;
      alglib.fisherldan(ldaDs, instances, attributes, uniqueClasses.Count, out info, out matrix);

      var result = new double[attributes * dimensions];
      for (int i = 0; i < attributes; i++)
        for (int j = 0; j < dimensions; j++)
          result[i * dimensions + j] = matrix[i, j];

      return result;
    }

  }
}

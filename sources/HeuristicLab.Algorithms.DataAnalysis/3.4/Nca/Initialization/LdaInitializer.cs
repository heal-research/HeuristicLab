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

using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.DataAnalysis;

namespace HeuristicLab.Algorithms.DataAnalysis {
  [Item("LDA", "Initializes the matrix by performing a linear discriminant analysis.")]
  [StorableClass]
  public class LdaInitializer : NcaInitializer {

    [StorableConstructor]
    protected LdaInitializer(bool deserializing) : base(deserializing) { }
    protected LdaInitializer(LdaInitializer original, Cloner cloner) : base(original, cloner) { }
    public LdaInitializer() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new LdaInitializer(this, cloner);
    }

    public override double[,] Initialize(IClassificationProblemData data, Scaling scaling, int dimensions) {
      var instances = data.TrainingIndices.Count();
      var attributes = data.AllowedInputVariables.Count();

      var ldaDs = new double[instances, attributes + 1];
      int j = 0;
      foreach (var a in data.AllowedInputVariables) {
        int i = 0;
        var sv = scaling.GetScaledValues(data.Dataset, a, data.TrainingIndices);
        foreach (var v in sv) {
          ldaDs[i++, j] = v;
        }
        j++;
      }
      j = 0;
      foreach (var tv in data.Dataset.GetDoubleValues(data.TargetVariable, data.TrainingIndices))
        ldaDs[j++, attributes] = tv;

      var uniqueClasses = data.Dataset.GetDoubleValues(data.TargetVariable, data.TrainingIndices).Distinct().Count();

      int info;
      double[,] matrix;
      alglib.fisherldan(ldaDs, instances, attributes, uniqueClasses, out info, out matrix);

      return matrix;
    }

  }
}

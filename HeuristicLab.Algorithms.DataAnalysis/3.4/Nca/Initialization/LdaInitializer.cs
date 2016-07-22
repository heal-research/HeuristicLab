#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

    public override double[,] Initialize(IClassificationProblemData data, int dimensions) {
      var instances = data.TrainingIndices.Count();
      var attributes = data.AllowedInputVariables.Count();

      var ldaDs = AlglibUtil.PrepareInputMatrix(data.Dataset,
                                                data.AllowedInputVariables.Concat(data.TargetVariable.ToEnumerable()),
                                                data.TrainingIndices);

      // map class values to sequential natural numbers (required by alglib)
      var uniqueClasses = data.Dataset.GetDoubleValues(data.TargetVariable, data.TrainingIndices)
                                        .Distinct()
                                        .Select((v, i) => new { v, i })
                                        .ToDictionary(x => x.v, x => x.i);

      for (int row = 0; row < instances; row++)
        ldaDs[row, attributes] = uniqueClasses[ldaDs[row, attributes]];

      int info;
      double[,] matrix;
      alglib.fisherldan(ldaDs, instances, attributes, uniqueClasses.Count, out info, out matrix);

      return matrix;
    }

  }
}

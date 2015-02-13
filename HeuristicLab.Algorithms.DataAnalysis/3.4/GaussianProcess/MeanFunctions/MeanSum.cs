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

using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Algorithms.DataAnalysis {
  [StorableClass]
  [Item(Name = "MeanSum", Description = "Sum of mean functions for Gaussian processes.")]
  public sealed class MeanSum : Item, IMeanFunction {
    [Storable]
    private ItemList<IMeanFunction> terms;

    [Storable]
    private int numberOfVariables;
    public ItemList<IMeanFunction> Terms {
      get { return terms; }
    }

    [StorableConstructor]
    private MeanSum(bool deserializing) : base(deserializing) { }
    private MeanSum(MeanSum original, Cloner cloner)
      : base(original, cloner) {
      this.terms = cloner.Clone(original.terms);
      this.numberOfVariables = original.numberOfVariables;
    }
    public MeanSum() {
      this.terms = new ItemList<IMeanFunction>();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new MeanSum(this, cloner);
    }

    public int GetNumberOfParameters(int numberOfVariables) {
      this.numberOfVariables = numberOfVariables;
      return terms.Select(t => t.GetNumberOfParameters(numberOfVariables)).Sum();
    }

    public void SetParameter(double[] p) {
      int offset = 0;
      foreach (var t in terms) {
        var numberOfParameters = t.GetNumberOfParameters(numberOfVariables);
        t.SetParameter(p.Skip(offset).Take(numberOfParameters).ToArray());
        offset += numberOfParameters;
      }
    }

    public ParameterizedMeanFunction GetParameterizedMeanFunction(double[] p, IEnumerable<int> columnIndices) {
      var termMf = new List<ParameterizedMeanFunction>();
      int totalNumberOfParameters = GetNumberOfParameters(numberOfVariables);
      int[] termIndexMap = new int[totalNumberOfParameters]; // maps k-th parameter to the correct mean-term
      int[] hyperParameterIndexMap = new int[totalNumberOfParameters]; // maps k-th parameter to the l-th parameter of the correct mean-term
      int c = 0;
      // get the parameterized mean function for each term
      for (int termIndex = 0; termIndex < terms.Count; termIndex++) {
        var numberOfParameters = terms[termIndex].GetNumberOfParameters(numberOfVariables);
        termMf.Add(terms[termIndex].GetParameterizedMeanFunction(p.Take(numberOfParameters).ToArray(), columnIndices));
        p = p.Skip(numberOfParameters).ToArray();

        for (int hyperParameterIndex = 0; hyperParameterIndex < numberOfParameters; hyperParameterIndex++) {
          termIndexMap[c] = termIndex;
          hyperParameterIndexMap[c] = hyperParameterIndex;
          c++;
        }
      }

      var mf = new ParameterizedMeanFunction();
      mf.Mean = (x, i) => termMf.Select(t => t.Mean(x, i)).Sum();
      mf.Gradient = (x, i, k) => {
        return termMf[termIndexMap[k]].Gradient(x, i, hyperParameterIndexMap[k]);
      };
      return mf;
    }
  }
}

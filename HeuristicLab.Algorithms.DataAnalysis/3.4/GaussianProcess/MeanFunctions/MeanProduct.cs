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
  [Item(Name = "MeanProduct", Description = "Product of mean functions for Gaussian processes.")]
  public sealed class MeanProduct : Item, IMeanFunction {
    [Storable]
    private ItemList<IMeanFunction> factors;

    [Storable]
    private int numberOfVariables;

    public ItemList<IMeanFunction> Factors {
      get { return factors; }
    }

    [StorableConstructor]
    private MeanProduct(bool deserializing)
      : base(deserializing) {
    }

    private MeanProduct(MeanProduct original, Cloner cloner)
      : base(original, cloner) {
      this.factors = cloner.Clone(original.factors);
      this.numberOfVariables = original.numberOfVariables;
    }

    public MeanProduct() {
      this.factors = new ItemList<IMeanFunction>();
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new MeanProduct(this, cloner);
    }

    public int GetNumberOfParameters(int numberOfVariables) {
      this.numberOfVariables = numberOfVariables;
      return factors.Select(t => t.GetNumberOfParameters(numberOfVariables)).Sum();
    }

    public void SetParameter(double[] p) {
      int offset = 0;
      foreach (var t in factors) {
        var numberOfParameters = t.GetNumberOfParameters(numberOfVariables);
        t.SetParameter(p.Skip(offset).Take(numberOfParameters).ToArray());
        offset += numberOfParameters;
      }
    }


    public ParameterizedMeanFunction GetParameterizedMeanFunction(double[] p, int[] columnIndices) {
      var factorMf = new List<ParameterizedMeanFunction>();
      int totalNumberOfParameters = GetNumberOfParameters(numberOfVariables);
      int[] factorIndexMap = new int[totalNumberOfParameters]; // maps k-th hyperparameter to the correct mean-term
      int[] hyperParameterIndexMap = new int[totalNumberOfParameters]; // maps k-th hyperparameter to the l-th hyperparameter of the correct mean-term
      int c = 0;
      // get the parameterized mean function for each term
      for (int factorIndex = 0; factorIndex < factors.Count; factorIndex++) {
        var numberOfParameters = factors[factorIndex].GetNumberOfParameters(numberOfVariables);
        factorMf.Add(factors[factorIndex].GetParameterizedMeanFunction(p.Take(numberOfParameters).ToArray(), columnIndices));
        p = p.Skip(numberOfParameters).ToArray();

        for (int hyperParameterIndex = 0; hyperParameterIndex < numberOfParameters; hyperParameterIndex++) {
          factorIndexMap[c] = factorIndex;
          hyperParameterIndexMap[c] = hyperParameterIndex;
          c++;
        }
      }

      var mf = new ParameterizedMeanFunction();
      mf.Mean = (x, i) => factorMf.Select(t => t.Mean(x, i)).Aggregate((a, b) => a * b);
      mf.Gradient = (x, i, k) => {
        double result = 1.0;
        int hyperParameterFactorIndex = factorIndexMap[k];
        for (int factorIndex = 0; factorIndex < factors.Count; factorIndex++) {
          if (factorIndex == hyperParameterFactorIndex) {
            // multiply gradient
            result *= factorMf[factorIndex].Gradient(x, i, hyperParameterIndexMap[k]);
          } else {
            // multiply mean
            result *= factorMf[factorIndex].Mean(x, i);
          }
        }
        return result;
      };
      return mf;
    }
  }
}

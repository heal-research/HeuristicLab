#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2017 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Algorithms.DataAnalysis {
  /// <summary>
  /// Helper class for incremental split calculation.
  /// Used while moving a potential splitter along the ordered training instances
  /// </summary>
  internal class UnivariateOnlineLR {
    #region state
    private readonly NeumaierSum targetMean;
    private readonly NeumaierSum attributeMean;
    private readonly NeumaierSum targetVarSum;
    private readonly NeumaierSum attributeVarSum;
    private readonly NeumaierSum comoment;
    private readonly NeumaierSum ssr;
    private int size;
    #endregion

    public double Ssr {
      get { return ssr.Get(); }
    }
    public int Size {
      get { return size; }
    }

    private double Beta {
      get { return comoment.Get() / attributeVarSum.Get(); }
    }
    private double Alpha {
      get { return targetMean.Get() - Beta * attributeMean.Get(); }
    }

    public UnivariateOnlineLR(ICollection<double> attributeValues, ICollection<double> targetValues) {
      if (attributeValues.Count != targetValues.Count) throw new ArgumentException("Targets and Attributes need to have the same length");
      size = attributeValues.Count;

      var yMean = targetValues.Average();
      var xMean = attributeValues.Average();
      targetMean = new NeumaierSum(yMean);
      attributeMean = new NeumaierSum(xMean);
      targetVarSum = new NeumaierSum(targetValues.VariancePop() * size);
      attributeVarSum = new NeumaierSum(attributeValues.VariancePop() * size);
      comoment = new NeumaierSum(attributeValues.Zip(targetValues, (x, y) => (x - xMean) * (y - yMean)).Sum());

      var beta = comoment.Get() / attributeVarSum.Get();
      var alpha = yMean - beta * xMean;
      ssr = new NeumaierSum(attributeValues.Zip(targetValues, (x, y) => y - alpha - beta * x).Sum(x => x * x));
    }

    public void Add(double attributeValue, double targetValue) {
      var predictOld = Predict(attributeValue, targetValue);

      size++;
      var dx = attributeValue - attributeMean.Get();
      var dy = targetValue - targetMean.Get();
      attributeMean.Add(dx / size);
      targetMean.Add(dy / size);
      var dx2 = attributeValue - attributeMean.Get();
      var dy2 = targetValue - targetMean.Get();
      attributeVarSum.Add(dx * dx2);
      targetVarSum.Add(dy * dy2);
      comoment.Add(dx * dy2);

      ssr.Add(predictOld * Predict(attributeValue, targetValue));
    }

    public void Remove(double attributeValue, double targetValue) {
      var predictOld = Predict(attributeValue, targetValue);

      var dx2 = attributeValue - attributeMean.Get();
      var dy2 = targetValue - targetMean.Get();
      attributeMean.Mul(size / (size - 1.0));
      targetMean.Mul(size / (size - 1.0));
      attributeMean.Add(-attributeValue / (size - 1.0));
      targetMean.Add(-targetValue / (size - 1.0));
      var dx = attributeValue - attributeMean.Get();
      var dy = targetValue - targetMean.Get();
      attributeVarSum.Add(-dx * dx2);
      targetVarSum.Add(-dy * dy2);
      comoment.Add(-dx * dy2);
      size--;

      ssr.Add(-predictOld * Predict(attributeValue, targetValue));
    }

    private double Predict(double attributeValue, double targetValue) {
      return targetValue - Alpha - Beta * attributeValue;
    }
  }
}
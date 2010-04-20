#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Drawing;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Problems.DataAnalysis;
using HeuristicLab.Operators;
using HeuristicLab.Problems.DataAnalysis.Symbolic;

namespace HeuristicLab.Problems.DataAnalysis.Regression.Symbolic {
  [StorableClass]
  public class SymbolicRegressionModel : DeepCloneable, IModel {
    [Storable]
    private SymbolicExpressionTree tree;
    public SymbolicExpressionTree SymbolicExpressionTree {
      get { return tree; }
    }
    [Storable]
    private SimpleArithmeticExpressionEvaluator evaluator;
    public SimpleArithmeticExpressionEvaluator Evaluator {
      get { return evaluator; }
    }
    private Dataset emptyDataset;
    private IEnumerable<int> firstRow = new int[] { 0 };

    public SymbolicRegressionModel() : base() { } // for cloning

    public SymbolicRegressionModel(SymbolicExpressionTree tree, IEnumerable<string> inputVariables)
      : base() {
      this.tree = tree;
      this.evaluator = new SimpleArithmeticExpressionEvaluator();
      emptyDataset = new Dataset(inputVariables, new double[1, inputVariables.Count()]);
    }

    #region IModel Members

    public double GetValue(double[] xs) {
      if (xs.Length != emptyDataset.Columns) throw new ArgumentException("Length of input vector doesn't match model");
      for (int i = 0; i < xs.Length; i++) {
        emptyDataset[0, i] = xs[i];
      }
      return evaluator.EstimatedValues(tree, emptyDataset, firstRow).First();
    }

    #endregion
  }
}

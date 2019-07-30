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

using System;
using System.Linq;
using System.Threading;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Problems.DataAnalysis;
using HEAL.Attic;

namespace HeuristicLab.Algorithms.DataAnalysis {
  [StorableType("F3E94907-C5FF-4658-A870-8013C61DD2E1")]
  [Item("ConstantLeaf", "A leaf type that uses constant models as leaf models")]
  public class ConstantLeaf : LeafBase {
    #region Constructors & Cloning
    [StorableConstructor]
    protected ConstantLeaf(StorableConstructorFlag _) : base(_) { }
    protected ConstantLeaf(ConstantLeaf original, Cloner cloner) : base(original, cloner) { }
    public ConstantLeaf() { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new ConstantLeaf(this, cloner);
    }
    #endregion

    #region IModelType
    public override bool ProvidesConfidence {
      get { return false; }
    }
    public override IRegressionModel Build(IRegressionProblemData pd, IRandom random, CancellationToken cancellationToken, out int numberOfParameters) {
      if (pd.Dataset.Rows < MinLeafSize(pd)) throw new ArgumentException("The number of training instances is too small to create a linear model");
      numberOfParameters = 1;
      return new PreconstructedLinearModel(pd.Dataset.GetDoubleValues(pd.TargetVariable).Average(), pd.TargetVariable);
    }

    public override int MinLeafSize(IRegressionProblemData pd) {
      return 0;
    }
    #endregion
  }
}
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
using System.Threading;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Problems.DataAnalysis;
using HEAL.Attic;

namespace HeuristicLab.Algorithms.DataAnalysis {
  [StorableType("5730B54C-7A8B-4CA7-8F37-7FF3F9848CD2")]
  [Item("ComponentReductionLinearLeaf", "A leaf type that uses principle component analysis to create smaller linear models as leaf models")]
  public class ComponentReductionLinearLeaf : LeafBase {
    public const string NumberOfComponentsParameterName = "NoComponents";
    public IFixedValueParameter<IntValue> NumberOfCompontentsParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters[NumberOfComponentsParameterName]; }
    }
    public int NumberOfComponents {
      get { return NumberOfCompontentsParameter.Value.Value; }
      set { NumberOfCompontentsParameter.Value.Value = value; }
    }

    #region Constructors & Cloning
    [StorableConstructor]
    protected ComponentReductionLinearLeaf(StorableConstructorFlag _) : base(_) { }
    protected ComponentReductionLinearLeaf(ComponentReductionLinearLeaf original, Cloner cloner) : base(original, cloner) { }
    public ComponentReductionLinearLeaf() {
      Parameters.Add(new FixedValueParameter<IntValue>(NumberOfComponentsParameterName, "The maximum number of principle components used (default=10)", new IntValue(10)));
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new ComponentReductionLinearLeaf(this, cloner);
    }
    #endregion

    #region IModelType
    public override bool ProvidesConfidence {
      get { return false; }
    }

    public override IRegressionModel Build(IRegressionProblemData pd, IRandom random,
      CancellationToken cancellationToken, out int numberOfParameters) {
      var pca = PrincipleComponentTransformation.CreateProjection(pd.Dataset, pd.TrainingIndices, pd.AllowedInputVariables, normalize: true);
      var pcdata = pca.TransformProblemData(pd);
      ComponentReducedLinearModel bestModel = null;
      var bestCvrmse = double.MaxValue;
      numberOfParameters = 1;
      for (var i = 1; i <= Math.Min(NumberOfComponents, pd.AllowedInputVariables.Count()); i++) {
        var pd2 = (IRegressionProblemData)pcdata.Clone();
        var inputs = new HashSet<string>(pca.ComponentNames.Take(i));
        foreach (var v in pd2.InputVariables.CheckedItems.ToArray())
          pd2.InputVariables.SetItemCheckedState(v.Value, inputs.Contains(v.Value.Value));
        double rmse;
        var model = PreconstructedLinearModel.CreateLinearModel(pd2, out rmse);
        if (rmse > bestCvrmse) continue;
        bestModel = new ComponentReducedLinearModel(pd2.TargetVariable, model, pca);
        numberOfParameters = i + 1;
        bestCvrmse = rmse;
      }
      return bestModel;
    }

    public override int MinLeafSize(IRegressionProblemData pd) {
      return NumberOfComponents + 2;
    }
    #endregion
  }
}
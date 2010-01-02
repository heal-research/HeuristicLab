#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.DataAnalysis;
using HeuristicLab.GP.Interfaces;

namespace HeuristicLab.GP.StructureIdentification.Classification {
  public abstract class GPClassificationEvaluatorBase : GPEvaluatorBase {

    public GPClassificationEvaluatorBase()
      : base() {
      AddVariableInfo(new VariableInfo("TargetClassValues", "The original class values of target variable (for instance negative=0 and positive=1).", typeof(ItemList<DoubleData>), VariableKind.In));
    }

    public override void Evaluate(IScope scope, IFunctionTree tree, ITreeEvaluator evaluator, Dataset dataset, int targetVariable, int start, int end) {

      ItemList<DoubleData> classes = GetVariableValue<ItemList<DoubleData>>("TargetClassValues", scope, true);
      double[] classesArr = new double[classes.Count];
      for (int i = 0; i < classesArr.Length; i++) classesArr[i] = classes[i].Data;
      Array.Sort(classesArr);
      double[] thresholds = new double[classes.Count - 1];
      for (int i = 0; i < classesArr.Length - 1; i++) {
        thresholds[i] = (classesArr[i] + classesArr[i + 1]) / 2.0;
      }

      Evaluate(scope, tree, evaluator, dataset, targetVariable, classesArr, thresholds, start, end);
    }

    public abstract void Evaluate(IScope scope, IFunctionTree tree, ITreeEvaluator evaluator, Dataset dataset, int targetVariable, double[] classes, double[] thresholds, int start, int end);
  }
}

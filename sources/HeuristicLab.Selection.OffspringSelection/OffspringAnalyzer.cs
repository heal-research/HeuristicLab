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
using System.Collections.Generic;
using System.Text;
using HeuristicLab.Core;
using HeuristicLab.Data;

namespace HeuristicLab.Selection.OffspringSelection {
  public class OffspringAnalyzer : OperatorBase {
    public override string Description {
      get { return @"TODO\r\nOperator description still missing ..."; }
    }

    public OffspringAnalyzer() {
      AddVariableInfo(new VariableInfo("Maximization", "Problem is a maximization problem", typeof(BoolData), VariableKind.In));
      AddVariableInfo(new VariableInfo("Quality", "Quality value", typeof(DoubleData), VariableKind.In));
      AddVariableInfo(new VariableInfo("ParentQualities", "Temporarily store parent qualities", typeof(DoubleArrayData), VariableKind.New | VariableKind.Out | VariableKind.In | VariableKind.Deleted));
      AddVariableInfo(new VariableInfo("SuccessfulChild", "True if the child was successful", typeof(BoolData), VariableKind.New));

      VariableInfo compFactorInfo = new VariableInfo("ComparisonFactor", "Factor for comparing the quality of a child with the qualities of its parents (0 = better than worst parent, 1 = better than best parent)", typeof(DoubleData), VariableKind.In);
      compFactorInfo.Local = true;
      AddVariableInfo(compFactorInfo);
      AddVariable(new Variable("ComparisonFactor", new DoubleData(0.5)));
      VariableInfo parentCount = new VariableInfo("ParentsCount", "How many parents created the offspring", typeof(IntData), VariableKind.In);
      parentCount.Local = true;
      AddVariableInfo(parentCount);
      AddVariable(new Variable("ParentsCount", new IntData(2)));
    }

    public override IOperation Apply(IScope scope) {
      bool maximize = GetVariableValue<BoolData>("Maximization", scope, true).Data;
      double compFact = GetVariableValue<DoubleData>("ComparisonFactor", scope, true).Data;
      int parentsCount = GetVariableValue<IntData>("ParentsCount", scope, true).Data;
      if (parentsCount < 1) throw new InvalidOperationException("OffspringAnalyzer: ParentsCount must be >= 1");

      DoubleArrayData qualities = GetVariableValue<DoubleArrayData>("ParentQualities", scope, false, false);
      if (qualities == null) {
        // fetch and store parent qualities
        IVariableInfo qualityInfo = GetVariableInfo("Quality");
        double[] qualitiesArray = new double[scope.SubScopes.Count];
        for (int i = 0; i < qualitiesArray.Length; i++)
          qualitiesArray[i] = scope.SubScopes[i].GetVariableValue<DoubleData>(qualityInfo.ActualName, false).Data;
        qualities = new DoubleArrayData(qualitiesArray);
        IVariableInfo parentQualitiesInfo = GetVariableInfo("ParentQualities");
        if (parentQualitiesInfo.Local)
          AddVariable(new Variable(parentQualitiesInfo.ActualName, qualities));
        else
          scope.AddVariable(new Variable(parentQualitiesInfo.ActualName, qualities));

        CompositeOperation next = new CompositeOperation();
        next.AddOperation(new AtomicOperation(SubOperators[0], scope));
        next.AddOperation(new AtomicOperation(this, scope));
        return next;
      } else {
        // analyze offspring
        for (int i = 0; i < scope.SubScopes.Count; i++) {
          double parent1 = double.MaxValue; // lowest quality parent
          double parent2 = double.MinValue; // highest quality parent
          for (int y = 0 ; y < parentsCount ; y++) {
            if (qualities.Data[parentsCount * i + y] < parent1) parent1 = qualities.Data[parentsCount * i + y];
            if (qualities.Data[parentsCount * i + y] > parent2) parent2 = qualities.Data[parentsCount * i + y];
          }
          IVariableInfo qualityInfo = GetVariableInfo("Quality");
          double child = scope.SubScopes[i].GetVariableValue<DoubleData>(qualityInfo.ActualName, false).Data;
          double threshold;

          if (!maximize)
            threshold = parent2 + (parent1 - parent2) * compFact;
          else
            threshold = parent1 + (parent2 - parent1) * compFact;

          IVariableInfo successfulInfo = GetVariableInfo("SuccessfulChild");
          BoolData successful;
          if (((!maximize) && (child < threshold)) ||
              ((maximize) && (child > threshold)))
            successful = new BoolData(true);
          else
            successful = new BoolData(false);
          scope.SubScopes[i].AddVariable(new Variable(successfulInfo.ActualName, successful));
        }

        // remove parent qualities again
        IVariableInfo parentQualitiesInfo = GetVariableInfo("ParentQualities");
        if (parentQualitiesInfo.Local)
          RemoveVariable(parentQualitiesInfo.ActualName);
        else
          scope.RemoveVariable(parentQualitiesInfo.ActualName);

        return null;
      }
    }
  }
}

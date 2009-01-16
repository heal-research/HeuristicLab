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
  /// <summary>
  /// Analyzes the offspring in a given scope whether it is successful or not.
  /// </summary>
  public class OffspringAnalyzer : OperatorBase {
    /// <inheritdoc select="summary"/>
    public override string Description {
      get { return @"TODO\r\nOperator description still missing ..."; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="OffspringAnalyzer"/> with six variable infos 
    /// (<c>Maximization</c>, <c>Quality</c>, <c>ParentQualities</c>, <c>SuccessfulChild</c>,
    /// <c>ComparisonFactor</c> and <c>ParentsCount</c>).
    /// </summary>
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

    /// <summary>
    /// Analyzes the offspring in the given scope whether the children are successful or not.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when <c>ParentsCount</c> smaller than 1.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the number of children is not constant or
    /// smaller than 1.</exception>
    /// <param name="scope">The scope whose offspring should be analyzed.</param>
    /// <returns>The next operation or null.</returns>
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
          qualitiesArray[i] = scope.SubScopes[i].GetVariableValue<DoubleData>(qualityInfo.FormalName, false).Data;
        qualities = new DoubleArrayData(qualitiesArray);
        IVariableInfo parentQualitiesInfo = GetVariableInfo("ParentQualities");
        if (parentQualitiesInfo.Local)
          AddVariable(new Variable(parentQualitiesInfo.ActualName, qualities));
        else
          scope.AddVariable(new Variable(scope.TranslateName(parentQualitiesInfo.FormalName), qualities));

        CompositeOperation next = new CompositeOperation();
        next.AddOperation(new AtomicOperation(SubOperators[0], scope));
        next.AddOperation(new AtomicOperation(this, scope));
        return next;
      } else {
        int crossoverEvents = qualities.Data.Length / parentsCount;
        int childrenPerCrossoverEvent = scope.SubScopes.Count / crossoverEvents;
        if (scope.SubScopes.Count % crossoverEvents != 0 ||
          childrenPerCrossoverEvent < 1) throw new InvalidOperationException("OffspringAnalyzer: The number of children per crossover event has to be constant and >= 1");
        // analyze offspring of all crossoverEvents
        for (int i = 0; i < crossoverEvents; i++) {
          double worstParent = double.MaxValue; // lowest quality parent
          double bestParent = double.MinValue; // highest quality parent
          for (int y = 0; y < parentsCount; y++) {
            if (qualities.Data[i * parentsCount + y] < worstParent) worstParent = qualities.Data[i * parentsCount + y];
            if (qualities.Data[i * parentsCount + y] > bestParent) bestParent = qualities.Data[i * parentsCount + y];
          }
          for (int j = 0; j < childrenPerCrossoverEvent; j++) {
            IVariableInfo qualityInfo = GetVariableInfo("Quality");
            double child = scope.SubScopes[i * childrenPerCrossoverEvent + j].GetVariableValue<DoubleData>(qualityInfo.FormalName, false).Data;
            double threshold;

            if (!maximize)
              threshold = bestParent + (worstParent - bestParent) * compFact;
            else
              threshold = worstParent + (bestParent - worstParent) * compFact;

            IVariableInfo successfulInfo = GetVariableInfo("SuccessfulChild");
            BoolData successful;
            if (((!maximize) && (child < threshold)) ||
                ((maximize) && (child > threshold)))
              successful = new BoolData(true);
            else
              successful = new BoolData(false);
            scope.SubScopes[i * childrenPerCrossoverEvent + j].AddVariable(new Variable(scope.TranslateName(successfulInfo.FormalName), successful));
          }
        }

        // remove parent qualities again
        IVariableInfo parentQualitiesInfo = GetVariableInfo("ParentQualities");
        if (parentQualitiesInfo.Local)
          RemoveVariable(parentQualitiesInfo.ActualName);
        else
          scope.RemoveVariable(scope.TranslateName(parentQualitiesInfo.FormalName));

        return null;
      }
    }
  }
}

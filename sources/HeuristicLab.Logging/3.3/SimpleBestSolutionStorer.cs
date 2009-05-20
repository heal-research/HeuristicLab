#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2009 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Logging {
  public class SimpleBestSolutionStorer : OperatorBase {

    public override string Description {
      get { return @"The simple best solution storer, stores the best solution.
If the rolling horizon is <= 0 the best solution is never forgotten. If this horizon is 1 only the actual best solution is stored.
If the horizon is n always the best of the last n solutions is stored.

The operator expects the solutions to be sorted so that the best solution is in the first scope."; }
    }

    public SimpleBestSolutionStorer()
      : base() {
      AddVariableInfo(new VariableInfo("Quality", "Quality value of a solution", typeof(DoubleData), VariableKind.In));
      AddVariableInfo(new VariableInfo("Maximization", "Whether the problem is a maximization problem or a minimization problem", typeof(BoolData), VariableKind.In));
      AddVariableInfo(new VariableInfo("BestSolution", "The scope representing the best solution", typeof(IScope), VariableKind.New | VariableKind.Out | VariableKind.In));
      AddVariableInfo(new VariableInfo("LastBestScopes", "Store a maximum of RollingHorizon solutions", typeof(ItemList<IScope>), VariableKind.New | VariableKind.Out | VariableKind.In));
      AddVariableInfo(new VariableInfo("RollingHorizon", "The rolling time horizon", typeof(IntData), VariableKind.In));
      GetVariableInfo("RollingHorizon").Local = true;
      AddVariable(new Variable("RollingHorizon", new IntData(0)));
    }

    public override IOperation Apply(IScope scope) {
      int horizon = GetVariableValue<IntData>("RollingHorizon", scope, true).Data;
      bool maximization = GetVariableValue<BoolData>("Maximization", scope, true).Data;
      double currentBestQuality, bestQuality;
      IVariable bestSolutionVar = null;
      IScope bestSolutionScope = null;
      IScope currentSolutionScope = (IScope)scope.SubScopes[0].Clone();

      #region variable setting
      IVariableInfo bestSolutionInfo = GetVariableInfo("BestSolution");
      if (bestSolutionInfo.Local == true)
        bestSolutionVar = GetVariable(bestSolutionInfo.ActualName);
      else bestSolutionVar = scope.GetVariable(scope.TranslateName("BestSolution"));
      
      if (bestSolutionVar != null) {
        bestSolutionScope = bestSolutionVar.Value as IScope;
        bestQuality = bestSolutionScope.GetVariableValue<DoubleData>(scope.TranslateName("Quality"), false).Data;
        currentBestQuality = currentSolutionScope.GetVariableValue<DoubleData>("Quality", false).Data;
      } else { // if no best solution exists then the first scope is added as the best solution and the operator is finished
        if (bestSolutionInfo.Local) AddVariable(new Variable(bestSolutionInfo.ActualName, currentSolutionScope));
        else scope.AddVariable(new Variable(scope.TranslateName("BestSolution"), currentSolutionScope));
        return null;
      }
      #endregion

      if (horizon <= 0) { // never forget the best quality
        if (currentBestQuality < bestQuality) {
          bestSolutionVar.Value = currentSolutionScope;
        }
      } else { // forget the best found quality after a certain time (given as horizon)
        ItemList<IScope> lastNBestScopes = scope.GetVariableValue<ItemList<IScope>>("LastBestScopes", false, false);
        if (lastNBestScopes == null) {
          lastNBestScopes = new ItemList<IScope>();
          scope.AddVariable(new Variable(scope.TranslateName("LastBestScopes"), lastNBestScopes));
        }
        bool currentIsNewBest = maximization && currentBestQuality > bestQuality || !maximization && currentBestQuality < bestQuality;
        if (currentIsNewBest) {
          bestSolutionScope = currentSolutionScope;
          bestSolutionVar.Value = bestSolutionScope;
          lastNBestScopes.Clear(); // the solutions that came before cannot be better, we don't need to keep them
        }
        lastNBestScopes.Add(currentSolutionScope); // add the current solution to the memory
        if (lastNBestScopes.Count > horizon) {
          if (!currentIsNewBest && lastNBestScopes.IndexOf(bestSolutionScope) == 0) { // if the best is to be "forgotton" the new best has to be found
            bool bestChanged = false;
            if (maximization) bestQuality = Double.MinValue;
            else bestQuality = Double.MaxValue;
            for (int i = 1; i < lastNBestScopes.Count; i++) {
              double quality = lastNBestScopes[i].GetVariableValue<DoubleData>("Quality", false).Data;
              if (maximization && quality > bestQuality
                || !maximization && quality < bestQuality) {
                bestSolutionScope = lastNBestScopes[i];
                bestQuality = quality;
                bestChanged = true;
              }
            }
            if (bestChanged) bestSolutionVar.Value = bestSolutionScope;
          }
          lastNBestScopes.RemoveAt(0);
        }
      }
      return null;
    }
  }
}

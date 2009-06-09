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
using HeuristicLab.Operators;

namespace HeuristicLab.Logging {
  /// <summary>
  /// Keeps a variable in the global scope that contains the scope representing the best solution.
  /// </summary>
  public class BestSolutionStorer : OperatorBase {
    /// <inheritdoc select="summary"/>
    public override string Description {
      get { return @"Keeps a variable in the global scope that contains the scope representing the best of run solution."; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="BestSolutionStorer"/> with three variable infos
    /// (<c>Quality</c>, <c>Maximization</c> and <c>BestSolution</c>).
    /// </summary>
    public BestSolutionStorer()
      : base() {
      AddVariableInfo(new VariableInfo("Quality", "Quality value of a solution", typeof(DoubleData), VariableKind.In));
      AddVariableInfo(new VariableInfo("Maximization", "Maximization problem", typeof(BoolData), VariableKind.In));
      AddVariableInfo(new VariableInfo("BestSolution", "The best solution of the run", typeof(IScope), VariableKind.New | VariableKind.In | VariableKind.Out));
    }

    /// <summary>
    /// Keeps a variable in the global scope that contains the scope representing the best solution.
    /// </summary>
    /// <param name="scope">The scope whose populations to check for the best solution.</param>
    /// <returns><c>null</c>.</returns>
    public override IOperation Apply(IScope scope) {
      if(scope.GetVariable(Guid.ToString() + "-Active") == null) {
        double[] qualities = new double[scope.SubScopes.Count];
        bool maximization = scope.GetVariableValue<BoolData>("Maximization", true).Data;
        for(int i = 0; i < scope.SubScopes.Count; i++)
          qualities[i] = scope.SubScopes[i].GetVariableValue<DoubleData>("Quality", false).Data;

        double smallest = qualities[0]; int smallestIndex = 0;
        double biggest = qualities[0]; int biggestIndex = 0;
        for(int i = 1; i < qualities.Length; i++) {
          if(qualities[i] < smallest) {
            smallest = qualities[i];
            smallestIndex = i;
          }
          if(qualities[i] > biggest) {
            biggest = qualities[i];
            biggestIndex = i;
          }
        }
        IVariableInfo qualityInfo = GetVariableInfo("Quality");
        IVariable bestSolutionVariable = scope.GetVariable(scope.TranslateName("BestSolution"));
        if(bestSolutionVariable != null) {

          double bestQuality = ((IScope)bestSolutionVariable.Value).GetVariableValue<DoubleData>(qualityInfo.ActualName, false).Data;

          // do nothing if the best solution of the current scope is not better than the best solution of the whole run so far.
          if((maximization && biggest <= bestQuality) ||
            (!maximization && smallest >= bestQuality)) return null;
        }

        IScope bestSolutionClone;
        if(maximization) {
          bestSolutionClone = (IScope)scope.SubScopes[biggestIndex].Clone();
        } else {
          bestSolutionClone = (IScope)scope.SubScopes[smallestIndex].Clone();
        }

        if(SubOperators.Count > 0) {
          scope.AddSubScope(bestSolutionClone);
          scope.AddVariable(new Variable(Guid.ToString() + "-Active", new BoolData(true)));

          CompositeOperation compOp = new CompositeOperation();
          AtomicOperation operation = new AtomicOperation(SubOperators[0], bestSolutionClone);
          AtomicOperation continuation = new AtomicOperation(this, scope);

          compOp.AddOperation(operation);
          compOp.AddOperation(continuation);
          return compOp;
        } else {
          StoreBestSolution(bestSolutionClone, scope);
          return null;
        }
      } else {  // operator already executed
        scope.RemoveVariable(Guid.ToString() + "-Active");
        IScope bestSolutionClone = scope.SubScopes[scope.SubScopes.Count - 1];
        scope.RemoveSubScope(bestSolutionClone);

        StoreBestSolution(bestSolutionClone, scope);
        return null;
      }
    }

    private void StoreBestSolution(IScope bestSolution, IScope scope) {
      SetValue(GetVariableInfo("BestSolution"), bestSolution, scope);
    }

    private void SetValue(IVariableInfo info, IScope data, IScope scope) {
      IVariable var = scope.GetVariable(info.ActualName);
      if(var == null) {
        var = new Variable(scope.TranslateName(info.FormalName), data);
        scope.AddVariable(var);
      } else {
        var.Value = data;
      }
    }
  }
}

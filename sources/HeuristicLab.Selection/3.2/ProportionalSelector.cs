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

namespace HeuristicLab.Selection {
  /// <summary>
  /// Copies or moves a number of sub scopes from a source scope to a target scope, their probability
  /// to be selected depending on their quality.
  /// </summary>
  public class ProportionalSelector : StochasticSelectorBase {
    /// <inheritdoc select="summary"/>
    public override string Description {
      get { return @"TODO\r\nOperator description still missing ..."; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="ProportionalSelector"/> with three variable infos 
    /// (<c>Maximization</c>, <c>Quality</c> and <c>Windowing</c>, being a local variable and initialized 
    /// with <c>true</c>) and the <c>CopySelected</c> flag set to <c>true</c>.
    /// </summary>
    public ProportionalSelector() {
      AddVariableInfo(new VariableInfo("Maximization", "Maximization problem", typeof(BoolData), VariableKind.In));
      AddVariableInfo(new VariableInfo("Quality", "Quality value", typeof(DoubleData), VariableKind.In));
      AddVariableInfo(new VariableInfo("Windowing", "Apply windowing strategy (selection probability is proportional to the quality differences and not to the total quality)", typeof(BoolData), VariableKind.In));
      GetVariableInfo("Windowing").Local = true;
      AddVariable(new Variable("Windowing", new BoolData(true)));
      GetVariable("CopySelected").GetValue<BoolData>().Data = true;
    }

    /// <summary>
    /// Copies or movies a number of sub scopes (<paramref name="selected"/>) in the given 
    /// <paramref name="source"/> to the given <paramref name="target"/>, selection takes place with respect
    /// to the quality of the scope.
    /// </summary>
    /// <param name="random">The random number generator.</param>
    /// <param name="source">The source scope from where to copy/move the sub scopes.</param>
    /// <param name="selected">The number of sub scopes to copy/move.</param>
    /// <param name="target">The target scope where to add the sub scopes.</param>
    /// <param name="copySelected">Boolean flag whether the sub scopes shall be moved or copied.</param>
    protected override void Select(IRandom random, IScope source, int selected, IScope target, bool copySelected) {
      bool maximization = GetVariableValue<BoolData>("Maximization", source, true).Data;
      IVariableInfo qualityInfo = GetVariableInfo("Quality");
      bool windowing = GetVariableValue<BoolData>("Windowing", source, true).Data;

      double[] qualities;
      double qualitySum;
      double selectedQuality;
      double sum;
      int j;

      GenerateQualitiesArray(source, maximization, qualityInfo, windowing, out qualities, out qualitySum);

      // perform selection
      for (int i = 0; i < selected; i++) {
        selectedQuality = random.NextDouble() * qualitySum;
        sum = 0;
        j = 0;
        while ((j < qualities.Length) && (sum <= selectedQuality)) {
          sum += qualities[j];
          j++;
        }
        IScope selectedScope = source.SubScopes[j - 1];
        if (copySelected)
          target.AddSubScope((IScope)selectedScope.Clone());
        else {
          source.RemoveSubScope(selectedScope);
          target.AddSubScope(selectedScope);
          GenerateQualitiesArray(source, maximization, qualityInfo, windowing, out qualities, out qualitySum);
        }
      }
    }

    /// <summary>
    /// Calculates the qualities of the sub scopes of the given <paramref name="source"/>.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when the sub scopes are not sorted according
    /// to their solution qualities or if the quality value is beyond zero and the <c>windowing</c>
    /// flag is set to <c>false</c>.</exception>
    /// <param name="source">The scource scope where to calculate the qualities.</param>
    /// <param name="maximization">Boolean flag whether is a maximization problem.</param>
    /// <param name="qualityInfo">The quality variable info.</param>
    /// <param name="windowing">Boolean flag whether the windowing strategy shall be applied.</param>
    /// <param name="qualities">Output parameter; contains all qualities of the sub scopes.</param>
    /// <param name="qualitySum">Output parameter; the sum of all qualities.</param>
    private void GenerateQualitiesArray(IScope source, bool maximization, IVariableInfo qualityInfo, bool windowing, out double[] qualities, out double qualitySum) {
      int subScopes = source.SubScopes.Count;
      qualities = new double[subScopes];
      qualitySum = 0;

      if (subScopes < 1) throw new InvalidOperationException("No source scopes to select available.");

      double best = source.SubScopes[0].GetVariableValue<DoubleData>(qualityInfo.FormalName, false).Data;
      double worst = source.SubScopes[subScopes - 1].GetVariableValue<DoubleData>(qualityInfo.FormalName, false).Data;
      double limit = Math.Min(worst * 2, double.MaxValue);
      double min = Math.Min(best, worst);
      double max = Math.Max(best, worst);
      double solutionQuality;

      // preprocess fitness values, apply windowing if desired
      for (int i = 0; i < qualities.Length; i++) {
        solutionQuality = source.SubScopes[i].GetVariableValue<DoubleData>(qualityInfo.FormalName, false).Data;
        if (solutionQuality < min || solutionQuality > max) {
          // something has obviously gone wrong here
          string errorMessage = "There is a problem with the ordering of the source sub-scopes in " + Name + ".\r\n" +
            "The quality of solution number " + i.ToString() + " is ";
          if (solutionQuality < min) errorMessage += "below";
          else errorMessage += "greater than";
          errorMessage += " the calculated qualities range:\r\n";
          errorMessage += solutionQuality.ToString() + " is outside the interval [ " + min.ToString() + " ; " + max.ToString() + " ].";
          throw new InvalidOperationException(errorMessage);
        }
        if (best != worst) {  // prevent division by zero
          if (windowing) {
            if (!maximization) {
              qualities[i] = 1 - ((solutionQuality - best) / (worst - best));
            } else {
              qualities[i] = (solutionQuality - worst) / (best - worst);
            }
          } else {
            if (solutionQuality < 0.0) throw new InvalidOperationException("ERROR in ProportionalSelector: Non-windowing is not working with quality values < 0. Use windowing.");
            if (!maximization) qualities[i] = limit - solutionQuality;
            else qualities[i] = solutionQuality;
          }
        } else {  // best == worst -> all fitness values are equal
          qualities[i] = 1;
        }
        qualitySum += qualities[i];
      }
      if (double.IsInfinity(qualitySum)) qualitySum = double.MaxValue;
    }
  }
}

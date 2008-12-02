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
using HeuristicLab.Permutation;

namespace HeuristicLab.Routing.TSP {
  /// <summary>
  /// Injects a new TSP tour in a given scope with all its neccessary variables.
  /// </summary>
  public class TSPTourInjector : OperatorBase {
    /// <inheritdoc/>
    public override string Description {
      get { return @"TODO\r\nOperator description still missing ..."; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="TSPTourInjector"/> with three variable infos 
    /// (<c>Coordinates</c>, <c>Permutation</c> and <c>Tour</c>).
    /// </summary>
    public TSPTourInjector() {
      AddVariableInfo(new VariableInfo("Coordinates", "City coordinates", typeof(DoubleMatrixData), VariableKind.In));
      AddVariableInfo(new VariableInfo("Permutation", "Permutation representing a TSP solution in path encoding", typeof(Permutation.Permutation), VariableKind.In));
      AddVariableInfo(new VariableInfo("Tour", "Object representing a whole TSP tour", typeof(TSPTour), VariableKind.New | VariableKind.Out));
    }

    /// <summary>
    /// Adds a new <see cref="TSPTour"/> to the given <paramref name="scope"/> with all 
    /// its neccessary variables.
    /// </summary>
    /// <param name="scope">The current scope where to inject the TSP tour.</param>
    /// <returns><c>null</c>.</returns>
    public override IOperation Apply(IScope scope) {
      DoubleMatrixData coordinates = GetVariableValue<DoubleMatrixData>("Coordinates", scope, true);
      Permutation.Permutation permutation = GetVariableValue<Permutation.Permutation>("Permutation", scope, false);
      TSPTour tour = GetVariableValue<TSPTour>("Tour", scope, false, false);
      if (tour == null) {
        tour = new TSPTour(coordinates, permutation);
        IVariableInfo info = GetVariableInfo("Tour");
        if (info.Local)
          AddVariable(new Variable(info.ActualName, tour));
        else
          scope.AddVariable(new Variable(scope.TranslateName(info.FormalName), tour));
      } else {
        tour.Coordinates = coordinates;
        tour.Tour = permutation;
      }
      return null;
    }
  }
}

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
using System.Xml;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;

namespace HeuristicLab.Routing.TSP {
  /// <summary>
  /// Injects a new TSP in a given scope with all its needed variables....
  /// </summary>
  public class TSPInjector : OperatorBase {
    /// <inheritdoc select="summary"/>
    public override string Description {
      get { return @"TODO\r\nOperator description still missing ..."; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="TSPInjector"/> with four variable infos 
    /// (<c>Maximization</c>, <c>Cities</c>, <c>Coordinates</c> and <c>BestKnownQuality</c>).
    /// </summary>
    public TSPInjector()
      : base() {
      AddVariableInfo(new VariableInfo("Maximization", "Set to false as TSP is a minimization problem", typeof(BoolData), VariableKind.New));
      AddVariableInfo(new VariableInfo("Cities", "Number of cities", typeof(IntData), VariableKind.New));
      AddVariable(new Variable("Cities", new IntData(0)));
      AddVariableInfo(new VariableInfo("Coordinates", "City coordinates", typeof(DoubleMatrixData), VariableKind.New));
      AddVariable(new Variable("Coordinates", new DoubleMatrixData(new double[0,0])));
      AddVariableInfo(new VariableInfo("BestKnownQuality", "Quality of best known solution", typeof(DoubleData), VariableKind.New));
      AddVariable(new Variable("InjectBestKnownQuality", new BoolData(false)));
      AddVariable(new Variable("BestKnownQuality", new DoubleData(0)));
    }

    /// <summary>
    /// Creates a new instance of <see cref="TSPInjectorView"/> to display the current instance.
    /// </summary>
    /// <returns>The created view as <see cref="TSPInjectorView"/>.</returns>
    public override IView CreateView() {
      return new TSPInjectorView(this);
    }

    /// <summary>
    /// Adds a new TSP to the given <paramref name="scope"/>, through adding the needed variables.
    /// </summary>
    /// <param name="scope">The current scope where to inject the variables.</param>
    /// <returns><c>null</c>.</returns>
    public override IOperation Apply(IScope scope) {
      scope.AddVariable(new Variable(scope.TranslateName("Maximization"), new BoolData(false)));
      scope.AddVariable(new Variable(scope.TranslateName("Cities"), (IItem)GetVariable("Cities").Value.Clone()));
      scope.AddVariable(new Variable(scope.TranslateName("Coordinates"), (IItem)GetVariable("Coordinates").Value.Clone()));
      if (GetVariable("InjectBestKnownQuality").GetValue<BoolData>().Data)
        scope.AddVariable(new Variable(scope.TranslateName("BestKnownQuality"), (IItem)GetVariable("BestKnownQuality").Value.Clone()));
      return null;
    }
  }
}

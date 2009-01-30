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

namespace HeuristicLab.TestFunctions {
  /// <summary>
  /// Injects the necessary variables for optimizing a test function
  /// </summary>
  public class TestFunctionInjector : OperatorBase {
    /// <inheritdoc select="summary"/>
    public override string Description {
      get {
        return @"Injects the necessary variables for optimizing a test function";
      }
    }

    /// <summary>
    /// Gets or sets the boolean flag whether it is an optimization problem or not.
    /// </summary>
    public bool Maximization {
      get { return GetVariable("Maximization").GetValue<BoolData>().Data; }
      set { GetVariable("Maximization").Value = new BoolData(value); }
    }

    /// <summary>
    /// Gets or sets the lower bound.
    /// </summary>
    public double LowerBound {
      get { return GetVariable("LowerBound").GetValue<DoubleData>().Data; }
      set { GetVariable("LowerBound").Value = new DoubleData(value); }
    }

    /// <summary>
    /// Gets or sets the upper bound.
    /// </summary>
    public double UpperBound {
      get { return GetVariable("UpperBound").GetValue<DoubleData>().Data; }
      set { GetVariable("UpperBound").Value = new DoubleData(value); }
    }

    /// <summary>
    /// Gets or sets the dimension.
    /// </summary>
    public int Dimension {
      get { return GetVariable("Dimension").GetValue<IntData>().Data; }
      set { GetVariable("Dimension").Value = new IntData(value); }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="TestFunctionInjector"/> with four variable infos
    /// (<c>Maximization</c>, <c>LowerBound</c>, <c>UpperBound</c> and <c>Dimension</c>).
    /// </summary>
    public TestFunctionInjector()
      : base() {
      AddVariable(new Variable("Maximization", new BoolData(false)));
      AddVariable(new Variable("LowerBound", new DoubleData(-32.76)));
      AddVariable(new Variable("UpperBound", new DoubleData(32.76)));
      AddVariable(new Variable("Dimension", new IntData(2)));
    }

    /// <summary>
    /// Injects the necessary variables for optimizing a test function.
    /// </summary>
    /// <param name="scope">The scope where to inject the variables.</param>
    /// <returns><c>null</c>.</returns>
    public override IOperation Apply(IScope scope) {
      scope.AddVariable((IVariable)GetVariable("Maximization").Clone());
      scope.AddVariable((IVariable)GetVariable("LowerBound").Clone());
      scope.AddVariable((IVariable)GetVariable("UpperBound").Clone());
      scope.AddVariable((IVariable)GetVariable("Dimension").Clone());
      return null;
    }

    /// <summary>
    /// Creates a new instance of <see cref="TestFunctionInjectorView"/> to represent the current 
    /// instance visually.
    /// </summary>
    /// <returns>The newly created view as <see cref="TestFunctionInjectorView"/>.</returns>
    public override IView CreateView() {
      return new TestFunctionInjectorView(this);
    }
  }
}

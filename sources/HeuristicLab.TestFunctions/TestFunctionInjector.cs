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
  public class TestFunctionInjector : OperatorBase {
    public override string Description {
      get {
        return @"Injects the necessary variables for optimizing a test function";
      }
    }

    public bool Maximization {
      get { return GetVariable("Maximization").GetValue<BoolData>().Data; }
      set { GetVariable("Maximization").Value = new BoolData(value); }
    }

    public double Minimum {
      get { return GetVariable("Minimum").GetValue<DoubleData>().Data; }
      set { GetVariable("Minimum").Value = new DoubleData(value); }
    }

    public double Maximum {
      get { return GetVariable("Maximum").GetValue<DoubleData>().Data; }
      set { GetVariable("Maximum").Value = new DoubleData(value); }
    }

    public int Length {
      get { return GetVariable("Length").GetValue<IntData>().Data; }
      set { GetVariable("Length").Value = new IntData(value); }
    }

    public TestFunctionInjector()
      : base() {
      AddVariable(new Variable("Maximization", new BoolData(false)));
      AddVariable(new Variable("Minimum", new DoubleData(-32.76)));
      AddVariable(new Variable("Maximum", new DoubleData(32.76)));
      AddVariable(new Variable("Length", new IntData(2)));
    }

    public override IOperation Apply(IScope scope) {
      scope.AddVariable((IVariable)GetVariable("Maximization").Clone());
      scope.AddVariable((IVariable)GetVariable("Minimum").Clone());
      scope.AddVariable((IVariable)GetVariable("Maximum").Clone());
      scope.AddVariable((IVariable)GetVariable("Length").Clone());
      return null;
    }

    public override IView CreateView() {
      return new TestFunctionInjectorView(this);
    }
  }
}

#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using System.Collections.Generic;
namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.GeneralSymbols {
  [StorableClass]
  public sealed class ValuesTreeNode : SymbolicExpressionTreeNode {
    private List<string> allowedFunctionsInBranch;
    [Storable]
    public IEnumerable<string> AllowedFunctionsInBranch {
      get { return allowedFunctionsInBranch; }
      set { allowedFunctionsInBranch = new List<string>(value); }
    }

    [Storable]
    private Dictionary<string, int> numberOfArguments;

    // copy constructor
    private ValuesTreeNode(ValuesTreeNode original)
      : base(original) {
      allowedFunctionsInBranch = new List<string>(original.allowedFunctionsInBranch);
      numberOfArguments = new Dictionary<string, int>(original.numberOfArguments);
    }

    public ValuesTreeNode(Values valuesSymbol)
      : base(valuesSymbol) {
      numberOfArguments = new Dictionary<string, int>();
      allowedFunctionsInBranch = new List<string>();
    }

    public int GetNumberOfArguments(string functionName) {
      return numberOfArguments[functionName];
    }

    public int SetNumberOfArguments(string functionName, int numberOfArguments) {
      return this.numberOfArguments[functionName] = numberOfArguments;
    }

    public override object Clone() {
      return new ValuesTreeNode(this);
    }
  }
}

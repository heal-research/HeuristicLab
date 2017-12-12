#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  [StorableClass]
  [Item("BinaryFactorVariable", "Represents a categorical variable (comparable to factors as in R) and it's value.")]
  public sealed class BinaryFactorVariable : VariableBase {

    private readonly Dictionary<string, List<string>> variableValues;

    [Storable]
    public IEnumerable<KeyValuePair<string, List<string>>> VariableValues {
      get { return variableValues; }
      set {
        if (value == null) throw new ArgumentNullException();
        variableValues.Clear();
        foreach (var kvp in value) {
          variableValues.Add(kvp.Key, new List<string>(kvp.Value));
        }
      }
    }

    [StorableConstructor]
    private BinaryFactorVariable(bool deserializing)
      : base(deserializing) {
      variableValues = new Dictionary<string, List<string>>();
    }
    private BinaryFactorVariable(BinaryFactorVariable original, Cloner cloner)
      : base(original, cloner) {
      variableValues =
        original.variableValues.ToDictionary(kvp => kvp.Key, kvp => new List<string>(kvp.Value));
    }
    public BinaryFactorVariable() : this("BinaryFactorVariable", "Represents a categorical variable (comparable to factors as in R) and it's value.") { }
    public BinaryFactorVariable(string name, string description)
      : base(name, description) {
      variableValues = new Dictionary<string, List<string>>();
    }

    public override ISymbolicExpressionTreeNode CreateTreeNode() {
      return new BinaryFactorVariableTreeNode(this);
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new BinaryFactorVariable(this, cloner);
    }

    public IEnumerable<string> GetVariableValues(string variableName) {
      return variableValues[variableName];
    }
  }
}

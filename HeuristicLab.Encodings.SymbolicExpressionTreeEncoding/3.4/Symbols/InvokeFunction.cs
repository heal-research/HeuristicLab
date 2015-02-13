#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding {
  /// <summary>
  /// Symbol for invoking automatically defined functions
  /// </summary>
  [StorableClass]
  [Item(InvokeFunction.InvokeFunctionName, InvokeFunction.InvokeFunctionDescription)]
  public sealed class InvokeFunction : Symbol, IReadOnlySymbol {
    public const string InvokeFunctionName = "InvokeFunction";
    public const string InvokeFunctionDescription = "Symbol that the invocation of another function.";
    private const int minimumArity = 0;
    private const int maximumArity = byte.MaxValue;

    public override int MinimumArity {
      get { return minimumArity; }
    }
    public override int MaximumArity {
      get { return maximumArity; }
    }

    [Storable]
    private string functionName;
    public string FunctionName {
      get { return functionName; }
    }

    [StorableConstructor]
    private InvokeFunction(bool deserializing) : base(deserializing) { }
    private InvokeFunction(InvokeFunction original, Cloner cloner)
      : base(original, cloner) {
      functionName = original.functionName;
      name = "Invoke: " + original.functionName;
    }
    public InvokeFunction(string functionName)
      : base("Invoke: " + functionName, InvokeFunction.InvokeFunctionDescription) {
      this.functionName = functionName;
    }

    public override ISymbolicExpressionTreeNode CreateTreeNode() {
      return new InvokeFunctionTreeNode(this);
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new InvokeFunction(this, cloner);
    }
  }
}

#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.ExternalEvaluation.GP {
  [Item("SymbolicExpressionTreeStringConverter", "Converts a symbolic expression tree into a string representation by iterating over all nodes in a prefix way. The string is added to the SolutionMessage's StringVars.")]
  [StorableClass]
  public class SymbolicExpressionTreeStringConverter : SymbolicExpressionTreeConverter {
    private ExternalEvaluationSymbolicExpressionTreeStringFormatter formatter;

    [StorableConstructor]
    protected SymbolicExpressionTreeStringConverter(bool deserializing) : base(deserializing) { }
    protected SymbolicExpressionTreeStringConverter(SymbolicExpressionTreeStringConverter original, Cloner cloner)
      : base(original, cloner) {
      formatter = new ExternalEvaluationSymbolicExpressionTreeStringFormatter();
      formatter.Indent = original.formatter.Indent;
    }
    public SymbolicExpressionTreeStringConverter()
      : base() {
      Initialize();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicExpressionTreeStringConverter(this, cloner);
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      Initialize();
    }

    private void Initialize() {
      formatter = new ExternalEvaluationSymbolicExpressionTreeStringFormatter();
      formatter.Indent = false;
    }

    protected override void ConvertSymbolicExpressionTree(SymbolicExpressionTree tree, string name, SolutionMessage.Builder builder) {
      string stringRep = formatter.Format(tree);
      stringRep.Replace(Environment.NewLine, "");
      SolutionMessage.Types.StringVariable.Builder stringVariable = SolutionMessage.Types.StringVariable.CreateBuilder();
      stringVariable.SetName(name).SetData(stringRep);
      builder.AddStringVars(stringVariable.Build());
    }
  }
}

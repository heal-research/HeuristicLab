#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HEAL.Attic;

namespace HeuristicLab.Problems.GeneticProgramming.Robocode {
  [StorableType("104E6124-7427-4639-A740-F68384CD8592")]
  // a symbol that can represent any user-defined fragment of code
  public sealed class CodeSymbol : Symbol {
    public override int MinimumArity { get { return 1; } }
    public override int MaximumArity { get { return 10; } }

    public string Prefix { get; set; }
    public string Suffix { get; set; }

    public override bool CanChangeName { get { return false; } } // cannot change, otherwise we cannot detect these symbols in the interpreter
    public override bool CanChangeDescription { get { return false; } }

    [StorableConstructor]
    private CodeSymbol(StorableConstructorFlag _) : base(_) { }
    private CodeSymbol(CodeSymbol original, Cloner cloner)
      : base(original, cloner) {
      Prefix = original.Prefix;
      Suffix = original.Suffix;
    }
    public CodeSymbol()
      : base("CodeSymbol", "The CodeSymbol symbol can represent any user-defined fragment of code.") {
      Prefix = string.Empty;
      Suffix = string.Empty;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new CodeSymbol(this, cloner);
    }
  }
}

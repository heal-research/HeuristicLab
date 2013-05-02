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

using System.Linq;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Problems.DataAnalysis.Symbolic;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding_34.Tests {
  public static class Grammars {
    public static ISymbolicExpressionGrammar CreateSimpleArithmeticGrammar() {
      var g = new TypeCoherentExpressionGrammar();
      g.ConfigureAsDefaultRegressionGrammar();
      g.Symbols.OfType<Variable>().First().Enabled = false;
      //var g = new SimpleArithmeticGrammar();
      g.MaximumFunctionArguments = 0;
      g.MinimumFunctionArguments = 0;
      g.MaximumFunctionDefinitions = 0;
      g.MinimumFunctionDefinitions = 0;
      return g;
    }

    public static ISymbolicExpressionGrammar CreateArithmeticAndAdfGrammar() {
      var g = new TypeCoherentExpressionGrammar();
      g.ConfigureAsDefaultRegressionGrammar();
      g.Symbols.OfType<Variable>().First().Enabled = false;
      g.MaximumFunctionArguments = 3;
      g.MinimumFunctionArguments = 0;
      g.MaximumFunctionDefinitions = 3;
      g.MinimumFunctionDefinitions = 0;
      return g;
    }
  }
}

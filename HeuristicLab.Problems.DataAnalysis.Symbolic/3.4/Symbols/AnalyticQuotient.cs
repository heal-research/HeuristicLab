#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HEAL.Attic;
namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  [StorableType("1BFD8CB7-7377-4130-9903-1E9A76349285")]
  [Item("Analytic Quotient", "The analytic quotient function aq(a,b) = a / sqrt(b²+1) can be used as an " +
    "alternative to protected division. See H. Drieberg and P. Rocket, The Use of an Analytic Quotient Operator" +
    " in Genetic Programming. IEEE Transactions on Evolutionary Computation, Vol. 17, No. 1, February 2013, pp. 146 -- 152")]
  public sealed class AnalyticQuotient : Symbol {
    private const int minimumArity = 2;
    private const int maximumArity = 2;

    public override int MinimumArity {
      get { return minimumArity; }
    }
    public override int MaximumArity {
      get { return maximumArity; }
    }

    [StorableConstructor]
    private AnalyticQuotient(StorableConstructorFlag _) : base(_) { }
    private AnalyticQuotient(AnalyticQuotient original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new AnalyticQuotient(this, cloner);
    }
    public AnalyticQuotient() : base("AnalyticQuotient", "The analytic quotient function aq(a,b) = a / sqrt(b²+1) can be used as an " +
    "alternative to protected division. See H. Drieberg and P. Rocket, The Use of an Analytic Quotient Operator" +
    " in Genetic Programming. IEEE Transactions on Evolutionary Computation, Vol. 17, No. 1, February 2013, pp. 146 -- 152") { }
  }
}

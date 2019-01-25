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
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HEAL.Attic;

namespace HeuristicLab.Optimization.Operators {
  [Item("UserDefinedEvaluator", "An evaluator that can be customized with operators which it will execute one after another.")]
  [StorableType("BE378550-EAD2-4D4B-A2FD-3CBC563B2C2B")]
  public class UserDefinedEvaluator : UserDefinedOperator, ISingleObjectiveEvaluator {
    public ILookupParameter<DoubleValue> QualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["Quality"]; }
    }

    [StorableConstructor]
    protected UserDefinedEvaluator(StorableConstructorFlag _) : base(_) { }
    protected UserDefinedEvaluator(UserDefinedEvaluator original, Cloner cloner) : base(original, cloner) { }
    public UserDefinedEvaluator()
      : base() {
      Parameters.Add(new LookupParameter<DoubleValue>("Quality", "The quality value of the solution."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new UserDefinedEvaluator(this, cloner);
    }
  }
}

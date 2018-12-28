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
using HEAL.Fossil;
using HeuristicLab.Parameters;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding {
  [StorableType("C8864746-308A-49F0-B76F-92D2C7E33574")]
  [Item("FullTreeShaker", "Manipulates all nodes that have local parameters.")]
  public sealed class FullTreeShaker : SymbolicExpressionTreeManipulator {
    private const string ShakingFactorParameterName = "ShakingFactor";
    #region parameter properties
    public IValueParameter<DoubleValue> ShakingFactorParameter {
      get { return (IValueParameter<DoubleValue>)Parameters[ShakingFactorParameterName]; }
    }
    #endregion
    #region properties
    public double ShakingFactor {
      get { return ShakingFactorParameter.Value.Value; }
      set { ShakingFactorParameter.Value.Value = value; }
    }
    #endregion
    [StorableConstructor]
    private FullTreeShaker(StorableConstructorFlag _) : base(_) { }
    private FullTreeShaker(FullTreeShaker original, Cloner cloner) : base(original, cloner) { }
    public FullTreeShaker()
      : base() {
      Parameters.Add(new FixedValueParameter<DoubleValue>(ShakingFactorParameterName, "The shaking factor that should be used for the manipulation of constants (default=1.0).", new DoubleValue(1.0)));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new FullTreeShaker(this, cloner);
    }

    protected override void Manipulate(IRandom random, ISymbolicExpressionTree tree) {
      tree.Root.ForEachNodePostfix(node => {
        if (node.HasLocalParameters) {
          node.ShakeLocalParameters(random, ShakingFactor);
        }
      });
    }
  }
}

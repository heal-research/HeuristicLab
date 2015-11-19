#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.LinearLinkageEncoding {
  [Item("Linear Linkage Manipulator", "Base class for linear linkage manipulators.")]
  [StorableClass]
  public abstract class LinearLinkageManipulator : InstrumentedOperator, ILinearLinkageManipulator, IStochasticOperator {
    public ILookupParameter<IRandom> RandomParameter {
      get { return (ILookupParameter<IRandom>)Parameters["Random"]; }
    }

    public ILookupParameter<LinearLinkage> LLEParameter {
      get { return (ILookupParameter<LinearLinkage>)Parameters["LLE"]; }
    }

    [StorableConstructor]
    protected LinearLinkageManipulator(bool deserializing) : base(deserializing) { }
    protected LinearLinkageManipulator(LinearLinkageManipulator original, Cloner cloner) : base(original, cloner) { }
    protected LinearLinkageManipulator() {
      Parameters.Add(new LookupParameter<IRandom>("Random", "The random number generator to use."));
      Parameters.Add(new LookupParameter<LinearLinkage>("LLE", "The encoding vector that is to be manipulated."));
    }

    public override IOperation InstrumentedApply() {
      var random = RandomParameter.ActualValue;
      var lle = LLEParameter.ActualValue;
      Manipulate(random, lle);
      return base.InstrumentedApply();
    }

    protected abstract void Manipulate(IRandom random, LinearLinkage lle);
  }
}

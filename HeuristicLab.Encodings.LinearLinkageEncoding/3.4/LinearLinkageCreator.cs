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
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HEAL.Fossil;

namespace HeuristicLab.Encodings.LinearLinkageEncoding {
  [Item("Linear Linkage Creator", "Base class for linear linkage creators.")]
  [StorableType("A3F7BDD5-B608-4C74-87C1-6D70F8D90B40")]
  public abstract class LinearLinkageCreator : InstrumentedOperator, ILinearLinkageCreator, IStochasticOperator {

    public ILookupParameter<IRandom> RandomParameter {
      get { return (ILookupParameter<IRandom>)Parameters["Random"]; }
    }

    public ILookupParameter<LinearLinkage> LLEParameter {
      get { return (ILookupParameter<LinearLinkage>)Parameters["LLE"]; }
    }

    public IValueLookupParameter<IntValue> LengthParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters["Length"]; }
    }

    [StorableConstructor]
    protected LinearLinkageCreator(StorableConstructorFlag _) : base(_) { }
    protected LinearLinkageCreator(LinearLinkageCreator original, Cloner cloner) : base(original, cloner) { }
    protected LinearLinkageCreator() {
      Parameters.Add(new LookupParameter<IRandom>("Random", "The random number generator to use."));
      Parameters.Add(new LookupParameter<LinearLinkage>("LLE", "The encoding vector that is to be created.") { Hidden = false });
      Parameters.Add(new ValueLookupParameter<IntValue>("Length", "The length of the vector that is to be created."));
    }

    public override IOperation InstrumentedApply() {
      var random = RandomParameter.ActualValue;
      LLEParameter.ActualValue = Create(random, LengthParameter.ActualValue.Value);
      return base.InstrumentedApply();
    }

    protected abstract LinearLinkage Create(IRandom random, int length);
  }
}

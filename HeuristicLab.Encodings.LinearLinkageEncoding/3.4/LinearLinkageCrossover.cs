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

using System;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HEAL.Fossil;

namespace HeuristicLab.Encodings.LinearLinkageEncoding {
  [Item("Linear Linkage Crossover", "Base class for linear linkage crossovers.")]
  [StorableType("AB45F937-ECD4-4658-BC9E-793EE0453A0C")]
  public abstract class LinearLinkageCrossover : InstrumentedOperator, ILinearLinkageCrossover, IStochasticOperator {

    public ILookupParameter<IRandom> RandomParameter {
      get { return (ILookupParameter<IRandom>)Parameters["Random"]; }
    }

    public ILookupParameter<LinearLinkage> ChildParameter {
      get { return (ILookupParameter<LinearLinkage>)Parameters["Child"]; }
    }

    public IScopeTreeLookupParameter<LinearLinkage> ParentsParameter {
      get { return (IScopeTreeLookupParameter<LinearLinkage>)Parameters["Parents"]; }
    }

    [StorableConstructor]
    protected LinearLinkageCrossover(StorableConstructorFlag _) : base(_) { }
    protected LinearLinkageCrossover(LinearLinkageCrossover original, Cloner cloner) : base(original, cloner) { }
    protected LinearLinkageCrossover() {
      Parameters.Add(new LookupParameter<IRandom>("Random", "The random number generator to use."));
      Parameters.Add(new LookupParameter<LinearLinkage>("Child", "The child that is to be created."));
      Parameters.Add(new ScopeTreeLookupParameter<LinearLinkage>("Parents", "The parents that are to be crossed."));
    }

    public override IOperation InstrumentedApply() {
      var random = RandomParameter.ActualValue;
      var parents = ParentsParameter.ActualValue;
      if (parents == null) throw new InvalidOperationException(Name + ": Parents could not be found.");
      var child = Cross(random, parents);
      ChildParameter.ActualValue = child;
      return base.InstrumentedApply();
    }

    protected abstract LinearLinkage Cross(IRandom random, ItemArray<LinearLinkage> parents);
  }
}

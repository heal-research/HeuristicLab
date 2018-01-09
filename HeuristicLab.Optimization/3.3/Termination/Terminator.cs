#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.Collections.Generic;
using System.Drawing;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Optimization {
  [Item("Terminator", "A base class for all termination criteria.")]
  [StorableClass]
  public abstract class Terminator : SingleSuccessorOperator, ITerminator {
    public static new Image StaticItemImage {
      get { return HeuristicLab.Common.Resources.VSImageLibrary.FlagRed; }
    }

    public ILookupParameter<BoolValue> TerminateParameter {
      get { return (ILookupParameter<BoolValue>)Parameters["Terminate"]; }
    }

    [StorableConstructor]
    protected Terminator(bool deserializing) : base(deserializing) { }
    protected Terminator(Terminator original, Cloner cloner) : base(original, cloner) { }

    protected Terminator()
      : base() {
      Parameters.Add(new LookupParameter<BoolValue>("Terminate", "The parameter which will be set to determine if execution should be terminated or should continue."));
    }

    public sealed override IOperation Apply() {
      if (!TerminateParameter.ActualValue.Value) { // If terminate flag is already set, no need to check further termination criteria.
        bool terminate = !CheckContinueCriterion();
        TerminateParameter.ActualValue.Value = terminate;
      }
      return base.Apply();
    }

    protected abstract bool CheckContinueCriterion();

    public override void CollectParameterValues(IDictionary<string, IItem> values) {
      base.CollectParameterValues(values);
      values["Type"] = new StringValue(GetType().GetPrettyName(false));
    }
  }
}
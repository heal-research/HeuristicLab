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
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HEAL.Attic;

namespace HeuristicLab.Algorithms.ALPS {
  [Item("ReseedingController", "An operator that controls if reseeding is needed.")]
  [StorableType("67294ABE-C118-41F0-B736-0576F30F664F")]
  public sealed class ReseedingController : SingleSuccessorOperator {
    public ILookupParameter<IntValue> GenerationsParameter {
      get { return (ILookupParameter<IntValue>)Parameters["Generations"]; }
    }
    public ILookupParameter<IntValue> AgeGapParameter {
      get { return (ILookupParameter<IntValue>)Parameters["AgeGap"]; }
    }
    public OperatorParameter FirstLayerOperatorParameter {
      get { return (OperatorParameter)Parameters["FirstLayerOperator"]; }
    }

    public IOperator FirstLayerOperator {
      get { return FirstLayerOperatorParameter.Value; }
      set { FirstLayerOperatorParameter.Value = value; }
    }

    [StorableConstructor]
    private ReseedingController(StorableConstructorFlag _) : base(_) { }

    private ReseedingController(ReseedingController original, Cloner cloner)
      : base(original, cloner) {
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new ReseedingController(this, cloner);
    }

    public ReseedingController()
      : base() {
      Parameters.Add(new LookupParameter<IntValue>("Generations", "The current number of generations."));
      Parameters.Add(new ValueLookupParameter<IntValue>("AgeGap", "The frequency of reseeding the lowest layer and scaling factor for the age-limits for the layers."));
      Parameters.Add(new OperatorParameter("FirstLayerOperator", "The operator that is performed on the first layer if reseeding is required."));
    }

    public override IOperation Apply() {
      int generations = GenerationsParameter.ActualValue.Value;
      int ageGap = AgeGapParameter.ActualValue.Value;

      var next = new OperationCollection(base.Apply());
      if (generations % ageGap == 0) {
        var layerZeroScope = ExecutionContext.Scope.SubScopes[0];
        if (FirstLayerOperator != null)
          next.Insert(0, ExecutionContext.CreateChildOperation(FirstLayerOperator, layerZeroScope));
      }
      return next;
    }
  }
}
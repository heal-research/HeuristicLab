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

using System;
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.BinaryVectorEncoding;
using HeuristicLab.Parameters;

namespace HeuristicLab.Problems.Binary {
  [Item("Deceptive Step Trap Problem", "Genome encodes completely separable blocks, where each block deceptive with fitness plateaus.")]
  [StorableType("89777145-7979-4B7B-B798-5F7C7E892A21")]
  [Creatable(CreatableAttribute.Categories.CombinatorialProblems, Priority = 230)]
  public class DeceptiveStepTrapProblem : DeceptiveTrapProblem {
    [Storable] private int offset = -1;
    [Storable] public IFixedValueParameter<IntValue> StepSizeParameter { get; private set; }

    public int StepSize {
      get { return StepSizeParameter.Value.Value; }
      set { StepSizeParameter.Value.Value = value; }
    }

    public DeceptiveStepTrapProblem() : base() {
      Parameters.Add(StepSizeParameter = new FixedValueParameter<IntValue>("Step Size", "", new IntValue(2)));
      offset = (TrapSize - StepSize) % StepSize;

      RegisterParameterEvents();
    }

    protected override int TrapMaximum {
      get { return (offset + TrapSize) / StepSize; }
    }

    protected override int Score(BinaryVector individual, int trapIndex, int trapSize) {
      int partial = base.Score(individual, trapIndex, trapSize);
      // introduce plateaus using integer division
      return (offset + partial) / StepSize;
    }

    [StorableConstructor]
    protected DeceptiveStepTrapProblem(StorableConstructorFlag _) : base(_) { }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterParameterEvents();
    }
    protected DeceptiveStepTrapProblem(DeceptiveStepTrapProblem original, Cloner cloner)
      : base(original, cloner) {
      offset = original.offset;
      StepSizeParameter = cloner.Clone(original.StepSizeParameter);

      RegisterParameterEvents();
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new DeceptiveStepTrapProblem(this, cloner);
    }

    private void RegisterParameterEvents() {
      StepSizeParameter.Value.ValueChanged += StepSizeOnChanged;
    }

    protected override void TrapSizeOnChanged(object sender, EventArgs e) {
      base.TrapSizeOnChanged(sender, e);
      offset = (TrapSize - StepSize) % StepSize;
    }

    private void StepSizeOnChanged(object sender, EventArgs e) {
      offset = (TrapSize - StepSize) % StepSize;
    }
  }
}

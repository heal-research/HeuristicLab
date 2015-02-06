#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
 * and the BEACON Center for the Study of Evolution in Action.
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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Algorithms.ParameterlessPopulationPyramid {
  // This code is based off the publication
  // B. W. Goldman and W. F. Punch, "Parameter-less Population Pyramid," GECCO, pp. 785–792, 2014
  // and the original source code in C++11 available from: https://github.com/brianwgoldman/Parameter-less_Population_Pyramid
  [Item("Deceptive Step Trap Problem", "Genome encodes completely separable blocks, where each block deceptive with fitness plateaus.")]
  [StorableClass]
  [Creatable("Parameterless Population Pyramid")]
  // In the GECCO paper, Section 4.1
  public class DeceptiveStepTrapProblem : DeceptiveTrapProblem {
    [StorableConstructor]
    protected DeceptiveStepTrapProblem(bool deserializing) : base(deserializing) { }
    protected DeceptiveStepTrapProblem(DeceptiveStepTrapProblem original, Cloner cloner)
      : base(original, cloner) {
      RegisterParameterEvents();
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new DeceptiveStepTrapProblem(this, cloner);
    }

    private const string StepSizeParameterName = "Step Size";

    public IFixedValueParameter<IntValue> StepSizeParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters[StepSizeParameterName]; }
    }

    public int StepSize {
      get { return StepSizeParameter.Value.Value; }
      set { StepSizeParameter.Value.Value = value; }
    }

    public DeceptiveStepTrapProblem()
      : base() {
      Parameters.Add(new FixedValueParameter<IntValue>(StepSizeParameterName, "", new IntValue(2)));
      RegisterParameterEvents();
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterParameterEvents();
    }

    private void RegisterParameterEvents() {
      TrapSizeParameter.Value.ValueChanged += (o, e) => { offset = -1; };
      StepSizeParameter.Value.ValueChanged += (o, e) => { offset = -1; };
    }


    private int offset = -1;
    private int Offset {
      get {
        if (offset == -1) offset = (TrapSize - StepSize) % StepSize;
        return offset;
      }
    }

    protected override int TrapMaximum {
      get { return (Offset + TrapSize) / StepSize; }
    }

    protected override int Score(bool[] individual, int trapIndex, int trapSize) {
      int partial = base.Score(individual, trapIndex, trapSize);
      // introduce plateaus using integer division
      return (Offset + partial) / StepSize;
    }
  }
}

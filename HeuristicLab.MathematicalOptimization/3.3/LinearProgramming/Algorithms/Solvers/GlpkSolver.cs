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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.MathematicalOptimization.LinearProgramming.Algorithms.Solvers.Base;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.MathematicalOptimization.LinearProgramming.Algorithms.Solvers {

  [Item("GLPK", "GLPK (https://www.gnu.org/software/glpk/) can be used out of the box.")]
  [StorableClass]
  public class GlpkSolver : ExternalIncrementalSolver {

    public GlpkSolver() {
      Parameters.Add(libraryNameParam = new FixedValueParameter<FileValue>(nameof(LibraryName),
        new FileValue { FileDialogFilter = FileDialogFilter, Value = Properties.Settings.Default.GlpkLibraryName }));

      programmingTypeParam.Value.ValueChanged += (sender, args) => {
        if (((EnumValue<LinearProgrammingType>)sender).Value == LinearProgrammingType.LinearProgramming) {
          incrementalityParam.Value = new BoolValue(true);
          incrementalityParam.Value.ValueChanged += (s, a) => {
            if (((BoolValue)s).Value) {
              qualityUpdateIntervalParam.Value = new TimeSpanValue(qualityUpdateIntervalParam.Value.Value);
            } else {
              qualityUpdateIntervalParam.Value = (TimeSpanValue)qualityUpdateIntervalParam.Value.AsReadOnly();
            }
          };
        } else {
          incrementalityParam.Value = (BoolValue)new BoolValue().AsReadOnly();
        }
      };
    }

    protected GlpkSolver(GlpkSolver original, Cloner cloner)
      : base(original, cloner) {
    }

    [StorableConstructor]
    protected GlpkSolver(bool deserializing)
      : base(deserializing) {
    }

    protected override OptimizationProblemType OptimizationProblemType =>
      LinearProgrammingType == LinearProgrammingType.LinearProgramming
        ? OptimizationProblemType.GLPK_LINEAR_PROGRAMMING
        : OptimizationProblemType.GLPK_MIXED_INTEGER_PROGRAMMING;
  }
}

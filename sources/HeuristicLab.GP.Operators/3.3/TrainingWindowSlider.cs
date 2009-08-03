#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using HeuristicLab.Core;
using HeuristicLab.Data;

namespace HeuristicLab.GP.Operators {
  public class TrainingWindowSlider : OperatorBase {

    private const string TRAINING_SAMPLES_START = "TrainingSamplesStart";
    private const string TRAINING_SAMPLES_END = "TrainingSamplesEnd";
    private const string TRAINING_WINDOW_START = "TrainingWindowStart";
    private const string TRAINING_WINDOW_END = "TrainingWindowEnd";
    private const string STEP_SIZE = "SlidingStepSize";

    public override string Description {
      get { return @"Modifies variables TrainingSamplesStart and TrainingSamplesEnd to have a continually sliding window over the whole training data set."; }
    }

    public TrainingWindowSlider() {
      AddVariableInfo(new VariableInfo(TRAINING_SAMPLES_START, "Start of whole training set", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo(TRAINING_SAMPLES_END, "End of whole training set", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo(TRAINING_WINDOW_START, "Start of training set window", typeof(IntData), VariableKind.In | VariableKind.Out));
      AddVariableInfo(new VariableInfo(TRAINING_WINDOW_END, "End of training set window", typeof(IntData), VariableKind.In | VariableKind.Out));
      AddVariableInfo(new VariableInfo(STEP_SIZE, "Number of samples to slide the window forward", typeof(IntData), VariableKind.In));
    }

    public override IOperation Apply(IScope scope) {
      int trainingSamplesStart = GetVariableValue<IntData>(TRAINING_SAMPLES_START, scope, true).Data;
      int trainingSamplesEnd = GetVariableValue<IntData>(TRAINING_SAMPLES_END, scope, true).Data;
      IntData trainingWindowStart = GetVariableValue<IntData>(TRAINING_WINDOW_START, scope, true);
      IntData trainingWindowEnd = GetVariableValue<IntData>(TRAINING_WINDOW_END, scope, true);
      int stepSize = GetVariableValue<IntData>(STEP_SIZE, scope, true).Data;

      if (trainingWindowEnd.Data + stepSize <= trainingSamplesEnd) {
        trainingWindowStart.Data = trainingWindowStart.Data + stepSize;
        trainingWindowEnd.Data = trainingWindowEnd.Data + stepSize;
      }

      return null;
    }
  }
}

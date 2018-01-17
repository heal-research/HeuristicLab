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

using System;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public class Powermeter : ResourceRegressionDataDescriptor {
    public override string Name { get { return "Powermeter"; } }
    public override string Description {
      get {
        return "This dataset contains measurements from a bike powermeter from three rides on different courses including flat and hilly terrain." +
               "The powermeter output should be estimated using velocity, ascent velocity, acceleration as well as heart rate and cadence data." +
               "Data from the first two rides can be used for training the model for power predictions for the third ride." +
               "AscentVelocity is calculated from velocity and measurements of a barometer. Measurements for heartrate, velocity, and AscentVelocity " +
               "have been smoothed using a quadratic Savitzky-Golay filter. All variables are measured at 1Hz.";
      }
    }
    protected override string TargetVariable { get { return "Power [W]"; } }
    protected override string[] VariableNames {
      get { return new string[] { "Date", "Power [W]", "HR [bpm]", "Cadence [bpm]", "Velocity [m/s]", "AscentVelocity [m/h]", "d(HR)/dt [bpm/s]", "Acceleration [m/s²]" }; }
    }
    protected override string[] AllowedInputVariables {
      get { return new string[] { "HR [bpm]", "Cadence [bpm]", "Velocity [m/s]", "AscentVelocity [m/h]", "d(HR)/dt [bpm/s]", "Acceleration [m/s²]" }; }
    }
    protected override int TrainingPartitionStart { get { return 0; } }
    protected override int TrainingPartitionEnd { get { return 13111; } }
    protected override int TestPartitionStart { get { return 13111; } }
    protected override int TestPartitionEnd { get { return 17039; } }
  }
}

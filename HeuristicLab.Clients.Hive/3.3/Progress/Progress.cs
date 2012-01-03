#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Clients.Hive {
  public class Progress : IProgress {
    private string status;
    public string Status {
      get {
        return this.status;
      }
      set {
        if (this.status != value) {
          this.status = value;
          OnStatusChanged();
        }
      }
    }

    private double progressValue;
    public double ProgressValue {
      get {
        return this.progressValue;
      }
      set {
        if (this.progressValue != value) {
          this.progressValue = value;
          OnProgressChanged();
        }
      }
    }

    public Progress() { }

    public Progress(string status) {
      this.Status = status;
    }

    public void Finish() {
      OnFinished();
    }

    #region Event Handler
    public event EventHandler Finished;
    private void OnFinished() {
      var handler = Finished;
      try {
        if (handler != null) handler(this, EventArgs.Empty);
      }
      catch (Exception) { }
    }

    public event EventHandler StatusChanged;
    private void OnStatusChanged() {
      var handler = StatusChanged;
      try {
        if (handler != null) handler(this, EventArgs.Empty);
      }
      catch (Exception) { }
    }

    public event EventHandler ProgressValueChanged;
    private void OnProgressChanged() {
      var handler = ProgressValueChanged;
      try {
        if (handler != null) handler(this, EventArgs.Empty);
      }
      catch (Exception) { }
    }
    #endregion
  }
}

﻿#region License Information
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace HeuristicLab.Hive.Client.Common {
  [Serializable]
  public class TestJob: JobBase {
    public override void Run() {
      for (int x = 0; x < 10; x++) {
        for (int y = 0; y < Int32.MaxValue; y++) ;
        if (abort == true) {
          Logging.getInstance().Info(this.ToString(), "Job Processing aborted");
          Debug.WriteLine("Job Abort Processing");
          break;
        }
        Logging.getInstance().Info(this.ToString(), "Iteration " + x + " done");
        Debug.WriteLine("Iteration " + x + " done");
      }      
      OnJobStopped();
    }
  }
}

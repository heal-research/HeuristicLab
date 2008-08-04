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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Core;
using System.Threading;

namespace HeuristicLab.Grid {
  internal class EngineRunner: MarshalByRefObject {
    private ManualResetEvent engineFinished = new ManualResetEvent(false);

    internal byte[] Execute(byte[] engine) {
      ProcessingEngine currentEngine = (ProcessingEngine)PersistenceManager.RestoreFromGZip(engine);
      engineFinished.Reset();
      currentEngine.Finished += new EventHandler(currentEngine_Finished);
      currentEngine.Execute();
      
      engineFinished.WaitOne();
            
      // if the engine was stopped because of an error (not suspended because of a breakpoint) 
      // it's not necessary to return the whole engine
      // instead just return an empty engine that has the aborted flag set
      if(currentEngine.Canceled && !currentEngine.Suspended) {
        currentEngine.Reset();
        currentEngine.OperatorGraph.Clear();
        currentEngine.Abort();
      }

      if(!currentEngine.Canceled && !currentEngine.Suspended) {
        currentEngine.OperatorGraph.Clear();
        currentEngine.GlobalScope.Clear();
      }

      return PersistenceManager.SaveToGZip(currentEngine);
    }

    void currentEngine_Finished(object sender, EventArgs e) {
      engineFinished.Set();
    }
  }
}

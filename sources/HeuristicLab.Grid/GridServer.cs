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
using System.Text;
using System.ServiceModel;

namespace HeuristicLab.Grid {
  [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext=false)]
  public class GridServer : IGridServer {

    private EngineStore engineStore;

    public GridServer(EngineStore engineStore) {
      this.engineStore = engineStore;
    }

    public Guid BeginExecuteEngine(byte[] engine) {
      Guid guid = Guid.NewGuid();
      engineStore.AddEngine(guid, engine);
      return guid;
    }

    public byte[] EndExecuteEngine(Guid guid) {
      return engineStore.GetResult(guid);
    }

    public void AbortEngine(Guid engine) {
      throw new NotImplementedException();
    }
  }
}

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
using HeuristicLab.Hive.Client.Common;

namespace HeuristicLab.Hive.Client.Core {
  public class Core {
    public void Start() {
      Logging.getInstance().Info(this.ToString(), "Info Message");
      Logging.getInstance().Error(this.ToString(), "Error Message");
      Logging.getInstance().Error(this.ToString(), "Exception Message", new Exception("Exception"));      

      Heartbeat beat = new Heartbeat();
      beat.Interval = 1000;
      beat.StartHeartbeat();
      ConfigurationManager.GetInstance();

      MessageQueue queue = MessageQueue.GetInstance();

      while (true) {
        MessageContainer container = queue.GetMessage();
        Console.WriteLine(container.Message.ToString());
      }
    }
  }
}

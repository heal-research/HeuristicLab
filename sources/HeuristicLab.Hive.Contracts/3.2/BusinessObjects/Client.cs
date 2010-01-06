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
using System.Runtime.Serialization;
                                   
namespace HeuristicLab.Hive.Contracts.BusinessObjects {

  public enum State { nullState, idle, calculating, offline, finished, abort, requestSnapshot, requestSnapshotSent, pending };

  [DataContract]
  [Serializable]
  public class ClientInfo : Resource {
    [DataMember]
    public int NrOfCores { get; set; }
    [DataMember]
    public int NrOfFreeCores { get; set; }
    [DataMember]
    public int CpuSpeedPerCore { get; set; }
    [DataMember]
    public int Memory { get; set; }
    [DataMember]
    public int FreeMemory { get; set; }
    [DataMember]
    public DateTime Login { get; set; }
    [DataMember]
    public State State { get; set; }
    [DataMember]
    public ClientConfig Config { get; set; }

    public override string ToString() {
      return base.ToString() + ", NrOfCores: " + NrOfCores + ", NrOfFreeCores " + NrOfFreeCores + ", Login: " + Login + ", State: " + State;
    }
  }
}

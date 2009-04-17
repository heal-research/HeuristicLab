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
using HeuristicLab.DataAccess;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Hive.Contracts.BusinessObjects {

  [DataContract]
  public class Job : PersistableObject {
    [DataMember]
    public State State { get; set; }
    [DataMember]
    public Guid UserId { get; set; }
    [DataMember]
    public ClientInfo Client { get; set; }
    [DataMember]
    public Job ParentJob { get; set; }
    [DataMember]
    public double Percentage { get; set; }
    [DataMember]
    public byte[] SerializedJob { get; set; }
    [DataMember]
    public DateTime DateCreated { get; set; }
    [DataMember]
    public DateTime DateCalculated { get; set; }
    [DataMember]
    public int Priority { get; set; }
    [DataMember]
    public int CoresNeeded { get; set; }
    [DataMember]
    public int MemoryNeeded { get; set; }
    [DataMember]
    public List<HivePluginInfo> PluginsNeeded { get; set; }

    public Job() {
      PluginsNeeded = new List<HivePluginInfo>();
    }
  }
}

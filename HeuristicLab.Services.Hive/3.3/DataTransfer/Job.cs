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
using System.Runtime.Serialization;

namespace HeuristicLab.Services.Hive.DataTransfer {
  [DataContract]
  [Serializable]
  public class Job : NamedHiveItem {
    [DataMember]
    public Guid OwnerUserId { get; set; }
    [DataMember]
    public DateTime DateCreated { get; set; }
    [DataMember]
    public Guid ProjectId { get; set; }
    [DataMember]
    public Permission Permission { get; set; } // the permission for the currently logged in user
    [DataMember]
    public string OwnerUsername { get; set; }
    [DataMember]
    public JobState State { get; set; }

    /* ==== some computed statistics ==== */
    [DataMember]
    public int JobCount { get; set; }
    [DataMember]
    public int FinishedCount { get; set; }
    [DataMember]
    public int CalculatingCount { get; set; }
    /* ================================== */

    public Job() { }

    public override string ToString() {
      return base.ToString() + "Name: " + Name + ", Description: " + Description;
    }
  }
}

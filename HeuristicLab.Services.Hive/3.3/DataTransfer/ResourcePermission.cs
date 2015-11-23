#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  public class ResourcePermission : HiveItem {
    // info: this class is derived from HiveItem to simplify handling on the client side, altough it does not have a Id.
    [DataMember]
    public Guid ResourceId { get; set; }
    [DataMember]
    public Guid GrantedUserId { get; set; }
    [DataMember]
    public Guid GrantedByUserId { get; set; }

    public ResourcePermission() { }
  }
}

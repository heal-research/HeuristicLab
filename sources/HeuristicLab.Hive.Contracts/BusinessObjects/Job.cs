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
using System.Runtime.Serialization;

namespace HeuristicLab.Hive.Contracts.BusinessObjects {

  [DataContract]
  public class Job: IHiveObject {
    [DataMember]
    public long Id { get; set; }
    [DataMember]
    public State State { get; set; }
    [DataMember]
    public User User { get; set; }
    [DataMember]
    public ClientInfo Client { get; set; }
    [DataMember]
    public Job ParentJob { get; set; }
    [DataMember]
    public double Percentage { get; set; }
    [DataMember]
    public byte[] SerializedJob { get; set; }

    public override bool Equals(object obj) {
      if (obj is Job) {
        return (obj as Job).Id.Equals(Id);
      } else      
        return base.Equals(obj);
    }

    public override int GetHashCode() {
      return Id.GetHashCode();
    }
  }
}

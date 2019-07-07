#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Runtime.Serialization;

namespace HeuristicLab.Services.OKB.Query.DataTransfer {
  [DataContract]
  public class Run {
    [DataMember]
    public long Id { get; set; }
    [DataMember]
    public Algorithm Algorithm { get; set; }
    [DataMember]
    public Problem Problem { get; set; }
    [DataMember]
    public DateTime CreatedDate { get; set; }
    [DataMember]
    public Guid UserId { get; set; }
    [DataMember]
    public Guid ClientId { get; set; }
    [DataMember]
    public IEnumerable<Value> ParameterValues { get; set; }
    [DataMember]
    public IEnumerable<Value> ResultValues { get; set; }
  }
}

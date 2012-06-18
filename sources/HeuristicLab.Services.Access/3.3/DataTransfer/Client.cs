#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Services.Access.DataTransfer {
  [DataContract]
  public class Client : Resource {
    [DataMember]
    public ClientConfiguration ClientConfiguration { get; set; }
    [DataMember]
    public ClientType ClientType { get; set; }
    [DataMember]
    public string HeuristicLabVersion { get; set; }
    [DataMember]
    public Country Country { get; set; }
    [DataMember]
    public HeuristicLab.Services.Access.DataTransfer.OperatingSystem OperatingSystem { get; set; }
    [DataMember]
    public int MemorySize { get; set; }
    [DataMember]
    public DateTime Timestamp { get; set; }
    [DataMember]
    public int NumberOfCores { get; set; }
    [DataMember]
    public string ProcessorType { get; set; }
    [DataMember]
    public double PerformanceValue { get; set; }
  }
}

#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using HEAL.Attic;
using HeuristicLab.Data;

namespace HeuristicLab.Problems.VehicleRouting.Interfaces {
  [StorableType("F4038025-9294-466D-A60D-45E7F98F9186")]
  public interface ISingleDepotProblemInstance : IVRPProblemInstance { }
  [StorableType("C6DA8FFB-DBA2-431B-A8C2-B685EB76D3F3")]
  public interface IMultiDepotProblemInstance : IVRPProblemInstance {
    IntValue Depots { get; }
    IntArray VehicleDepotAssignment { get; }
  }
  [StorableType("C172524A-C4B2-49DA-AC95-6619CB51544B")]
  public interface ICapacitatedProblemInstance : IVRPProblemInstance {
    DoubleValue OverloadPenalty { get; }
    DoubleValue CurrentOverloadPenalty { get; set; }
  }
  [StorableType("94A5B1EF-6C46-402B-B31A-76BB19745A52")]
  public interface IHeterogenousCapacitatedProblemInstance : ICapacitatedProblemInstance {
    DoubleArray Capacity { get; }
  }
  [StorableType("F789C488-BD06-40C2-A75C-C1FFE0361B77")]
  public interface IHomogenousCapacitatedProblemInstance : ICapacitatedProblemInstance {
    DoubleValue Capacity { get; }
  }
  [StorableType("5E6207F9-10E5-4428-BC6E-C583389F9D86")]
  public interface ITimeWindowedProblemInstance : IVRPProblemInstance {
    DoubleArray ReadyTime { get; }
    DoubleArray DueTime { get; }
    DoubleArray ServiceTime { get; }
    DoubleValue TimeFactor { get; }
    DoubleValue TardinessPenalty { get; }
    DoubleValue CurrentTardinessPenalty { get; set; }
  }
  [StorableType("A55869C3-00D4-481B-BBC8-201C510934AA")]
  public interface IPickupAndDeliveryProblemInstance : IVRPProblemInstance {
    IntArray PickupDeliveryLocation { get; }
    DoubleValue PickupViolationPenalty { get; }
    DoubleValue CurrentPickupViolationPenalty { get; set; }

    int GetPickupDeliveryLocation(int city);
  }
}

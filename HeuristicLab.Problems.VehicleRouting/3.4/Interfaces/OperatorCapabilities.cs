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

namespace HeuristicLab.Problems.VehicleRouting.Interfaces {
  [StorableType("08484417-9E4E-4104-AAFD-4197046A5CAC")]
  public interface IGeneralVRPOperator : IVRPOperator { }
  [StorableType("575729DA-15DA-4979-B8B5-F6878FE7FAE7")]
  public interface ISingleDepotOperator : IVRPOperator { }
  [StorableType("31032e91-4065-466c-81d2-59d121c768d2")]
  public interface INotSingleDepotOperator : IVRPOperator { }
  [StorableType("58F40290-ACB2-4E19-8064-2944EAD9BE0A")]
  public interface IMultiDepotOperator : IVRPOperator { }
  [StorableType("96703ffe-94a5-464b-9dea-57b05e361eb8")]
  public interface INotMultiDepotOperator : IVRPOperator { }
  [StorableType("3439CF1A-DDE5-4E67-A6B3-8B6A2C640637")]
  public interface ICapacitatedOperator : IVRPOperator { }
  [StorableType("0827648C-B4EF-450C-86BE-B145624FCC6C")]
  public interface IHomogenousCapacitatedOperator : ICapacitatedOperator { }
  [StorableType("D9F4AD37-6D17-4376-874C-5773BFDE9A77")]
  public interface IHeterogenousCapacitatedOperator : ICapacitatedOperator { }
  [StorableType("7d2e4513-fbd9-4ef4-8512-aceb6a4dafcf")]
  public interface INotCapacitatedOperaor : IVRPOperator { }
  [StorableType("43A35B3E-B3F4-4A16-9940-CFDA9E288F78")]
  public interface ITimeWindowedOperator : IVRPOperator { }
  [StorableType("0195366c-f965-475f-8fc0-3cd11ce57e1f")]
  public interface INotTimeWindowedOperator : IVRPOperator { }
  [StorableType("D2F69F8C-357D-4EC7-AC78-EE04324A27C6")]
  public interface IPickupAndDeliveryOperator : IVRPOperator { }
  [StorableType("c4e8f0a0-e5a2-41a1-b0c2-9932b5bbdd9a")]
  public interface INotPickupAndDeliveryOperator : IVRPOperator { }
}

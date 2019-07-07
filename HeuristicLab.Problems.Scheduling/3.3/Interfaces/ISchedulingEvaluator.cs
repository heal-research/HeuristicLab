﻿#region License Information
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

using HeuristicLab.Core;
using HeuristicLab.Encodings.ScheduleEncoding;
using HeuristicLab.Optimization;
using HEAL.Attic;

namespace HeuristicLab.Problems.Scheduling {
  [StorableType("9E6BC36E-D5A6-4C64-9512-391C382E3527")]
  /// <summary>
  /// An evaluator for a scheduling problem representation
  /// </summary>
  public interface ISchedulingEvaluator : ISingleObjectiveEvaluator {
    ILookupParameter<IScheduleDecoder> ScheduleDecoderParameter { get; }
    ILookupParameter<IScheduleEvaluator> ScheduleEvaluatorParameter { get; }
  }
}

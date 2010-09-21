#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using DA = HeuristicLab.Services.OKB.DataAccess;
using DT = HeuristicLab.Services.OKB.DataTransfer;

namespace HeuristicLab.Services.OKB {
  public static class Convert {
    #region Platform
    public static DT.Platform ToDto(DA.Platform source) {
      return new DT.Platform { Id = source.Id, Name = source.Name, Description = source.Description };
    }
    public static DA.Platform ToEntity(DT.Platform source) {
      return new DA.Platform { Id = source.Id, Name = source.Name, Description = source.Description };
    }
    public static void ToEntity(DT.Platform source, DA.Platform target) {
      target.Id = source.Id; target.Name = source.Name; target.Description = source.Description;
    }
    #endregion

    #region AlgorithmClass
    public static DT.AlgorithmClass ToDto(DA.AlgorithmClass source) {
      return new DT.AlgorithmClass { Id = source.Id, Name = source.Name, Description = source.Description };
    }
    public static DA.AlgorithmClass ToEntity(DT.AlgorithmClass source) {
      return new DA.AlgorithmClass { Id = source.Id, Name = source.Name, Description = source.Description };
    }
    public static void ToEntity(DT.AlgorithmClass source, DA.AlgorithmClass target) {
      target.Id = source.Id; target.Name = source.Name; target.Description = source.Description;
    }
    #endregion

    #region Algorithm
    public static DT.Algorithm ToDto(DA.Algorithm source) {
      return new DT.Algorithm { Id = source.Id, Name = source.Name, Description = source.Description, PlatformId = source.PlatformId, AlgorithmClassId = source.AlgorithmClassId };
    }
    public static DA.Algorithm ToEntity(DT.Algorithm source) {
      return new DA.Algorithm { Id = source.Id, Name = source.Name, Description = source.Description, PlatformId = source.PlatformId, AlgorithmClassId = source.AlgorithmClassId };
    }
    public static void ToEntity(DT.Algorithm source, DA.Algorithm target) {
      target.Id = source.Id; target.Name = source.Name; target.Description = source.Description; target.PlatformId = source.PlatformId; target.AlgorithmClassId = source.AlgorithmClassId;
    }
    #endregion
  }
}

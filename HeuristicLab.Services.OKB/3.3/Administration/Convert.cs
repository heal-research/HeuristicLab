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

using System.Linq;
using System.Security.Cryptography;
using DA = HeuristicLab.Services.OKB.DataAccess;
using DT = HeuristicLab.Services.OKB.Administration.DataTransfer;

namespace HeuristicLab.Services.OKB.Administration {
  public static class Convert {
    #region Platform
    public static DT.Platform ToDto(DA.Platform source) {
      if (source == null) return null;
      return new DT.Platform { Id = source.Id, Name = source.Name, Description = source.Description };
    }
    public static DA.Platform ToEntity(DT.Platform source) {
      if (source == null) return null;
      return new DA.Platform { Id = source.Id, Name = source.Name, Description = source.Description };
    }
    public static void ToEntity(DT.Platform source, DA.Platform target) {
      if ((source != null) && (target != null)) {
        target.Id = source.Id; target.Name = source.Name; target.Description = source.Description;
      }
    }
    #endregion

    #region AlgorithmClass
    public static DT.AlgorithmClass ToDto(DA.AlgorithmClass source) {
      if (source == null) return null;
      return new DT.AlgorithmClass { Id = source.Id, Name = source.Name, Description = source.Description };
    }
    public static DA.AlgorithmClass ToEntity(DT.AlgorithmClass source) {
      if (source == null) return null;
      return new DA.AlgorithmClass { Id = source.Id, Name = source.Name, Description = source.Description };
    }
    public static void ToEntity(DT.AlgorithmClass source, DA.AlgorithmClass target) {
      if ((source != null) && (target != null)) {
        target.Id = source.Id; target.Name = source.Name; target.Description = source.Description;
      }
    }
    #endregion

    #region Algorithm
    public static DT.Algorithm ToDto(DA.Algorithm source) {
      if (source == null) return null;
      return new DT.Algorithm { Id = source.Id, Name = source.Name, Description = source.Description, PlatformId = source.PlatformId, AlgorithmClassId = source.AlgorithmClassId, DataTypeName = source.DataType.Name, DataTypeTypeName = source.DataType.TypeName };
    }
    public static DA.Algorithm ToEntity(DT.Algorithm source, DA.OKBDataContext okb) {
      if (source == null) return null;
      return new DA.Algorithm { Id = source.Id, Name = source.Name, Description = source.Description, PlatformId = source.PlatformId, AlgorithmClassId = source.AlgorithmClassId, DataType = Convert.ToEntity(source.DataTypeName, source.DataTypeTypeName, okb) };
    }
    public static void ToEntity(DT.Algorithm source, DA.Algorithm target, DA.OKBDataContext okb) {
      if ((source != null) && (target != null)) {
        target.Id = source.Id; target.Name = source.Name; target.Description = source.Description; target.PlatformId = source.PlatformId; target.AlgorithmClassId = source.AlgorithmClassId; target.DataType = Convert.ToEntity(source.DataTypeName, source.DataTypeTypeName, okb);
      }
    }
    #endregion

    #region ProblemClass
    public static DT.ProblemClass ToDto(DA.ProblemClass source) {
      if (source == null) return null;
      return new DT.ProblemClass { Id = source.Id, Name = source.Name, Description = source.Description };
    }
    public static DA.ProblemClass ToEntity(DT.ProblemClass source) {
      if (source == null) return null;
      return new DA.ProblemClass { Id = source.Id, Name = source.Name, Description = source.Description };
    }
    public static void ToEntity(DT.ProblemClass source, DA.ProblemClass target) {
      if ((source != null) && (target != null)) {
        target.Id = source.Id; target.Name = source.Name; target.Description = source.Description;
      }
    }
    #endregion

    #region Problem
    public static DT.Problem ToDto(DA.Problem source) {
      if (source == null) return null;
      return new DT.Problem { Id = source.Id, Name = source.Name, Description = source.Description, PlatformId = source.PlatformId, ProblemClassId = source.ProblemClassId, DataTypeName = source.DataType.Name, DataTypeTypeName = source.DataType.TypeName };
    }
    public static DA.Problem ToEntity(DT.Problem source, DA.OKBDataContext okb) {
      if (source == null) return null;
      return new DA.Problem { Id = source.Id, Name = source.Name, Description = source.Description, PlatformId = source.PlatformId, ProblemClassId = source.ProblemClassId, DataType = Convert.ToEntity(source.DataTypeName, source.DataTypeTypeName, okb) };
    }
    public static void ToEntity(DT.Problem source, DA.Problem target, DA.OKBDataContext okb) {
      if ((source != null) && (target != null)) {
        target.Id = source.Id; target.Name = source.Name; target.Description = source.Description; target.PlatformId = source.PlatformId; target.ProblemClassId = source.ProblemClassId; target.DataType = Convert.ToEntity(source.DataTypeName, source.DataTypeTypeName, okb);
      }
    }
    #endregion

    #region DataType
    private static DA.DataType ToEntity(string dataTypeName, string dataTypeTypeName, DA.OKBDataContext okb) {
      if ((dataTypeName == null) || (dataTypeTypeName == null)) return null;
      var entity = okb.DataTypes.Where(x => (x.Name == dataTypeName) && (x.TypeName == dataTypeTypeName)).FirstOrDefault();
      if (entity == null)
        entity = new DA.DataType() { Id = 0, Name = dataTypeName, TypeName = dataTypeTypeName };
      return entity;
    }
    #endregion

    #region BinaryData
    internal static DA.BinaryData ToEntity(byte[] data, DA.OKBDataContext okb) {
      if (data == null) return null;
      byte[] hash;
      using (SHA1 sha1 = SHA1.Create()) {
        hash = sha1.ComputeHash(data);
      }
      var entity = okb.BinaryDatas.Where(x => x.Hash.Equals(hash)).FirstOrDefault();
      if (entity == null)
        entity = new DA.BinaryData() { Id = 0, Data = data, Hash = hash };
      return entity;
    }
    #endregion
  }
}

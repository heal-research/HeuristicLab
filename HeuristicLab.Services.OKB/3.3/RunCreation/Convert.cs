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
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using DA = HeuristicLab.Services.OKB.DataAccess;
using DT = HeuristicLab.Services.OKB.RunCreation.DataTransfer;

namespace HeuristicLab.Services.OKB.RunCreation {
  public static class Convert {
    #region ToDto
    public static DT.Algorithm ToDto(DA.Algorithm source) {
      if (source == null) return null;
      return new DT.Algorithm { Id = source.Id, Name = source.Name, Description = source.Description, AlgorithmClass = Convert.ToDto(source.AlgorithmClass), DataType = Convert.ToDto(source.DataType) };
    }

    public static DT.Problem ToDto(DA.Problem source) {
      if (source == null) return null;
      return new DT.Problem { Id = source.Id, Name = source.Name, Description = source.Description, ProblemClass = Convert.ToDto(source.ProblemClass), DataType = Convert.ToDto(source.DataType) };
    }

    private static DT.DataType ToDto(DA.DataType source) {
      if (source == null) return null;
      return new DT.DataType { Name = source.Name, TypeName = source.TypeName };
    }

    private static DT.AlgorithmClass ToDto(DA.AlgorithmClass source) {
      if (source == null) return null;
      return new DT.AlgorithmClass { Name = source.Name, Description = source.Description };
    }

    private static DT.ProblemClass ToDto(DA.ProblemClass source) {
      if (source == null) return null;
      return new DT.ProblemClass { Name = source.Name, Description = source.Description };
    }
    #endregion

    #region ToEntity
    public static DA.Run ToEntity(DT.Run source, DA.OKBDataContext okb) {
      if (source == null) return null;

      List<DA.BinaryData> binCache = new List<DA.BinaryData>();

      DA.Run entity = new DA.Run { Id = 0, AlgorithmId = source.AlgorithmId, ProblemId = source.ProblemId, CreatedDate = source.CreatedDate, UserId = source.UserId, ClientId = source.ClientId };
      foreach (var value in source.ParameterValues)
        entity.Values.Add(Convert.ToEntity(value, entity, DA.ValueNameCategory.Parameter, okb, binCache));
      foreach (var value in source.ResultValues)
        entity.Values.Add(Convert.ToEntity(value, entity, DA.ValueNameCategory.Result, okb, binCache));
      return entity;
    }

    private static DA.Value ToEntity(DT.Value source, DA.Run run, DA.ValueNameCategory category, DA.OKBDataContext okb, List<DA.BinaryData> binCache) {
      if (source == null) return null;
      var entity = new DA.Value();
      entity.Run = run;
      entity.DataType = Convert.ToEntity(source.DataType, okb);
      if (source is DT.BoolValue) {
        entity.ValueName = Convert.ToEntity(source.Name, category, DA.ValueNameType.Bool, okb);
        entity.BoolValue = ((DT.BoolValue)source).Value;
      } else if (source is DT.IntValue) {
        entity.ValueName = Convert.ToEntity(source.Name, category, DA.ValueNameType.Int, okb);
        entity.IntValue = ((DT.IntValue)source).Value;
      } else if (source is DT.TimeSpanValue) {
        entity.ValueName = Convert.ToEntity(source.Name, category, DA.ValueNameType.TimeSpan, okb);
        entity.LongValue = ((DT.TimeSpanValue)source).Value;
      } else if (source is DT.LongValue) {
        entity.ValueName = Convert.ToEntity(source.Name, category, DA.ValueNameType.Long, okb);
        entity.LongValue = ((DT.LongValue)source).Value;
      } else if (source is DT.FloatValue) {
        entity.ValueName = Convert.ToEntity(source.Name, category, DA.ValueNameType.Float, okb);
        entity.FloatValue = ((DT.FloatValue)source).Value;
      } else if (source is DT.DoubleValue) {
        entity.ValueName = Convert.ToEntity(source.Name, category, DA.ValueNameType.Double, okb);
        entity.DoubleValue = ((DT.DoubleValue)source).Value;
      } else if (source is DT.PercentValue) {
        entity.ValueName = Convert.ToEntity(source.Name, category, DA.ValueNameType.Percent, okb);
        entity.DoubleValue = ((DT.PercentValue)source).Value;
      } else if (source is DT.StringValue) {
        entity.ValueName = Convert.ToEntity(source.Name, category, DA.ValueNameType.String, okb);
        entity.StringValue = ((DT.StringValue)source).Value;
      } else if (source is DT.BinaryValue) {
        entity.ValueName = Convert.ToEntity(source.Name, category, DA.ValueNameType.Binary, okb);
        entity.BinaryData = Convert.ToEntity(((DT.BinaryValue)source).Value, okb, binCache);
      } else {
        throw new ArgumentException("Unknown value type.", "source");
      }
      return entity;
    }

    private static DA.DataType ToEntity(DT.DataType source, DA.OKBDataContext okb) {
      if (source == null) return null;
      var entity = okb.DataTypes.Where(x => (x.Name == source.Name) && (x.TypeName == source.TypeName)).FirstOrDefault();
      if (entity == null)
        entity = new DA.DataType() { Id = 0, Name = source.Name, TypeName = source.TypeName };
      return entity;
    }

    private static DA.ValueName ToEntity(string name, DA.ValueNameCategory category, DA.ValueNameType type, DA.OKBDataContext okb) {
      if (string.IsNullOrEmpty(name)) return null;
      var entity = okb.ValueNames.Where(x => (x.Name == name) && (x.Category == category) && (x.Type == type)).FirstOrDefault();
      if (entity == null)
        entity = new DA.ValueName() { Id = 0, Name = name, Category = category, Type = type };
      return entity;
    }

    private static DA.BinaryData ToEntity(byte[] data, DA.OKBDataContext okb, List<DA.BinaryData> binCache) {
      if (data == null) return null;
      byte[] hash;
      using (SHA1 sha1 = SHA1.Create()) {
        hash = sha1.ComputeHash(data);
      }

      var cachedBinaryData = binCache.Where(x => x.Hash.SequenceEqual(hash)).FirstOrDefault();
      if (cachedBinaryData != null)
        return cachedBinaryData;

      var entity = okb.BinaryDatas.Where(x => x.Hash.Equals(hash)).FirstOrDefault();
      if (entity == null) {
        entity = new DA.BinaryData() { Id = 0, Data = data, Hash = hash };
        binCache.Add(entity);
      }

      return entity;
    }
    #endregion
  }
}

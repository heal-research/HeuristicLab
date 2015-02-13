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
using System.Collections.Generic;
using System.Linq;
using DA = HeuristicLab.Services.OKB.DataAccess;
using DT = HeuristicLab.Services.OKB.Query.DataTransfer;

namespace HeuristicLab.Services.OKB.Query {
  public static class Convert {
    public static DT.Run ToDto(DA.Run source, bool includeBinaryValues) {
      if (source == null) return null;
      var dto = new DT.Run { Id = source.Id, Algorithm = Convert.ToDto(source.Algorithm), Problem = Convert.ToDto(source.Problem), CreatedDate = source.CreatedDate, ClientId = source.ClientId, UserId = source.UserId };
      dto.ParameterValues = source.Values.Where(x => x.ValueName.Category == DA.ValueNameCategory.Parameter).Select(x => Convert.ToDto(x, includeBinaryValues)).ToArray();
      dto.ResultValues = source.Values.Where(x => x.ValueName.Category == DA.ValueNameCategory.Result).Select(x => Convert.ToDto(x, includeBinaryValues)).ToArray();
      return dto;
    }

    public static DT.Run ToDto(DA.Run source, bool includeBinaryValues, IEnumerable<DT.ValueName> valueNames) {
      if (source == null) return null;
      var dto = new DT.Run { Id = source.Id, Algorithm = Convert.ToDto(source.Algorithm), Problem = Convert.ToDto(source.Problem), CreatedDate = source.CreatedDate, ClientId = source.ClientId, UserId = source.UserId };
      dto.ParameterValues = source.Values.Where(x => x.ValueName.Category == DA.ValueNameCategory.Parameter && valueNames.Count(y => y.Name == x.ValueName.Name) > 0).Select(x => Convert.ToDto(x, includeBinaryValues)).ToArray();
      dto.ResultValues = source.Values.Where(x => x.ValueName.Category == DA.ValueNameCategory.Result && valueNames.Count(y => y.Name == x.ValueName.Name) > 0).Select(x => Convert.ToDto(x, includeBinaryValues)).ToArray();
      return dto;
    }

    private static DT.Algorithm ToDto(DA.Algorithm source) {
      if (source == null) return null;
      return new DT.Algorithm { Name = source.Name, Description = source.Description, AlgorithmClass = source.AlgorithmClass.Name, Platform = source.Platform.Name, DataType = Convert.ToDto(source.DataType) };
    }

    private static DT.Problem ToDto(DA.Problem source) {
      if (source == null) return null;
      return new DT.Problem { Name = source.Name, Description = source.Description, ProblemClass = source.ProblemClass.Name, Platform = source.Platform.Name, DataType = Convert.ToDto(source.DataType) };
    }

    private static DT.DataType ToDto(DA.DataType source) {
      if (source == null) return null;
      return new DT.DataType { Name = source.Name, TypeName = source.TypeName };
    }

    private static DT.Value ToDto(DA.Value source, bool includeBinaryValues) {
      if (source == null) return null;
      if (source.ValueName.Type == DA.ValueNameType.Bool) {
        return new DT.BoolValue { Name = source.ValueName.Name, DataType = Convert.ToDto(source.DataType), Value = source.BoolValue.GetValueOrDefault() };
      } else if (source.ValueName.Type == DA.ValueNameType.Int) {
        return new DT.IntValue { Name = source.ValueName.Name, DataType = Convert.ToDto(source.DataType), Value = source.IntValue.GetValueOrDefault() };
      } else if (source.ValueName.Type == DA.ValueNameType.TimeSpan) {
        return new DT.TimeSpanValue { Name = source.ValueName.Name, DataType = Convert.ToDto(source.DataType), Value = source.LongValue.GetValueOrDefault() };
      } else if (source.ValueName.Type == DA.ValueNameType.Long) {
        return new DT.LongValue { Name = source.ValueName.Name, DataType = Convert.ToDto(source.DataType), Value = source.LongValue.GetValueOrDefault() };
      } else if (source.ValueName.Type == DA.ValueNameType.Float) {
        return new DT.FloatValue { Name = source.ValueName.Name, DataType = Convert.ToDto(source.DataType), Value = source.FloatValue.GetValueOrDefault() };
      } else if (source.ValueName.Type == DA.ValueNameType.Double) {
        return new DT.DoubleValue { Name = source.ValueName.Name, DataType = Convert.ToDto(source.DataType), Value = source.DoubleValue.GetValueOrDefault() };
      } else if (source.ValueName.Type == DA.ValueNameType.Percent) {
        return new DT.PercentValue { Name = source.ValueName.Name, DataType = Convert.ToDto(source.DataType), Value = source.DoubleValue.GetValueOrDefault() };
      } else if (source.ValueName.Type == DA.ValueNameType.String) {
        return new DT.StringValue { Name = source.ValueName.Name, DataType = Convert.ToDto(source.DataType), Value = source.StringValue };
      } else if (source.ValueName.Type == DA.ValueNameType.Binary) {
        return new DT.BinaryValue { Name = source.ValueName.Name, DataType = Convert.ToDto(source.DataType), Value = includeBinaryValues ? source.BinaryData.Data.ToArray() : null };
      } else {
        throw new ArgumentException("Unknown value type.", "source");
      }
    }

    public static DT.ValueName ToDto(DA.ValueName source) {
      if (source == null) return null;
      return new DT.ValueName() { Id = source.Id, Category = source.Category, Name = source.Name };
    }
  }
}

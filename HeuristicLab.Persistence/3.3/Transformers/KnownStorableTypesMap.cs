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

using System;
using System.Collections.Generic;
using HEAL.Attic;

namespace HeuristicLab.Persistence.Transformers {
  public class KnownStorableTypesMap : IStorableTypeMap {

    public IEnumerable<Tuple<Guid, Type>> KnownStorableTypes {
      get {
        return new[] {
          Tuple.Create(new Guid("ECC12A57-DA8D-43D9-9EC7-FCAC878A4D69"), typeof(System.Drawing.Font)),
          Tuple.Create(new Guid("494C1352-A1C3-47A3-8754-A0B19B8F1567"), typeof(System.Drawing.FontStyle)),
          Tuple.Create(new Guid("41147B67-4C7A-4907-9F6C-509568395EDB"), typeof(System.Drawing.GraphicsUnit)),
          Tuple.Create(new Guid("E8348C94-9817-4164-9C98-377689F83F30"), typeof(System.Drawing.Bitmap)),
          // Tuple.Create(new Guid("4AF3A1F6-49CD-4172-8394-FE464889250E"), Type.GetType("System.CultureAwareRandomizedComparer")),
          // Tuple.Create(new Guid("4C7F657C-B69E-4D1A-8812-3254D6219FD2"), Type.GetType("System.OrdinalRandomizedComparer")),
          // Tuple.Create(new Guid("0549A3DF-1FD4-487A-B06B-8C572DFFFBEB"), Type.GetType("System.Security.PermissionTokenKeyComparer")),
          // Tuple.Create(new Guid("65D2DE74-8BDF-4C74-9005-81A2C3991DC5"), Type.GetType("System.Collections.CompatibleComparer")),
          Tuple.Create(new Guid("3DA30D50-2337-4487-AECD-F2DB3ECED834"), Type.GetType("System.Collections.StructuralEqualityComparer")),
          // Tuple.Create(new Guid("8A19DA46-FE80-4776-9DB4-B17E6182D104"), Type.GetType("System.Collections.Generic.SByteEnumEqualityComparer`1")),
          // Tuple.Create(new Guid("F6F6EFB9-B631-4ACC-9326-F492A6A63011"), Type.GetType("System.Collections.Generic.ShortEnumEqualityComparer`1")),
          // Tuple.Create(new Guid("BA0AB604-C052-4E08-8F9E-A5D8098F16A6"), Type.GetType("System.Collections.Generic.RandomizedStringEqualityComparer")),
          // Tuple.Create(new Guid("00C8C940-63D9-43FF-99BA-9C69301BF043"), Type.GetType("System.Collections.Generic.RandomizedObjectEqualityComparer")),
        };
      }
    }
  }
}

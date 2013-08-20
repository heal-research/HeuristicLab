#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Persistence.Tests {

  [StorableClass]
  class DemoClass {

    [Storable(Name = "TestProperty", DefaultValue = 12)]
    public object o = null;

    [Storable]
    public int x = 2;

    public int y = 0;
  }

  [StorableClass]
  class Base {
    public string baseName;
    [Storable]
    public virtual string Name {
      get { return "Base"; }
      set { baseName = value; }
    }
  }

  [StorableClass]
  class Override : Base {
    [Storable]
    public override string Name {
      get { return "Override"; }
      set { base.Name = value; }
    }
  }

  [StorableClass]
  class Intermediate : Override {
  }

  [StorableClass]
  class New : Intermediate {
    public string newName;
    [Storable]
    public new string Name {
      get { return "New"; }
      set { newName = value; }
    }
  }
}

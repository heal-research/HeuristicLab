#region License Information
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

using HEAL.Fossil;

namespace HeuristicLab.Persistence.Tests {

  [StorableType("EF55A82D-7D9B-486E-B811-4DCC389FDB05")]
  class DemoClass {

    [Storable(Name = "TestProperty", DefaultValue = 12)]
    public object o = null;

    [Storable]
    public int x = 2;

    public int y = 0;

    [StorableConstructor]
    protected DemoClass(StorableConstructorFlag _) {
    }
    public DemoClass() {
    }
  }

  [StorableType("31F18F9A-C25D-449D-900A-FEBF95D7CE39")]
  class Base {
    public string baseName;
    [Storable]
    public virtual string Name {
      get { return "Base"; }
      set { baseName = value; }
    }

    [StorableConstructor]
    protected Base(StorableConstructorFlag _) {
    }
    public Base() {
    }
  }

  [StorableType("DF26C284-08C4-4703-A1A4-AFE834079B85")]
  class Override : Base {
    [Storable]
    public override string Name {
      get { return "Override"; }
      set { base.Name = value; }
    }

    [StorableConstructor]
    protected Override(StorableConstructorFlag _) : base(_) {
    }
    public Override() {
    }
  }

  [StorableType("6BFB1984-6670-4D5E-AEAC-3E77C627AF98")]
  class Intermediate : Override {
    [StorableConstructor]
    protected Intermediate(StorableConstructorFlag _) : base(_) {
    }
    public Intermediate() {
    }
  }

  [StorableType("856F9BDB-A20C-4B80-981B-5B4F5188FBCD")]
  class New : Intermediate {
    public string newName;
    [Storable]
    public new string Name {
      get { return "New"; }
      set { newName = value; }
    }

    [StorableConstructor]
    protected New(StorableConstructorFlag _) : base(_) {
    }
    public New() {
    }
  }
}

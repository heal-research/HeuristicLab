using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Text;

namespace HeuristicLab.Modeling.Database.SQLServerCompact {
  [Table(Name = "ModelData")]
  public class ModelData {

    public ModelData() {
      this.model = default(EntityRef<Model>);
    }

    public ModelData(Model model, byte[] data)
      : base() {
      this.modelId = model.Id;
      this.data = data;
    }


    private int modelId;
    [Column(Storage = "modelId", IsPrimaryKey = true)]
    public int ModelId {
      get { return this.modelId; }
      private set {
        if (modelId != value) {
          if (model.HasLoadedOrAssignedValue)
            throw new ForeignKeyReferenceAlreadyHasValueException();
          modelId = value;
        }
      }
    }

    private EntityRef<Model> model;
    [Association(Storage = "model", ThisKey = "ModelId", OtherKey = "Id", IsForeignKey = true)]
    public Model Model {
      get { return model.Entity; }
    }

    private byte[] data;
    [Column(Storage = "data", DbType = "image", CanBeNull = false)]
    public byte[] Data {
      get { return this.data; }
      private set { this.data = value; }
    }
  }
}

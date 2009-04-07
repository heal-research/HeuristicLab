using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HeuristicLab.DataAccess.ADOHelper {
  public class BinaryRelationHelper<AdapterT, RowT>
    where AdapterT : new()
    where RowT : System.Data.DataRow
    {
    IDataAdapterWrapper<AdapterT, BinaryRelation, RowT> tableAdapterWrapper;
    
    public BinaryRelationHelper(
      IDataAdapterWrapper<AdapterT, BinaryRelation, RowT> tableAdapterWrapper) {
      this.tableAdapterWrapper = tableAdapterWrapper;
    }

    public Session Session {
      set {
        tableAdapterWrapper.Session = value;
      }
    }

    public void UpdateRelationships(Guid objectA, IList<Guid> relationships) {
      //firstly check for created references
      IList<Guid> existing =
        this.GetRelationships(objectA);

      foreach (Guid relationship in relationships) {
        if (!existing.Contains(relationship)) {
          BinaryRelation rel = 
            new BinaryRelation();
          rel.Id = objectA;
          rel.Id2 = relationship;
          
          RowT inserted = 
            tableAdapterWrapper.InsertNewRow(rel);

          tableAdapterWrapper.UpdateRow(inserted);
        }
      }

      //secondly check for deleted references
      ICollection<Guid> deleted =
        new List<Guid>();

      foreach (Guid relationship in existing) {
        if(!relationships.Contains(relationship)) {
          deleted.Add(relationship);
        }
      }

      foreach (Guid relationship in deleted) {
        RowT toDelete =
          FindRow(objectA, relationship);
        if (toDelete != null) {
          toDelete.Delete();
          tableAdapterWrapper.UpdateRow(toDelete);
        }
      }
    }

    public IList<Guid> GetRelationships(Guid objectA) {
      IList<Guid> result =
        new List<Guid>();

      IEnumerable<RowT> rows = 
        tableAdapterWrapper.FindById(objectA);

      foreach(RowT row in rows) {
        result.Add((Guid)row[1]);
      }

      return result;
    }

    private RowT FindRow(Guid objectA, Guid objectB) {
      IEnumerable<RowT> rows =
       tableAdapterWrapper.FindById(objectA);

      RowT found = null;

      foreach (RowT row in rows) {
        if (row[1].Equals(objectB)) {
          found = row;
          break;
        }
      }

      return found;
    }
  }
}

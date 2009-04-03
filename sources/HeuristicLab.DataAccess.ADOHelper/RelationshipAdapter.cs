using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HeuristicLab.DataAccess.ADOHelper {
  public class RelationshipAdapter<AdapterT, RowT>
    where AdapterT : new()
    where RowT : System.Data.DataRow
    {
    IDataAdapterWrapper<AdapterT, Relationship, RowT> tableAdapterWrapper;
    
    public RelationshipAdapter(
      IDataAdapterWrapper<AdapterT, Relationship, RowT> tableAdapterWrapper) {
      this.tableAdapterWrapper = tableAdapterWrapper;
    }
    
    public void UpdateRelationships(Guid objectA, IList<Guid> relationships) {
      //first check for created references
      IList<Guid> existing =
        this.GetRelationships(objectA);

      foreach (Guid relationship in relationships) {
        if (!existing.Contains(relationship)) {
          Relationship rel = 
            new Relationship();
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

    public Session Session {
      set {
        tableAdapterWrapper.Session = value;
      }
    }
  }
}

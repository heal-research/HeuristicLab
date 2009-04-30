using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HeuristicLab.DataAccess.ADOHelper {
  public class ManyToManyRelationHelper<AdapterT, RowT>
    where AdapterT : new()
    where RowT : System.Data.DataRow
    {
    ITableAdapterWrapper<AdapterT, ManyToManyRelation, RowT> tableAdapterWrapper;
    
    public ManyToManyRelationHelper(
      ITableAdapterWrapper<AdapterT, ManyToManyRelation, RowT> tableAdapterWrapper) {
      this.tableAdapterWrapper = tableAdapterWrapper;
    }

    public Session Session {
      set {
        tableAdapterWrapper.Session = value;
      }
    }

    public void UpdateRelationships(Guid objectA,
      IList<Guid> relationships, int childIndex) {
      UpdateRelationships(objectA, relationships, null, childIndex);
    }

    public void UpdateRelationships(Guid objectA, 
      IList<Guid> relationships,
      IList<object> additionalAttributes, int childIndex) {
      //firstly check for created references
      IList<Guid> existing =
        this.GetRelationships(objectA, childIndex); 

      foreach (Guid relationship in relationships) {
        if (!existing.Contains(relationship)) {
          ManyToManyRelation rel = 
            new ManyToManyRelation();
          rel.Id = objectA;
          rel.Id2 = relationship;
          if(additionalAttributes != null)
            rel.AdditionalAttributes = 
              new List<object>(additionalAttributes);
          
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
          FindRow(objectA, relationship, childIndex);
        if (toDelete != null) {
          toDelete.Delete();
          tableAdapterWrapper.UpdateRow(toDelete);
        }
      }
    }

    public IList<Guid> GetRelationships(Guid objectA, int childIndex) {
      IList<Guid> result =
        new List<Guid>();

      IEnumerable<RowT> rows = 
        tableAdapterWrapper.FindById(objectA);

      foreach(RowT row in rows) {
        result.Add((Guid)row[childIndex]);
      }

      return result;
    }

    private RowT FindRow(Guid objectA, Guid objectB, int childIndex) {
      IEnumerable<RowT> rows =
       tableAdapterWrapper.FindById(objectA);

      RowT found = null;

      foreach (RowT row in rows) {
        if (row[childIndex].Equals(objectB)) {
          found = row;
          break;
        }
      }

      return found;
    }
  }
}

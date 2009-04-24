using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.DataAccess.ADOHelper;
using System.Data.SqlClient;
using HeuristicLab.Security.Contracts.BusinessObjects;
using System.Data.Common;

namespace HeuristicLab.Security.ADODataAccess.TableAdapterWrapper {
  class PermissionOwner_UserGroupAdapterWrapper :
   TableAdapterWrapperBase<
       dsSecurityTableAdapters.PermissionOwner_UserGroupTableAdapter,
       ManyToManyRelation,
       dsSecurity.PermissionOwner_UserGroupRow> {
    public override void UpdateRow(dsSecurity.PermissionOwner_UserGroupRow row) {
      TransactionalAdapter.Update(row);
    }

    public override dsSecurity.PermissionOwner_UserGroupRow
      InsertNewRow(ManyToManyRelation relation) {
      dsSecurity.PermissionOwner_UserGroupDataTable data =
        new dsSecurity.PermissionOwner_UserGroupDataTable();

      dsSecurity.PermissionOwner_UserGroupRow row = data.NewPermissionOwner_UserGroupRow();
      row.UserGroupId = relation.Id;
      row.PermissionOwnerId = relation.Id2;

      data.AddPermissionOwner_UserGroupRow(row);

      return row;
    }

    public override IEnumerable<dsSecurity.PermissionOwner_UserGroupRow>
      FindById(Guid id) {
      return TransactionalAdapter.GetDataByUserGroupId(id);
    }

    public override IEnumerable<dsSecurity.PermissionOwner_UserGroupRow>
      FindAll() {
      return TransactionalAdapter.GetData();
    }

    protected override void SetConnection(DbConnection connection) {
      adapter.Connection = connection as SqlConnection;
    }

    protected override void SetTransaction(DbTransaction transaction) {
      adapter.Transaction = transaction as SqlTransaction;
    }
  }
}

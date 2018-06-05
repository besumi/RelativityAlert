using kCura.Relativity.Client;
using Relativity.API;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using DTOs = kCura.Relativity.Client.DTOs;

namespace RelativityAlertPIEH
{
    public class Helper
    {
        public static int GetFieldArtifactID(string name, IRSAPIClient proxy, IAPILog logger)
        {
            int currentWorkspaceArtifactID = proxy.APIOptions.WorkspaceID;

            TextCondition cond1 = new TextCondition("Name", TextConditionEnum.EqualTo, name);
            WholeNumberCondition cond2 = new WholeNumberCondition("Object Type", NumericConditionEnum.EqualTo, 10);
            CompositeCondition compCondition = new CompositeCondition(cond1, CompositeConditionEnum.And, cond2);

            DTOs.Query<DTOs.Field> q = new DTOs.Query<DTOs.Field>
            {
                Condition = compCondition,
                Fields = DTOs.FieldValue.AllFields
            };

            DTOs.QueryResultSet<DTOs.Field> result = new DTOs.QueryResultSet<DTOs.Field>();

            try
            {
                result = proxy.Repositories.Field.Query(q);
            }
            catch (Exception ex)
            {
                logger.LogError("RSAPI exception on Read() in workspace {caseArtifactID} for field {fieldName}: {exception}",
                    currentWorkspaceArtifactID, name, ex.Message);
            }

            if (!result.Success)
            {
                logger.LogError("RSAPI Read() Unsuccessful in workspace {caseArtifactID} on field {fieldName}: {message}",
                    currentWorkspaceArtifactID, name, result.Message);
                logger.LogError("First RSAPI error in Results: {message}",
                    result.Results.FirstOrDefault().Message);
            }
            else
            {
                return result.Results.FirstOrDefault().Artifact.ArtifactID;
            }

            return 0;
        }

        public static bool FindFieldOnLayout(IDBContext context, int fieldArtifactID, int layoutArtifactID, IAPILog logger)
        {
            string sql =
            @"SELECT COUNT(1)
            FROM LayoutField
            WHERE FieldArtifactID = @fieldArtifactID
                AND LayoutArtifactID = @layoutArtifactID";

            SqlParameter fieldArtifact = new SqlParameter("@fieldArtifactID", SqlDbType.Int);
            SqlParameter layoutArtifact = new SqlParameter("@layoutArtifactID", SqlDbType.Int);
            fieldArtifact.Value = fieldArtifactID;
            layoutArtifact.Value = layoutArtifactID;

            logger.LogDebug("Querying for field {fieldArtifactID} on layout {layoutArtifactID}", fieldArtifactID, layoutArtifactID);
            var count = context.ExecuteSqlStatementAsScalar<int>(sql, new SqlParameter[] { fieldArtifact, layoutArtifact });
            logger.LogDebug("Count returned from LayoutField: {count}", count);

            if (count > 0)
            {
                return true;
            }

            return false;
        }
    }
}

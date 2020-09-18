using Relativity.API;
using Relativity.Services.Objects;
using Relativity.Services.Objects.DataContracts;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace RelativityAlertPIEH
{
    public class Helper
    {
		public async static Task<int> GetFieldArtifactID(string fieldName, IEHHelper helper, IAPILog logger)
		{
			int fieldArtifactId = 0;
			using (IObjectManager objectManager = helper.GetServicesManager().CreateProxy<IObjectManager>(ExecutionIdentity.System))
			{
				var queryRequest = new QueryRequest()
				{
					ObjectType = new ObjectTypeRef() { Name = "Field" },
					Condition = $"'Name' == '{fieldName}' AND 'Object Type' == 'Document'"
				};

				var queryResult = await objectManager.QuerySlimAsync(helper.GetActiveCaseID(), queryRequest, 0, 1);

				if (queryResult.TotalCount > 0)
				{
					fieldArtifactId = queryResult.Objects.Select(x => x.ArtifactID).FirstOrDefault();
					logger.LogVerbose("Alert field artifactID: {fieldArtifactID}", fieldArtifactId);
				}
			}

			return fieldArtifactId;
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

        public static string SanitizeAlertText(string rawText)
        {
            string sanitized = rawText;
            sanitized = sanitized.Replace(Environment.NewLine, "\\n");
            sanitized = sanitized.Replace("<script>alert('", "");
            sanitized = sanitized.Replace("<script> alert('", "");
            sanitized = sanitized.Replace("');</script>", "");
            sanitized = sanitized.Replace("')</script>", "");
            sanitized = sanitized.Replace("') </script>", "");

            return sanitized;
        }
    }
}

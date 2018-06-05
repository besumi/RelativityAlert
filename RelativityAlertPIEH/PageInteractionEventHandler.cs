using kCura.EventHandler;
using kCura.Relativity.Client;
using Relativity.API;
using System;

namespace RelativityAlertPIEH
{
    [kCura.EventHandler.CustomAttributes.Description("Relativity Alert - Page Interaction Event Handler")]
    [System.Runtime.InteropServices.Guid("F278FB0A-5462-401E-BD01-A3B88D48A905")]
    public class PageInteractionEventhandler : PageInteractionEventHandler
    {
        private string _fieldName = "Alert";

        public override Response PopulateScriptBlocks()
        {
            Response retVal = new Response()
            {
                Success = true,
                Message = string.Empty
            };

            int currentWorkspaceArtifactID = Helper.GetActiveCaseID();
            int currentLayoutArtifactID = this.ActiveLayout.ArtifactID;
            int currentDocumentArtifactID = this.ActiveArtifact.ArtifactID;

            IAPILog logger = Helper.GetLoggerFactory().GetLogger();

            int fieldArtifactID = 0;

            using (IRSAPIClient proxy =
                Helper.GetServicesManager().CreateProxy<IRSAPIClient>(ExecutionIdentity.System))
            {
                proxy.APIOptions.WorkspaceID = currentWorkspaceArtifactID;

                fieldArtifactID = RelativityAlertPIEH.Helper.GetFieldArtifactID(_fieldName, proxy, logger);
            }

            if (fieldArtifactID < 1)
            {
                logger.LogDebug("Alert field {fieldName} not found in workspace {caseArtifactID}",
                    _fieldName, currentWorkspaceArtifactID);
                return retVal;
            }

            IDBContext workspaceDbContext = this.Helper.GetDBContext(currentWorkspaceArtifactID);
            bool fieldExists = RelativityAlertPIEH.Helper.FindFieldOnLayout(workspaceDbContext, fieldArtifactID, currentLayoutArtifactID, logger);

            if (!fieldExists)
            {
                logger.LogVerbose($"Alert NOT fired because { _fieldName } field not present on workspace {{caseArtifactID}}, layout {{layoutArtifactID}}",
                    currentWorkspaceArtifactID, currentLayoutArtifactID);
                return retVal;
            }

            string alertFieldText = "";

            if (!this.ActiveArtifact.Fields[_fieldName].Value.IsNull)
            {
                alertFieldText = this.ActiveArtifact.Fields[_fieldName].Value.Value.ToString();
                alertFieldText = alertFieldText.Replace(Environment.NewLine, "\\n");
            }

            if (!String.IsNullOrEmpty(alertFieldText))
            {
                string alert = $"<script type=\"text/javascript\"> alert(\"{ alertFieldText }\"); </script>";
                this.RegisterClientScriptBlock(new ScriptBlock() { Key = "alertKey", Script = alert });
                logger.LogDebug("Alert fired on workspace {caseArtifactID}, layout {layoutArtifactID}, and document {documentArtifactID}",
                    currentWorkspaceArtifactID, currentLayoutArtifactID, currentDocumentArtifactID);
            }
            else
            {
                logger.LogVerbose("Alert NOT fired on workspace {caseArtifactID}, layout {layoutArtifactID}, and document {documentArtifactID}",
                    currentWorkspaceArtifactID, currentLayoutArtifactID, currentDocumentArtifactID);
            }

            logger.LogVerbose("Alert text detected: {alertFieldText}",
                alertFieldText);

            return retVal;
        }
        public override FieldCollection RequiredFields
        {
            get
            {
                FieldCollection retVal = new FieldCollection();
                retVal.Add(new kCura.EventHandler.Field(_fieldName));
                return retVal;
            }
        }
    }
}
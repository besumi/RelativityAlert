# Relativity Alert

This is a custom application/[event handler](https://platform.relativity.com/9.6/Content/Customizing_workflows/Page_Interaction_event_handlers.htm) for [Relativity](https://www.relativity.com).

It displays a Javascript alert to the end user if the following criteria are met:

- The workspace contains a field called Alert on the Document object
- The Alert field is populated with text on the specific document
- The Alert field is present on the active layout

## Contents

* /RAP/
	* Contains a RAP file for direct import in to Relativity along with instructions.
* /RelativityAlertPIEH/
	* Contains the C# source code for the application's Page Interaction Event Handler.
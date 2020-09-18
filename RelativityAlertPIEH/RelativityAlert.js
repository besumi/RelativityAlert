(function (eventNames, convenienceApi) {
	var eventHandlers = {};
	eventHandlers[eventNames.PAGE_LOAD_COMPLETE] = function () {
		convenienceApi.fieldHelper.getValue("Alert").then(function (value) {
			if (value != null) {
				console.log("Relativity Alert: alert field has value:" + value);
				value = value.replace("<script>alert('", "")
					.replace("<script>alert(\"", "")
					.replace("<script> alert('", "")
					.replace("<script> alert(\"", "")
					.replace("<script type=\"text/javascript\">alert('", "")
					.replace("<script type=\"text/javascript\">alert(\"", "")
					.replace("<script type=\"text/javascript\"> alert('", "")
					.replace("<script type=\"text/javascript\"> alert(\"", "")
					.replace("');</script>", "")
					.replace("\");</script>", "")
					.replace("'); </script>", "")
					.replace("\"); </script>", "");
				console.log("Relativity Alert: field value after sanitization:" + value);
				alert(value);
			};
		});
	}

	return eventHandlers
}(eventNames, convenienceApi));
function getColor(severity) {
	return ["Green", "Yellow", "Red"][severity];
}

function getStatus(startDate, completeDate, percentComplete) {
	var now = new Date();
	if(percentComplete === 100) {
		return "Complete";
	} else if(now < startDate) {
		return "Pending";
	} else if(now < completeDate && percentComplete) {
		return "In progress";
	} else {
		return "Late";
	}
}

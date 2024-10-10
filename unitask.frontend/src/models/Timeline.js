class Timeline {
    constructor(timelineId, timelineName, description, startDate, endDate) {
        this.timelineId = timelineId;
        this.timelineName = timelineName;
        this.description = description;
        this.startDate = new Date(startDate);
        this.endDate = new Date(endDate);
    }
}

export default Timeline;
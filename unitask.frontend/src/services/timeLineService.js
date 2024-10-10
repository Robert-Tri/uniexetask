import axios from "axios";
import Timeline from "../models/Timeline";

export const getTimelines = () => {
    return axios.get('https://localhost:7289/api/timeline')
        .then(response => {
            console.log('Timeline data:', response);
            if (response.data.success) {
                return response.data.data.map(timeline => new Timeline(
                    timeline.timelineId,
                    timeline.timelineName,
                    timeline.description,
                    new Date(timeline.startDate),
                    new Date(timeline.endDate)
                ));
            }
            return [];
        })
        .catch(error => {
            console.error('Error fetching timeline data:', error);
            return [];
        });
};

export async function insertTimeline(data) {
    try {
        const response = await axios.post(`https://localhost:7289/api/timeline`, data);

        if (response.data.success) {
            return response.data.data; 
        } else {
            console.error("Error inserting timeline:", response.data.errorMessage);
            return false; 
        }
    } catch (error) {
        console.error("Error inserting timeline:", error);
        throw error; 
    }
}

export async function updateTimeline(data) {
    try {
        const { timelineId, ...rest } = data;
        const timelineData = {
            timelineId: timelineId,
            ...rest,
        };
        const response = await axios.put(`https://localhost:7289/api/timeline`, timelineData);

        if (response.data.success) {
            return response.data.data; 
        } else {
            console.error("Error updating timeline:", response.data.errorMessage);
            return false; 
        }
    } catch (error) {
        console.error("Error updating timeline:", error);
        throw error; 
    }
}

export function deleteTimeline(id) {
    return axios.delete(`https://localhost:7289/api/timeline/${id}`)
        .then(response => {
            console.log('Timeline deleted:', response);
            return response.data; 
        })
        .catch(error => {
            console.error('Error deleting timeline:', error);
            throw error; 
        });
}

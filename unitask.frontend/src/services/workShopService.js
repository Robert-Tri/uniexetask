import axios from "axios";
import Workshop from "../models/WorkShop";

export const getWorkShops = () => {
    return axios.get('https://localhost:7289/api/workshop')
        .then(response => {
            console.log('Workshop data:', response);
            if (response.data.success) {
                return response.data.data.map(workshop => new Workshop(
                    workshop.workshopId,
                    workshop.name,
                    workshop.description,
                    new Date(workshop.startDate),
                    new Date(workshop.endDate),
                    workshop.location,
                    workshop.regUrl,
                    workshop.status
                ));
            }
            return [];
        })
        .catch(error => {
            console.error('Error fetching workshop data:', error);
            return [];
        });
};

export async function insertWorkshop(data) {
    try {
        const response = await axios.post(`https://localhost:7289/api/workshop`, data);

        if (response.data.success) {
            return response.data.data; 
        } else {
            console.error("Error inserting workshop:", response.data.errorMessage);
            return false; 
        }
    } catch (error) {
        console.error("Error inserting workshop:", error);
        throw error; 
    }
}


export async function updateWorkshop(data) {
    try {
        const { workshop_id, ...rest } = data;
        const workshopData = {
            workshopId: workshop_id,
            ...rest,
        };
        const response = await axios.put(`https://localhost:7289/api/workshop`, workshopData);

        if (response.data.success) {
            return response.data.data; 
        } else {
            console.error("Error updating workshop:", response.data.errorMessage);
            return false; 
        }
    } catch (error) {
        console.error("Error updating workshop:", error);
        throw error; 
    }
}

export function deleteWorkshop(id) {
    return axios.delete(`https://localhost:7289/api/workshop/${id}`)
        .then(response => {
            console.log('Workshop deleted:', response);
            return response.data; 
        })
        .catch(error => {
            console.error('Error deleting workshop:', error);
            throw error; 
        });
}

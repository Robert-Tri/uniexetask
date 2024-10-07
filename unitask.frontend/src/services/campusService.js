import axios from "axios";
import Campus from "../models/Campus"

export const getAllCampuses = () => {
    return axios.get('https://localhost:7289/api/campus')
        .then(response => {
            console.log('Campus data:', response);
            if (response.data.success) {
                return response.data.data.map(campus => new Campus(
                    campus.campusId, 
                    campus.campusCode, 
                    campus.campusName, 
                    campus.location
                ));
            }
            return []; 
        })
        .catch(error => {
            console.error('Error fetching campus data:', error);
            return [];
        });
};

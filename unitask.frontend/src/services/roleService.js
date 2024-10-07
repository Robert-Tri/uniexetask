import axios from "axios";
import Role from "../models/Role"; 
export const getAllRoles = () => {
    return axios.get('https://localhost:7289/api/role') 
        .then(response => {
            console.log('Role data:', response);
            if (response.data.success) {
                return response.data.data.map(role => new Role(
                    role.roleId,
                    role.name,
                    role.description,
                ));
            }
            return []; 
        })
        .catch(error => {
            console.error('Error fetching role data:', error);
            return [];
        });
};

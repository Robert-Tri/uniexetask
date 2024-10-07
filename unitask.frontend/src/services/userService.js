import axios from "axios";

const KEYS = {
    users: 'users',
    userId: 'userId'
}


export function insertUser(data) {
    let users = getAllUsers();
    data['id'] = generateUserId();
    users.push(data);
    localStorage.setItem(KEYS.users, JSON.stringify(users));
}

export async function updateUser(data) {
    try {
        const { campusName, roleName, user_id, ...rest } = data;
            const userData = {
                userId: user_id,
                ...rest,
            };
        const response = await axios.put(`https://localhost:7289/api/user`, userData);

        if (response.data.success) {
            return response.data.data; 
        } else {
            console.error("Error updating user:", response.data.errorMessage);
            return false; 
        }
    } catch (error) {
        console.error("Error updating user:", error);
        throw error; 
    }
}

export function deleteUser(id) {
    return axios.delete(`https://localhost:7289/api/user/${id}`)
        .then(response => {
            console.log('User deleted:', response);
            return response.data; 
        })
        .catch(error => {
            console.error('Error deleting user:', error);
            throw error; 
        });
}


export function generateUserId() {
    if (localStorage.getItem(KEYS.userId) == null)
        localStorage.setItem(KEYS.userId, '0');
    var id = parseInt(localStorage.getItem(KEYS.userId));
    localStorage.setItem(KEYS.userId, (++id).toString());
    return id;
}

export function getAllUsers() {
    return axios.get('https://localhost:7289/api/user')
        .then(response => {
            console.log('API Response:', response);
            if (response.data.success) {
                const formattedData = response.data.data.map(user => ({
                    user_id: user.userId,
                    fullName: user.fullName,
                    email: user.email,
                    phone: user.phone,
                    status: user.status,
                    campusName: user.campus.campusName || 'Unknown',
                    roleName: user.role.name,
                }));
                return formattedData; // Return the formatted user data
            }
        })
        .catch(error => {
            console.error('Error fetching data from API:', error);
            return []; // Return an empty array in case of an error
        });
}

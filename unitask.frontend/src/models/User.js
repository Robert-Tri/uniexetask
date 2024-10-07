class User {
    constructor(user_id, fullName, email, phone, campusId, status, roleId) {
      this.user_id = user_id;
      this.fullName = fullName;
      this.email = email;
      this.phone = phone;
      this.campusId = campusId;
      this.status = status;
      this.roleId = roleId;
    }
}

export default User;
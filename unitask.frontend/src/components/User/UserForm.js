import React, { useState, useEffect } from 'react'
import { Grid } from '@material-ui/core';
import Controls from "../../components/controls/Controls";
import { useForm, Form } from '../../components/useForm';
import * as userService from "../../services/userService"; // Giữ nguyên
import * as campusService from "../../services/campusService";
import * as roleService from "../../services/roleService";




const initialFValues = {
    user_id: 0,
    fullName: '',
    email: '',
    phone: '', 
    campusId: '', 
    status: '', 
    roleId: '', 
}

export default function UserForm(props) { 
    const { addOrEdit, recordForEdit } = props
    const [campusItems, setCampusItems] = useState([]);
    const [roleItems, setRoleItems] = useState([]);

    const validate = (fieldValues = values) => {
        let temp = { ...errors }
        if ('fullName' in fieldValues)
            temp.fullName = fieldValues.fullName ? "" : "This field is required."
        if ('email' in fieldValues)
            temp.email = (/$^|.+@.+..+/).test(fieldValues.email) ? "" : "Email is not valid."
        if ('phone' in fieldValues)
            temp.phone = fieldValues.phone.length > 9 ? "" : "Minimum 10 numbers required." 
        if ('campusId' in fieldValues)
            temp.campusId = fieldValues.campusId.length !== 0 ? "" : "This field is required." 
        if ('roleId' in fieldValues)
            temp.roleId = fieldValues.roleId.length !== 0 ? "" : "This field is required." 
        setErrors({
            ...temp
        })

        if (fieldValues === values)
            return Object.values(temp).every(x => x === "")
    }

    const {
        values,
        setValues,
        errors,
        setErrors,
        handleInputChange,
        resetForm
    } = useForm(initialFValues, true, validate);
    

    const handleSubmit = e => {
        e.preventDefault()
        if (validate()) {
            addOrEdit(values, resetForm);
        }
    }

    useEffect(() => {
        if (recordForEdit != null)
            setValues({
                ...recordForEdit
            })
    }, [recordForEdit])

    useEffect(() => {
        const fetchCampuses = async () => {
            const campuses = await campusService.getAllCampuses();
            console.log('Fetched campuses:', campuses); 
            const formattedCampuses = campuses.map(campus => ({
                id: campus.campusId,
                title: campus.campusName
            }));
            console.log('Formatted campuses:', formattedCampuses);
            setCampusItems(formattedCampuses); 
        };
        fetchCampuses();
    }, []);

    useEffect(() => {
        const fetchRoles = async () => { 
            const roles = await roleService.getAllRoles(); 
            console.log('Fetched roles:', roles);
            const formattedRoles = roles.map(role => ({
                id: role.roleId,
                title: role.name 
            }));
            console.log('Formatted roles:', formattedRoles);
            setRoleItems(formattedRoles);
        };
        fetchRoles();
    }, []);

    return (
        <Form onSubmit={handleSubmit}>
            <Grid container>
                <Grid item xs={6}>
                    <Controls.Input
                        name="fullName"
                        label="Full Name"
                        value={values.fullName}
                        onChange={handleInputChange}
                        error={errors.fullName}
                    />
                    <Controls.Input
                        label="Email"
                        name="email"
                        value={values.email}
                        onChange={handleInputChange}
                        error={errors.email}
                    />
                    <Controls.Input
                        label="Phone" 
                        name="phone"
                        value={values.phone}
                        onChange={handleInputChange}
                        error={errors.phone}
                    />
                </Grid>
                <Grid item xs={6}>
                    <Controls.Select
                        name="roleId" 
                        label="Role"
                        value={values.roleId}
                        onChange={handleInputChange}
                        options={roleItems} 
                        error={errors.roleId}
                    />
                    <Controls.Select
                        name="campusId"
                        label="Campus"
                        value={values.campusId}
                        onChange={handleInputChange}
                        options={campusItems} 
                        error={errors.campusId}
                    />
                    <Controls.Checkbox
                        name="status"
                        label="Active User" 
                        value={values.status}
                        onChange={handleInputChange}
                    />
                    <div>
                        <Controls.Button
                            type="submit"
                            text="Submit" />
                        <Controls.Button
                            text="Reset"
                            color="default"
                            onClick={resetForm} />
                    </div>
                </Grid>
            </Grid>
        </Form>
    )
}

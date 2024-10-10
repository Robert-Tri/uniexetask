import React, { useState, useEffect } from 'react';
import { Grid } from '@material-ui/core';
import Controls from "../../components/controls/Controls";
import { useForm, Form } from '../../components/useForm';

const initialFValues = {
    timelineId: 0,
    timelineName: '',
    startDate: new Date(),
    endDate: new Date(),
    startTime: new Date(),
    endTime: new Date(),
    description: ''
};

export default function TimelineForm(props) {
    const { addOrEdit, recordForEdit } = props;

    const validate = (fieldValues = values) => {
        let temp = { ...errors }
        if ('timelineName' in fieldValues)
            temp.timelineName = fieldValues.timelineName ? "" : "This field is required.";
        if ('startDate' in fieldValues)
            temp.startDate = fieldValues.startDate ? "" : "This field is required.";
        if ('endDate' in fieldValues)
            temp.endDate = fieldValues.endDate ? "" : "This field is required.";
        if ('startTime' in fieldValues)
            temp.startTime = fieldValues.startTime ? "" : "This field is required.";
        if ('endTime' in fieldValues)
            temp.endTime = fieldValues.endTime ? "" : "This field is required.";
        if ('status' in fieldValues)
            temp.status = fieldValues.status ? "" : "This field is required.";

        if (fieldValues.endDate && fieldValues.startDate) {
            temp.endDate = new Date(fieldValues.endDate) < new Date(fieldValues.startDate) ? "End date cannot be before start date." : "";
        }
        if (fieldValues.endTime && fieldValues.startTime) {
            temp.endTime = new Date(fieldValues.endTime) < new Date(fieldValues.startTime) ? "End time cannot be before start time." : "";
        }

        setErrors({
            ...temp
        });

        if (fieldValues === values)
            return Object.values(temp).every(x => x === "");
    };

    const {
        values,
        setValues,
        errors,
        setErrors,
        handleInputChange,
        resetForm
    } = useForm(initialFValues, true, validate);

    const handleSubmit = e => {
        e.preventDefault();
        if (validate()) {
            addOrEdit(values, resetForm);
        }
    };

    useEffect(() => {
        if (recordForEdit != null)
            setValues({
                ...recordForEdit
            });
    }, [recordForEdit]);

    return (
        <Form onSubmit={handleSubmit}>
            <Grid container>
                <Grid item xs={6}>
                    <Controls.Input
                        name="timelineName"
                        label="Timeline Name"
                        value={values.timelineName}
                        onChange={handleInputChange}
                        error={errors.timelineName}
                    />
                    <Controls.Input
                        name="description"
                        label="Description"
                        value={values.description}
                        onChange={handleInputChange}
                        error={errors.description}
                    />
                    <Controls.DatePicker
                        name="startDate"
                        label="Start Date"
                        value={values.startDate}
                        onChange={handleInputChange}
                        error={errors.startDate}
                    />
                    <Controls.DatePicker
                        name="endDate"
                        label="End Date"
                        value={values.endDate}
                        onChange={handleInputChange}
                        error={errors.endDate}
                    />
                </Grid>
                <Grid item xs={6}>
                    <Controls.TimePicker
                        name="startTime"
                        label="Start Time"
                        value={values.startTime}
                        onChange={handleInputChange}
                        error={errors.startTime}
                    />
                    <Controls.TimePicker
                        name="endTime"
                        label="End Time"
                        value={values.endTime}
                        onChange={handleInputChange}
                        error={errors.endTime}
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
    );
}

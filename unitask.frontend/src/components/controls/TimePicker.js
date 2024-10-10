import React from 'react';
import { MuiPickersUtilsProvider, KeyboardTimePicker } from '@material-ui/pickers';
import DateFnsUtils from '@date-io/date-fns';

export default function TimePicker(props) {
    const { name, label, value, onChange } = props;

    const convertToDefEventPara = (name, value) => ({
        target: {
            name, value
        }
    });

    return (
        <MuiPickersUtilsProvider utils={DateFnsUtils}>
            <KeyboardTimePicker
                variant="inline"
                inputVariant="outlined"
                label={label}
                name={name}
                value={value}
                onChange={time => onChange(convertToDefEventPara(name, time))}
                ampm={false} // Nếu bạn muốn sử dụng định dạng 24 giờ
                // format="HH:mm" // Chỉ định định dạng hiển thị
                KeyboardButtonProps={{
                    'aria-label': 'change time',
                }}
            />
        </MuiPickersUtilsProvider>
    );
}

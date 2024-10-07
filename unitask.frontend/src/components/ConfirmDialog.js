import React from 'react';
import { Dialog, DialogActions, DialogContent, DialogTitle, Typography } from '@material-ui/core';
import Controls from "./controls/Controls";

export default function ConfirmDialog(props) {
    const { confirmDialog, setConfirmDialog } = props;

    return (
        <Dialog open={confirmDialog.isOpen}>
            <DialogTitle>
                {confirmDialog.title}
            </DialogTitle>
            <DialogContent>
                <Typography>
                    {confirmDialog.subTitle}
                </Typography>
            </DialogContent>
            <DialogActions>
                <Controls.Button
                    text="No"
                    color="default"
                    onClick={() => setConfirmDialog({ ...confirmDialog, isOpen: false })}
                />
                <Controls.Button
                    text="Yes"
                    color="secondary"
                    onClick={confirmDialog.onConfirm}
                />
            </DialogActions>
        </Dialog>
    );
}

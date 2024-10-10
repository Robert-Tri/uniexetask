import React, { useState, useEffect } from 'react';
import TimelineForm from './TimelineForm';
import PageHeader from "../../components/PageHeader";
import EventAvailableTwoToneIcon from '@material-ui/icons/EventAvailableTwoTone';
import { Paper, makeStyles, TableBody, TableRow, TableCell, Toolbar, InputAdornment } from '@material-ui/core';
import useTable from "../../components/useTable";
import * as timelineService from "../../services/timeLineService";
import Controls from '../../components/controls/Controls';
import { Search, Add as AddIcon, EditOutlined, Delete } from '@material-ui/icons';
import Popup from '../../components/Popup';
import ConfirmDialog from '../../components/ConfirmDialog';
import { toast } from 'react-toastify';
import ActionButton from '../../components/controls/ActionButton';

const useStyles = makeStyles(theme => ({
    pageContent: { margin: theme.spacing(5), padding: theme.spacing(3) },
    searchInput: { width: '75%' },
    newButton: { position: 'absolute', right: '10px' }
}));

const headCells = [
    { id: 'timelineId', label: 'Timeline ID' },
    { id: 'timelineName', label: 'Timeline Name' },
    { id: 'description', label: 'Description' },
    { id: 'startDate', label: 'Start Date' },
    { id: 'endDate', label: 'End Date' },
    { id: 'actions', label: 'Actions', disableSorting: true }
];

export default function Timelines() {
    const classes = useStyles();
    const [records, setRecords] = useState([]);
    const [filterFn, setFilterFn] = useState({ fn: items => items });
    const [openPopup, setOpenPopup] = useState(false);
    const [confirmDialog, setConfirmDialog] = useState({ isOpen: false, title: '', subTitle: '' });
    const [recordForEdit, setRecordForEdit] = useState(null);

    useEffect(() => {
        const fetchData = async () => {
            const timelines = await timelineService.getTimelines();
            setRecords(timelines);
        };
        fetchData();
    }, []);

    const handleDelete = timelineId => {
        setConfirmDialog({
            isOpen: true,
            title: 'Are you sure you want to delete this timeline?',
            subTitle: "You can't undo this operation",
            onConfirm: () => onDeleteConfirm(timelineId)
        });
    };

    const onDeleteConfirm = timelineId => {
        timelineService.deleteTimeline(timelineId).then(() => {
            setRecords(records.filter(item => item.timelineId !== timelineId));
            toast.success('Delete timeline successfully!', {
                position: "top-right",
                autoClose: 5000,
                hideProgressBar: false,
                closeOnClick: true,
                pauseOnHover: true,
                draggable: true,
                progress: undefined,
                theme: "colored",
            });
        });
        setConfirmDialog({ ...confirmDialog, isOpen: false });
    };

    const { TblContainer, TblHead, TblPagination, recordsAfterPagingAndSorting } = useTable(records, headCells, filterFn);

    const handleSearch = e => {
        const value = e.target.value.toLowerCase();
        setFilterFn({
            fn: items => (value === "" ? items : items.filter(x => x.timelineName.toLowerCase().includes(value)))
        });
    };

    const addOrEdit = async (timeline, resetForm) => {
        if (timeline.timelineId === 0) {
            await timelineService.insertTimeline(timeline);
            toast.success('Insert timeline successfully!', {
                position: "top-right",
                autoClose: 5000,
                hideProgressBar: false,
                closeOnClick: true,
                pauseOnHover: true,
                draggable: true,
                progress: undefined,
                theme: "colored",
            });
        } else {
            await timelineService.updateTimeline(timeline);
            toast.success('Update timeline successfully!', {
                position: "top-right",
                autoClose: 5000,
                hideProgressBar: false,
                closeOnClick: true,
                pauseOnHover: true,
                draggable: true,
                progress: undefined,
                theme: "colored",
            });
        }
        resetForm();
        setRecordForEdit(null);
        setOpenPopup(false);
        const timelines = await timelineService.getTimelines();
        setRecords(timelines);
    };

    const openInPopup = item => {
        setRecordForEdit(item);
        setOpenPopup(true);
    };

    return (
        <>
            <PageHeader title="New Timeline" subTitle="Timeline management with validation" icon={<EventAvailableTwoToneIcon fontSize="large" />} />
            <Paper className={classes.pageContent}>
                <Toolbar>
                    <Controls.Input
                        label="Search Timelines"
                        className={classes.searchInput}
                        InputProps={{ startAdornment: <InputAdornment position="start"><Search /></InputAdornment> }}
                        onChange={handleSearch}
                    />
                    <Controls.Button
                        text="Add New"
                        variant="outlined"
                        startIcon={<AddIcon />}
                        className={classes.newButton}
                        onClick={() => { setOpenPopup(true); setRecordForEdit(null); }}
                    />
                </Toolbar>
                <TblContainer>
                    <TblHead />
                    <TableBody>
                        {recordsAfterPagingAndSorting().length > 0 ? (
                            recordsAfterPagingAndSorting().map(item => (
                                <TableRow key={item.timelineId}>
                                    <TableCell>{item.timelineId}</TableCell>
                                    <TableCell>{item.timelineName}</TableCell>
                                    <TableCell>{item.description}</TableCell>
                                    <TableCell>
                                        {new Date(item.startDate).toLocaleString('en-US', {
                                            timeZone: 'Asia/Bangkok',
                                            year: 'numeric',
                                            month: '2-digit',
                                            day: '2-digit',
                                            hour: '2-digit',
                                            minute: '2-digit',
                                            hour12: false
                                        })}
                                    </TableCell>
                                    <TableCell>
                                        {new Date(item.endDate).toLocaleString('en-US', {
                                            timeZone: 'Asia/Bangkok',
                                            year: 'numeric',
                                            month: '2-digit',
                                            day: '2-digit',
                                            hour: '2-digit',
                                            minute: '2-digit',
                                            hour12: false
                                        })}
                                    </TableCell>
                                    <TableCell>
                                        <ActionButton bgColor="#CBD2F0" textColor="#3E5B87" onClick={() => openInPopup(item)}>
                                            <EditOutlined fontSize="small" />
                                        </ActionButton>
                                        <ActionButton bgColor="#FFB3B3" textColor="#C62828" onClick={() => handleDelete(item.timelineId)}>
                                            <Delete fontSize="small" />
                                        </ActionButton>
                                    </TableCell>
                                </TableRow>
                            ))
                        ) : (
                            <TableRow>
                                <TableCell colSpan={headCells.length} style={{ textAlign: 'center' }}>
                                    No records found.
                                </TableCell>
                            </TableRow>
                        )}
                    </TableBody>
                </TblContainer>
                <TblPagination />
            </Paper>
            <Popup title="Timeline Form" openPopup={openPopup} setOpenPopup={setOpenPopup}>
                <TimelineForm recordForEdit={recordForEdit} addOrEdit={addOrEdit} />
            </Popup>
            <ConfirmDialog confirmDialog={confirmDialog} setConfirmDialog={setConfirmDialog} />
        </>
    );
}

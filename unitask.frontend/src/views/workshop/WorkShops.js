import React, { useState, useEffect } from 'react';
import WorkShopForm from './WorkShopForm';
import PageHeader from "../../components/PageHeader";
import EventAvailableTwoToneIcon from '@material-ui/icons/EventAvailableTwoTone';
import { Paper, makeStyles, TableBody, TableRow, TableCell, Toolbar, InputAdornment } from '@material-ui/core';
import useTable from "../../components/useTable";
import * as workshopService from "../../services/workShopService";
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
    { id: 'workshopId', label: 'Workshop ID' },
    { id: 'name', label: 'Workshop Name' },
    { id: 'location', label: 'Location' },
    { id: 'startDate', label: 'Start Date' },
    { id: 'endDate', label: 'End Date' },
    { id: 'status', label: 'Status' },
    { id: 'actions', label: 'Actions', disableSorting: true }
];

export default function WorkShops() {
    const classes = useStyles();
    const [records, setRecords] = useState([]);
    const [filterFn, setFilterFn] = useState({ fn: items => items });
    const [openPopup, setOpenPopup] = useState(false);
    const [confirmDialog, setConfirmDialog] = useState({ isOpen: false, title: '', subTitle: '' });
    const [recordForEdit, setRecordForEdit] = useState(null);

    useEffect(() => {
        const fetchData = async () => {
            const workshops = await workshopService.getWorkShops();
            setRecords(workshops);
        };
        fetchData();
    }, []);

    const handleDelete = workshopId => {
        setConfirmDialog({
            isOpen: true,
            title: 'Are you sure you want to delete this workshop?',
            subTitle: "You can't undo this operation",
            onConfirm: () => onDeleteConfirm(workshopId)
        });
    };

    const onDeleteConfirm = workshopId => {
        workshopService.deleteWorkshop(workshopId).then(() => {
            setRecords(records.filter(item => item.workshopId !== workshopId));
            toast.success('Delete workshop successfully!');
        });
        setConfirmDialog({ ...confirmDialog, isOpen: false });
    };

    const { TblContainer, TblHead, TblPagination, recordsAfterPagingAndSorting } = useTable(records, headCells, filterFn);

    const handleSearch = e => {
        const value = e.target.value.toLowerCase();
        setFilterFn({
            fn: items => (value === "" ? items : items.filter(x => x.name.toLowerCase().includes(value)))
        });
    };

    const addOrEdit = async (workshop, resetForm) => {
        if (workshop.workshopId === 0) {
            await workshopService.insertWorkshop(workshop);
            toast.success('Insert workshop successfully!');
        } else {
            await workshopService.updateWorkshop(workshop);
            toast.success('Update workshop successfully!');
        }
        resetForm();
        setRecordForEdit(null);
        setOpenPopup(false);
        const workshops = await workshopService.getWorkShops();
        setRecords(workshops); // Update records after add/edit
    };

    const openInPopup = item => {
        setRecordForEdit(item);
        setOpenPopup(true);
    };

    return (
        <>
            <PageHeader title="New Workshop" subTitle="Workshop management with validation" icon={<EventAvailableTwoToneIcon fontSize="large" />} />
            <Paper className={classes.pageContent}>
                <Toolbar>
                    <Controls.Input
                        label="Search Workshops"
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
                                <TableRow key={item.workshopId}>
                                    <TableCell>{item.workshopId}</TableCell>
                                    <TableCell>{item.name}</TableCell>
                                    <TableCell>{item.location}</TableCell>
                                    <TableCell>{new Date(item.startDate).toLocaleDateString()}</TableCell>
                                    <TableCell>{new Date(item.endDate).toLocaleDateString()}</TableCell>
                                    <TableCell>
                                        <span style={{
                                            padding: '4px 8px',
                                            borderRadius: '4px',
                                            backgroundColor: item.status === 'Active' ? '#d0f0c0' : '#f8d7da',
                                            color: item.status === 'Active' ? '#006400' : '#721c24',
                                            fontWeight: 'bold'
                                        }}>
                                            {item.status}
                                        </span>
                                    </TableCell>
                                    <TableCell>
                                        <ActionButton bgColor="#CBD2F0" textColor="#3E5B87" onClick={() => openInPopup(item)}>
                                            <EditOutlined fontSize="small" />
                                        </ActionButton>
                                        <ActionButton bgColor="#FFB3B3" textColor="#C62828" onClick={() => handleDelete(item.workshopId)}>
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
            <Popup title="Workshop Form" openPopup={openPopup} setOpenPopup={setOpenPopup}>
                <WorkShopForm recordForEdit={recordForEdit} addOrEdit={addOrEdit} />
            </Popup>
            <ConfirmDialog confirmDialog={confirmDialog} setConfirmDialog={setConfirmDialog} />
        </>
    );
}

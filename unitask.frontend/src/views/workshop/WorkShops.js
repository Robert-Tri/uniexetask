import React, { useState, useEffect } from 'react';
import WorkShopForm from './WorkShopForm';
import PageHeader from "../../components/PageHeader";
import EventAvailableTwoToneIcon from '@material-ui/icons/EventAvailableTwoTone';
import { Paper, makeStyles, TableBody, TableRow, TableCell, Toolbar, InputAdornment } from '@material-ui/core';
import useTable from "../../components/useTable";
import * as workshopService from "../../services/workShopService";
import Controls from '../../components/controls/Controls';
import { Search } from "@material-ui/icons";
import AddIcon from '@material-ui/icons/Add';
import Popup from '../../components/Popup';
import EditOutlinedIcon from '@material-ui/icons/EditOutlined';
import DeleteIcon from '@material-ui/icons/Delete';
import ConfirmDialog from '../../components/ConfirmDialog';
import { toast } from 'react-toastify';
import ActionButton from '../../components/controls/ActionButton';

const useStyles = makeStyles(theme => ({
    pageContent: {
        margin: theme.spacing(5),
        padding: theme.spacing(3)
    },
    searchInput: {
        width: '75%'
    },
    newButton: {
        position: 'absolute',
        right: '10px'
    }
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
    const [recordForEdit, setRecordForEdit] = useState(null);
    const [records, setRecords] = useState([]);
    const [filterFn, setFilterFn] = useState({ fn: items => { return items; } });
    const [openPopup, setOpenPopup] = useState(false);
    const [confirmDialog, setConfirmDialog] = useState({ isOpen: false, title: '', subTitle: '' });

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
            onConfirm: () => { onDeleteConfirm(workshopId) }
        });
    };

    const onDeleteConfirm = workshopId => {
        setConfirmDialog({ ...confirmDialog, isOpen: false });
        workshopService.deleteWorkshop(workshopId).then(() => {
            setRecords(records.filter(item => item.workshopId !== workshopId));
            toast.success('Delete workshop successfully!', {
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
    };

    const {
        TblContainer,
        TblHead,
        TblPagination,
        recordsAfterPagingAndSorting
    } = useTable(records, headCells, filterFn);

    const handleSearch = e => {
        let target = e.target;
        setFilterFn({
            fn: items => {
                if (target.value === "")
                    return items;
                else
                    return items.filter(x => x.name.toLowerCase().includes(target.value));
            }
        });
    };

    const addOrEdit = async (workshop, resetForm) => {
        if (workshop.workshopId === 0){
            await workshopService.insertWorkshop(workshop);
            toast.success('Insert workshop successfully!', {
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
            
        
        else {
            await workshopService.updateWorkshop(workshop);
            toast.success('Update workshop successfully!', {
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
        workshopService.getWorkShops().then(data => setRecords(data));
    };

    const openInPopup = item => {
        setRecordForEdit(item);
        setOpenPopup(true);
    };

    return (
        <>
            <PageHeader
                title="New Workshop"
                subTitle="Workshop management with validation"
                icon={<EventAvailableTwoToneIcon fontSize="large" />}
            />
            <Paper className={classes.pageContent}>
                <Toolbar>
                    <Controls.Input
                        label="Search Workshops"
                        className={classes.searchInput}
                        InputProps={{
                            startAdornment: (<InputAdornment position="start">
                                <Search />
                            </InputAdornment>)
                        }}
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
                        {
                            recordsAfterPagingAndSorting().map(item =>
                            (<TableRow key={item.workshopId}>
                                <TableCell>{item.workshopId}</TableCell>
                                <TableCell>{item.name}</TableCell>
                                <TableCell>{item.location}</TableCell>
                                <TableCell>{item.startDate.toLocaleDateString()}</TableCell>
                                <TableCell>{item.endDate.toLocaleDateString()}</TableCell>
                                <TableCell>
                                    <span
                                        style={{
                                            padding: '4px 8px',
                                            borderRadius: '4px',
                                            backgroundColor: item.status === 'Active' ? '#d0f0c0' : '#f8d7da',
                                            color: item.status === 'Active' ? '#006400' : '#721c24',
                                            fontWeight: 'bold'
                                        }}
                                    >
                                        {item.status}
                                    </span>
                                </TableCell>
                                <TableCell>
                                    <ActionButton
                                        bgColor="#CBD2F0"
                                        textColor="#3E5B87"
                                        onClick={() => { openInPopup(item); }}
                                    >
                                        <EditOutlinedIcon fontSize="small" />
                                    </ActionButton>
                                    <ActionButton
                                        bgColor="#FFB3B3"
                                        textColor="#C62828"
                                        onClick={() => handleDelete(item.workshopId)}
                                    >
                                        <DeleteIcon fontSize="small" />
                                    </ActionButton>
                                </TableCell>
                            </TableRow>))
                        }
                    </TableBody>
                </TblContainer>
                <TblPagination />
            </Paper>
            <Popup
                title="Workshop Form"
                openPopup={openPopup}
                setOpenPopup={setOpenPopup}
            >
                <WorkShopForm
                    recordForEdit={recordForEdit}
                    addOrEdit={addOrEdit} />
            </Popup>
            <ConfirmDialog
                confirmDialog={confirmDialog}
                setConfirmDialog={setConfirmDialog}
            />
        </>
    );
}

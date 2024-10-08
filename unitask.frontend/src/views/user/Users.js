import React, { useState, useEffect } from 'react';
import UserForm from './UserForm';
import PageHeader from "../../components/PageHeader";
import PeopleOutlineTwoToneIcon from '@material-ui/icons/PeopleOutlineTwoTone';
import { Paper, makeStyles, TableBody, TableRow, TableCell, Toolbar, InputAdornment } from '@material-ui/core';
import useTable from "../../components/useTable";
import * as userService from "../../services/userService";
import * as roleService from "../../services/roleService";
import * as campusService from '../../services/campusService';
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
    { id: 'user_id', label: 'UserID' },
    { id: 'fullName', label: 'User Name' },
    { id: 'email', label: 'Email Address (Personal)' },
    { id: 'phone', label: 'Mobile Number' },
    { id: 'campusName', label: 'Campus' },
    { id: 'status', label: 'Status' },
    { id: 'role', label: 'Role' },
    { id: 'actions', label: 'Actions', disableSorting: true }
];

export default function Users() {
    const classes = useStyles();
    const [records, setRecords] = useState([]);
    const [filterFn, setFilterFn] = useState({ fn: items => items });
    const [openPopup, setOpenPopup] = useState(false);
    const [confirmDialog, setConfirmDialog] = useState({ isOpen: false, title: '', subTitle: '' });
    const [recordForEdit, setRecordForEdit] = useState(null);
    const [campusMap, setCampusMap] = useState(new Map());
    const [roleMap, setRoleMap] = useState(new Map());

    useEffect(() => {
        const fetchData = async () => {
            const [users, campuses, roles] = await Promise.all([
                userService.getAllUsers(),
                campusService.getAllCampuses(),
                roleService.getAllRoles()
            ]);
            setRecords(users);
            setCampusMap(new Map(campuses.map(campus => [campus.campusName, campus.campusId])));
            setRoleMap(new Map(roles.map(role => [role.name, role.roleId])));
        };
        fetchData();
    }, []);

    const handleDelete = userId => {
        setConfirmDialog({
            isOpen: true,
            title: 'Are you sure you want to delete this user?',
            subTitle: "You can't undo this operation",
            onConfirm: () => onDeleteConfirm(userId)
        });
    };

    const onDeleteConfirm = userId => {
        userService.deleteUser(userId).then(() => {
            setRecords(records.map(item => (item.user_id === userId ? { ...item, status: false } : item)));
            toast.success('Delete user successfully!');
        });
        setConfirmDialog({ ...confirmDialog, isOpen: false });
    };

    const { TblContainer, TblHead, TblPagination, recordsAfterPagingAndSorting } = useTable(records, headCells, filterFn);

    const handleSearch = e => {
        const value = e.target.value.toLowerCase();
        setFilterFn({ fn: items => (value === "" ? items : items.filter(x => x.fullName.toLowerCase().includes(value))) });
    };

    const addOrEdit = async (user, resetForm) => {
        user.user_id === 0 ? await userService.insertUser(user) : await userService.updateUser(user);
        resetForm();
        setRecordForEdit(null);
        setOpenPopup(false);
        setRecords(await userService.getAllUsers());
        toast.success('Update user successfully!');
    };

    const openInPopup = item => {
        setRecordForEdit({ ...item, campusId: campusMap.get(item.campusName) || '', roleId: roleMap.get(item.roleName) || '' });
        setOpenPopup(true);
    };

    return (
        <>
            <PageHeader title="New User" subTitle="Form design with validation" icon={<PeopleOutlineTwoToneIcon fontSize="large" />} />
            <Paper className={classes.pageContent}>
                <Toolbar>
                    <Controls.Input
                        label="Search Users"
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
                                <TableRow key={item.user_id}>
                                    <TableCell>{item.user_id}</TableCell>
                                    <TableCell>{item.fullName}</TableCell>
                                    <TableCell>{item.email}</TableCell>
                                    <TableCell>{item.phone}</TableCell>
                                    <TableCell>{item.campusName}</TableCell>
                                    <TableCell>
                                        <span style={{
                                            padding: '4px 8px',
                                            borderRadius: '4px',
                                            backgroundColor: item.status ? '#d0f0c0' : '#f8d7da',
                                            color: item.status ? '#006400' : '#721c24',
                                            fontWeight: 'bold'
                                        }}>
                                            {item.status ? 'Active' : 'Inactive'}
                                        </span>
                                    </TableCell>
                                    <TableCell>{item.roleName}</TableCell>
                                    <TableCell>
                                        <ActionButton bgColor="#CBD2F0" textColor="#3E5B87" onClick={() => openInPopup(item)}>
                                            <EditOutlined fontSize="small" />
                                        </ActionButton>
                                        <ActionButton bgColor="#FFB3B3" textColor="#C62828" onClick={() => handleDelete(item.user_id)}>
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
            <Popup title="User Form" openPopup={openPopup} setOpenPopup={setOpenPopup}>
                <UserForm recordForEdit={recordForEdit} addOrEdit={addOrEdit} />
            </Popup>
            <ConfirmDialog confirmDialog={confirmDialog} setConfirmDialog={setConfirmDialog} />
        </>
    );
}

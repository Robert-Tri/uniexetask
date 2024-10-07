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
    const [recordForEdit, setRecordForEdit] = useState(null);
    const [records, setRecords] = useState([]);
    const [filterFn, setFilterFn] = useState({ fn: items => { return items; } });
    const [openPopup, setOpenPopup] = useState(false);
    const [confirmDialog, setConfirmDialog] = useState({ isOpen: false, title: '', subTitle: '' });
    const [campusMap, setCampusMap] = useState(new Map());
    const [roleMap, setRoleMap] = useState(new Map());

    useEffect(() => {
        const fetchData = async () => {
            const users = await userService.getAllUsers();
            setRecords(users);

            const campuses = await campusService.getAllCampuses();
            const map = new Map(campuses.map(campus => [campus.campusName, campus.campusId]));
            setCampusMap(map);

            const roles = await roleService.getAllRoles();
            const roleMap = new Map(roles.map(role => [role.name, role.roleId]));
            setRoleMap(roleMap);
        };

        fetchData();
    }, []);

    const handleDelete = userId => {
        setConfirmDialog({
            isOpen: true,
            title: 'Are you sure you want to delete this user?',
            subTitle: "You can't undo this operation",
            onConfirm: () => { onDeleteConfirm(userId) }
        });
    };

    const onDeleteConfirm = userId => {
        setConfirmDialog({ ...confirmDialog, isOpen: false });
        userService.deleteUser(userId).then(() => {
            setRecords(records.map(item =>
                item.user_id === userId ? { ...item, status: false } : item
            ));
            toast.success('Delete user successfully!', {
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
                    return items.filter(x => x.fullName.toLowerCase().includes(target.value));
            }
        });
    };

    const addOrEdit = async (user, resetForm) => {
        if (user.user_id === 0)
            await userService.insertUser(user);
        else {
            await userService.updateUser(user);
            toast.success('Update user successfully!', {
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
        userService.getAllUsers().then(data => setRecords(data));
    };

    const openInPopup = item => {
        const campusId = campusMap.get(item.campusName) || '';
        const roleId = roleMap.get(item.roleName) || '';

        const updatedItem = {
            ...item,
            campusId: campusId,
            roleId: roleId
        };

        setRecordForEdit(updatedItem);
        setOpenPopup(true);
    };

    return (
        <>
            <PageHeader
                title="New User"
                subTitle="Form design with validation"
                icon={<PeopleOutlineTwoToneIcon fontSize="large" />}
            />
            <Paper className={classes.pageContent}>
                <Toolbar>
                    <Controls.Input
                        label="Search Users"
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
                            (<TableRow key={item.user_id}>
                                <TableCell>{item.user_id}</TableCell>
                                <TableCell>{item.fullName}</TableCell>
                                <TableCell>{item.email}</TableCell>
                                <TableCell>{item.phone}</TableCell>
                                <TableCell>{item.campusName}</TableCell>
                                <TableCell>
                                    <span
                                        style={{
                                            padding: '4px 8px',
                                            borderRadius: '4px',
                                            backgroundColor: item.status ? '#d0f0c0' : '#f8d7da',
                                            color: item.status ? '#006400' : '#721c24',
                                            fontWeight: 'bold'
                                        }}
                                    >
                                        {item.status ? 'Active' : 'Inactive'}
                                    </span>
                                </TableCell>
                                <TableCell>{item.roleName}</TableCell>
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
                                        onClick={() => handleDelete(item.user_id)}
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
                title="User Form" // Đổi tiêu đề
                openPopup={openPopup}
                setOpenPopup={setOpenPopup}
            >
                <UserForm
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

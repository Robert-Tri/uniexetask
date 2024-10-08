import React, { useState, useEffect } from 'react'; 
import { Typography, Grid, Select, MenuItem, FormControl, InputLabel, Table, TableHead, TableBody, TableRow, TableCell, Checkbox, Button, CircularProgress, Backdrop } from '@mui/material';
import PageContainer from '../../components/container/PageContainer';
import DashboardCard from '../../components/shared/DashboardCard';
import axios from 'axios';


const RolePermission = () => {
  const [rolesData, setRolesData] = useState([]);
  const [featuresData, setFeaturesData] = useState([]);
  const [permissionsData, setPermissionsData] = useState([]);
  const [selectedRole, setSelectedRole] = useState('');
  const [rolePermissions, setRolePermissions] = useState({});
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    const fetchRoles = async () => {
      setLoading(true);
      try {
        const response = await axios.get('https://localhost:7289/api/role-permission/roles');
        setRolesData(response.data); 
      } catch (error) {
        console.error('Error fetching roles:', error);
      } finally {
        setLoading(false);
      }
    };

    fetchRoles();
  }, []);

  useEffect(() => {
    if (selectedRole) {
      const fetchPermissionsByRole = async () => {
        setLoading(true); // Bắt đầu loading
        try {
          const response = await axios.get(`https://localhost:7289/api/role-permission/permissions?role=${selectedRole}`);
          if (response.data.success) {
            const { features, permissions, permissionsWithRole } = response.data.data; // Lấy dữ liệu từ response
            setFeaturesData(features);
            setPermissionsData(permissions);
            const initialPermissions = permissions.reduce((acc, permission) => {
              acc[permission.permissionId] = false; // Mặc định là false
              return acc;
            }, {});
            
            // Cập nhật các quyền của role
            permissionsWithRole.forEach(rp => {
              initialPermissions[rp.permissionId] = true; // Thiết lập giá trị là true cho quyền có trong role
            });
            setRolePermissions(initialPermissions);
          } else {
            console.error(response.data.ErrorMessage);
          }
        } catch (error) {
          console.error('Error fetching permissions:', error);
        } finally {
          setLoading(false);
        }
      };

      fetchPermissionsByRole();
    }
  }, [selectedRole]);

  const handleRoleChange = (event) => {
    setSelectedRole(event.target.value);
  };

  const handlePermissionChange = (permissionId) => {
    setRolePermissions((prevPermissions) => ({
      ...prevPermissions,
      [permissionId]: !prevPermissions[permissionId],
    }));
  };

  const handleSave = async () => {
    try {
      setLoading(true);
      
      const payload = {
        roleName: selectedRole,
        permissions: Object.keys(rolePermissions).filter((permissionId) => rolePermissions[permissionId])
      };
  
      const response = await axios.post('https://localhost:7289/api/role-permission/update', payload, {
        headers: {
          'Content-Type': 'application/json'
        }
      });
            
      if (response.data.success) {
        alert('Cập nhật quyền thành công!');
      } else {
        console.error(response.data.ErrorMessage);
        alert('Cập nhật quyền thất bại!');
      }
    } catch (error) {
      console.error('Error updating permissions:', error);
      alert('Đã xảy ra lỗi khi cập nhật quyền.');
    } finally {
      setLoading(false); // Tắt loading sau khi lưu xong
    }
  };

  return (
    <PageContainer title="Role Permission Management" description="This is the role permission management page.">
      <Backdrop sx={{ color: '#fff', zIndex: (theme) => theme.zIndex.drawer + 1 }} open={loading}>
        <CircularProgress color="inherit" />
      </Backdrop>
      <DashboardCard title="Role Permission Management">
        <Grid container spacing={2} alignItems="center" justifyContent="space-between" paddingBottom="10px">
          <Grid item xs={12} sm={4}>
            <FormControl fullWidth>
              <InputLabel id="role-select-label" size='small'>Chọn Role</InputLabel>
              <Select
                labelId="role-select-label"
                id="roleSelect"
                value={selectedRole}
                onChange={handleRoleChange}
                label="Chọn Role"
                size='small'
              >
                <MenuItem value="">
                  <em>-- Chọn Role --</em>
                </MenuItem>
                {rolesData.map((role) => (
                  <MenuItem key={role} value={role}> 
                    {role} {}
                  </MenuItem>
                ))}
              </Select>
            </FormControl>
          </Grid>
          <Grid item>
            <Button
              variant="contained"
              color="primary"
              onClick={handleSave}
              disabled={!selectedRole}
            >
              Lưu thay đổi
            </Button>
          </Grid>
        </Grid>

        {selectedRole && (
          <div>
            <Table sx={{ borderCollapse: 'collapse'}}>
              <TableHead>
                <TableRow>
                  <TableCell
                    sx={{ backgroundColor: '#5d87ff', color: 'white', border: '1px solid #ddd' }}
                  >
                    Tính năng
                  </TableCell>
                  <TableCell
                    sx={{ backgroundColor: '#5d87ff', color: 'white', border: '1px solid #ddd' }}
                    align="center"
                  >
                    Quyền
                  </TableCell>
                </TableRow>
              </TableHead>
              <TableBody>
                {featuresData.map((feature) => (
                  <TableRow key={feature.featureId}>
                    <TableCell style={{ verticalAlign: 'top', border: '1px solid #ddd' }}>
                      {feature.name}
                    </TableCell>
                    <TableCell style={{ border: '1px solid #ddd' }}>
                      <Grid container spacing={1}>
                        {permissionsData
                          .filter((permission) => permission.featureId === feature.featureId)
                          .map((permission) => (
                            <Grid item xs={4} key={permission.permissionId}>
                              <Checkbox
                                checked={rolePermissions[permission.permissionId] || false}
                                onChange={() => handlePermissionChange(permission.permissionId)}
                              />
                              {permission.name}
                            </Grid>
                          ))}
                      </Grid>
                    </TableCell>
                  </TableRow>
                ))}
              </TableBody>
            </Table>
          </div>
        )}
      </DashboardCard>
    </PageContainer>
  );
};

export default RolePermission;

import React, { useState, useEffect } from 'react';
import {
  Container,
  Typography,
  List,
  ListItem,
  ListItemIcon,
  ListItemText,
  ListItemSecondaryAction,
  IconButton,
  CircularProgress,
  Box,
  Tooltip
} from '@mui/material';
import {
  Description as DescriptionIcon,
  Image as ImageIcon,
  Slideshow as SlideshowIcon,
  TableChart as TableChartIcon,
  Archive as ArchiveIcon,
  InsertDriveFile as InsertDriveFileIcon,
  Download as DownloadIcon
} from '@mui/icons-material';

const fileIcons = {
  pdf: <DescriptionIcon color="error" />,
  png: <ImageIcon color="primary" />,
  pptx: <SlideshowIcon color="secondary" />,
  xlsx: <TableChartIcon color="success" />,
  zip: <ArchiveIcon color="action" />,
  default: <InsertDriveFileIcon color="disabled" />
};

const Files = () => {
  const [files, setFiles] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    setTimeout(() => {
      const mockData = [
        { id: 1, name: 'Document1.pdf', size: 120, url: '#', type: 'pdf', modified: '2024-04-01' },
        { id: 2, name: 'Image1.png', size: 220, url: '#', type: 'png', modified: '2024-04-02' },
        { id: 3, name: 'Presentation.pptx', size: 300, url: '#', type: 'pptx', modified: '2024-04-03' },
        { id: 4, name: 'Spreadsheet.xlsx', size: 450, url: '#', type: 'xlsx', modified: '2024-04-04' },
        { id: 5, name: 'Archive.zip', size: 1024, url: '#', type: 'zip', modified: '2024-04-05' },
        { id: 6, name: 'UnknownFile.xyz', size: 150, url: '#', type: 'xyz', modified: '2024-04-06' },
      ];
      setFiles(mockData);
      setLoading(false);
    }, 1000);
  }, []);

  return (
    <Container>
      <Typography variant="h4" gutterBottom>Files</Typography>
      {loading ? (
        <Box display="flex" justifyContent="center" alignItems="center" height="50vh">
          <CircularProgress />
        </Box>
      ) : (
        <Box
          sx={{
            maxHeight: '50vh', // Shortened height
            overflowY: 'auto',
            border: '1px solid #e0e0e0',
            borderRadius: 1,
            padding: 1
          }}
        >
          <List>
            {files.map(({ id, name, size, url, type, modified }) => (
              <ListItem key={id} divider>
                <ListItemIcon>{fileIcons[type] || fileIcons.default}</ListItemIcon>
                <ListItemText
                  primary={name}
                  secondary={`Size: ${size} KB • ${new Date(modified).toLocaleDateString('en-US')}`}
                />
                <ListItemSecondaryAction>
                  <Tooltip title="Download">
                    <IconButton edge="end" href={url} target="_blank">
                      <DownloadIcon />
                    </IconButton>
                  </Tooltip>
                </ListItemSecondaryAction>
              </ListItem>
            ))}
          </List>
        </Box>
      )}
    </Container>
  );
};

export default Files;

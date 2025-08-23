import React, { useState, useEffect } from 'react';
import { 
  Container, 
  Grid, 
  Typography, 
  Box, 
  CircularProgress, 
  Alert, 
  Paper,
  Drawer,
  IconButton,
  Button
} from '@mui/material';
import { useLocation } from 'react-router-dom';
import FilterListIcon from '@mui/icons-material/FilterList';
import CloseIcon from '@mui/icons-material/Close';
import AlbumCard from '../components/AlbumCard';
import AlbumFilter from '../components/AlbumFilter';
import Pagination from '../components/Pagination';
import { fetchAlbums } from '../services/api';
import { ALBUM_SORT_FIELDS } from '../constants/albumSortFields';

const AlbumsPage = () => {
  const location = useLocation();
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [albums, setAlbums] = useState([]);
  const [totalCount, setTotalCount] = useState(0);
  const [pageCount, setPageCount] = useState(0);
  const [isFilterOpen, setIsFilterOpen] = useState(false);
  
  const getInitialFilters = () => {
    const searchParams = new URLSearchParams(location.search);
    const bandId = searchParams.get('bandId');
    const distributorId = searchParams.get('distributorId');
    
    return {
      page: 1,
      pageSize: 20,
      sortBy: ALBUM_SORT_FIELDS.RELEASE_DATE,
      sortAscending: false,
      ...(bandId && { bandId }),
      ...(distributorId && { distributorId })
    };
  };
  
  const [filters, setFilters] = useState(getInitialFilters);

  useEffect(() => {
    setFilters(getInitialFilters());
  }, [location.search]);
  
  useEffect(() => {
    const fetchData = async () => {
      try {
        setLoading(true);
        const response = await fetchAlbums(filters);
        
        if (response.data) {
          setAlbums(response.data.items || []);
          setTotalCount(response.data.totalCount || 0);
          setPageCount(response.data.pageCount || 0);
        }
      } catch (err) {
        console.error('Error fetching albums:', err);
        setError('Failed to load albums. Please try again later.');
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, [filters]);

  const handleFilterChange = (newFilters) => {
    // Reset to page 1 when filters change
    setFilters({
      ...newFilters,
      page: 1
    });
    // Close filter drawer after applying
    setIsFilterOpen(false);
  };

  const handlePageChange = (newPage) => {
    setFilters({
      ...filters,
      page: newPage
    });
  };

  const handlePageSizeChange = (newPageSize) => {
    setFilters({
      ...filters,
      pageSize: newPageSize,
      page: 1 // Reset to first page when changing page size
    });
  };

  const toggleFilterDrawer = () => {
    setIsFilterOpen(!isFilterOpen);
  };

  return (
    <Container maxWidth="xl" sx={{ py: 4 }}>
      <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 3 }}>
        <Typography variant="h4" component="h1">
          Metal Releases
        </Typography>
        <Button 
          variant="contained"
          color="primary"
          startIcon={<FilterListIcon />}
          onClick={toggleFilterDrawer}
          sx={{ fontWeight: 'bold' }}
        >
          Filters
        </Button>
      </Box>

      {error && (
        <Alert severity="error" sx={{ my: 2 }}>
          {error}
        </Alert>
      )}

      {loading ? (
        <Box sx={{ display: 'flex', justifyContent: 'center', my: 4 }}>
          <CircularProgress />
        </Box>
      ) : albums.length > 0 ? (
        <>
          <Box sx={{ mb: 3 }}>
            <Pagination
              currentPage={filters.page}
              totalPages={pageCount}
              totalItems={totalCount}
              pageSize={filters.pageSize}
              onPageChange={handlePageChange}
              onPageSizeChange={handlePageSizeChange}
            />
          </Box>
          
          <Box sx={{ width: '100%', mb: 4 }}>
            <Grid 
              container 
              spacing={3}
              sx={{
                display: 'grid',
                gridTemplateColumns: {
                  xs: 'repeat(1, 1fr)',
                  sm: 'repeat(2, 1fr)',
                  md: 'repeat(3, 1fr)',
                  lg: 'repeat(4, 1fr)',
                  xl: 'repeat(5, 1fr)'
                },
                gap: 3,
                alignItems: 'stretch'
              }}
            >
              {albums.map((album) => (
                <Box 
                  key={album.id}
                  sx={{ 
                    display: 'flex',
                    height: '100%'
                  }}
                >
                  <AlbumCard album={album} />
                </Box>
              ))}
            </Grid>
          </Box>
          
          <Box sx={{ mt: 3, mb: 4 }}>
            <Pagination
              currentPage={filters.page}
              totalPages={pageCount}
              totalItems={totalCount}
              pageSize={filters.pageSize}
              onPageChange={handlePageChange}
              onPageSizeChange={handlePageSizeChange}
            />
          </Box>
        </>
      ) : (
        <Paper sx={{ p: 4, my: 3, textAlign: 'center' }}>
          <Typography variant="h6" color="text.secondary">
            No albums found matching your criteria.
          </Typography>
          <Typography variant="body2" color="text.secondary" sx={{ mt: 1 }}>
            Try adjusting your filters to see more results.
          </Typography>
        </Paper>
      )}

      {/* Filter drawer */}
      <Drawer
        anchor="right"
        open={isFilterOpen}
        onClose={toggleFilterDrawer}
        sx={{
          '& .MuiDrawer-paper': {
            width: { xs: '100%', sm: 400 },
            boxSizing: 'border-box',
            backgroundColor: 'background.paper',
            borderTopLeftRadius: { xs: 0, sm: 8 },
            borderBottomLeftRadius: { xs: 0, sm: 8 },
            boxShadow: '-4px 0 20px rgba(0,0,0,0.2)'
          },
        }}
      >
        <Box sx={{ position: 'relative', p: 1 }}>
          <IconButton 
            onClick={toggleFilterDrawer}
            sx={{ position: 'absolute', right: 8, top: 8, zIndex: 5 }}
          >
            <CloseIcon />
          </IconButton>
          <AlbumFilter 
            onFilterChange={handleFilterChange} 
            initialFilters={filters}
          />
        </Box>
      </Drawer>
    </Container>
  );
};

export default AlbumsPage; 
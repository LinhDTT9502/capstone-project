import React, { useEffect, useState } from 'react';
import mapboxgl from 'mapbox-gl';
import { Input } from '@material-tailwind/react';
import { fetchBranchs } from '../services/branchService';

const ListBranchs = () => {
  const [branches, setBranches] = useState([]);
  const [searchQuery, setSearchQuery] = useState('');
  const [filteredBranches, setFilteredBranches] = useState([]);
  const [selectedBranch, setSelectedBranch] = useState(null);
  const [map, setMap] = useState(null);

  // Initialize Mapbox
  useEffect(() => {
    // Check if Mapbox GL is available
    if (mapboxgl) {
      mapboxgl.accessToken = import.meta.env.VITE_MAPBOX_API_KEY;
      const mapInstance = new mapboxgl.Map({
        container: 'map', // The ID of the div where the map should render
        style: 'mapbox://styles/mapbox/streets-v11', // Map style
        center: [106.6297, 10.8231], // Default location (Ho Chi Minh City)
        zoom: 15,
      });
      setMap(mapInstance);
    }
  }, []);

  // Fetch all branches
  useEffect(() => {
    const loadBranches = async () => {
      try {
        const data = await fetchBranchs();
        setBranches(data);
        setFilteredBranches(data);
      } catch (error) {
        console.error('Error fetching branch data:', error);
      }
    };

    loadBranches();
  }, []);

  // Handle search input
  const handleSearch = (e) => {
    const query = e.target.value.toLowerCase();
    setSearchQuery(query);

    const filtered = branches.filter(
      (branch) =>
        branch.branchName.toLowerCase().includes(query) ||
        branch.location.toLowerCase().includes(query)
    );

    setFilteredBranches(filtered);
  };

  // Handle branch selection
  const handleBranchClick = (branch) => {
    setSelectedBranch(branch);
    if (map) {
      // Using Mapbox Geocoding API to get coordinates
      fetch(`https://api.mapbox.com/geocoding/v5/mapbox.places/${encodeURIComponent(branch.location)}.json?access_token=${mapboxgl.accessToken}`)
        .then(response => response.json())
        .then(data => {
          const [longitude, latitude] = data.features[0]?.geometry.coordinates || [];
          if (longitude && latitude) {
            map.setCenter([longitude, latitude]);
            new mapboxgl.Marker()
              .setLngLat([longitude, latitude])
              .setPopup(new mapboxgl.Popup().setHTML(`<h3>${branch.branchName}</h3><p>${branch.location}</p>`))
              .addTo(map);
          }
        })
        .catch(error => {
          console.error('Error geocoding branch location:', error);
        });
    }
  };

  return (
    <div className="flex h-screen">
      {/* Left Column: Branch List */}
      <div className="w-1/3 p-4 overflow-y-auto">
        <h1 className="text-2xl font-bold mb-4">Branch List</h1>

        {/* Search Bar */}
        <Input
          label="Search by branch name or location"
          value={searchQuery}
          onChange={handleSearch}
          className="mb-4"
        />

        {/* Branch List */}
        <div>
          {filteredBranches.length > 0 ? (
            filteredBranches.map((branch) => (
              <div
                key={branch.id}
                onClick={() => handleBranchClick(branch)}
                className={`border rounded-lg p-4 mb-4 cursor-pointer hover:bg-blue-50 ${
                  selectedBranch?.id === branch.id ? 'bg-blue-100' : ''
                }`}
              >
                <h2 className="text-xl font-semibold">{branch.branchName}</h2>
                <p className="text-gray-600">{branch.location}</p>
                <p className="text-gray-800 font-medium">Hotline: {branch.hotline}</p>
              </div>
            ))
          ) : (
            <p>No branches found.</p>
          )}
        </div>
      </div>

      {/* Right Column: Map */}
      <div className="w-2/3">
        <div id="map" className="w-full h-full"></div>
      </div>
    </div>
  );
};

export default ListBranchs;

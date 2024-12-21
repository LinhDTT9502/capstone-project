import React, { useEffect, useState } from 'react';
import mapboxgl from 'mapbox-gl';
import { Input } from '@material-tailwind/react';
import { fetchBranchs } from '../services/branchService';

const ListBranches = () => {
  const [branches, setBranches] = useState([]);
  const [searchQuery, setSearchQuery] = useState('');
  const [filteredBranches, setFilteredBranches] = useState([]);
  const [selectedBranch, setSelectedBranch] = useState(null);
  // const [map, setMap] = useState(null);
  const [marker, setMarker] = useState(null); // To track the active marker
  const geocodeCache = new Map(); // Cache for geocoded results

  // Initialize Mapbox
  // useEffect(() => {
  //   const token = "pk.eyJ1IjoiYWlvbmlhYWEiLCJhIjoiY20zc2l4OTNqMDk5dTJqc2U2aXF2cWZvaSJ9.dM3Z4hHnaD-tTioBrl6_Dg";

  //   mapboxgl.accessToken = token;

  //   const mapInstance = new mapboxgl.Map({
  //     container: 'map', // Map container ID
  //     style: 'mapbox://styles/mapbox/streets-v11',
  //     center: [106.6297, 10.8231], // Default center (Ho Chi Minh City)
  //     zoom: 12,
  //   });

  //   setMap(mapInstance);

  //   return () => {
  //     mapInstance.remove(); // Cleanup on component unmount
  //   };
  // }, []);

  // Fetch all branches
  useEffect(() => {
    const loadBranches = async () => {
      try {
        const data = await fetchBranchs(); // Replace this with your API call
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

  // Geocode the branch location
  // const geocodeLocation = async (location) => {
  //   if (geocodeCache.has(location)) {
  //     return geocodeCache.get(location); // Return cached result
  //   }

  //   try {
  //     const response = await fetch(
  //       `https://api.mapbox.com/geocoding/v5/mapbox.places/${encodeURIComponent(location)}.json?access_token=${mapboxgl.accessToken}`
  //     );
  //     const data = await response.json();

  //     if (data.features.length > 0) {
  //       const coordinates = data.features[0].geometry.coordinates;
  //       geocodeCache.set(location, coordinates); // Cache the result
  //       return coordinates;
  //     } else {
  //       console.warn(`No coordinates found for location: ${location}`);
  //       return null;
  //     }
  //   } catch (error) {
  //     console.error('Error geocoding location:', error);
  //     return null;
  //   }
  // };

  // Handle branch selection
  const handleBranchClick = async (branch) => {
    setSelectedBranch(branch);

    // if (map) {
    //   const coordinates = await geocodeLocation(branch.location);

    //   if (coordinates) {
    //     // Center map and add marker
    //     map.flyTo({ center: coordinates, zoom: 15 });

    //     // Remove existing marker
    //     if (marker) {
    //       marker.remove();
    //     }

    //     // Add a new marker
    //     const newMarker = new mapboxgl.Marker()
    //       .setLngLat(coordinates)
    //       .setPopup(
    //         new mapboxgl.Popup().setHTML(
    //           `<h3>${branch.branchName}</h3><p>${branch.location}</p>`
    //         )
    //       )
    //       .addTo(map);

    //     setMarker(newMarker); // Update the marker state
    //   }
    // }
  };

  return (
    <div className="flex h-screen">
    {/* Left Column: Branch List */}
    <div className="w-1/3 p-4 overflow-y-auto">
      <h1 className="text-2xl font-bold mb-4">Danh sách hệ thống cửa hàng</h1>
  
      {/* Search Bar */}
      <Input
        label="Tìm kiếm tên hệ thống hoặc vị trí..."
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
          <p>Không có hệ thống nào.</p>
        )}
      </div>
    </div>
  
    {/* Right Column: Branch Image */}
    <div className="w-2/3 p-4 flex items-center justify-center">
      {selectedBranch?.imgAvatarPath ? (
        <img
          src={selectedBranch.imgAvatarPath}
          alt={selectedBranch.branchName}
          className="max-w-full max-h-full object-contain rounded-lg shadow-lg"
        />
      ) : (
        <p className="text-gray-500">Chọn một cửa hàng để xem hình ảnh.</p>
      )}
    </div>
  </div>
  
  );
};

export default ListBranches;

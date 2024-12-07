import React, { useEffect, useState } from 'react';
import mapboxgl from 'mapbox-gl';
import { Input } from '@material-tailwind/react';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faLocationDot } from '@fortawesome/free-solid-svg-icons';
import { fetchBranchs } from '../services/branchService';

const ListBranchs = () => {
  const [branches, setBranches] = useState([]);
  const [searchQuery, setSearchQuery] = useState('');
  const [filteredBranches, setFilteredBranches] = useState([]);
  const [selectedBranch, setSelectedBranch] = useState(null);
  const [map, setMap] = useState(null);
  const [marker, setMarker] = useState(null); // To track the active marker
  const geocodeCache = new Map(); // Cache for geocoded results

  // Initialize Mapbox
  useEffect(() => {
    const token = "pk.eyJ1IjoiYWlvbmlhYWEiLCJhIjoiY20zc2l4OTNqMDk5dTJqc2U2aXF2cWZvaSJ9.dM3Z4hHnaD-tTioBrl6_Dg";

    mapboxgl.accessToken = token;

    const mapInstance = new mapboxgl.Map({
      container: 'map',
      style: 'mapbox://styles/mapbox/streets-v11',
      center: [106.6297, 10.8231], // Default center (Ho Chi Minh City)
      zoom: 12,
    });

    setMap(mapInstance);
  }, []);

  // map.addControl(
  //   new MapboxGeocoder({
  //     accessToken: mapboxgl.accessToken,
  //     mapboxgl: mapboxgl
  //   })
  // );

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

  // Geocode the branch location
  const geocodeLocation = async (location) => {
    if (geocodeCache.has(location)) {
      return geocodeCache.get(location); // Return cached result
    }

    try {
      const response = await fetch(
        `https://api.mapbox.com/geocoding/v5/mapbox.places/${encodeURIComponent(location)}.json?access_token=${mapboxgl.accessToken}`
      );
      const data = await response.json();

      if (data.features.length > 0) {
        const coordinates = data.features[0].geometry.coordinates;
        geocodeCache.set(location, coordinates); // Cache the result
        return coordinates;
      } else {
        console.warn(`No coordinates found for location: ${location}`);
        return null;
      }
    } catch (error) {
      console.error('Error geocoding location:', error);
      return null;
    }
  };

  // Handle branch selection
  const handleBranchClick = async (branch) => {
    setSelectedBranch(branch);

    if (map) {
      const coordinates = await geocodeLocation(branch.location);

      if (coordinates) {
        console.log(`Branch: ${branch.branchName}, Coordinates: ${coordinates}`); // Debugging

        // Center map and add marker
        map.flyTo({ center: coordinates, zoom: 15 });

        // Remove existing marker
        if (marker) {
          marker.remove();
        }

        // Create a custom marker element
        const markerElement = document.createElement('div');
        markerElement.innerHTML = `<div style="color: red; font-size: 24px;">
          <i class="fa-solid fa-location-dot"></i>
        </div>`;
        markerElement.style.cursor = 'pointer';

        // Add the custom marker
        const newMarker = new mapboxgl.Marker({ element: markerElement })
          .setLngLat(coordinates)
          .setPopup(
            new mapboxgl.Popup().setHTML(
              `<h3>${branch.branchName}</h3><p>${branch.location}</p>`
            )
          )
          .addTo(map);

        setMarker(newMarker); // Update the marker state

        console.log('Marker added successfully.');
      } else {
        console.error('No coordinates found for this branch location.');
      }
    } else {
      console.error('Map instance is not ready.');
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
                className={`border rounded-lg p-4 mb-4 cursor-pointer hover:bg-blue-50 ${selectedBranch?.id === branch.id ? 'bg-blue-100' : ''
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
      <div className="w-2/3 h-fit">
        <div id="map" className=""></div>
      </div>
    </div>
  );
};

export default ListBranchs;

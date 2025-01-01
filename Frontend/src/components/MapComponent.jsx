import React, { useRef, useEffect, useState } from 'react';
import * as maptilersdk from '@maptiler/sdk';
import "@maptiler/sdk/dist/maptiler-sdk.css";
import '../App.css';

const MapComponent = (branch) => {

    const mapContainer = useRef(null);
    const map = useRef(null);
    const [location, setLocation] = useState({ lng: 106.62869854500008, lat: 10.749413720000064 });
    const zoom = 14;
    maptilersdk.config.apiKey = 'n3xDVt0XouHoByyvQqLj';

    useEffect(() => {

        // if (!branch?.branch.Location) return;

        const fetchGeocode = async () => {
            try {
                var location = branch.branch.location;
                if (location !== null) {
                    const response = await fetch(`https://rsapi.goong.io/geocode?address=${location}&api_key=RQTJWCxy1wZJfrmW7iwGtJqwfJLhYGvbSMSlXFgi`);
                    console.log(response);

                    const data = await response.json();

                    if (data.results && data.results[0]) {
                        const { lng, lat } = data.results[0].geometry.location;
                        setLocation({ lng, lat });

                        if (map.current) {
                            map.current.setCenter([lng, lat]);
                            new maptilersdk.Marker({ color: "#FF0000" })
                                .setLngLat([lng, lat])
                                .addTo(map.current);
                        }
                    }
                }

            } catch (error) {
                console.error("Error fetching geocode:", error);
            }
        };

        fetchGeocode();
    }, [branch]);

    useEffect(() => {
        if (map.current) return; // stops map from intializing more than once

        map.current = new maptilersdk.Map({
            container: mapContainer.current,
            style: maptilersdk.MapStyle.OPENSTREETMAP,
            center: [location.lng, location.lat],
            zoom: zoom
        });

        new maptilersdk.Marker({ color: "#FF0000" })
            .setLngLat([location.lng, location.lat])
            .addTo(map.current);
    }, [location.lng, location.lat, zoom]);

    return (
        <div className="map-wrap">
            <div ref={mapContainer} className="map" />
        </div>
    );
}

export default MapComponent;
window.mapInterop = {
    map: null,
    markers: [],

    initMap: function (lat, lon) {
        this.map = L.map('map').setView([lat, lon], 13);

        L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
            attribution: '&copy; OpenStreetMap'
        }).addTo(this.map);

        // User marker
        L.marker([lat, lon]).addTo(this.map)
            .bindPopup("You are here")
            .openPopup();
    },

    addHospitals: function (hospitals) {
        console.log("hospitals got: ", hospitals)

        hospitals.forEach(h => {
            const marker = L.marker([h.latitude, h.longitude])
                .addTo(this.map)
                .bindPopup(`<b>${h.name}</b><br>${h.address}`);

            this.markers.push(marker);
        });
    },

    // focusHospital: function (lat, lon) {
    //     this.map.setView([lat, lon], 16);
    // }
    focusHospital: function (lat, lon) {

        if (!this.map) return;

        // Remove previous focus marker
        if (this.focusedMarker) {
            this.map.removeLayer(this.focusedMarker);
        }

        // Custom big marker icon
        // const bigIcon = L.icon({
        //     iconUrl: '/images/marker-hospital.png', // YOU must add this
        //     iconSize: [48, 48],
        //     iconAnchor: [24, 48],
        //     popupAnchor: [0, -48]
        // });

        // // BIG visible circle marker
        // this.focusedMarker = L.marker([lat, lon], {
        //     // radius: 14,
        //     // color: '#d32f2f',
        //     // fillColor: '#ff5252',
        //     // fillOpacity: 0.9,
        //     // weight: 4
        //     icon: bigIcon,
        //     zIndexOffset: 1000
        // }).addTo(this.map);

        // Smooth zoom & pan
        this.map.flyTo([lat, lon], 16, {
            animate: true,
            duration: 0.8
        });
    },

    /* -----------------------------
   Set / update user location
----------------------------- */
    setUserLocation: function (lat, lon) {

        if (!this.map) return;

        // Remove previous user marker
        if (this.userMarker) {
            this.map.removeLayer(this.userMarker);
        }

        // User location icon
        // const userIcon = L.divIcon({
        //     className: 'user-location-marker',
        //     html: '<div class="pulse"></div>',
        //     iconSize: [16, 16],
        //     iconAnchor: [8, 8]
        // });

        // Add new marker
        this.userMarker = L.marker([lat, lon]).addTo(this.map)
            .bindPopup("You are here")
            .openPopup();

        // Smooth recenter
        this.map.flyTo([lat, lon], 13, {
            animate: true,
            duration: 0.6
        });
    }


};

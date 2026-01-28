// window.locationInterop = {

//     searchAddress: async function (query) {
//         const url =
//             `https://nominatim.openstreetmap.org/search` +
//             `?format=json` +
//             `&q=${encodeURIComponent(query)}` +
//             `&countrycodes=ca` +        // 🇨🇦 Canada only
//             `&addressdetails=1` +
//             `&limit=5`;
//         const res = await fetch(url, {
//             headers: {
//                 "Accept": "application/json"
//             }
//         });

//         const data = await res.json();

//         return data.map(x => ({
//             displayName: x.display_name,
//             lat: parseFloat(x.lat),
//             lon: parseFloat(x.lon)
//         }));
//     },

//     getCurrentLocation: function () {
//         return new Promise((resolve, reject) => {
//             if (!navigator.geolocation) {
//                 reject();
//                 return;
//             }

//             navigator.geolocation.getCurrentPosition(
//                 pos => resolve({
//                     lat: pos.coords.latitude,
//                     lng: pos.coords.longitude
//                 }),
//                 err => reject(err)
//             );
//         });
//     }
// };
window.locationInterop = {
    searchAddress: async function (query) {
        const res = await fetch(`/api/geo/search?q=${encodeURIComponent(query)}`);
        if (!res.ok) throw new Error("Geo search failed");
        const data = await res.json();

        return data.map(x => ({
            display_name: x.display_name,
            lat: parseFloat(x.lat),   // ✅ FIX
            lon: parseFloat(x.lon)    // ✅ FIX
        }));
    },
    getCurrentLocation: function () {
        return new Promise((resolve, reject) => {
            if (!navigator.geolocation) {
                reject("Geolocation not supported");
                return;
            }

            navigator.geolocation.getCurrentPosition(
                pos => resolve({
                    lat: pos.coords.latitude,
                    lng: pos.coords.longitude
                }),
                err => reject(err)
            );
        });
    }
};

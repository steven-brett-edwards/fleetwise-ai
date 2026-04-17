export const environment = {
    production: true,
    // Absolute URL of the deployed API on Render. The frontend static site is
    // on a different origin (fleetwise-frontend.onrender.com), so we can't use
    // a relative path -- CORS is enabled on the API to permit this origin.
    apiUrl: 'https://fleetwise-api.onrender.com/api',
};

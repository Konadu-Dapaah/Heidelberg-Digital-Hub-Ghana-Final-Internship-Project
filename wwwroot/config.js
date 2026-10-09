// Frontend settings shared by every page.
const IS_LOCAL = ["localhost", "127.0.0.1"].includes(window.location.hostname);

window.APP_CONFIG = {
  // Where the API lives.
  //  - Running locally, the page and API share a host, so this is "" (same origin).
  //  - Deployed on Vercel, this is your Render URL (no trailing slash).
  API_BASE: IS_LOCAL ? "" : "https://commute360-api.onrender.com",

  // The Maps JavaScript API key is visible to anyone who opens the page,
  // so RESTRICT it in Google Cloud Console (HTTP referrers + Maps JavaScript API only).
  GOOGLE_MAPS_API_KEY: "AIzaSyDqWs5onXNVQYKJ1KtQHHOcgCGzmWT25V8",

  MAP_ID: "184cc814228b9f1ec0242b5f",

  // Only used until the route's stops load
  DEFAULT_CENTER: { lat: 6.6745, lng: -1.5716 }
};
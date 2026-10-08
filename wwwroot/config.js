// Frontend settings shared by track.html and driver.html.
// The Maps JavaScript API key is visible to anyone who opens the page,
// so RESTRICT it in Google Cloud Console (HTTP referrers + Maps JavaScript API only).
window.APP_CONFIG = {
  GOOGLE_MAPS_API_KEY: "AIzaSyDqWs5onXNVQYKJ1KtQHHOcgCGzmWT25V8",
  // "DEMO_MAP_ID" is fine for development. Create your own Map ID in
  // Google Cloud Console > Map Management for production styling.
  MAP_ID: "184cc814228b9f1ec0242b5f",
  // Only used until the route's stops load
  DEFAULT_CENTER: { lat: 6.6745, lng: -1.5716 }
};
// Shared helpers for every page: token, API calls and role-based redirects.
(function () {
  const apiBase = window.location.port ? "" : "http://localhost:5291";
  const API_BASE_URL = "https://7t2sq1ln-5291.uks1.devtunnels.ms";
  // Role destination map
  const HOME = {
    Admin: "/admin-dashboard.html",
    Driver: "/driver.html",
    Staff: "/staff-dashboard.html"
  };

  function token() {
    return localStorage.getItem("token");
  }

  function logout() {
    localStorage.removeItem("token");
    window.location.href = "/login.html";
  }

  async function api(path, options = {}) {
    const res = await fetch(apiBase + path, {
      ...options,
      headers: {
        "Content-Type": "application/json",
        Authorization: "Bearer " + token(),
        ...(options.headers || {})
      }
    });

    if (res.status === 401) {
      localStorage.removeItem("token");
      window.location.href = "/login.html";
      throw new Error("Unauthorized");
    }
    return res;
  }

  async function me() {
    const res = await api("/api/me");
    if (!res.ok) throw new Error("Could not load your profile.");
    return res.json();
  }

  function homeFor(role) {
    return HOME[role] || HOME.Staff;
  }

  // Call this right after a successful login or registration.
  async function goHome() {
    const user = await me();
    window.location.href = homeFor(user.role);
  }

  // Enforces authorization per page
  async function requireRole(roles) {
    if (!token()) {
      window.location.href = "/login.html";
      throw new Error("Unauthorized");
    }
    const user = await me();
    if (!roles.includes(user.role)) {
      window.location.href = homeFor(user.role);
      throw new Error("Wrong role");
    }
    return user;
  }

  window.Auth = { apiBase, token, api, me, homeFor, goHome, requireRole, logout };
})();
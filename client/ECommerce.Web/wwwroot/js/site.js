// ===== ShopNow - Global JavaScript =====
var API_BASE = "http://localhost:5253";

// ===== Auth UI Management =====
function updateAuthUI() {
  var token = localStorage.getItem("jwt_token");
  if (token) {
    $(".auth-required").show();
    $(".no-auth").hide();
    var name =
      localStorage.getItem("user_name") ||
      localStorage.getItem("user_email") ||
      "User";
    $("#nav-username").text(name);
  } else {
    $(".auth-required").hide();
    $(".no-auth").show();
  }
}

function logout() {
  Swal.fire({
    title: "Sign Out?",
    text: "Are you sure you want to sign out?",
    icon: "question",
    showCancelButton: true,
    confirmButtonText: '<i class="fas fa-sign-out-alt me-1"></i>Yes, sign out',
    cancelButtonText: "Cancel",
  }).then(function (result) {
    if (result.isConfirmed) {
      localStorage.removeItem("jwt_token");
      localStorage.removeItem("user_email");
      localStorage.removeItem("user_name");
      Swal.fire({
        title: "Signed Out",
        text: "You have been signed out successfully.",
        icon: "success",
        timer: 1200,
        showConfirmButton: false,
      }).then(function () {
        window.location.href = "/";
      });
    }
  });
}

function togglePassword(inputId, btn) {
  var input = document.getElementById(inputId);
  var icon = btn.querySelector("i");
  if (input.type === "password") {
    input.type = "text";
    icon.classList.replace("fa-eye", "fa-eye-slash");
  } else {
    input.type = "password";
    icon.classList.replace("fa-eye-slash", "fa-eye");
  }
}

// ===== AJAX Helpers =====
function getAuthHeaders() {
  var headers = { "Content-Type": "application/json" };
  var token = localStorage.getItem("jwt_token");
  if (token) {
    headers["Authorization"] = "Bearer " + token;
  }
  return headers;
}

function apiGet(url, onSuccess, onError) {
  $.ajax({
    url: API_BASE + url,
    method: "GET",
    headers: getAuthHeaders(),
    success: function (res) {
      if (res.isSuccess !== undefined) {
        if (res.isSuccess) {
          onSuccess(res.data);
        } else {
          if (onError) onError(res);
          else toastError(res.error || "Request failed.");
        }
      } else {
        onSuccess(res);
      }
    },
    error: function (xhr) {
      if (onError) onError(xhr);
      else handleApiError(xhr);
    },
  });
}

function apiRequest(method, url, data, onSuccess, onError) {
  var opts = {
    url: API_BASE + url,
    method: method,
    headers: getAuthHeaders(),
    success: function (res) {
      if (res.isSuccess !== undefined) {
        if (res.isSuccess) {
          onSuccess(res.data);
        } else {
          if (onError) onError(res);
          else toastError(res.error || "Request failed.");
        }
      } else {
        onSuccess(res);
      }
    },
    error: function (xhr) {
      if (onError) onError(xhr);
      else handleApiError(xhr);
    },
  };
  if (data) {
    opts.contentType = "application/json";
    opts.data = JSON.stringify(data);
  }
  $.ajax(opts);
}

function handleApiError(xhr) {
  var msg = "An error occurred.";
  if (xhr.status === 401) {
    msg = "Please login to continue.";
    localStorage.removeItem("jwt_token");
    updateAuthUI();
  } else if (xhr.status === 400 && xhr.responseJSON) {
    if (xhr.responseJSON.errors) {
      var errs = [];
      for (var key in xhr.responseJSON.errors) {
        errs.push(xhr.responseJSON.errors[key].join(", "));
      }
      msg = errs.join("\n");
    } else {
      msg = xhr.responseJSON.error || xhr.responseJSON.title || msg;
    }
  } else if (xhr.responseJSON) {
    msg = xhr.responseJSON.error || xhr.responseJSON.title || msg;
  }
  toastError(msg);
}

// ===== Toast Helpers =====
function toastSuccess(msg) {
  Swal.mixin({
    toast: true,
    position: "top-end",
    showConfirmButton: false,
    timer: 3000,
    timerProgressBar: true,
  }).fire({ icon: "success", title: msg });
}

function toastError(msg) {
  Swal.mixin({
    toast: true,
    position: "top-end",
    showConfirmButton: false,
    timer: 4000,
    timerProgressBar: true,
  }).fire({ icon: "error", title: msg });
}

// ===== HTML Escape =====
function escapeHtml(text) {
  if (!text) return "";
  var div = document.createElement("div");
  div.appendChild(document.createTextNode(text));
  return div.innerHTML;
}

// ===== Route Protection =====
function checkRouteAccess() {
  var path = window.location.pathname.toLowerCase();
  var token = localStorage.getItem("jwt_token");
  if (!token && path.startsWith("/admin")) {
    window.location.href =
      "/Account/Login?returnUrl=" +
      encodeURIComponent(window.location.pathname);
    return false;
  }
  return true;
}

// ===== Init =====
$(function () {
  updateAuthUI();
  checkRouteAccess();

  // Active nav link
  var path = window.location.pathname.toLowerCase();
  $(".navbar-nav .nav-link").each(function () {
    var href = $(this).attr("href");
    if (href && href.toLowerCase() === path) {
      $(this).addClass("active");
    } else if (path === "/" && href === "/") {
      $(this).addClass("active");
    }
  });
});

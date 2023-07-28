export const localStorageService = {
    getToken: function () {
        return localStorage.getItem("token");
    },
    setToken: function (token) {
        localStorage.setItem("token", token);
    },

    getRefreshToken: function () {
        return localStorage.getItem("refreshToken");
    },
    setRefreshToken: function (token) {
        localStorage.setItem("refreshToken", token);
    },
};

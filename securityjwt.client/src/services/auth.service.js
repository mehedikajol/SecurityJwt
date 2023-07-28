import http from "./../global/http";

const route = "/auth";

export const authService = {
    login: function (data) {
        return http.post(route + "/login", data);
    },

    register: function (data) {
        return http.post(route + "/register", data);
    },
};

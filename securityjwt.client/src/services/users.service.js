import http from "./../global/http";

const route = "/users";

export const usersService = {
    getUser: function () {
        return http.get(route);
    },
};

import axios from "axios";
import { localStorageService } from "../services/localStorage.service";

const server = "https://localhost:7077";

const http = axios.create({
    baseURL: `${server}/api`,
    headers: {
        "Content-Type": "application/json",
    },
});

http.interceptors.request.use(
    (config) => {
        const token = localStorage.getItem("token");
        config.headers["Authorization"] = `Bearer ${token}`;
        return config;
    },
    (error) => {
        return Promise.reject(error);
    }
);

http.interceptors.response.use(
    (fullFilled) => {
        return fullFilled;
    },
    async (error) => {
        const originalConfig = error.config;

        if (originalConfig.url !== "/auth/login" && error.response) {
            if (error.response.status === 401 && !originalConfig._retry) {
                originalConfig._retry = true;

                try {
                    const token = localStorage.getItem("token");
                    const reToken = localStorage.getItem("refreshToken");
                    const rs = await http.post("/auth/refreshToken", {
                        jwtToken: token,
                        refreshToken: reToken,
                    });

                    const { jwtToken, refreshToken } = rs.data;
                    console.log(rs.data);
                    localStorageService.setToken(jwtToken);
                    localStorageService.setRefreshToken(refreshToken);
                } catch (_error) {
                    return Promise.reject(_error);
                }
            }
        }

        console.log(error);
    }
);

export default http;

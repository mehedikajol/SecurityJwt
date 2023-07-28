import { useState } from "react";
import { Link } from "react-router-dom";
import { authService } from "../services/auth.service";
import { localStorageService } from "../services/localStorage.service";

const Login = () => {
    const [data, setData] = useState([]);

    const handleInputChange = (event) => {
        const { name, value } = event.target;
        setData({ ...data, [name]: value });
    };

    const handleSubmit = (event) => {
        event.preventDefault();
        authService
            .login(data)
            .then((response) => {
                if (response.data.isSuccess) {
                    localStorageService.setToken(response.data.jwtToken);
                    localStorageService.setRefreshToken(
                        response.data.refreshToken
                    );
                } else {
                    alert(response.data.errorMessage);
                }
            })
            .catch((err) => {
                console.log(err);
            });
        //console.log(data);
    };
    /* response
        errorMessage: ""
        isSuccess: true
        jwtToken: "ERTTJu9c"
        refreshToken: "N93-9e36-5ef6abe6ed51"
    */

    return (
        <section className="bg-[#F4F7FF] py-20">
            <div className="container">
                <div className="flex flex-wrap -mx-4">
                    <div className="w-full px-4">
                        <div className="max-w-[525px] mx-auto text-center bg-white rounded-lg relative overflow-hidden py-16 px-10 sm:px-12 md:px-[60px]">
                            <form onSubmit={handleSubmit}>
                                <div className="mb-4">
                                    <input
                                        type="email"
                                        name="email"
                                        onChange={handleInputChange}
                                        placeholder="Email"
                                        className="w-full rounded-md border bordder-[#E9EDF4] py-3 px-5 bg-[#FCFDFE] text-base text-body-color placeholder-[#ACB6BE] outline-none focus-visible:shadow-none focus:border-primary"
                                    />
                                </div>
                                <div className="mb-4">
                                    <input
                                        type="password"
                                        name="password"
                                        onChange={handleInputChange}
                                        placeholder="Password"
                                        className="w-full rounded-md border bordder-[#E9EDF4] py-3 px-5 bg-[#FCFDFE] text-base text-body-color placeholder-[#ACB6BE] outline-none focus-visible:shadow-none focus:border-primary"
                                    />
                                </div>
                                <div className="mb-8">
                                    <input
                                        type="submit"
                                        value="Login"
                                        className="w-full rounded-md border bordder-primary py-3 px-5 bg-primary text-base text-white cursor-pointer hover:bg-opacity-90 transition bg-red-600"
                                    />
                                </div>
                            </form>
                            <a
                                href="/"
                                className="text-base inline-block mb-2 text-[#adadad hover:underline hover:text-primary"
                            >
                                Forget Password?
                            </a>
                            <p className="text-base text-[#adadad]">
                                Not a member yet?
                                <Link
                                    to={"/register"}
                                    className="text-primary hover:underline"
                                >
                                    Register
                                </Link>
                            </p>
                        </div>
                    </div>
                </div>
            </div>
        </section>
    );
};

export default Login;

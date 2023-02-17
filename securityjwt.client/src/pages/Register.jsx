import { useState } from "react";
import { Link } from "react-router-dom";
import { authService } from "../services/auth.service";
import { localStorageService } from "../services/localStorage.service";

const Register = () => {
    const [data, setData] = useState([]);

    const handleInputChange = (event) => {
        const { name, value } = event.target;
        setData({ ...data, [name]: value });
    };

    const handleSubmit = (event) => {
        event.preventDefault();
        authService
            .register(data)
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
        <section className="bg-[#F4F7FF]">
            <div className="container">
                <div className="flex flex-wrap -mx-4">
                    <div className="w-full px-4">
                        <div className="max-w-[525px] mx-auto text-center bg-white rounded-lg relative overflow-hidden py-16 px-10 sm:px-12 md:px-[60px]">
                            <form onSubmit={handleSubmit}>
                                <div className="mb-4">
                                    <input
                                        type="text"
                                        name="firstName"
                                        onChange={handleInputChange}
                                        placeholder="First Name"
                                        className="w-full rounded-md border bordder-[#E9EDF4] py-3 px-5 bg-[#FCFDFE] text-base text-body-color placeholder-[#ACB6BE] outline-none focus-visible:shadow-none focus:border-primary"
                                    />
                                </div>
                                <div className="mb-4">
                                    <input
                                        type="text"
                                        name="lastName"
                                        onChange={handleInputChange}
                                        placeholder="Last Name"
                                        className="w-full rounded-md border bordder-[#E9EDF4] py-3 px-5 bg-[#FCFDFE] text-base text-body-color placeholder-[#ACB6BE] outline-none focus-visible:shadow-none focus:border-primary"
                                    />
                                </div>
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
                                <div className="mb-4">
                                    <input
                                        type="password"
                                        name="confirmPassword"
                                        onChange={handleInputChange}
                                        placeholder="Confirm Password"
                                        className="w-full rounded-md border bordder-[#E9EDF4] py-3 px-5 bg-[#FCFDFE] text-base text-body-color placeholder-[#ACB6BE] outline-none focus-visible:shadow-none focus:border-primary"
                                    />
                                </div>
                                <div className="mb-8">
                                    <input
                                        type="submit"
                                        value="Register"
                                        className="w-full rounded-md border bordder-primary py-3 px-5 bg-primary text-base text-white cursor-pointer hover:bg-opacity-90 transition bg-red-600"
                                    />
                                </div>
                            </form>
                            <p className="text-base text-[#adadad]">
                                Already a member?
                                <Link
                                    to={"/login"}
                                    className="text-primary hover:underline"
                                >
                                    Login
                                </Link>
                            </p>
                        </div>
                    </div>
                </div>
            </div>
        </section>
    );
};

export default Register;

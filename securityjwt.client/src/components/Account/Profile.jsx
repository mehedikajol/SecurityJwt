import { useEffect, useState } from "react";
import { usersService } from "../../services/users.service";

const Profile = () => {
    const [users, setUsers] = useState([]);

    useEffect(() => {
        usersService
            .getUser()
            .then((response) => {
                setUsers(response.data);
            })
            .catch((err) => console.log(err));
    }, []);

    return users.map((user) => {
        return (
            <>
                <div className="mb-8" key={user.email}>
                    <div className="grid md:grid-cols-2 lg:grid-cols-3 gap-5 mb-2">
                        <h2>First Name: {user.firstName}</h2>
                        <h2>Last Name: {user.lastName}</h2>
                        <h3>Email: {user.email}</h3>
                        <h4>Phone number: {user.phoneNumber}</h4>
                    </div>
                </div>
            </>
        );
    });
};

export default Profile;

import { Route, Routes } from "react-router-dom";
import Layout from "./components/Layout";
import Account from "./pages/Account";
import Billing from "./pages/Billing";
import Edit from "./pages/Edit";
import Home from "./pages/Home";
import Settings from "./pages/Settings";

function App() {
    return (
        <Layout>
            <Routes>
                <Route path="/" element={<Home />} />
                <Route path="account" element={<Account />} />
                <Route path="billing" element={<Billing />} />
                <Route path="edit" element={<Edit />} />
                <Route path="settings" element={<Settings />} />
            </Routes>
        </Layout>
    );
}

export default App;

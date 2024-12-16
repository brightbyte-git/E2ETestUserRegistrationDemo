import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import logo from './logo.svg';
import NavBar from "./components/layout/Navbar";
import Login from "./components/authentication/Login";
import Registration from "./components/authentication/Registration";
import { useUser } from "./components/context/UserContext";
import './App.css';
import Dashboard from "./components/dashboard/Dashboard";
import PrivateRoute from "./components/authentication/PrivateRoute";

function App() {

  const { userData, setUserData } = useUser();


  return (
    <div className="h-screen w-full bg-neutral-900 text-neutral-50">
    <Router>
        <NavBar /> {/* Display NavBar on all pages */}
        <Routes>
            <Route path="/login" element={<Login setUserData={setUserData} />} />
            <Route path="/register" element={<Registration />} /> {/* Add registration route */}
            <Route path="/" element={<PrivateRoute isAuthenticated={userData.token} />}>
                <Route path="dashboard" element={<Dashboard tenantId={userData.tenantId} />} />
            </Route>
        </Routes>
    </Router>
</div>
  );

}

export default App;

import {useContext} from "react";
import {LoginContext} from "../App";
import {useLocation, Navigate} from "react-router-dom";

export default function EnsureLoggedIn({ children }: { children: JSX.Element }) {
    let loginContext = useContext(LoginContext);
    let location = useLocation();

    if (loginContext.username === null)
        return <Navigate to="/login" state={{ from: location }} replace />;

    return children;
}

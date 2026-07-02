import type { ReactElement } from "react";
import { useSelector } from "react-redux";
import { Outlet, useNavigate } from "react-router-dom";

export default function ProtectedRoutes(): ReactElement {
    const user = useSelector((state: any) => state.auth.user);
    const navigate = useNavigate();
    if (user == null) {
        navigate("/login");
    }
    return (
        <Outlet />
    )
}
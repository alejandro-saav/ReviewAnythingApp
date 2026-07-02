import { useSelector } from "react-redux";
import { Navigate, Outlet } from "react-router-dom";

export default function ProtectedRoute() {
    const user = useSelector((state: any) => state.auth.user);

    return user ? (
        <Outlet />
    ) : (
        <Navigate to="login" state={{ from: location.pathname }} replace />
    )
}
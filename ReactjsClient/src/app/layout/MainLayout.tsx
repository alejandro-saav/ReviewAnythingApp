import type { ReactElement } from "react";
import Nav from "./Nav";
import { Outlet } from "react-router-dom";
import Footer from "./Footer";

export default function MainLayout(): ReactElement {
    return (
        <>
            <Nav />

            <main style={{ background: "#222" }}>
                <Outlet />
            </main>

            <Footer />
        </>
    );
}
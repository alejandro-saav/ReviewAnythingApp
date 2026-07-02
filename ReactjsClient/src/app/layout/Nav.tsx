import { useState, type ReactElement } from "react";
import styles from "./Nav.module.css";
import { useDispatch, useSelector } from "react-redux";
import { type UserInformation } from "../../types/AuthTypes";
import { isNullOrWhiteSpace } from "../../utils/helperFunctions";
import { Link, NavLink, useNavigate } from "react-router-dom";
import { Logout } from "../../services/AuthService";
import { setUser } from "../../pages/Auth/authSlice";

export default function Nav(): ReactElement {
    const [showModal, setShowModal] = useState<boolean>(false);
    const user: UserInformation | null = useSelector((state: any) => state.auth.user);
    const navigate = useNavigate();
    const dispatch = useDispatch();

    async function HandleLogout(): Promise<void> {
        const logoutResponse = await Logout();
        setShowModal(false);
        if (!logoutResponse) console.log("Something went wrong while trying to log out the user.");
        dispatch(setUser(null));
        navigate("/");
    }
    return (
        <nav className={styles.navbar}>
            <div className={styles.navContainer}>
                <Link to="/" className={styles.logo}>ReviewAnything</Link>
                <div className={`${styles.hamburgerIcon}`} onClick={() => setShowModal(prev => !prev)}>
                    <svg
                        xmlns="http://www.w3.org/2000/svg"
                        width="24"
                        height="24"
                        fill="currentColor"
                        viewBox="0 0 24 24"
                    >
                        <path d="M3 6h18M3 12h18M3 18h18" stroke="currentColor" strokeWidth="2" strokeLinecap="round" />
                    </svg>
                </div>
                <ul className={`${styles.navLinks} ${showModal ? styles.open : ""}`}>
                    <li onClick={() => setShowModal(false)}><NavLink className={({ isActive }) =>
                        isActive ? styles.active : ""
                    } to="/">Home</NavLink></li>
                    <li onClick={() => setShowModal(false)}><NavLink className={({ isActive }) =>
                        isActive ? styles.active : ""
                    } to="/explore">Explore</NavLink></li>
                    <li onClick={() => setShowModal(false)}><NavLink className={({ isActive }) =>
                        isActive ? styles.active : ""
                    } to="/explorecategories">Categories</NavLink></li>
                    <li onClick={() => setShowModal(false)}><NavLink className={({ isActive }) =>
                        isActive ? styles.active : ""
                    } to="/write">Write Review</NavLink></li>
                    <li onClick={() => setShowModal(false)} className={styles.profile}>
                        {user ? !isNullOrWhiteSpace(user?.profileImage) ?
                            < img id="profile-img" src={user?.profileImage ?? ""} alt="profile-image" className={styles.profileImage} onClick={() => setShowModal(prev => !prev)} />
                            : (
                                <div className={styles.svgContainer} onClick={() => setShowModal(prev => !prev)}>
                                    <svg xmlns="http://www.w3.org/2000/svg" width="30" height="30" viewBox="0 0 24 24"
                                        fill="none">
                                        <circle cx="12" cy="12" r="11" stroke="#FE5D26" strokeWidth="2" fill="none" />
                                        <path
                                            d="M12 12c2.21 0 4-1.79 4-4s-1.79-4-4-4-4 1.79-4 4 1.79 4 4 4zm0 2c-2.67 0-8 1.34-8 4v2h16v-2c0-2.66-5.33-4-8-4z"
                                            fill="#FE5D26" />
                                    </svg>
                                </div>
                            )
                            :
                            <NavLink className={({ isActive }) =>
                                isActive ? styles.active : ""
                            } to="/login">Login</NavLink>
                        }

                        {showModal && user != null &&
                            <>
                                <div className={styles.modalOverlay} onClick={() => setShowModal(false)}></div>
                                <div className={styles.navModal}>
                                    <ul className={styles.navModalList}>
                                        <li onClick={() => setShowModal(false)}><NavLink className={({ isActive }) =>
                                            isActive ? styles.active : ""
                                        } to="/edit-profile">
                                            <svg aria-hidden="true" focusable="false"
                                                className="octicon octicon-person Octicon-sc-9kayk9-0" viewBox="0 0 16 16"
                                                width="16" height="16" fill="currentColor" display="inline-block"
                                                overflow="visible" style={{ verticalAlign: "text-bottom" }}>
                                                <path
                                                    d="M10.561 8.073a6.005 6.005 0 0 1 3.432 5.142.75.75 0 1 1-1.498.07 4.5 4.5 0 0 0-8.99 0 .75.75 0 0 1-1.498-.07 6.004 6.004 0 0 1 3.431-5.142 3.999 3.999 0 1 1 5.123 0ZM10.5 5a2.5 2.5 0 1 0-5 0 2.5 2.5 0 0 0 5 0Z"></path>
                                            </svg>
                                            Your profile
                                        </NavLink></li>
                                        <li onClick={() => setShowModal(false)}><NavLink className={({ isActive }) =>
                                            isActive ? styles.active : ""
                                        } to="/myreviews">
                                            <svg aria-hidden="true" focusable="false" className="octicon octicon-book"
                                                viewBox="0 0 16 16" width="16" height="16" fill="currentColor"
                                                style={{ verticalAlign: "text-bottom" }}>
                                                <path
                                                    d="M0 1.75A.75.75 0 0 1 .75 1h4.253c1.227 0 2.317.59 3 1.501A3.743 3.743 0 0 1 11.006 1h4.245a.75.75 0 0 1 .75.75v10.5a.75.75 0 0 1-.75.75h-4.507a2.25 2.25 0 0 0-1.591.659l-.622.621a.75.75 0 0 1-1.06 0l-.622-.621A2.25 2.25 0 0 0 5.258 13H.75a.75.75 0 0 1-.75-.75Zm7.251 10.324.004-5.073-.002-2.253A2.25 2.25 0 0 0 5.003 2.5H1.5v9h3.757a3.75 3.75 0 0 1 1.994.574ZM8.755 4.75l-.004 7.322a3.752 3.752 0 0 1 1.992-.572H14.5v-9h-3.495a2.25 2.25 0 0 0-2.25 2.25Z"></path>
                                            </svg>
                                            My reviews</NavLink></li>
                                        <li onClick={() => setShowModal(false)} > <NavLink className={({ isActive }) =>
                                            isActive ? styles.active : ""
                                        } to="/likedreviews">
                                            <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor"
                                                className="bi bi-hand-thumbs-up" viewBox="0 0 16 16">
                                                <path
                                                    d="M8.864.046C7.908-.193 7.02.53 6.956 1.466c-.072 1.051-.23 2.016-.428 2.59-.125.36-.479 1.013-1.04 1.639-.557.623-1.282 1.178-2.131 1.41C2.685 7.288 2 7.87 2 8.72v4.001c0 .845.682 1.464 1.448 1.545 1.07.114 1.564.415 2.068.723l.048.03c.272.165.578.348.97.484.397.136.861.217 1.466.217h3.5c.937 0 1.599-.477 1.934-1.064a1.86 1.86 0 0 0 .254-.912c0-.152-.023-.312-.077-.464.201-.263.38-.578.488-.901.11-.33.172-.762.004-1.149.069-.13.12-.269.159-.403.077-.27.113-.568.113-.857 0-.288-.036-.585-.113-.856a2 2 0 0 0-.138-.362 1.9 1.9 0 0 0 .234-1.734c-.206-.592-.682-1.1-1.2-1.272-.847-.282-1.803-.276-2.516-.211a10 10 0 0 0-.443.05 9.4 9.4 0 0 0-.062-4.509A1.38 1.38 0 0 0 9.125.111zM11.5 14.721H8c-.51 0-.863-.069-1.14-.164-.281-.097-.506-.228-.776-.393l-.04-.024c-.555-.339-1.198-.731-2.49-.868-.333-.036-.554-.29-.554-.55V8.72c0-.254.226-.543.62-.65 1.095-.3 1.977-.996 2.614-1.708.635-.71 1.064-1.475 1.238-1.978.243-.7.407-1.768.482-2.85.025-.362.36-.594.667-.518l.262.066c.16.04.258.143.288.255a8.34 8.34 0 0 1-.145 4.725.5.5 0 0 0 .595.644l.003-.001.014-.003.058-.014a9 9 0 0 1 1.036-.157c.663-.06 1.457-.054 2.11.164.175.058.45.3.57.65.107.308.087.67-.266 1.022l-.353.353.353.354c.043.043.105.141.154.315.048.167.075.37.075.581 0 .212-.027.414-.075.582-.05.174-.111.272-.154.315l-.353.353.353.354c.047.047.109.177.005.488a2.2 2.2 0 0 1-.505.805l-.353.353.353.354c.006.005.041.05.041.17a.9.9 0 0 1-.121.416c-.165.288-.503.56-1.066.56z" />
                                            </svg>
                                            Liked reviews</NavLink></li >
                                        <li onClick={() => setShowModal(false)} > <NavLink className={({ isActive }) =>
                                            isActive ? styles.active : ""
                                        } to="/mycomments">
                                            <svg aria-hidden="true" focusable="false" className="octicon octicon-comment"
                                                viewBox="0 0 16 16" width="16" height="16" fill="currentColor"
                                                style={{ verticalAlign: "text-bottom" }}>
                                                <path
                                                    d="M1 2.75C1 1.784 1.784 1 2.75 1h10.5c.966 0 1.75.784 1.75 1.75v7.5A1.75 1.75 0 0 1 13.25 12H9.06l-2.573 2.573A1.458 1.458 0 0 1 4 13.543V12H2.75A1.75 1.75 0 0 1 1 10.25Zm1.75-.25a.25.25 0 0 0-.25.25v7.5c0 .138.112.25.25.25H5a.75.75 0 0 1 .75.75v2.19l2.72-2.72a.749.749 0 0 1 .53-.22h4.25a.25.25 0 0 0 .25-.25v-7.5a.25.25 0 0 0-.25-.25Z"></path>
                                            </svg>
                                            My comments</NavLink></li >
                                        <li onClick={HandleLogout} >
                                            <svg aria-hidden="true" focusable="false"
                                                className="octicon octicon-sign-out Octicon-sc-9kayk9-0" viewBox="0 0 16 16"
                                                width="16" height="16" fill="currentColor" display="inline-block"
                                                overflow="visible" style={{ verticalAlign: "text-bottom" }}>
                                                <path
                                                    d="M2 2.75C2 1.784 2.784 1 3.75 1h2.5a.75.75 0 0 1 0 1.5h-2.5a.25.25 0 0 0-.25.25v10.5c0 .138.112.25.25.25h2.5a.75.75 0 0 1 0 1.5h-2.5A1.75 1.75 0 0 1 2 13.25Zm10.44 4.5-1.97-1.97a.749.749 0 0 1 .326-1.275.749.749 0 0 1 .734.215l3.25 3.25a.75.75 0 0 1 0 1.06l-3.25 3.25a.749.749 0 0 1-1.275-.326.749.749 0 0 1 .215-.734l1.97-1.97H6.75a.75.75 0 0 1 0-1.5Z"></path>
                                            </svg>
                                            Sign out</li >
                                    </ul >
                                </div >
                            </>
                        }
                    </li >
                </ul >
            </div >
        </nav >
    )
}
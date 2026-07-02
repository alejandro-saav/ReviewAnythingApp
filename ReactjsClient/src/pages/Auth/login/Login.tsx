import { type ReactElement, useEffect, useState } from "react";
import type { LoginRequest } from "../../../types/AuthTypes";
import { LoginHandler, SignInWithGoogle } from "../../../services/AuthService";
import { Link, replace, useLocation, useNavigate, useSearchParams } from "react-router-dom";
import { useDispatch } from "react-redux";
import { setUser } from "../authSlice";
import GoogleBtn from "../../../components/GoogleBtn";
import styles from "./Login.module.css";

export default function Login(): ReactElement {
    const navigate = useNavigate();

    const [form, setForm] = useState<LoginRequest>({
        email: "",
        password: "",
        rememberMe: false
    });

    const [loading, setLoading] = useState<boolean>(false);
    const [searchParams] = useSearchParams();
    const googleCode: string | null = searchParams.get("code");
    const googleError: string | null = searchParams.get("error");
    const location = useLocation();
    const from = location.state?.from || "/";

    useEffect(() => {
        async function GoogleSignIn() {
            const googleResponse = await SignInWithGoogle(googleCode!);
            if (googleResponse) {
                dispatch(setUser(googleResponse));
                navigate(from, { replace: true });
            }
        }
        if (googleCode) {
            GoogleSignIn();
        }
    }, [])

    const [formError, setFormError] = useState<string | null>(null);

    function handleChange(e: React.ChangeEvent<HTMLInputElement>): void {
        const { name, value } = e.target;

        setForm(prev => ({
            ...prev,
            [name]: value
        }));
    }

    const dispatch = useDispatch();

    async function handleSubmit(e: React.FormEvent<HTMLFormElement>): Promise<void> {
        e.preventDefault();
        setLoading(true);
        setFormError(null);

        var response = await LoginHandler(form);
        if (response == null) {
            setFormError("Invalid username or password");
            setLoading(false);
            return;
        }
        dispatch(setUser(response));
        navigate(from, { replace: true });
    }
    return (
        <div className={styles.loginContainer}>
            <div className={styles.loginWrapper}>
                <div className={styles.loginPanel}>
                    <h2 className={styles.panelTitle}>Login</h2>
                    <form method="post" id="loginForm" onSubmit={handleSubmit}>
                        <div className={styles.formGroup}>
                            <label htmlFor="Email">Email</label>
                            <input type="text" id="Email" name="Email"
                                onChange={handleChange} className={styles.formInput}
                                placeholder="Email Address" />
                        </div>

                        <div className={styles.formGroup}>
                            <label htmlFor="Password">Password</label>
                            <input type="password" id="Password" name="Password" onChange={handleChange}
                                className={styles.formInput} placeholder="Password" />
                        </div>

                        <div className={styles.formOptions}>
                            <Link to="/forgot-password" className={styles.forgotLink}>Forgot password?</Link>
                        </div>

                        <button type="submit" className={styles.btnPrimary} id="submitLoginBtn" disabled={loading}>
                            {loading ?
                                <div className={styles.spinnerContainer} id="loadingSpinner">
                                    <span className={styles.spinner}></span>
                                    <span>Loading...</span>
                                </div>
                                :
                                <span id="loginBtnText">Login</span>
                            }
                        </button>

                        {formError &&
                            <div className={styles.errorMessage}>{formError}</div>
                        }
                    </form>

                    <div className={styles.needAccount}><span>Need an account? <Link to="/signup">Sign up</Link></span></div>

                    <div className={styles.divider}>or</div>
                    <GoogleBtn />
                    {googleError && <span className={styles.errorMessage}>Something went wrong while trying to sign you in with google. Please try again.</span>}
                </div>
            </div>
        </div>
    )
}
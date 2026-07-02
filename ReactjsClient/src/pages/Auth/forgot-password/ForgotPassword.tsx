import { useState, type ReactElement } from "react";
import { isNullOrWhiteSpace } from "../../../utils/helperFunctions";
import styles from './ForgotPassword.module.css';
import { type ForgotPasswordRequest } from "../../../types/AuthTypes";
import { ForgotPasswordHandler } from "../../../services/AuthService";
import { Link } from "react-router-dom";

export default function ForgotPassword(): ReactElement {
    const [success, setSuccess] = useState<boolean>(false);
    const [isLoading, setIsLoading] = useState<boolean>(false);
    const [errorMessage, setErrorMessage] = useState<string | null>(null);

    const [emailReq, setEmailReq] = useState<ForgotPasswordRequest>({
        email: ""
    });

    function handleChange(e: React.ChangeEvent<HTMLInputElement>): void {
        const { name, value } = e.target;

        setEmailReq(prev => ({
            ...prev,
            [name]: value
        }));
    }

    async function SubmitHandler() {
        setIsLoading(true);
        if (isNullOrWhiteSpace(emailReq.email)) {
            setIsLoading(false);
            setErrorMessage("Email is required.");
            return;
        }

        var response = await ForgotPasswordHandler(emailReq);
        if (response) {
            setSuccess(true);
        } else {
            setErrorMessage("Could not send reset link email, please try again.");
        }
        setIsLoading(false);
    }
    return (
        <>
            <div className={styles.forgotPasswordContainer}>

                <div className={styles.forgotPasswordWrapper}>
                    <div className={styles.logo}>
                        <h1>ReviewAnything</h1>
                    </div>

                    {success ?
                        <div className={styles.successMessage}>
                            A password reset link has been sent to your email address.
                        </div>

                        :

                        <>
                            <h2 className={styles.subtitle}>Reset Your Password</h2>
                            <p style={{ textAlign: "center", color: "#eee", marginBottom: "10px" }}>
                                Enter your email address and we'll send you a link to reset your password.
                            </p>

                            <form onSubmit={SubmitHandler}>
                                <div className={styles.formGroup}>
                                    <label htmlFor="email">Email Address</label>
                                    <div className={styles.inputWrapper}>
                                        <input type="email"
                                            placeholder="Email Address" className={styles.formInput} id="email" name="Email" onChange={handleChange} />
                                    </div>
                                </div>

                                <button type="submit" className={styles.btnPrimary} disabled={isLoading}>
                                    {isLoading
                                        ?
                                        <div className={styles.spinnerContainer}>
                                            <span className={styles.spinner}></span>
                                            <span>Sending...</span>
                                        </div>
                                        :
                                        <span>Send Reset Link</span>
                                    }
                                </button>
                                {!isNullOrWhiteSpace(errorMessage) &&
                                    <div className={styles.errorMessage}>{errorMessage}</div>
                                }
                            </form>

                            <div className={styles.backLink}>
                                <Link to="/login">← Back to Login</Link>
                            </div>
                        </>
                    }
                </div>
            </div >
        </>
    );
}
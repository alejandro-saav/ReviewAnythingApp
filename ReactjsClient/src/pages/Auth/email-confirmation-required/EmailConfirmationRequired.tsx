import type { ReactElement } from "react";
import styles from './EmailConfirmationRequired.module.css';
import { Link } from "react-router-dom";

export default function EmailConfirmationRequired(): ReactElement {
    function ResendEmailVerification() { }
    return (
        <div className={styles.confirmationContainer}>
            <div className={styles.confirmationCard}>
                <div className={styles.iconContainer}>
                    <div className={styles.emailIcon}>
                        <svg width="64" height="64" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
                            <path d="M4 4H20C21.1 4 22 4.9 22 6V18C22 19.1 21.1 20 20 20H4C2.9 20 2 19.1 2 18V6C2 4.9 2.9 4 4 4Z" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" />
                            <polyline points="22,6 12,13 2,6" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" />
                        </svg>
                    </div>
                </div>

                <h1 className={styles.title}>Check Your Email</h1>

                <p className={styles.message}>
                    Please go to the email address you registered with and confirm your email to complete the process.
                </p>

                <div className={styles.infoBox}>
                    <p className={styles.infoText}>
                        <strong>Didn't receive the email?</strong><br />
                        Check your spam folder or contact support if you need assistance.
                    </p>
                </div>

                <div className={styles.actionButtons}>
                    <button className={`${styles.btn} ${styles.btnPrimary}`} onClick={ResendEmailVerification}>
                        Resend Email
                    </button>
                    <Link to="/Login" className={`${styles.btn} ${styles.btnSecondary}`}>
                        Login
                    </Link>
                </div>
            </div>
        </div>
    );
}
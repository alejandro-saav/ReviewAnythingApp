import { useEffect, useState } from 'react';
import styles from './ConfirmEmail.module.css';
import { Link, useSearchParams } from 'react-router-dom';
import { isNullOrWhiteSpace } from '../../../utils/helperFunctions';
import { ConfirmEmailHandler } from '../../../services/AuthService';

export default function ConfirmEmail() {
    const [isSuccess, setIsSuccess] = useState<boolean>(false);
    const [isLoading, setIsLoading] = useState<boolean>(true);

    const [searchParams] = useSearchParams();
    const userId: string | null = searchParams.get("userId");
    const token: string | null = searchParams.get("token");

    useEffect(() => {
        if (isNullOrWhiteSpace(userId) || isNullOrWhiteSpace(token)) {
            setIsLoading(false);
            return;
        }
        async function ConfirmEmail() {
            const response: boolean = await ConfirmEmailHandler(userId!, token!);
            if (response) setIsSuccess(true);
            setIsLoading(false);
        }
        ConfirmEmail();
    }, [])

    function ResendConfirmationEmail() { }
    return (
        <>
            {isLoading ?
                <div>IS LOADING ---</div> :
                <div className={styles.confirmationContainer}>
                    <div className={styles.confirmationCard}>
                        <div className={styles.iconContainer}>
                            {isSuccess ? (
                                <div className={styles.successIcon}>
                                    <svg width="50" height="50" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
                                        <path d="M22 11.08V12C21.9988 14.1564 21.3005 16.2547 20.0093 17.9818C18.7182 19.7088 16.9033 20.9725 14.8354 21.5839C12.7674 22.1953 10.5573 22.1219 8.53447 21.3746C6.51168 20.6273 4.78465 19.2461 3.61096 17.4371C2.43727 15.628 1.87979 13.4905 2.02168 11.3363C2.16356 9.18203 2.99721 7.13214 4.39828 5.49883C5.79935 3.86553 7.69279 2.72636 9.79619 2.24223C11.8996 1.75809 14.1003 1.95185 16.07 2.79999" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" />
                                        <polyline points="22,4 12,14.01 9,11.01" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" />
                                    </svg>
                                </div>
                            ) : (
                                <div className={styles.errorIcon}>
                                    <svg width="64" height="64" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
                                        <circle cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="2" />
                                        <line x1="15" y1="9" x2="9" y2="15" stroke="currentColor" strokeWidth="2" />
                                        <line x1="9" y1="9" x2="15" y2="15" stroke="currentColor" strokeWidth="2" />
                                    </svg>
                                </div>
                            )}
                        </div>

                        {isSuccess ? (
                            <>
                                <h1 className={`${styles.title} ${styles.success}`}>Email Confirmed!</h1>

                                <p className={styles.message}>
                                    Great! Your email has been successfully confirmed. Your account is now fully activated and ready to use.
                                </p>

                                <div className={`${styles.infoBox} ${styles.successBox}`}>
                                    <p className={styles.infoText}>
                                        <strong>What's next?</strong><br />
                                        You can now log in and start using all the features of your account.
                                    </p>
                                </div>

                                <div className={styles.actionButtons}>
                                    <Link to="/login" className={`${styles.btn} ${styles.btnPrimary}`}>
                                        Back to Login Page
                                    </Link>
                                </div>
                            </>
                        ) : (
                            <>
                                <h1 className={`${styles.title} ${styles.error}`}>Email Confirmation Failed</h1>

                                <p className={styles.message}>
                                    Unfortunately, we couldn't confirm your email address. This might be due to an expired or invalid confirmation link.
                                </p>

                                <div className={`${styles.infoBox} ${styles.errorBox}`}>
                                    <p className={styles.infoText}>
                                        <strong>What can you do?</strong><br />
                                        Try requesting a new confirmation email or contact our support team for assistance.
                                    </p>
                                </div>

                                <div className={styles.actionButtons}>
                                    <button className={`${styles.btn} ${styles.btnPrimary}`} onClick={ResendConfirmationEmail}>
                                        Resend Confirmation
                                    </button>
                                    <Link to="/login" className={`${styles.btn} ${styles.btnTertiary}`}>
                                        Back to Login Page
                                    </Link>
                                </div>
                            </>
                        )}
                    </div>
                </div>
            }
        </>
    );
}
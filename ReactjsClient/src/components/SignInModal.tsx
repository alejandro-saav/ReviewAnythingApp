import type { ReactElement } from "react";
import styles from "./SignInModal.module.css";
import { Link } from "react-router-dom";

export default function SignInModal(): ReactElement {
    return (
        <div className={styles.modal}>
            <h3 className={styles.modalHeader}>Want to join the community?</h3>
            <h5 className={styles.modalContent}>Join with your account to continue</h5>
            <Link className={styles.modalLink} to={"/login"}>Sign in</Link>
        </div>
    )
}
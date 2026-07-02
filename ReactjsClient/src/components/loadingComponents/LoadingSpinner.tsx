import type { ReactElement } from "react";
import styles from "./LoadingSpinner.module.css"

export default function LoadingSpinner(): ReactElement {
    return (
        <div className={styles.spinnerContainer}>
            <span className={styles.spinner}></span>
            <span>Loading...</span>
        </div>
    )
}
import type { ReactElement, ReactNode } from "react";
import styles from "./ReviewGrid.module.css"

interface Props {
    children: ReactNode
}
export default function ReviewGrid({ children }: Props): ReactElement {
    return (
        <div className={styles.reviewsGrid}>
            {children}
        </div>
    )
}
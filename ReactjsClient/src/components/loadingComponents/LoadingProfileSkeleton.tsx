import type { ReactElement } from "react";
import styles from "./LoadingProfileSkeleton.module.css"

export default function LoadingProfileSkeleton(): ReactElement {
    return (
        <div className={styles.container}>
            <div className={styles.profileHeader}>
                <div className={`${styles.skeleton} ${styles.skeletonProfileImage}`}></div>

                <div className={styles.profileInfo}>
                    <div className={`${styles.skeleton} ${styles.skeletonName}`}></div>
                    <div className={`${styles.skeleton} ${styles.skeletonUsername}`}></div>
                    <div className={`${styles.skeleton} ${styles.skeletonMemberDate}`}></div>
                    <div className={`${styles.skeleton} ${styles.skeletonBio}`}></div>
                </div>
            </div>
        </div>
    )
}
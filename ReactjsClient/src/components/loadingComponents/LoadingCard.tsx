import type { ReactElement } from "react";
import styles from "./LoadingCard.module.css";

export default function LoadingCard(): ReactElement {
    return (
        <div className={styles.reviewCard}>
            <div className={styles.reviewAuthor}>
                <div className={`${styles.skeleton} ${styles.skeletonCircle} ${styles.skeletonProfile}`}></div>
                <div className={styles.authorInfo}>
                    <div className={`${styles.skeleton} ${styles.skeletonUsername}`}></div>
                    <div className={`${styles.skeleton} ${styles.skeletonBadge}`}></div>
                </div>
            </div>
            <div className={styles.helperContainer}>
                <div className={styles.reviewHeader}>
                    <div>
                        <div className={`${styles.skeleton} ${styles.skeletonTitle}`}></div>
                        <div className={`${styles.skeleton} ${styles.skeletonCategory}`}></div>
                    </div>
                </div>
                <div className={styles.reviewRating}>
                    <div className={`${styles.skeleton} ${styles.skeletonStar}`}></div>
                    <div className={`${styles.skeleton} ${styles.skeletonStar}`}></div>
                    <div className={`${styles.skeleton} ${styles.skeletonStar}`}></div>
                    <div className={`${styles.skeleton} ${styles.skeletonStar}`}></div>
                    <div className={`${styles.skeleton} ${styles.skeletonStar}`}></div>
                    <div className={`${styles.skeleton} ${styles.skeletonRatingText}`}></div>
                </div>
                <div className={styles.reviewContent}>
                    <div className={`${styles.skeleton} ${styles.skeletonContentLine}`}></div>
                    <div className={`${styles.skeleton} ${styles.skeletonContentLine}`}></div>
                    <div className={`${styles.skeleton} ${styles.skeletonContentLine}`}></div>
                </div>
                <div className={styles.reviewTags}>
                    <div className={styles.tagsContainer}>
                        <div className={`${styles.skeleton} ${styles.skeletonTag}`}></div>
                        <div className={`${styles.skeleton} ${styles.skeletonTag}`}></div>
                        <div className={`${styles.skeleton} ${styles.skeletonTag}`}></div>
                    </div>
                    <div className={styles.reviewActions}>
                        <div className={`${styles.skeleton} ${styles.skeletonAction}`}></div>
                        <div className={`${styles.skeleton} ${styles.skeletonAction}`}></div>
                    </div>
                </div>
            </div>
        </div>
    )
}
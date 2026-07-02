import type { ReactElement } from "react";
import type { LikedReviews } from "../types/ReviewTypes";
import { isNullOrWhiteSpace } from "../utils/helperFunctions";
import { Link } from "react-router-dom";
import styles from "./ReviewCard.module.css"
import ProfileIcon from "./ProfileIcon";

interface ReviewCardProp {
    review: LikedReviews
}

export default function ReviewCard({ review }: ReviewCardProp): ReactElement {
    return (
        <div className={styles.reviewCard} data-category="books" data-rating="5" data-tags="fantasy,epic,bestseller">
            <div className={styles.reviewAuthor}>
                <Link to={`/profile/${review.user?.userId ?? 0}`}>
                    {isNullOrWhiteSpace(review.user?.profileImage) ?
                        <ProfileIcon width={30} height={30} />
                        :
                        <img
                            src={review.user?.profileImage!}
                            alt="Profile" className={styles.profileImg} />
                    }
                </Link>
                <div className={styles.authorInfo}>
                    <Link className={styles.username} to={`/profile/${review.user?.userId}`}>{review.user?.userName ?? ""}</Link>
                    <div className={styles.authorBadge}>Followers: {review.creatorFollowers}</div>
                </div>
            </div>
            <div className={styles.helperContainer}>
                <div className={styles.reviewHeader}>
                    <div>
                        <Link className={styles.reviewTitle} to={`/review/${review.reviewId}`}>{review.title}</Link>
                        <div className={styles.reviewCategory}>{review.category}</div>
                    </div>
                </div>
                <div className={styles.reviewRating}>
                    {[...Array(5)].map((_, index) =>
                        <span key={index} className={`${styles.star} ${index < review.rating ? styles.filled : styles.empty}`}>★</span>
                    )}
                    <span className={styles.ratingText}>{review.rating} / 5 stars</span>
                </div>
                <div className={styles.reviewContent}>
                    {review.content}
                </div>
                <div className={styles.reviewTags}>
                    <div className={styles.tagsContainer}>
                        {review.tags.map((tag, index) =>
                            <span key={index} className={styles.reviewTag}>{tag}</span>
                        )}
                    </div>
                    <div className={styles.reviewActions}>
                        <button className={styles.actionBtn}>👍 {review.likes}</button>
                        <button className={styles.actionBtn}>💬 {review.numberOfComments}</button>
                    </div>
                </div>
            </div>
        </div>
    )
}
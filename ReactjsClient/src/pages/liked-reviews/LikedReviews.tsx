import styles from "./LikedReviews.module.css"
import { useEffect, useState, type ReactElement } from "react";
import { type Category, type LikedReviews } from "../../types/ReviewTypes";
import Pagination from "../../components/Pagination";
import { useLocation } from "react-router-dom";
import { FetchLikedReviews } from "../../services/ReviewService";
import LoadingCard from "../../components/loadingComponents/LoadingCard";
import ReviewCard from "../../components/ReviewCard";
import FilterReviews from "../../components/FilterReviews";
import ReviewGrid from "../../components/ReviewGrid";
import { getRatingAverageFromReviews } from "../../utils/helperFunctions";

export default function LikedReviews(): ReactElement {
    const [isLoading, setIsLoading] = useState<boolean>(true);
    const [errorFetchingReviews, setErrorFetchingReviews] = useState<string | null>(null);
    const [reviews, setReviews] = useState<LikedReviews[]>([]);

    const location = useLocation();
    const queryString = location.search;

    useEffect(() => {
        window.scrollTo({ top: 0, behavior: "smooth" });
        async function fetchData() {
            const reviewsResponse: LikedReviews[] | null = await FetchLikedReviews(queryString);
            if (reviewsResponse == null) {
                setErrorFetchingReviews("Something went wrong while trying to get the reviews you have liked, please try reloading the page.");
                return;
            };
            setReviews(reviewsResponse);
            setIsLoading(false);
        }
        fetchData();
    }, [location])
    return (
        <>
            <div className={styles.likereviewsContainer}>
                <div className={styles.likereviewsWrapper}>
                    <div className={styles.header}>
                        <h1>♥ Liked Reviews</h1>
                        <p>Reviews you've liked from the community</p>
                        <div className={styles.stats}>
                            <div className={styles.statItem}>
                                <div className={styles.statNumber}>{reviews.length}</div>
                                <div className={styles.statLabel}>Reviews Liked</div>
                            </div>
                            <div className={styles.statItem}>
                                <div className={styles.statNumber}>{new Set(reviews.map(review => review.user?.userName)).size}</div>
                                <div className={styles.statLabel}>Different Authors</div>
                            </div>
                            <div className={styles.statItem}>
                                <div className={styles.statNumber}>{reviews.length > 0 ? getRatingAverageFromReviews(reviews) : 0}</div>
                                <div className={styles.statLabel}>Avg Rating</div>
                            </div>
                        </div>
                    </div>
                    <div className={styles.contentContainer}>
                        <FilterReviews />
                        <ReviewGrid>
                            {isLoading ?
                                [...Array(9)].map((_, index) =>
                                    <LoadingCard key={index} />
                                ) :
                                reviews.map((review, index) =>
                                    <ReviewCard key={index} review={review} />
                                )
                            }
                        </ReviewGrid>
                    </div>

                    {reviews.length > 0 && <Pagination totalReviews={reviews[0].total} />}
                </div>
            </div>
        </>
    );
}
import { useEffect, useState, type ReactElement } from "react";
import Pagination from "../../components/Pagination";
import FilterReviews from "../../components/FilterReviews";
import ReviewCard from "../../components/ReviewCard";
import LoadingCard from "../../components/loadingComponents/LoadingCard";
import type { LikedReviews } from "../../types/ReviewTypes";
import { useLocation } from "react-router-dom";
import { FetchUserReviews } from "../../services/ReviewService";
import { getRatingAverageFromReviews } from "../../utils/helperFunctions";
import styles from "./MyReviews.module.css";
import ReviewGrid from "../../components/ReviewGrid";

export default function MyReviews(): ReactElement {
    const [isLoading, setIsLoading] = useState<boolean>(true);
    const [errorFetchingReviews, setErrorFetchingReviews] = useState<string | null>(null);
    const [myReviews, setMyReviews] = useState<LikedReviews[]>([]);

    const location = useLocation();
    const queryString = location.search;

    useEffect(() => {
        window.scrollTo({ top: 0, behavior: "smooth" });
        async function fetchData() {
            const reviewsResponse: LikedReviews[] | null = await FetchUserReviews(queryString);
            if (reviewsResponse == null) {
                setErrorFetchingReviews("Something went wrong while trying to get your reviews, please try reloading the page.");
                setIsLoading(false);
                return;
            };
            setMyReviews(reviewsResponse);
            setIsLoading(false);
        }
        fetchData();
    }, [location])
    return (
        <div className={styles.myreviewsContainer}>
            <div className={styles.myreviewsWrapper}>
                <div className={styles.header}>
                    <h1>My Reviews</h1>
                    <p>All your reviews in one place</p>
                    <div className={styles.stats}>
                        <div className={styles.statItem}>
                            <div className={styles.statNumber} id="totalReviews">{myReviews.length}</div>
                            <div className={styles.statLabel}>Total Reviews</div>
                        </div>
                        <div className={styles.statItem}>
                            <div className={styles.statNumber}
                                id="avgRating">{myReviews.length > 0 ? getRatingAverageFromReviews(myReviews) : 0}</div>
                            <div className={styles.statLabel}>Average Rating</div>
                        </div>
                        <div className={styles.statItem}>
                            <div className={styles.statNumber} id="totalLikes">{myReviews.length > 0 ? myReviews.reduce((accumulator, next) => accumulator + next.likes, 0) : 0}</div>
                            <div className={styles.statLabel}>Total Likes</div>
                        </div>
                    </div>
                </div>
                <div className={styles.contentContainer}>
                    <FilterReviews />

                    {errorFetchingReviews ? <div className={styles.errorMessage}>Something went wrong while loading the reviews, please try again.</div> :
                        <ReviewGrid>
                            {isLoading ?
                                [...Array(9)].map((_, index) =>
                                    <LoadingCard key={index} />
                                ) :
                                myReviews.map((review, index) =>
                                    <ReviewCard key={index} review={review} />
                                )
                            }
                        </ReviewGrid>
                    }
                </div>
                {myReviews.length > 0 && <Pagination totalReviews={myReviews[0].total} />}
            </div>
        </div>
    )
}
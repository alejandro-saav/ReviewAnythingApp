import { useEffect, useState, type ReactElement } from "react"
import Pagination from "../../components/Pagination";
import FilterReviews from "../../components/FilterReviews";
import LoadingCard from "../../components/loadingComponents/LoadingCard";
import ReviewCard from "../../components/ReviewCard";
import { useLocation } from "react-router-dom";
import { GetExploreReviewPageData } from "../../services/ReviewService";
import type { LikedReviews } from "../../types/ReviewTypes";
import styles from "./Explore.module.css";
import ReviewGrid from "../../components/ReviewGrid";

export default function Explore(): ReactElement {
    const [isLoading, setIsLoading] = useState<boolean>(true);
    const [errorFetchingReviews, setErrorFetchingReviews] = useState<string | null>(null);
    const [reviews, setReviews] = useState<LikedReviews[]>([]);

    const location = useLocation();
    const queryString = location.search;

    useEffect(() => {
        window.scrollTo({ top: 0, behavior: "smooth" });
        async function fetchData() {
            const reviewsResponse: LikedReviews[] | null = await GetExploreReviewPageData(queryString);
            if (reviewsResponse == null) {
                setErrorFetchingReviews("Something went wrong while trying to get our last reviews, please try reloading the page.");
                setIsLoading(false);
                return;
            };
            setReviews(reviewsResponse);
            setIsLoading(false);
        }
        fetchData();
    }, [location])
    return (
        <div className={styles.exploreContainer}>
            <div className={styles.exploreWrapper}>
                <div className={styles.pageHeader}>
                    <h1 className={styles.pageTitle}>Explore Reviews</h1>
                    <p className={styles.pageSubtitle}>Discover authentic reviews from our community. Find insights on books, movies,
                        technology, and everything in between.</p>
                </div>
                <div className={styles.contentContainer}>
                    <FilterReviews />

                    {errorFetchingReviews ? <div className={styles.errorMessage}>Something went wrong while loading the reviews, please try again.</div> :
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
                    }
                </div>
                {reviews.length > 0 && <Pagination totalReviews={reviews[0].total} />}
            </div>
        </div>

    );
}
import type { Category, CommentVoteRequest, LikedReviews, ReviewPageData, Comment, WriteReviewModel, ReviewModel } from "../types/ReviewTypes";
import { BASE_API_URL } from "../utils/const";

export async function FetchCategories(): Promise<Category[] | null> {
    try {
        const response = await fetch(BASE_API_URL + "/api/categories");
        if (!response.ok) throw new Error();
        return await response.json();
    } catch (error) {
        console.log("Something went wrong while trying fetching the categories.");
        return null;
    }
}

export async function FetchLikedReviews(queryString: string): Promise<LikedReviews[] | null> {
    try {
        const response = await fetch(BASE_API_URL + `/api/reviews/liked-reviews${queryString}`, {
            credentials: "include"
        });
        if (!response.ok) throw new Error();
        return await response.json();
    } catch (error) {
        return null;
    }
}

export async function FetchUserReviews(queryString: string): Promise<LikedReviews[] | null> {
    try {
        const response = await fetch(BASE_API_URL + `/api/reviews/myreviews${queryString}`, {
            credentials: "include"
        });

        if (!response) throw new Error();
        return await response.json();
    } catch (error) {
        return null;
    }
}

export async function FetchReviewPageData(reviewId: number): Promise<ReviewPageData | null> {
    try {
        const response = await fetch(BASE_API_URL + `/api/reviews/${reviewId}/page-data`, {
            credentials: "include"
        });

        if (!response) return null;
        return await response.json();
    } catch (error) {
        return null;
    }
}

export async function PostCommentVote(commentVote: CommentVoteRequest): Promise<boolean> {
    try {
        const response = await fetch(BASE_API_URL + `/api/comment/comment-votes`, {
            method: "POST",
            body: JSON.stringify(commentVote),
            credentials: "include",
            headers: {
                "Content-Type": "application/json"
            }
        });

        if (!response.ok) return false;

        return true;
    } catch (error) {
        return false;
    }
}

export async function PostComment(reviewId: number, comment: string): Promise<Comment | null> {
    try {
        const response = await fetch(BASE_API_URL + `/api/comment`, {
            method: "POST",
            body: JSON.stringify({ ReviewId: reviewId, Content: comment }),
            credentials: "include",
            headers: {
                "Content-Type": "application/json"
            }
        });

        if (!response.ok) return null;

        return await response.json();
    } catch (error) {
        return null;
    }
}

export async function PostReviewVote(reviewId: number, voteType: number): Promise<number | null> {
    try {
        const response = await fetch(BASE_API_URL + `/api/reviews/review-votes`, {
            method: "POST",
            body: JSON.stringify({ ReviewId: reviewId, VoteType: voteType }),
            credentials: "include",
            headers: {
                "Content-Type": "application/json"
            }
        });

        if (!response.ok) return null;
        return response.status;
    } catch (error) {
        return null;
    }
}

export async function PostReview(review: WriteReviewModel): Promise<ReviewModel | null> {
    try {
        const response = await fetch(BASE_API_URL + `/api/reviews`, {
            method: "POST",
            body: JSON.stringify(review),
            credentials: "include",
            headers: {
                "Content-Type": "application/json"
            }
        });
        if (!response.ok) return null;

        return await response.json();
    } catch (error) {
        return null;
    }
}

export async function GetExploreReviewPageData(queryParams: string): Promise<LikedReviews[] | null> {
    try {
        const response = await fetch(BASE_API_URL + `/api/reviews/explore${queryParams}`, {
            credentials: "include",
        });
        if (!response.ok) return null;

        return await response.json();
    } catch (error) {
        return null;
    }
}


export async function GetLatestReviewIds(): Promise<number[] | null> {
    try {
        let idsAmount = 100;
        const response = await fetch(BASE_API_URL + `/api/reviews/latest?amount=${idsAmount}`, {
            credentials: "include",
        });
        if (!response.ok) return null;

        return await response.json();
    } catch (error) {
        return null;
    }
}

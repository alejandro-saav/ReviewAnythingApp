import type { UserInformation } from "./AuthTypes"

export interface LikedReviews {
    reviewId: number;
    category: string;
    title: string;
    content: string;
    likes: number;
    lastEditDate: Date;
    rating: number;
    tags: string[];
    numberOfComments: number;
    user: UserInformation | null;
    creatorFollowers: number;
    total: number;
}

export interface Category {
    categoryId: number;
    categoryName: string;
}

export interface UserComments {
    commentId: number;
    content: string;
    reviewId: number;
    lastEditDate: string;
    likes: number;
    reviewTitle: string;
    userName: string | null;
    profileImage: string | null;
}

export interface UserInfoNeededToShowOnComment {
    userId: number;
    userName: string;
    profileImage: string | null;
    reviewCount: number;
    followerCount: number;
}

export interface Comment {
    commentId: number;
    content: string;
    reviewId: number;
    lastEditDate: string;
    userInformation: UserInfoNeededToShowOnComment;
    likes: number;
}

export interface ReviewModel {
    reviewId: number;
    title: string;
    content: string;
    creationDate: string;
    lastEditDate: string;
    rating: number;
    user: UserInformation;
    itemId: number;
    tags: string[];
    upVoteCount: number;
    downVoteCount: number;
    totalVotes: number;
}

export interface WriteReviewModel {
    title: string;
    content: string;
    rating: number;
    itemId?: number;
    tags: string[];
    categoryId: number;
}

export interface CommentVoteRequest {
    commentId: number;
    voteType: number;
    reviewId: number;
}

export interface CommentVoteResponse {
    commentId: number;
    userVote: number;
}

export interface ReviewPageData {
    review: ReviewModel;
    comments: Comment[];
    userReviewVote: number | null;
    commentVotes: CommentVoteResponse[];
    followedUserIds: number[];
}
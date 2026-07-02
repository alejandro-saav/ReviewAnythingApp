import type { UserInformation } from "./AuthTypes";

export interface UserPageData {
    userSummary: UserInformation;
    totalReviews: number;
    totalComments: number;
    followers: UserInformation[];
    following: UserInformation[];
    isCurrentUserFollowing: boolean;
}
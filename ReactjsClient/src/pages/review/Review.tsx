import { useEffect, useState, type ReactElement } from "react";
import styles from "./Review.module.css"
import { Link, useNavigate, useParams } from "react-router-dom";
import { FetchReviewPageData, PostReviewVote } from "../../services/ReviewService";
import type { ReviewPageData } from "../../types/ReviewTypes";
import ProfileIcon from "../../components/ProfileIcon";
import { formatDate, isNullOrWhiteSpace } from "../../utils/helperFunctions";
import { useSelector } from "react-redux";
import type { UserInformation } from "../../types/AuthTypes";
import { FollowUser, UnFollowUser } from "../../services/UserService";
import SignInModal from "../../components/SignInModal";
import CommentSection from "../../components/CommentSection";
import LikeDislikeBtn from "../../components/LikeDislikeBtn";


export default function Review(): ReactElement {
    const user: UserInformation | null = useSelector((state: any) => state.auth.user);
    const [disableVoteBtn, setDisableVoteBtn] = useState<boolean>(false);

    const [review, setReview] = useState<ReviewPageData | undefined>(undefined);
    const [showModal, setShowModal] = useState<boolean>(false);
    const { reviewId } = useParams();
    const navigate = useNavigate();
    useEffect(() => {
        window.scrollTo({ top: 0, behavior: "smooth" });
        async function FetchReview() {
            const response = await FetchReviewPageData(+reviewId!);
            if (response != null) {
                setReview(response);
            } else {
                navigate("/notfound");
            }
        }
        FetchReview();
    }, []);

    async function HandleUserFollow(targetUserId: number): Promise<void> {
        if (user == null) {
            setShowModal(true);
            return;
        }
        if (review?.followedUserIds.includes(targetUserId)) {
            const unfollowResponse = await UnFollowUser(+targetUserId);
            if (unfollowResponse) {
                setReview({ ...review, followedUserIds: review.followedUserIds.filter(userIds => userIds != targetUserId) })
            }
        } else {
            if (review == undefined) return;
            const followResponse = await FollowUser(+targetUserId);
            if (followResponse) {
                setReview({ ...review, followedUserIds: [...review.followedUserIds, targetUserId] })
            }
        }
    }

    async function SubmitReviewVote(vote: number) {
        if (user == null) {
            setShowModal(true);
            return;
        }
        setDisableVoteBtn(true);

        if (vote != 1 && vote != -1) return;

        const submitCommentResponse = await PostReviewVote(+reviewId!, vote);
        if (submitCommentResponse) {
            setReview((prev: ReviewPageData | undefined) => {
                if (prev != undefined) {
                    let newReviewCount = vote == 1 && (prev.userReviewVote == -1 || prev.userReviewVote == null) ? prev.review.upVoteCount++ : prev.userReviewVote == 1 && (vote == -1 || vote == 1) ? prev.review.upVoteCount-- : prev.review.upVoteCount;

                    let newVote = prev.userReviewVote == vote ? null : vote;
                    return { ...prev, userReviewVote: newVote, review: { ...prev.review, upVoteCount: newReviewCount } }
                }
            });
        } else {
            // Something went wrong with the post request.
            console.log("FAILED!");
        };
        setDisableVoteBtn(false);
    }
    return (
        <>
            {review == undefined ?
                <span style={{ display: "block", height: "100vh" }}></span>
                :
                <div className={styles.container} onClick={(e) => {
                    e.stopPropagation();
                    setShowModal(false);
                }}>
                    <div className={styles.header}>
                        <h1 className={styles.title}>{review.review.title}</h1>
                        <div className={styles.metaInfo}>
                            {review.review.user.userId == 0 ?
                                <div className={styles.userDeleted}>
                                    <ProfileIcon width={30} height={30} />
                                    User Deleted
                                </div>
                                :
                                <Link className={styles.username} to={`/profile/${review.review.user.userId}`}>
                                    {isNullOrWhiteSpace(review.review.user.profileImage) ?
                                        <ProfileIcon width={30} height={30} />
                                        :
                                        <img src={`${review.review.user.profileImage}`} className={styles.profilePictureImage}
                                            alt="Profile image" />
                                    }
                                    {isNullOrWhiteSpace(review.review.user.userName) ? "User Deleted" : review.review.user.userName}
                                </Link>
                            }
                            <div className={styles.lastEdit}>Last edited: {formatDate(review.review.lastEditDate)}</div>
                        </div>
                    </div>

                    <div className={styles.contentSection}>
                        <div className={styles.ratingSection}>
                            <div className={styles.stars}>
                                {[1, 2, 3, 4, 5].map((item, index) => {
                                    if (review.review.rating >= item) {
                                        return (
                                            <span key={index} className={`${styles.star} ${styles.selected}`}>
                                                ⭐
                                            </span>
                                        )
                                    } else {
                                        return (
                                            <span key={index} className={`${styles.star} ${styles.noSelected}`}>
                                                ☆
                                            </span>
                                        )
                                    }
                                })}
                            </div>
                        </div>

                        <div className={styles.reviewContent}>
                            {review.review.content}
                        </div>

                        <div className={styles.tagsSection}>
                            <div className={styles.tagsTitle}>Tags</div>
                            <div className={styles.tags}>
                                {review.review.tags.map((tag, index) =>
                                    <span key={index} className={styles.tag}>{tag}</span>
                                )}
                            </div>
                        </div>

                        <div className={styles.votingSection}>
                            <div className={styles.voteButtons}>
                                <LikeDislikeBtn btnType="like" isActive={review.userReviewVote == 1} likeCount={review.review.upVoteCount} SubmitReviewVote={SubmitReviewVote} context="review" disable={disableVoteBtn} />
                                <LikeDislikeBtn btnType="dislike" isActive={review.userReviewVote == -1} SubmitReviewVote={SubmitReviewVote} context="review" disable={disableVoteBtn} />
                            </div>
                        </div>
                        <CommentSection HandleUserFollow={HandleUserFollow} comments={review.comments} userCommentVotes={review.commentVotes} usersFollowingIds={review.followedUserIds} HandleModal={setShowModal} reviewId={+reviewId!} HandleReviewPageData={setReview} />
                    </div >
                </div >
            }
            {showModal && <SignInModal />}
        </>
    )
}
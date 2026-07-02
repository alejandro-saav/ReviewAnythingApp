import { BiDislike, BiLike } from "react-icons/bi";
import styles from "./LikeDislikeBtn.module.css";
import type { ReactElement } from "react";

interface BaseProps {
    btnType: "like" | "dislike";
    isActive: boolean;
    likeCount?: number;
    disable: boolean;
}

interface ReviewVoteProps extends BaseProps {
    context: "review";
    SubmitReviewVote: (voteType: number) => Promise<void>;
}

interface CommentVoteProps extends BaseProps {
    context: "comment";
    SubmitCommentVote: (voteType: number, commentId: number) => Promise<void>;
    commentId: number;
}

type LikeDislikeBtnProps = ReviewVoteProps | CommentVoteProps;

export default function LikeDislikeBtn(props: LikeDislikeBtnProps): ReactElement {
    const { btnType, isActive, likeCount, disable } = props;
    return (
        <button disabled={disable} className={`${styles.voteBtn} ${isActive && styles.active}`}
            onClick={(e: React.MouseEvent) => {
                e.stopPropagation();
                const voteType = btnType == "like" ? 1 : -1;
                if (props.context == "review") {
                    props.SubmitReviewVote(voteType);
                } else {
                    props.SubmitCommentVote(voteType, props.commentId);
                }
            }}>
            <span className={styles.voteIcon}>{btnType == "like" ? <BiLike size={18} /> : <BiDislike size={18} />}</span>
            {likeCount && <span>{likeCount}</span>}
        </button>
    )
}
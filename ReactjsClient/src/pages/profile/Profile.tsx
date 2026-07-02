import { useEffect, useState, type ReactElement } from "react";
import { useSelector } from "react-redux";
import type { UserInformation } from "../../types/AuthTypes";
import type { UserPageData } from "../../types/PagesTypes";
import { Link, useNavigate, useParams } from "react-router-dom";
import { FollowUser, GetUserPageData, UnFollowUser } from "../../services/UserService";
import LoadingProfileSkeleton from "../../components/loadingComponents/LoadingProfileSkeleton";
import SignInModal from "../../components/SignInModal";
import { isNullOrWhiteSpace } from "../../utils/helperFunctions";
import styles from "./Profile.module.css";

export default function Profile(): ReactElement {
    const user: UserInformation | null = useSelector((state: any) => state.auth.user);
    const [userData, setUserData] = useState<UserPageData>();
    const [isLoading, setIsLoading] = useState<boolean>(true);
    const [showModal, setShowModal] = useState<boolean>(false);

    const { userId } = useParams();
    const navigate = useNavigate();
    if (!Number.isFinite(+userId!)) {
        navigate("/notfound")
    }

    useEffect(() => {
        window.scrollTo({ top: 0, behavior: "smooth" });
        setIsLoading(true);
        async function fetchUserPageData() {
            const response = await GetUserPageData(+userId!);
            if (response != null) {
                setUserData(response);
            } else {
                navigate("/notfound");
            }
            setIsLoading(false);
        }
        fetchUserPageData();
    }, [userId])

    async function HandleFollowUser(e: any) {
        e.stopPropagation();
        if (user == null) {
            setShowModal(true);
            return;
        }
        if (userData?.isCurrentUserFollowing) {
            const unfollowResponse = await UnFollowUser(+userId!);
            if (unfollowResponse) {
                setUserData({ ...userData, isCurrentUserFollowing: false, followers: userData.followers.filter(users => users.userId != user.userId) })
            }
        } else {
            if (userData == undefined) return;
            const followResponse = await FollowUser(+userId!);
            if (followResponse) {
                setUserData({ ...userData, isCurrentUserFollowing: true, followers: [...userData.followers, user] })
            }
        }
    }
    return (
        <div onClick={(e) => {
            e.stopPropagation();
            setShowModal(false)
        }}>
            <h1 className={styles.titleHeader}>Profile </h1>
            {isLoading ?
                <LoadingProfileSkeleton />
                :
                <>
                    <div className={styles.container}>
                        <div className={styles.profileHeader}>
                            <img
                                src={userData?.userSummary?.profileImage ?? undefined}
                                alt="Profile Picture" className={styles.profileImage} id="profileImage" />

                            <div className={styles.profileInfo}>
                                <h1 className={styles.profileName} id="fullName">{`${userData?.userSummary.firstName} ${userData?.userSummary.lastName}`}</h1>
                                <div className={styles.username} id="username">{userData?.userSummary.userName}</div>
                                <p className={styles.bio} id="bio">{userData?.userSummary.bio}</p>

                                <div className={styles.stats}>
                                    <div className={styles.statItem}>
                                        <span className={styles.statNumber} id="reviewCount">{userData?.totalReviews}</span>
                                        <span className={styles.statLabel}>Reviews</span>
                                    </div>
                                    <div className={styles.statItem}>
                                        <span className={styles.statNumber} id="commentCount">{userData?.totalComments}</span>
                                        <span className={styles.statLabel}>Comments</span>
                                    </div>
                                    <div onClick={HandleFollowUser}>
                                        {user && user.userId != userData?.userSummary.userId &&
                                            <button className={styles.btn}>
                                                {userData?.isCurrentUserFollowing ?
                                                    <span>Following</span>
                                                    :
                                                    <span>Follow</span>
                                                }
                                            </button>
                                        }
                                    </div>
                                </div>
                            </div>
                        </div >

                        <div className={styles.socialSections}>
                            <div className={styles.followersSection}>
                                <h3 className={styles.header}>Followers (<span id="followerCount">{userData?.followers.length}</span>)</h3>
                                {userData && userData.followers.length > 0 ?
                                    userData?.followers.map((follower, index) =>
                                        <div className={styles.followersGrid} id="followersGrid">
                                            <Link key={index} to={`/profile/${follower.userId}`} className={styles.followerItem}>
                                                <img src={follower.profileImage ?? undefined} alt="User Profile Picture" className={styles.followerAvatar} width="50"
                                                    height="50" />
                                                <div className={styles.followerName}>{follower.userName}</div>
                                            </Link>
                                        </div>
                                    )
                                    :
                                    <p>No followers to show.</p>
                                }
                            </div>

                            <div className={styles.followingSection}>
                                <h3 className={styles.header}>Following (<span id="followingCount">{userData?.following.length}</span>)</h3>
                                {userData && userData?.following.length > 0 ?
                                    userData.following.map((followee, index) =>
                                        <div className={styles.followingGrid} id="followingGrid">
                                            <Link to={`/profile/${followee.userId}`} className={styles.followerItem} key={index} >
                                                {isNullOrWhiteSpace(followee.profileImage) ?
                                                    <div className={styles.profilePhoto}>@{followee.userName?.substring(0, 2)}</div>
                                                    :
                                                    <>
                                                        <img src={followee.profileImage ?? undefined} alt={followee.userName ?? ""} className={styles.followerAvatar} width="50"
                                                            height="50" />
                                                        <div className={styles.followerName}>{followee.userName}</div>
                                                    </>
                                                }
                                            </Link>
                                        </div>
                                    )

                                    :
                                    <p>No users following this user yet.</p>
                                }
                            </div>
                        </div>
                    </div>
                    {showModal && <SignInModal />}
                </>
            }
        </div>
    )
}
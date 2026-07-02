import Home from '../pages/home/Home'
import Login from '../pages/Auth/login/Login'
import './App.css'
import { Routes, Route } from "react-router-dom"
import MainLayout from './layout/MainLayout'
import SignUp from '../pages/Auth/signup/SignUp'
import EmailConfirmationRequired from '../pages/Auth/email-confirmation-required/EmailConfirmationRequired'
import ConfirmEmail from '../pages/Auth/confirm-email/ConfirmEmail'
import { useDispatch } from 'react-redux'
import { useEffect, useState } from 'react'
import { GetUserInfo, PostNewVisit } from '../services/UserService'
import { setUser } from '../pages/Auth/authSlice'
import LikedReviews from '../pages/liked-reviews/LikedReviews'
import MyComments from '../pages/mycomments/MyComments'
import MyReviews from '../pages/myreviews/MyReviews'
import Profile from '../pages/profile/Profile'
import NotFound from '../pages/notfound/NotFound'
import Review from '../pages/review/Review'
import WriteReview from '../pages/write-review/WriteReview'
import Explore from '../pages/explore/Explore'
import ProtectedRoute from '../components/ProtectedRoute'
import ExploreCategories from '../pages/explore-categories/ExploreCategories'
import EditProfile from '../pages/edit-profile/EditProfile'

function App() {
  const dispatch = useDispatch();
  const [loading, setLoading] = useState<boolean>(true);
  useEffect(() => {
    async function fetchUserInfo() {
      const user = await GetUserInfo();
      if (user != null) dispatch(setUser(user));
      setLoading(false);
    }

    async function LogNewVisit(): Promise<void> {
      if (await cookieStore.get("visit_tracked") != null) return;
      const logResponse = await PostNewVisit();
      if (logResponse) {
        try {
          await cookieStore.set({
            name: "visit_tracked",
            value: "1",
            expires: Date.now() + (30 * 60 * 1000),
            sameSite: "lax"
          });
        } catch (error) {
        }
      }
    }
    fetchUserInfo();
    LogNewVisit();
  }, [])

  if (loading) return;
  return (
    <>
      <Routes>
        <Route element={<MainLayout />}>
          <Route element={<ProtectedRoute />}>
            <Route path="likedreviews" element={<LikedReviews />} />
            <Route path="mycomments" element={<MyComments />} />
            <Route path="myreviews" element={<MyReviews />} />
            <Route path="write" element={<WriteReview />} />
            <Route path="edit-profile" element={<EditProfile />} />
          </Route>
          <Route path="explorecategories" element={<ExploreCategories />} />
          <Route path="/" element={<Home />} />
          <Route path="profile/:userId" element={<Profile />} />
          <Route path="review/:reviewId" element={<Review />} />
          <Route path="explore" element={<Explore />} />
          <Route path="*" element={<NotFound />} />
        </Route>
        <Route path="login" element={<Login />} />
        <Route path="signup" element={<SignUp />} />
        <Route path="email-confirmation-required" element={<EmailConfirmationRequired />} />
        <Route path="confirm-email" element={<ConfirmEmail />} />
      </Routes>
    </>
  )
}

export default App

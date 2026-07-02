import type { ReactElement } from "react";
import styles from "./Footer.module.css";
import { Link } from "react-router-dom";

export default function Footer(): ReactElement {
    return (
        <footer className={styles.footer}>
            <div className={styles.footerContainer}>
                <div className={styles.footerSection}>
                    <h3>ReviewAnything</h3>
                    <p>The ultimate platform for honest reviews about everything. Join our community of authentic voices.</p>
                </div>
                <div className={styles.footerSection}>
                    <h3>Explore</h3>
                    <ul>
                        <li><Link to="#">Trending Reviews</Link></li>
                        <li><Link to="#">Categories</Link></li>
                        <li><Link to="#">Top Reviewers</Link></li>
                        <li><Link to="#">Recent Activity</Link></li>
                    </ul>
                </div>
                <div className={styles.footerSection}>
                    <h3>Community</h3>
                    <ul>
                        <li><Link to="#">Guidelines</Link></li>
                        <li><Link to="#">Help Center</Link></li>
                        <li><Link to="#">Contact Us</Link></li>
                        <li><Link to="#">Blog</Link></li>
                    </ul>
                </div>
                <div className={styles.footerSection}>
                    <h3>Account</h3>
                    <ul>
                        <li><Link to="#">Sign Up</Link></li>
                        <li><Link to="#">Log In</Link></li>
                        <li><Link to="#">Write Review</Link></li>
                        <li><Link to="#">My Profile</Link></li>
                    </ul>
                </div>
            </div>
            <div className={styles.footerBottom}>
                <p>&copy; {new Date().getFullYear()} ReviewAnything. All rights reserved.</p>
            </div>
        </footer>
    )
}
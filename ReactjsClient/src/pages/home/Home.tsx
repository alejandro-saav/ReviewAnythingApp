import styles from './Home.module.css';

export default function Home() {
    return (
        <>
            <section className={styles.hero} id="home">
                <div className={styles.heroContainer}>
                    <div className={styles.heroContent}>
                        <h1>Review Anything. Share Everything.</h1>
                        <p>Your voice matters. Share honest reviews about products, places, movies, books, and literally anything
                            else. Join a community that values authentic opinions.</p>
                        <div className={styles.ctaButtons}>
                            <a href="/write" className={`${styles.btn} ${styles.btnPrimary}`}>✍️ Write a Review</a>
                            <a href="/explore" className={`${styles.btn} ${styles.btnSecondary}`}>🔍 Explore Reviews</a>
                        </div>
                    </div>
                    <div className={styles.heroVisual}>
                        <div className={styles.floatingCards}>
                            <div className={styles.floatingCard}>
                                <div className={styles.cardRating}>⭐⭐⭐⭐⭐</div>
                                <div className={styles.cardTitle}>iPhone 15 Pro</div>
                                <div className={styles.cardText}>Amazing camera quality, but battery life could be better...</div>
                            </div>
                            <div className={styles.floatingCard}>
                                <div className={styles.cardRating}>⭐⭐⭐⭐☆</div>
                                <div className={styles.cardTitle}>Dune: Part Two</div>
                                <div className={styles.cardText}>Visually stunning masterpiece with incredible cinematography...</div>
                            </div>
                            <div className={styles.floatingCard}>
                                <div className={styles.cardRating}>⭐⭐⭐☆☆</div>
                                <div className={styles.cardTitle}>Local Coffee Shop</div>
                                <div className={styles.cardText}>Great atmosphere but service can be slow during peak hours...</div>
                            </div>
                        </div>
                    </div>
                </div>
            </section>

            {/* Stats Section */}
            <section className={styles.stats}>
                <div className={styles.statsContainer}>
                    <div className={styles.statItem}>
                        <h3>50K+</h3>
                        <p>Active Reviewers</p>
                    </div>
                    <div className={styles.statItem}>
                        <h3>1M+</h3>
                        <p>Reviews Written</p>
                    </div>
                    <div className={styles.statItem}>
                        <h3>500+</h3>
                        <p>Categories</p>
                    </div>
                    <div className={styles.statItem}>
                        <h3>4.8★</h3>
                        <p>Community Rating</p>
                    </div>
                </div>
            </section>

            {/* Categories Section */}
            <section className={styles.categories} id="categories">
                <div className={styles.categoriesContainer}>
                    <h2 className={styles.sectionTitle}>Review Everything</h2>
                    <p className={styles.sectionSubtitle}>From tech gadgets to local restaurants, movies to books - if it exists, you can
                        review it</p>
                    <div className={styles.categoryGrid}>
                        <div className={styles.categoryCard}>
                            <span className={styles.categoryIcon}>📱</span>
                            <h3>Technology</h3>
                            <p>Phones, laptops, apps, and all things tech</p>
                        </div>
                        <div className={styles.categoryCard}>
                            <span className={styles.categoryIcon}>🍕</span>
                            <h3>Food & Restaurants</h3>
                            <p>Discover amazing places to eat and drink</p>
                        </div>
                        <div className={styles.categoryCard}>
                            <span className={styles.categoryIcon}>🎬</span>
                            <h3>Entertainment</h3>
                            <p>Movies, TV shows, games, and more</p>
                        </div>
                        <div className={styles.categoryCard}>
                            <span className={styles.categoryIcon}>📚</span>
                            <h3>Books & Media</h3>
                            <p>Literature, audiobooks, and educational content</p>
                        </div>
                        <div className={styles.categoryCard}>
                            <span className={styles.categoryIcon}>🏨</span>
                            <h3>Travel & Places</h3>
                            <p>Hotels, destinations, and travel experiences</p>
                        </div>
                        <div className={styles.categoryCard}>
                            <span className={styles.categoryIcon}>🛍️</span>
                            <h3>Shopping</h3>
                            <p>Products, brands, and shopping experiences</p>
                        </div>
                    </div>
                </div>
            </section>

            {/* Featured Reviews */}
            <section className={styles.featured} id="explore">
                <div className={styles.featuredContainer}>
                    <h2 className={styles.sectionTitle}>Trending Reviews</h2>
                    <p className={styles.sectionSubtitle}>See what the community is talking about</p>
                    <div className={styles.reviewsGrid}>
                        <div className={styles.reviewCard}>
                            <div className={styles.reviewHeader}>
                                <h3 className={styles.reviewTitle}>MacBook Air M3</h3>
                                <span className={styles.reviewRating}>⭐⭐⭐⭐⭐</span>
                            </div>
                            <p className={styles.reviewText}>Absolutely incredible performance for the price point. The M3 chip handles
                                everything I throw at it, from video editing to gaming. Battery life is outstanding - easily gets me
                                through a full day of work.</p>
                            <div className={styles.reviewMeta}>
                                <span>by TechReviewer2024</span>
                                <span>2 hours ago</span>
                            </div>
                        </div>
                        <div className={styles.reviewCard}>
                            <div className={styles.reviewHeader}>
                                <h3 className={styles.reviewTitle}>The Bear - Season 3</h3>
                                <span className={styles.reviewRating}>⭐⭐⭐⭐☆</span>
                            </div>
                            <p className={styles.reviewText}>Another solid season that maintains the show's incredible tension and character
                                development. Some episodes felt a bit slower than previous seasons, but the emotional payoff is
                                worth it.</p>
                            <div className={styles.reviewMeta}>
                                <span>by FilmBuff</span>
                                <span>5 hours ago</span>
                            </div>
                        </div>
                        <div className={styles.reviewCard}>
                            <div className={styles.reviewHeader}>
                                <h3 className={styles.reviewTitle}>AirPods Pro 2</h3>
                                <span className={styles.reviewRating}>⭐⭐⭐⭐☆</span>
                            </div>
                            <p className={styles.reviewText}>The noise cancellation is top-tier and the spatial audio is genuinely impressive.
                                However, they can become uncomfortable during extended wear sessions. Great for commuting
                                though.</p>
                            <div className={styles.reviewMeta}>
                                <span>by AudiophileAnna</span>
                                <span>1 day ago</span>
                            </div>
                        </div>
                    </div>
                </div>
            </section>

            {/* CTA Section */}
            <section className={styles.ctaSection} id="write">
                <div className={styles.ctaContainer}>
                    <h2>Ready to Share Your Voice?</h2>
                    <p>Join thousands of reviewers helping others make better decisions. Your opinion matters and can help shape the
                        experiences of others.</p>
                    <a href="/write" className={`${styles.btn} ${styles.btnWhite}`}>Start Writing Reviews</a>
                </div>
            </section>
        </>
    );
}
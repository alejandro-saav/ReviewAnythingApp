import styles from './ExploreCategories.module.css';

export default function ExploreCategories() {
    return (
        <>
            <div className={styles.floatingShapes}>
                <div className={styles.shape}></div>
                <div className={styles.shape}></div>
                <div className={styles.shape}></div>
            </div>

            <div className={styles.container}>
                <div className={styles.header}>
                    <h1>Categories</h1>
                    <p>Discover and review everything that matters to you. Choose a category to start exploring.</p>
                </div>

                <div className={styles.categoriesGrid} id="categoriesGrid">
                    <a className={styles.categoryCard} data-category="books" href="/explore?Category=Books">
                        <div className={styles.categoryIcon}>📚</div>
                        <h3>Books</h3>
                        <p>Novels, non-fiction, poetry, and everything literary. Share your thoughts on the latest reads.</p>
                    </a>

                    <a className={styles.categoryCard} data-category="movies" href="/explore?Category=Movies">
                        <div className={styles.categoryIcon}>🎬</div>
                        <h3>Movies</h3>
                        <p>From blockbusters to indie films. Review the latest releases and timeless classics.</p>
                    </a>

                    <a className={styles.categoryCard} data-category="music" href="/explore?category=Music">
                        <div className={styles.categoryIcon}>🎵</div>
                        <h3>Music</h3>
                        <p>Albums, singles, concerts, and artists. Share your musical discoveries and favorites.</p>
                    </a>

                    <a className={styles.categoryCard} data-category="games" href="/explore?Category=Games">
                        <div className={styles.categoryIcon}>🎮</div>
                        <h3>Games</h3>
                        <p>Video games, board games, mobile games. Rate and review your gaming experiences.</p>
                    </a>

                    <a className={styles.categoryCard} data-category="technology" href="/explore?Category=Technology">
                        <div className={styles.categoryIcon}>⚡</div>
                        <h3>Technology</h3>
                        <p>Gadgets, apps, software, and tech innovations. Help others make informed decisions.</p>
                    </a>

                    <a className={styles.categoryCard} data-category="art" href="/explore?Category=Art">
                        <div className={styles.categoryIcon}>🎨</div>
                        <h3>Art</h3>
                        <p>Paintings, sculptures, digital art, and exhibitions. Express your artistic perspectives.</p>
                    </a>

                    <a className={styles.categoryCard} data-category="science" href="/explore?Category=Science">
                        <div className={styles.categoryIcon}>🔬</div>
                        <h3>Science</h3>
                        <p>Research papers, documentaries, and scientific discoveries. Dive into the world of knowledge.</p>
                    </a>
                </div>
            </div>
        </>
    );
}
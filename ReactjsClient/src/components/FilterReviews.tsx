import { useEffect, useState, type ReactElement } from "react";
import styles from "./FilterReviews.module.css"
import { useSearchParams } from "react-router-dom";
import type { Category } from "../types/ReviewTypes";
import { CATEGORIES_FALLBACK, SORT_OPTIONS, TAG_REGEX } from "../utils/const";
import { FetchCategories } from "../services/ReviewService";
import { isNullOrWhiteSpace } from "../utils/helperFunctions";

export default function FilterReviews(): ReactElement {
    const [searchParams, setSearchParams] = useSearchParams();

    // Get params
    const searchParam: string | null = searchParams.get("search");
    const categoryParam: string | null = searchParams.get("category");
    const ratingParam: string | null = searchParams.get("rating");
    const sortParam: string | null = searchParams.get("sort");
    const tagsParam: string[] = searchParams.get("tags")?.split(",") ?? [];
    // States:
    // Error state to show when the user type an invalid tag.
    const [errorTagMessage, setErrorTagMessage] = useState<string | null>(null);
    // Categories state
    const [categories, setCategories] = useState<Category[]>([]);

    // Hover star state to style stars that are hover
    const [hoverStar, setHoverStar] = useState<number>(ratingParam != null ? +ratingParam : 0);

    // Fetch the categories
    useEffect(() => {
        async function FetchCategoriesData(): Promise<void> {
            const categoriesResponse: Category[] | null = await FetchCategories();
            if (categoriesResponse != null) {
                setCategories(categoriesResponse);
            } else {
                setCategories(CATEGORIES_FALLBACK);
            }
        }
        FetchCategoriesData();
    }, [])

    function HandleParam(key: string, value: string | null): void {
        const newParams = new URLSearchParams(searchParams);
        if (value == null) {
            if (key == "rating") {
                setHoverStar(0);
            }
            newParams.delete(key);
        } else {
            newParams.set(key, value);
        }
        setSearchParams(newParams);
    }

    // Set tag param
    function HandleTagInput(e: React.KeyboardEvent<HTMLInputElement>) {
        const tagValue: string = e.currentTarget.value;
        if (e.key == "Enter" && !isNullOrWhiteSpace(tagValue)) {
            setErrorTagMessage(null);
            e.currentTarget.value = "";
            if (tagsParam.length >= 5) {
                setErrorTagMessage("Max tags set. Max tags allowed 5");
                return;
            }

            if (tagsParam.includes(tagValue)) {
                setErrorTagMessage("Tag already set.")
                return;
            }

            if (!TAG_REGEX.test(tagValue)) {
                setErrorTagMessage("Only letters, digits, underscores and hyphens allowed.");
                return;
            }

            if (tagsParam.length > 0) {
                HandleParam("tags", tagsParam.join(",") + "," + tagValue);
            } else {
                HandleParam("tags", tagValue);
            }
        }
    }

    function HandleTagRemoval(tag: string): void {
        var newTags = tagsParam.filter(tags => tags != tag);
        if (newTags.length > 0) {
            HandleParam("tags", newTags.join(","));
        } else {
            HandleParam("tags", null);
        }
    }
    return (
        <div className={styles.filtersSection}>
            <input type="text" className={styles.searchBar} placeholder="Search reviews, titles..."
                id="searchInput" onChange={(e) => {
                    HandleParam("search", e.target.value);
                }} value={searchParam ?? ""} />

            <div className={styles.filtersGrid}>
                <div className={styles.filterGroup}>
                    <h3>Categories</h3>
                    <div className={styles.categoryFilters}>
                        <div className={`${styles.categoryChip} ${isNullOrWhiteSpace(categoryParam) ? styles.active : ""}`}
                            onClick={() => HandleParam("category", null)} data-category="all">All
                        </div>
                        {categories.map((category, index) =>
                            <div key={index} className={`${styles.categoryChip} ${categoryParam == category.categoryName ? styles.active : ""}`}
                                data-category="books"
                                onClick={() => HandleParam("category", category.categoryName)}>{category.categoryName}</div>
                        )
                        }
                    </div>
                </div>
                <div className={styles.filterGroup}>
                    <h3>Rating</h3>
                    <div className={styles.ratingFilter}>
                        <div className={`${styles.ratingOption} ${ratingParam == null ? styles.active : ""}`} data-rating="all"
                            onClick={() => HandleParam("rating", null)}>
                            <span className={styles.allRatings}>All Ratings</span>
                        </div>
                        <div className={styles.starFather}>
                            <div className={styles.starContainer}>
                                {[1, 2, 3, 4, 5].map((rating, index) => (
                                    <span key={index} className={`${styles.star} ${hoverStar >= rating ? "" : styles.empty}`} onClick={() => HandleParam("rating", rating + "")} onMouseEnter={() => setHoverStar(rating)} onMouseLeave={() => setHoverStar(ratingParam != null ? +ratingParam : 0)}>★</span>
                                ))}
                            </div>
                            <span>{" "}Stars</span>
                        </div>
                    </div>
                </div>

                <div className={styles.filterGroup}>
                    <h3>Sort By</h3>
                    <select className={styles.sortDropdown} id="sortSelect" onChange={(e) => HandleParam("sort", e.target.value)} defaultValue="">
                        <option value="" disabled>-- Order by --</option>
                        {SORT_OPTIONS.map((option, index) =>
                            <option key={index} value={option}>{option}</option>
                        )}
                    </select>
                </div>

                <div className={styles.filterGroup}>
                    <h3>Tags</h3>
                    <input type="text" className={styles.tagInput} placeholder="Add tag..." id="tagInput" onKeyDown={HandleTagInput} />
                    {errorTagMessage &&
                        <span className={styles.errorMessage}>{errorTagMessage}
                        </span>
                    }
                    <div className={styles.filterTags} id="filterTags">
                        {tagsParam.map((tag, index) => (
                            <div className={styles.tag} key={index}>
                                <span className={styles.spTag}>{tag}</span>
                                <button className={styles.removeTagBtn} type="button" onClick={() => HandleTagRemoval(tag)}>x</button>
                            </div>
                        ))
                        }
                    </div>
                </div>
            </div>
        </div>
    )
}
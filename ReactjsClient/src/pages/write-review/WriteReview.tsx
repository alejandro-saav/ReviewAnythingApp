import { useEffect, useState, type ReactElement } from "react";
import { type Category, type WriteReviewModel } from "../../types/ReviewTypes";
import { FetchCategories, PostReview } from "../../services/ReviewService";
import { CATEGORIES_FALLBACK, TAG_REGEX } from "../../utils/const";
import { useForm, Controller, type SubmitHandler } from "react-hook-form";
import styles from "./WriteReview.module.css"
import { isNullOrWhiteSpace } from "../../utils/helperFunctions";
import { useNavigate } from "react-router-dom";

export default function WriteReview(): ReactElement {
    const [categories, setCategories] = useState<Category[]>(CATEGORIES_FALLBACK);
    const [hoverStar, setHoverStar] = useState<number>(0);
    const [newTag, setNewTag] = useState<string>("");

    useEffect(() => {
        window.scrollTo({ top: 0, behavior: "smooth" });
        async function GetCategories(): Promise<void> {
            const response: Category[] | null = await FetchCategories();
            if (response != null) {
                setCategories(response);
            }
        }
        GetCategories();
    }, []);

    const { register, handleSubmit, setError, formState: { errors, isSubmitting }, control, watch, setValue, clearErrors } = useForm<WriteReviewModel>({
        defaultValues: {
            rating: 0,
            tags: [] as string[],
        }
    });

    const currentTags = watch("tags");

    function AddTag() {
        clearErrors("tags");
        if (isNullOrWhiteSpace(newTag)) {
            return;
        }
        if (currentTags.length >= 5) {
            setError("tags", { type: "max", message: "Max tags set. Max tags allowed 5" }
            );
            return;
        }

        if (currentTags.includes(newTag)) {
            setError("tags", { type: "value", message: "Tag already set." })
            return;
        }

        if (!TAG_REGEX.test(newTag.trim())) {
            setError("tags", { type: "pattern", message: "Only letters, digits, underscores and hyphens allowed." });
            return;
        }

        setValue("tags", [...currentTags, newTag]);
    }
    function RemoveTag(tag: string) {
        setValue("tags", currentTags.filter(t => t != tag))
    }

    const navigate = useNavigate();

    const OnSubmitReview: SubmitHandler<WriteReviewModel> = async (data) => {
        const postReviewResponse = await PostReview(data);
        if (postReviewResponse) {
            navigate(`/review/${postReviewResponse.reviewId}`);
        } else {
            setError("root", {
                type: "root",
                message: "Something went wrong when trying to post your review, please try again."
            })
        }
    }
    return (
        <div className={styles.writeReviewContainer}>
            <div className={styles.writeReviewWrapper}>
                <h1 className={styles.pageTitle}>Write a Review</h1>
                <form onSubmit={handleSubmit(OnSubmitReview)}>
                    <div className={styles.formGroup}>
                        <label className={styles.formLabel} htmlFor="reviewTitle">Review Title</label>
                        <input type="text" id="reviewTitle" className={`${styles.formInput} ${styles.reviewTitleInput}`}
                            placeholder="Enter your review title..." {...register("title", { required: { value: true, message: "This field is required" }, maxLength: { value: 100, message: "Max length 100 characters." } })} />
                    </div>
                    {errors.title && <span className={styles.errorMessage}>{errors.title.message}</span>}

                    <div className={styles.formGroup}>
                        <div className={styles.ratingContainer}>
                            <label className={styles.formLabel} htmlFor="reviewContent">Review Content</label>
                            <Controller
                                name="rating"
                                control={control}
                                rules={{
                                    required: "Please select a rating",
                                    min: { value: 1, message: "Rating must be at least 1" }
                                }}
                                render={({ field: { onChange, value }, fieldState: { error } }) => (
                                    <div className={styles.starContainer}>
                                        {error && <span className={styles.errorMessage}>{error.message} {"★"}</span>}
                                        {[1, 2, 3, 4, 5].map((rating) => (
                                            <span key={rating} className={`${styles.star} ${hoverStar < rating && styles.empty}`} onMouseEnter={() => setHoverStar(rating)} onMouseLeave={() => setHoverStar(value)} onClick={() => onChange(rating)}>★</span>
                                        ))}
                                    </div>
                                )}
                            />
                        </div>
                        <textarea id="reviewContent" className={`${styles.formInput} ${styles.formTextarea}`}
                            placeholder="Share your thoughts and experience..." {...register("content", { required: { value: true, message: "This field is required" }, maxLength: { value: 10000, message: "Max length allow is 10.000 characters" }, minLength: { value: 100, message: "Min length is 500 characters" } })}>
                        </textarea>
                        {errors.content && <span className={styles.errorMessage}>{errors.content.message}</span>}
                        <div className={styles.displayTagsContainer} >
                            <label htmlFor="tag-input" className={styles.formLabel}>Tags:</label>
                            {currentTags.map((tag, index) =>
                                <div className={styles.tag} key={index}>
                                    <span className={styles.spTag}>{tag}</span>
                                    <button className={styles.removeTagBtn} type="button" onClick={() => RemoveTag(tag)}>x</button>
                                </div>
                            )
                            }
                        </div>
                        <div className={styles.tagsContainer}>
                            <input id="tag-input" className={styles.formInput}
                                placeholder="Add 5 tags max. (no special characters allow, just letters and numbers)" type="text" autoComplete="off" onChange={(e) => setNewTag(e.target.value)} onKeyDown={(e) => {
                                    if (e.key === "Enter") {
                                        e.preventDefault();
                                        AddTag();
                                    }
                                }} />
                            <button className={styles.addTagBtn} onClick={() => AddTag()} type="button">Add tag</button>
                        </div>
                    </div>
                    {errors.tags && <span className={styles.errorMessage}>{errors.tags.message}</span>}
                    <div className={styles.categoriesContainer}>
                        <label className={styles.formLabel} htmlFor="categories">Category:</label>
                        <select id="categories" {...register("categoryId", { required: { value: true, message: "Category is required" } })} className={styles.categories} defaultValue="">
                            <option value="" disabled>-- Select a category --</option>
                            {categories.map((category, index) =>
                                <option className={styles.categoryBtn} value={category.categoryId} key={index}>
                                    {category.categoryName}
                                </option>
                            )
                            }
                        </select>
                    </div>
                    {errors.categoryId && <span className={styles.errorMessage}>{errors.categoryId.message}</span>}
                    <button type="submit" className={styles.submitBtn}>Create Review</button>
                </form >
                {errors.root && <span className={styles.errorMessage}>{errors.root.message}</span>}
            </div >
        </div >
    )
}
import type { Category } from "../types/ReviewTypes";

export const BASE_API_URL_NETWORK: string = import.meta.env.VITE_API_URL_NETWORK;
export const BASE_API_URL: string = import.meta.env.VITE_API_URL;

export const SORT_OPTIONS: string[] = ["rating_asc", "rating_desc", "date_asc", "date_desc"]

export const CATEGORIES_FALLBACK: Category[] = [{
    categoryId: 1,
    categoryName: "Books"
}, {
    categoryId: 2,
    categoryName: "Movies"
}, {
    categoryId: 3,
    categoryName: "Music"
}, {
    categoryId: 4,
    categoryName: "Games"
}, {
    categoryId: 5,
    categoryName: "Technology"
}, {
    categoryId: 6,
    categoryName: "Art"
}, {
    categoryId: 7,
    categoryName: "Science"
}];

export const TAG_REGEX = /^[a-zA-Z0-9-_]+$/;
import type { LikedReviews } from "../types/ReviewTypes";

export function isNullOrWhiteSpace(str: string | null | undefined): boolean {
    return str == null || str == undefined || str.trim() === '';
}

export function GetPaginationRange(currentPage: number, totalPages: number, visiblePages: number = 5): number[] {
    let half = Math.trunc(visiblePages / 2);
    let start = currentPage - half;
    let end = currentPage + half;

    if (start < 1) {
        end += (1 - start);
        start = 1;
    }

    if (end > totalPages) {
        start -= (end - totalPages);
        end = totalPages;
    }

    if (start < 1) {
        start = 1;
    }

    var result: number[] = [];
    for (let i = start; i <= end; i++) result.push(i);
    return result;
}

export function formatDate(date: string): string {
    const dateFromString: string[] = date.split("-");
    const newDate = new Date(+dateFromString[0], +dateFromString[1] - 1, +dateFromString[2].substring(0, 2));
    return newDate.toLocaleDateString("en-US", {
        weekday: "long",
        day: "numeric",
        month: "long",
        year: "numeric"
    });
}

export function getRatingAverageFromReviews(reviews: LikedReviews[]): number {
    const sum = reviews.reduce((accumulator, next) => accumulator + next.rating, 0);
    const ratingAvg = (sum / reviews.length).toFixed(1);
    return +ratingAvg;
}
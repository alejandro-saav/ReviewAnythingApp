import type { ReactElement } from "react";
import styles from "./Pagination.module.css";
import { useSearchParams } from "react-router-dom";
import { GetPaginationRange } from "../utils/helperFunctions";

type PaginationProps = {
    totalReviews: number;
}
export default function Pagination({ totalReviews }: PaginationProps): ReactElement {
    const [searchParams, setSearchParams] = useSearchParams();
    const currentPage: string | number = searchParams.get("page") ?? 1;
    const totalPages: number = Math.trunc(totalReviews / 9) + 1;
    const pagesNumbersToDisplay: number[] = GetPaginationRange(+currentPage, totalPages);
    return (
        <div className={styles.pagination}>
            <button className={styles.pageBtn} onClick={() => setSearchParams({ page: `${+currentPage - 1}` })} disabled={+currentPage === 1}>← Previous</button>
            {pagesNumbersToDisplay.map((pageNumber, index) =>
                <button key={index} className={`${styles.pageBtn} ${pageNumber == +currentPage ? styles.active : ""}`}
                    onClick={() => setSearchParams({ page: pageNumber + "" })}>{pageNumber}</button>
            )}
            <button className={styles.pageBtn} onClick={() => setSearchParams({ page: `${+currentPage + 1}` })} disabled={+currentPage === totalPages}>Next →</button>
        </div>
    )
}
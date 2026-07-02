import { createSlice, type PayloadAction } from "@reduxjs/toolkit";
import type { AuthState, UserInformation } from "../../types/AuthTypes";

const initialState: AuthState = {
    user: null
}
const authSlice = createSlice({
    name: 'auth',
    initialState,
    reducers: {
        clearUser: (state) => {
            state.user = null;
        },

        setUser: (state, action: PayloadAction<UserInformation | null>) => {
            state.user = action.payload;
        }
    }
});

export const { clearUser, setUser } = authSlice.actions;
export default authSlice.reducer;
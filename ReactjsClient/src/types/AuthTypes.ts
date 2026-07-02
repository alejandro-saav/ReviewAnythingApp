export type User = {
    userId: number,
    userName: string,
    email: string,
    firstName: string,
    lastName: string | null,
    phoneNumber: string | null,
    profileImage: string | null,
    bio: string | null,
    creationDate: Date | null
}

export type UserInformation = {
    profileImage: string | null,
    firstName: string | null,
    lastName: string | null,
    bio: string | null,
    userName: string | null,
    userId: number
}

export type AuthState = {
    user: UserInformation | null
}

export type LoginRequest = {
    email: string,
    password: string,
    rememberMe: boolean
}

export type LoginResponse = {
    success: boolean,
    message: string | null,
    errorMessage: string | null,
    token: string | null,
    userResponse: User,
    errors: string[] | null
}

export type ForgotPasswordRequest = {
    email: string
}

export type GoogleAuthResult = {
    success: boolean,
    error: string,
    customJwt: string
}

export type RegisterRequest = {
    userName: string,
    firstName: string,
    lastName: string | null,
    email: string,
    errorMessage: string,
    password: string,
    phone: string | null,
    profileImage: File | null,
    bio: string | null
}

export type RequestPasswordRequest = {
    errorMessage: string,
    password: string
}

export type ResetPasswordViewModel = {
    errorMessage: string,
    newPassword: string,
    confirmPassword: string
}

export interface UpdateUserInfoRequest {
    profileImage: File | null;
    firstName: string | null;
    lastName: string | null;
    bio: string | null;
    deleteImage: boolean;
}
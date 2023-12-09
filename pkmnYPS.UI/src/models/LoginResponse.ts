export interface LoginResponse {
    data: LoginResponseData,
    error: Boolean,
    httpResponseCode: Number,
    errorMessage: string
}

export interface LoginResponseData {
    accessToken: string,
    expiry: number
}
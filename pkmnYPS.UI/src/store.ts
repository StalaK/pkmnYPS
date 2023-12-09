import { reactive } from 'vue'

export const accessTokenStore = reactive({
    accessToken: "",
    accessTokenExpiry: 0,
    setToken(token: string, expiry: number) {
        this.accessToken = token;
        this.accessTokenExpiry = expiry;
    }
});
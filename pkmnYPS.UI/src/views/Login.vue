<template>
    <main>
        <h2>Login</h2>
        <form @submit.prevent="login">
            <label for="email">Email:</label>
            <input id="email" type="text" v-model="email" />
            
            <label for="password">Password:</label>
            <input id="password" type="password" v-model="password" />
            
            <label class="error" v-if="loginError.length > 0">{{ loginError }}</label>

            <div>
                <input class="submit-row" type="submit" value="Login"/>
                <router-link class="submit-row" to="/register">Register</router-link>
            </div>
        </form>
    </main>
</template>

<script lang="ts">

import { accessTokenStore } from '@/store'
import { useRouter } from 'vue-router'
import { type LoginRequest } from '@/models/LoginRequest'
import { type LoginResponse } from '@/models/LoginResponse'

export default {
    setup() {
        const router = useRouter();
        return { router };
    },
    data() {
        return {
            email: "",
            password: "",
            loginError: ""
        }
    },
    mounted() {
        let accessTokenPresent = accessTokenStore.accessToken.length > 0;
        
        let currentDate = new Date();
        let accessTokenExpired = currentDate.getTime() > accessTokenStore.accessTokenExpiry;

        if (accessTokenPresent && !accessTokenExpired) {
            this.router.push({ path: '/pokedex' });
        }
    },
    methods: {
        async login() {
            this.loginError = "";

            let loginRequest: LoginRequest = {
                email: this.email,
                password: this.password
            };

            let response = await fetch(`${import.meta.env.VITE_API_URL}/Auth/Login`, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify(loginRequest)
            });

            let responseData: LoginResponse = await response.json();
            
            if (responseData.error) {
                this.loginError = responseData.errorMessage
            } else {
                accessTokenStore.setToken(responseData.data.accessToken, responseData.data.expiry);
                this.router.push({ path: '/pokedex' });
            }
        }
    }

}

</script>

<style scoped>

input {
    display: block;
}

.error {
    color: red;
}

.submit-row {
    display: inline-block;
    margin: 5px;
}

</style>
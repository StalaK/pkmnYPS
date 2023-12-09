
<template>
    <main>
        <h2>Registration</h2>
        <div v-if="registrationSuccessful">
            Registration successful! <router-link to="/">Click here to login</router-link>.
        </div>
        <div v-else>
            <form @submit.prevent="register">
                <label for="email">Email:</label>
                <input id="email" type="text" v-model="email" />
                
                <label for="password">Password:</label>
                <input id="password" type="password" v-model="password" />
                
                <label class="error" v-if="registrationError.length > 0">{{ registrationError }}</label>

                <div>
                    <input class="submit-row" type="submit" value="Register"/>
                    <router-link class="submit-row" to="/">Login</router-link>
                </div>
            </form>
        </div>
    </main>
</template>

<script lang="ts">
import { accessTokenStore } from '@/store'
import { useRouter } from 'vue-router'
import type { RegistrationRequest } from '@/models/RegistrationRequest'
import type { RegistrationResponse } from '@/models/RegistrationResponse'

export default {
    setup() {
        const router = useRouter();
        return { router };
    },
    data() {
        return {
            email: "",
            password: "",
            registrationError: "",
            registrationSuccessful: false
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
        async register() {
            this.registrationError = "";

            let registrationRequest: RegistrationRequest = {
                email: this.email,
                password: this.password
            }

            let response = await fetch(`${import.meta.env.VITE_API_URL}/Auth/Register`, {
                method: "PUT",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify(registrationRequest)
            });

            let responseData: RegistrationResponse = await response.json();
            
            if (responseData.error) {
                this.registrationError = responseData.errorMessage
            } else {
                this.registrationSuccessful = true;
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
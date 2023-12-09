
<template>
    <div>
        <form @submit.prevent="searchForPokemon">
            <input id="searchTerm" type="text" placeholder="Search by Pokemon name or number" size="30" v-model="searchTerm" />
            <input class="submit-button" type="submit" value="search" />
        </form>
    </div>
</template>

<script lang="ts">
import { accessTokenStore } from '../store'
import type { GetPokemonResponse } from '@/models/GetPokemonResponse'

export default {
    components: {
        accessTokenStore
    },
    data() {
        return {
            searchTerm: "" as string | number
        }
    },
    methods: {
        async searchForPokemon() {
            let response = await fetch(`${import.meta.env.VITE_API_URL}/Pokemon/${this.searchTerm}`, {
                method: "GET",
                headers: {
                    "Content-Type": "application/json",
                    authorization: `Bearer ${accessTokenStore.accessToken}`
                }
            });

            let responseData: GetPokemonResponse = await response.json();

            this.searchTerm = "";

            if (responseData.error) {
                this.$emit("searchError", responseData.errorMessage)
            } else {
                this.$emit("searchSuccess", responseData.data)
            }
        }
    }
}

</script>

<style scoped>

.submit-button {
    margin-left: 5px;
}

</style>
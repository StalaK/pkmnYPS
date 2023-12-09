
<template>
    <main>
        <div v-show="pokemonFound">
            <SpriteViewer 
                :pokemonName=pokemonData.name
                :spriteUrl=pokemonData.sprite />

            Name: {{ pokemonData.name }} <br />
            Height: {{ pokemonData.height }} <br />
            Weight: {{ pokemonData.weight }} <br />
            Abilities: {{ pokemonData.abilities?.join(', ') ?? "" }} <br />
            Types: {{ pokemonData.types?.join(', ') ?? "" }} <br />
        </div>
        <div class="error" v-show="errorMessage.length > 0">
            {{ errorMessage }}
        </div>
        <PokeFinder 
            @searchSuccess="displayPokemon"
            @searchError="handleError"/>
    </main>
</template>

<script lang="ts">

import { accessTokenStore } from '../store'
import PokeFinder from '../components/PokeFinder.vue'
import SpriteViewer from '../components/SpriteViewer.vue'
import { useRouter } from 'vue-router';
import { handleError } from 'vue';
import type { PokemonData } from '../models/PokemonData'

export default {
    components: {
        PokeFinder,
        SpriteViewer
    },
    setup() {
        const router = useRouter();
        return { router };
    },
    data() {
        return {
            pokemonFound: false,
            errorMessage: "",
            pokemonData: {} as PokemonData
        }
    },
    mounted() {
        let accessTokenPresent = accessTokenStore.accessToken.length > 0;
        
        let currentDate = new Date();
        let accessTokenExpired = currentDate.getTime() > accessTokenStore.accessTokenExpiry;

        if (!accessTokenPresent || accessTokenExpired) {
            this.router.push({ path: '/' })
        }
    },
    methods: {
        displayPokemon(pokemonData: PokemonData) {
            this.errorMessage = "";
            this.pokemonFound = true;
            this.pokemonData = pokemonData;
        },
        handleError(errorMessage: string) {
            this.errorMessage = errorMessage;
        }
    }
}

</script>

<style scoped>

.error {
    color: red;
}

</style>
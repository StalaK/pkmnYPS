<template>
    <div>
        <img :src="spriteUrl" :alt="pokemonName" />
    </div>
</template>

<script lang="ts">

export default {
    props: {
        spriteUrl: String,
        pokemonName: String
    }
}

</script>
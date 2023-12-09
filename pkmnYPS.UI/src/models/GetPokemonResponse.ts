import type { PokemonData } from "./PokemonData";

export interface GetPokemonResponse {
    data: PokemonData,
    error: Boolean,
    httpResponseCode: Number,
    errorMessage: string
}
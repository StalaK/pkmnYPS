export interface PokemonData {
    name: string,
    sprite: string
    height: number,
    weight: number,
    abilities: Array<string> | undefined,
    types: Array<string> | undefined
}

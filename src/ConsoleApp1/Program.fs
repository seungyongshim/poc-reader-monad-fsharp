open System
open FSharpPlus
open FSharpPlus.Data

type IUserRepository =
    abstract GetUser : email : string -> string

type IShoppingListRepository =
    abstract AddToCart : shoppingList : string list -> string

let addShoppingListM email = monad {
    let getUser email = Reader(fun (env : #IUserRepository) -> env.GetUser email)
    let addToShoppingList shoppingListItems = Reader(fun (env : #IShoppingListRepository) -> env.AddToCart shoppingListItems)
    let! _ = getUser email
    let shoppingListItems = ["apple"; "banana"; "orange"]
    return! addToShoppingList shoppingListItems
}

type MockDataEnv() =
    interface IUserRepository with
        member _.GetUser email = "Sandeep"
    interface IShoppingListRepository with
        member _.AddToCart shoppingListItems =
            sprintf "Added the follwing items %A to the cart" shoppingListItems


Reader.run (addShoppingListM "sandeep@test.com") (MockDataEnv()) |> ignore 

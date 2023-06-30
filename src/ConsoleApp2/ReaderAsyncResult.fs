namespace ConsoleApp2

open FSharpPlus.Data

type AsyncResult<'a, 'b> = Async<Result<'a, 'b>>


[<AutoOpen>]
module ReaderAsyncResult =
    let retn x = Reader(fun _ -> async {
        return Ok x
    })

    let bind (f:'a -> Reader<'b, AsyncResult<'c, 'd>>) xActionResult : Reader<'b, AsyncResult<'c, 'd>> =
        let retn x =
            let newAction env = x
            Reader(newAction)
        Reader(fun env ->
            let xAsyncResult = Reader.run xActionResult env 
            async {
                let! xResult = xAsyncResult
                let yAction =
                    match xResult with      
                    | Ok x -> f x
                    | Error e -> retn (
                        async {
                            return (Error e)
                        })
            return! Reader.run yAction env 
            }
        )

    type ReaderAsyncResultBuilder() =
        member _.Return(x) = retn x
        member _.ReturnFrom(x) = x
        member _.Bind(x, f) = bind f x
        member _.Zero() = retn ()

    let readerAsyncResult = ReaderAsyncResultBuilder()

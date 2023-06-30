open ConsoleApp2
open System
open FSharpPlus.Data

type OrganisationId = OrganisationId of Guid 
type IOrganisationIdGenerator =
    abstract GenerateId : unit -> AsyncResult<OrganisationId, string>
type IDataAccess =
    abstract Save : organisationId : OrganisationId -> AsyncResult<unit, string>

let getOrganisationId =
    let generateId = Reader(fun (env : #IOrganisationIdGenerator) -> env.GenerateId())
    readerAsyncResult {
        let! orgId = generateId
        return orgId
    }

type MockEnv =
    interface IOrganisationIdGenerator with
        member _.GenerateId() = async {
            return Ok (OrganisationId Guid.NewGuid())
        }

Reader.run getOrganisationId MockEnv 

namespace Mahamudra.System.Web

//Nancy web api
open Nancy 
//Railway pattern
open Mahamudra.System.Railway

open System.Data
open System.IO
open OCR

// :> obj Up-casting operator. Get function has to return an object
type App() as this =
    inherit NancyModule()
    do
        this.Get.["/"] <- fun _ -> this.Response.AsFile("index.html", "text/html") :> obj
        this.Post.["/uploads"] <- fun _ -> 
            //now we use the bind function from the railway pattern library
            let readTextinImages' = (bind Domain.readTextinImages)
            let result = Domain.uploadImages >> readTextinImages' 
            let result' = result this.Request.Files
//            let a = match result' with
//                    | Success x:(((string * float32) * string) List) *string-> fst x
//                    | Failure y -> List.empty
                     

            let response = this.Response.AsJson(result')
            response :> obj
 
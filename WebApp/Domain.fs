namespace Mahamudra.System.Web

open Mahamudra.System.Railway
open System.IO
open Nancy
open System.Collections.Generic
open Mahamudra.Imaging
open System.Drawing

open Mahamudra.System.Data.Sql  
open System.Data

module Domain =
    let __UPLOAD__ = __SOURCE_DIRECTORY__ + @"\bin\Debug\uploads\"

    [<Literal>]
    let DB_PATH = __SOURCE_DIRECTORY__ + @"\bin\Debug\app_data\catalog.db"

    let CN_STRING = sprintf @"Data Source=%s;Version=3;" DB_PATH

    [<Literal>]
    let INSERT_SUPPLIERS = @"INSERT INTO [prodotti] (name, codice, confidenza) (@name, @codice, @confidenza)"

    let insertProducts (name, codice, confidenza) = 
         Sequel.nonQuery SQLite
                         CN_STRING
                         CommandType.Text 
                         ([("@name",name);("@codice",codice);("@confidenza",confidenza)]|>List.toSeq) INSERT_SUPPLIERS 


    let uploadImages (files:IEnumerable<HttpFile>) =
        try
            let nf = [for f:HttpFile in files do
                            let filename = Path.Combine(__UPLOAD__, f.Name) 
                            use filestr = new FileStream(filename, FileMode.Create)
                            f.Value.CopyTo(filestr)    
                            yield f.Name]
            Success nf
        with
            | ex -> Failure ex.Message

    let readTextinImages (files:string list) =
        try
            let rd = [for f in files do
                        let filename = Path.Combine(__UPLOAD__, f) 
                        use image =  new System.Drawing.Bitmap(filename)
                        yield (OCR.estraiTestodaImmagine(image),f)
                        ] 
            Success rd
        with
            | ex -> Failure ex.Message

    let readTextinImages2 (files:string list) =
        try
            let rd = [for f in files do
                        let filename = Path.Combine(__UPLOAD__, f) 
                        use image =  OCR.convertiImmagineBiancoNero2(filename)
                        yield (OCR.estraiTestodaImmagine(image),f)
                        ] 
            Success rd
        with
            | ex -> Failure ex.Message
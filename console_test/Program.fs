// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.

open Mahamudra.Imaging
open System
open System.IO
open System.Drawing

[<EntryPoint>]
let main argv = 
    let __UPLOAD__ = __SOURCE_DIRECTORY__ + @"\bin\Debug\"
    let fileName ="angels2.png"
    let filePath = Path.Combine(__UPLOAD__, fileName) 
    use image =  new Bitmap(filePath)
    let text = OCR.estraiTestodaImmagine(image) 
    printfn "%s" (fst text)

    use image2 = OCR.convertiImmagineBiancoNero(filePath)
    let b2 = new Bitmap(image2)
    b2.Save(Path.Combine(__UPLOAD__, "angel2.png"))
    let text2 = OCR.estraiTestodaImmagine(image2) 
    printfn "%s" (fst text2)

    use image3 = OCR.convertiImmagineBiancoNero2(filePath)
    let b3 = new Bitmap(image3)
    b3.Save(Path.Combine(__UPLOAD__, "angel3.png"))
    let text3 = OCR.estraiTestodaImmagine(image3) 
    printfn "%s" (fst text3)

    use image4 = OCR.convertiImmagineGrigio(filePath)
    let b4 = new Bitmap(image4)
    b4.Save(Path.Combine(__UPLOAD__, "angels4.png"))
    let text4 = OCR.estraiTestodaImmagine(image4) 

    printfn "%s" (fst text4)
    Console.Read() |> ignore
    0 // return an integer exit code

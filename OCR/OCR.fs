namespace Mahamudra.Imaging
open System
open System.Drawing
open System.Text.RegularExpressions
open System.IO
open ImageProcessor

open Tesseract

open ImageMagick
 
module OCR =

    let convertiImmagineBiancoNero(file_img:string) =
        let photoBytes = File.ReadAllBytes(file_img)
        use inStream = new MemoryStream(photoBytes)
        use outStream = new MemoryStream()
        use imageFactory = new ImageFactory() 
        let imf = imageFactory.Load(inStream)
                    .Contrast(100)
                    .Brightness(0)
                    .Save(outStream) 
        Image.FromStream(outStream)

    let estraiTestodaImmagine (img:Image) = 
        let original = new Bitmap(img)
        use engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default)
        use local_img = PixConverter.ToPix original
        use page = engine.Process local_img
        // Result
        (page.GetText().TrimEnd('\r','\n'), page.GetMeanConfidence())

//    let estraiTestodaImmagine2 (img:Image) = 
//        let original = new Bitmap(img)
//        // Use ImageMagick to process image before OCR
//        // 
//        use img = new MagickImage(original)
//        let cleaner = TextCleanerScript()
//        let cleaned = cleaner.Execute(img).ToBitmap()
//
//        use engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default)
//        use local_img = PixConverter.ToPix cleaned
//        use page = engine.Process local_img
//        // Result
//        (page.GetText().TrimEnd('\r','\n'), page.GetMeanConfidence())

    let filterByConfidence (testo:string, conf:float32) =
        if (conf > 0.51f) then testo else String.Empty

    let estraiCodicedaTesto (testo:string) = 
        let pattern = "[a-zA-Z0-9]{4,}" //at least 4 alphanumeric characters
        let re = new Regex(pattern)
        let m = re.Match(testo) in
            if m.Success then testo else String.Empty
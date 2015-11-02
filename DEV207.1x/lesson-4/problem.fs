open System
open System.IO

type Shot = { 
    angle : float;
    speed : float;
    distance : float;
    name : string 
}

let g = 9.81
let angle x y = atan (y / x)
let angleOfReach distance speed = 0.5 * asin((g * distance) / (pown speed 2))
let distance speed angle = (sin 2.0 * angle) * (pown speed 2) / g

let (|Hit|_|) shot = if abs(shot.angle - (angleOfReach shot.distance shot.speed)) < 0.00000001 then Some() else None
let (|Miss|_|) shot = 
    match angleOfReach shot.distance shot.speed with
    | angle when abs(shot.angle - angle) <  0.00000001 -> Some(1.0)
    | angle when Double.IsNaN angle -> None
    | angle -> Some(angle)

let ParseShot (values : string array) = { 
    angle = angle (float values.[0]) (float values.[1]);
    speed = float values.[2]; 
    distance = float values.[3]; 
    name = values.[4] 
}

let ProcessFile path = File.ReadLines(path) |> Seq.map (fun line -> line.Split(',')) |> Seq.map ParseShot

[<EntryPoint>]    
let main argv = 
    match argv.Length with
    | 0 -> printfn "usage: ./program file1 file2 ..."; 0
    | _ ->
        try
            let shots = Seq.ofArray argv |> Seq.map ProcessFile |> Seq.concat
            for shot in shots do
                match shot with
                | Hit -> printfn "%s hits!" shot.name
                | Miss(angle) -> printfn "%s misses. Angle of %f degrees required." shot.name (angle * 180.0 / Math.PI)
                | _ -> printfn "%s is an impossible shot." shot.name
            0
        with
        | :? System.IO.FileNotFoundException ->
            printfn "error: file not found."
            -1
        | Failure msg ->
            printfn "error: %s" msg
            -1
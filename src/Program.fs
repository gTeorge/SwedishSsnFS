open System

let random = Random()

module Console =
    let log =
        let lockObj = obj()
        fun color bclr s ->
            lock lockObj (fun _ ->
                Console.ForegroundColor <- color
                Console.BackgroundColor <- bclr
                printf "%s" s
                Console.ResetColor())
    let error = log ConsoleColor.Yellow ConsoleColor.DarkRed

module String = 
    let padLeft (num: int) (str: string) =
        str.PadLeft(num, '0')
    let join = (+)

module Int =
    let isEven num = (num % 2) = 0
    let sumDigits num =
        num
        |> string
        |> Seq.toList
        |> List.map (fun char -> char |> string |> int)
        |> List.sum
    let lastDigit num =
        num % 10

module Dates = 
    let startDate = DateTime(1901, 1, 1)
    let range = (DateTime.Today - startDate).Days
    let getRandomDate = startDate.AddDays(random.Next(range) |> float).ToString("yyMMdd")

module Generator =
    let addRandomNums addedDigits (text: string) = 
        random.Next(addedDigits*10)
        |> string
        |> String.padLeft addedDigits
        |> String.join text

    let addCheckSum (ssn: string) =
        10 - (ssn
            |> Seq.toList
            |> List.mapi (fun i char -> char 
                                     |> string 
                                     |> int
                                     |> function
                                        | x when Int.isEven i -> x * 2 
                                        | x -> x
                                     |> Int.sumDigits)
            |> List.sum
            |> Int.lastDigit)
        |> Int.lastDigit
        |> string
        |> String.join ssn

    let getPrivateSSN = 
        Dates.getRandomDate
        |> addRandomNums 3
        |> addCheckSum

    let getOrganisationSSN =
        (Dates.getRandomDate |> int) + 2000 
        |> string
        |> String.padLeft 6
        |> addRandomNums 3
        |> addCheckSum



[<EntryPoint>]
let main argv =
    match argv with
    | [||] -> Generator.getPrivateSSN
              |> String.join "Private SSN:       "
              |> printfn "%s"
              Generator.getOrganisationSSN
              |> String.join "Organisation SSN:  "
              |> printfn "%s"
    | _ -> "Unexpected run parameters"
           |> sprintf "%s"
           |> Console.error
    0

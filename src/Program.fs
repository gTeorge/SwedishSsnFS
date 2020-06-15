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
    let blue = log ConsoleColor.Yellow ConsoleColor.DarkBlue
    let red = log ConsoleColor.Yellow ConsoleColor.DarkRed

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

    let getOrganizationSSN =
        (Dates.getRandomDate |> int) + 20000 
        |> string
        |> addRandomNums 3
        |> addCheckSum



[<EntryPoint>]
let main argv =
    Generator.getPrivateSSN
    |> String.join "Private SSN:       "
    |> sprintf "%s"
    |> Console.blue
    printfn ""
    Generator.getOrganizationSSN
    |> String.join "Organization SSN:  "
    |> sprintf "%s"
    |> Console.red
    0

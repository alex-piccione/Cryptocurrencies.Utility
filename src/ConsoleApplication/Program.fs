open Spectre.Console

AnsiConsole.Write(FigletText("Cryptocurrencies Utility", Color=Color.CadetBlue))
AnsiConsole.WriteLine ""

let rec execute () =
    //AnsiConsole.Clear()
    let action = 
        AnsiConsole.Prompt(SelectionPrompt<string>(Title="What do you want to do?")
            .AddChoices("Generate Ethereum address", "Generate XRP address", "Exit"))
    
    match action with
    | "Exit" -> AnsiConsole.WriteLine "Bye bye"
    | "Generate Ethereum address" -> 
        AnsiConsole.Clear()
        AnsiConsole.MarkupLine "[blue]Generate Ethereum Address[/]\n"

        let address = utility.GenerateNewAddress()
        AnsiConsole.MarkupLine $"Private Key: [yellow]{address.PrivateKeyHex}[/]"
        AnsiConsole.MarkupLine $"Address    : [yellow]{address.Address}[/]"
    | "Generate XRP address" -> 

        AnsiConsole.Clear()
        AnsiConsole.MarkupLine "[blue]Generate XRP Address[/]\n"

        let address = xrpl.main.GenerateNewAddress()
        AnsiConsole.MarkupLine $"Private Key: [yellow]{address.PrivateKeyHex}[/]"
        AnsiConsole.MarkupLine $"Address    : [yellow]{address.Address}[/]"

    //| "Generate TOTP code" ->
    //    if secretKey.IsNone
    //    then secretKey <- Some(AnsiConsole.Ask<string>("Secret Key:"))
    //    TOTP.GenerateCode (secretKey.Value)

    | _ -> AnsiConsole.WriteLine $"Unknown action: {action}"

    if action <> "Exit" then execute()

execute()

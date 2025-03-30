open Spectre.Console

AnsiConsole.Write(FigletText("Cryptocurrencies Utility", Color=Color.CadetBlue))
AnsiConsole.WriteLine ""

let rec execute () =
    //AnsiConsole.Clear()
    let action = 
        AnsiConsole.Prompt(SelectionPrompt<string>(Title="What do you want to do?")
            .AddChoices("Generate Ethereum address", "Generate XRPL Account", "Exit"))
    
    match action with
    | "Exit" -> AnsiConsole.WriteLine "Bye bye"
    | "Generate Ethereum address" -> 
        AnsiConsole.Clear()
        AnsiConsole.MarkupLine "[blue]Generate Ethereum Address[/]\n"

        let address = utility.GenerateNewAddress()
        AnsiConsole.MarkupLine $"Private Key: [yellow]{address.PrivateKeyHex}[/]"
        AnsiConsole.MarkupLine $"Address    : [yellow]{address.Address}[/]"
    | "Generate XRPL Account" -> 

        AnsiConsole.Clear()
        AnsiConsole.MarkupLine "[blue]Generate XRPL Account[/]\n"

        //for x in [1..1000000] do
        let account = xrpl.main.GenerateAccount()
        AnsiConsole.MarkupLine $"Address: [yellow]{account.Address}[/]"
        AnsiConsole.MarkupLine $"Secret : [yellow]{account.Secret}[/]"
        

    //| "Generate TOTP code" ->
    //    if secretKey.IsNone
    //    then secretKey <- Some(AnsiConsole.Ask<string>("Secret Key:"))
    //    TOTP.GenerateCode (secretKey.Value)

    | _ -> AnsiConsole.MarkupLine $"[red]Unknown action: {action}[/]"

    if action <> "Exit" then execute()

execute()

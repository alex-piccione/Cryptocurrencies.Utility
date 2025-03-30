namespace tests.xrpl

open NUnit.Framework
open FsUnit
open xrpl.main
open test_data


[<TestFixture>]
type XRPL() =

    // from: https://xrpl.org/docs/concepts/accounts/addresses#address-encoding
    [<TestCase("9434799226374926EDA3B54B1B461B4ABF7237962EAE18528FEA67595397FA32", "rDTXLQ7ZKZVKz33zJbHjgVShjsBnqMBhmN")>]
    member _.``Verify address derivation from known public key``(publicKeyHex: string, expectedAddress: string) =
        let publicKeyBytes = System.Convert.FromHexString(publicKeyHex)
        let address = xrpl.main.deriveAddress publicKeyBytes
        address |> should equal expectedAddress

    [<Test>]
    member _.``Generate Account (Ed25519)`` () =
        for case in test_data.Ed25519Cases do
            let account = xrpl.main.generateAccountFromSeed case.Seed
            account.Address |> should equal case.Address

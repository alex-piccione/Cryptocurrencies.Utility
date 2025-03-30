module xrpl.main

open System
open System.Security.Cryptography
open Org.BouncyCastle.Crypto.Digests
open Org.BouncyCastle.Crypto.Parameters

type PaymentRequest = { From:string; To:string; ToTag:string option; Amount: decimal; Note: string option}
    
let ExecutePayment (request:PaymentRequest) =
 
    ()



type GeneratedAccount = { Address:string; Secret: string}

let private generateSeed () : byte[] =
    let rng = RandomNumberGenerator.Create()
    let seed = Array.zeroCreate<byte> 16
    rng.GetBytes(seed)
    seed

let private deriveSecret (seed: byte[]) : string =
    // Secret format: [0x21][16-byte seed][4-byte checksum]
    let payload = Array.append [| 0x21uy |] seed
    use sha256 = SHA256.Create()
    let checksum = sha256.ComputeHash(sha256.ComputeHash(payload)) |> Array.take 4
    Array.concat [ payload; checksum ] |> base58.Encode

let deriveAddress (publicKey: byte array) : string =

    // 1. Prepend Ed25519 prefix (0xED)
    let masterPublicKey = Array.append [|0xEDuy|] publicKey

    // 2. Calculate the Account ID (RIPEMD-160 of SHA-256 of the public key)
    let sha256 = SHA256.Create()
    let ripemd160Digest = new RipeMD160Digest()
    let sha256Hash = sha256.ComputeHash(masterPublicKey)

    ripemd160Digest.BlockUpdate(sha256Hash, 0, sha256Hash.Length)
    let accountId = Array.zeroCreate<byte> (ripemd160Digest.GetDigestSize())
    ripemd160Digest.DoFinal(accountId, 0) |> ignore

    // 3. Add version byte (0x00) and checksum
    let payload = Array.append [|0x00uy|] accountId
    let checksum = sha256.ComputeHash(sha256.ComputeHash(payload)) |> Array.take 4

    // 4. Base58 encode (using correct alphabet order)
    Array.concat [ payload; checksum ] |> base58.Encode


let internal generateAccountFromSeed (seed:byte array) =

    use sha512 = SHA512.Create()

    let secret = deriveSecret seed
   
    let privateKey = sha512.ComputeHash(seed) |> Array.take 32

    let privateKeyParameter = Ed25519PrivateKeyParameters(privateKey, 0)
    let publicKey = privateKeyParameter.GeneratePublicKey().GetEncoded()

    let address = deriveAddress publicKey

    { Address = address; Secret = secret }

let GenerateAccount () : GeneratedAccount =
    let seed = generateSeed()
    generateAccountFromSeed seed
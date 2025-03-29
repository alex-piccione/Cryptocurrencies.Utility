module xrpl.main

open System.Security.Cryptography
open Org.BouncyCastle.Crypto
open Org.BouncyCastle.Crypto.Generators
open Org.BouncyCastle.Crypto.Parameters
open Org.BouncyCastle.Security
open Org.BouncyCastle.Crypto.Digests
open xrpl.base58

type GeneratedAddress = { Address:string; PrivateKeyHex: string}

// Generate an Ed25519 key pair
let private generateEd25519KeyPair () : AsymmetricCipherKeyPair =
    let keyPairGenerator = new Ed25519KeyPairGenerator()
    let secureRandom = new SecureRandom()
    let keyGenerationParameters = KeyGenerationParameters(secureRandom, 256) // 256 bits for Ed25519
    keyPairGenerator.Init(keyGenerationParameters)
    keyPairGenerator.GenerateKeyPair()

// Get the private and public keys bytes from the key pair
let private getKeysBytes (keyPair : AsymmetricCipherKeyPair) : byte array * byte array =
    let privateKeyParams = keyPair.Private :?> Ed25519PrivateKeyParameters
    let publicKeyParams = keyPair.Public :?> Ed25519PublicKeyParameters
    privateKeyParams.GetEncoded(), publicKeyParams.GetEncoded()


// Derive the XRP address from the public key bytes
let deriveAddress (publicKeyBytes : byte array) : string =
    // 1. Calculate the Account ID (RIPEMD-160 of SHA-256 of the public key)
    let sha256 = SHA256.Create()
    let ripemd160Digest = new RipeMD160Digest()
    let sha256Hash = sha256.ComputeHash(publicKeyBytes)

    ripemd160Digest.BlockUpdate(sha256Hash, 0, sha256Hash.Length)
    let accountId = Array.zeroCreate<byte> (ripemd160Digest.GetDigestSize())
    ripemd160Digest.DoFinal(accountId, 0) |> ignore

    // 2. XRP address version byte (0x00)
    let versionByte = [| byte 0 |]

    // 3. Combine version byte and Account ID
    let payload = Array.concat [ versionByte; accountId ]

    // 4. Calculate the checksum (first 4 bytes of SHA-256 hash of the payload)
    let hash = SHA256.Create().ComputeHash(payload)
    let checksum = Array.sub hash 0 4

    // 5. Combine payload and checksum
    let addressBytes = Array.concat [ payload; checksum ]

    // 6. Encode to Base58 using BogaNet.Encoder
    let address = XrpBase58.Encode addressBytes

    address


let GenerateNewAddress () =
    let keyPair = generateEd25519KeyPair ()
    let privateKeyBytes, publicKeyBytes = getKeysBytes keyPair
    let privateKeyHex = System.Convert.ToHexString privateKeyBytes
    let address = deriveAddress publicKeyBytes

    {Address=address; PrivateKeyHex=privateKeyHex}


type PaymentRequest = { From:string; To:string; ToTag:string option; Amount: decimal; Note: string option}

let ExecutePayment (request:PaymentRequest) =


    ()
module utility



open System
open System.Security.Cryptography
open Org.BouncyCastle.Crypto.Digests
open Org.BouncyCastle.Utilities
open Org.BouncyCastle.Math
open Org.BouncyCastle.Asn1.X9

type GeneratedAddress = {Address: string; PrivateKeyHex: string}

/// Generate a random private key, represented in Base64 and HEX
let private generatePrivateKey () : string * string =
    let privateKeyBytes = Array.zeroCreate<byte> 32
    RandomNumberGenerator.Fill(privateKeyBytes)
    let base64 = Convert.ToBase64String(privateKeyBytes)
    let hex = Convert.ToHexString(privateKeyBytes)
    base64, hex


let private derivePublicKey (privateKeyBase64 : string) : string =
    let privateKeyBytes = Convert.FromBase64String privateKeyBase64
    let privateKeyBigInt = new BigInteger(1, privateKeyBytes)
    let domainParams = ECNamedCurveTable.GetByName("secp256k1")

    let g = domainParams.G
    let q = g.Multiply(privateKeyBigInt).Normalize()
    let publicKeyHex = "04" + q.XCoord.ToBigInteger().ToString(16).PadLeft(64, '0') + q.YCoord.ToBigInteger().ToString(16).PadLeft(64, '0')
    publicKeyHex



let private deriveEthereumAddress (publicKeyHex : string) : string =
    let publicKeyBytes = Encoders.Hex.DecodeStrict publicKeyHex
    // Remove the first byte (0x04) which indicates uncompressed public key
    let publicKeyUncompressed = publicKeyBytes[1..]

    let keccak256Digest = new KeccakDigest(256)
    keccak256Digest.BlockUpdate(publicKeyUncompressed, 0, publicKeyUncompressed.Length)
    let keccak256HashBytes = Array.zeroCreate<byte> 32
    keccak256Digest.DoFinal(keccak256HashBytes, 0) |> ignore

    // Take the last 20 bytes (40 hex characters)
    let addressBytes = keccak256HashBytes[12..]
    let encodedAddressBytes = Encoders.Hex.Encode( addressBytes, 0, addressBytes.Length)
    "0x" + System.Text.Encoding.ASCII.GetString(encodedAddressBytes)

let GenerateNewAddress () =
    let base64, hex = generatePrivateKey ()
    let publicKeyHex = derivePublicKey base64
    let address = deriveEthereumAddress publicKeyHex

    {Address=address; PrivateKeyHex=hex}



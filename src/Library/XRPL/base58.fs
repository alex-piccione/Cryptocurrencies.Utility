module xrpl.base58

open System

type XrpBase58() =
   static let alphabet = "rpshnaf39wBUDNEGHJKLM4PQRST7VWXYZ2bcdeCg65jkm8oFqi1tuvAxyz"

   static member internalEncode(input: ReadOnlySpan<byte>, output: Span<char>, numZeroes: int) : int =
       if numZeroes = input.Length then
           // Fill with first alphabet character for all-zero input
           if output.Length < numZeroes then 0
           else
               output.Slice(0, numZeroes).Fill(alphabet[0])
               numZeroes
       else
           // Create a big integer representation of the input
           let mutable num = 0I
           for i = 0 to input.Length - 1 do
               num <- num * 256I + bigint input.[i]
           
           // Encode to base 58
           let mutable encoded = ResizeArray<char>()
           let mutable n = num
           
           while n > 0I do
               let r = int (n % 58I)
               encoded.Insert(0, alphabet.[r])
               n <- n / 58I
           
           // Prepend zeros
           let zeroPrefix = String(alphabet.[0], numZeroes)
           let encodedStr = zeroPrefix + String(encoded.ToArray())
           
           // Copy to output
           let charsToCopy = min encodedStr.Length output.Length
           for i = 0 to charsToCopy - 1 do
               output.[i] <- encodedStr.[i]
           
           charsToCopy

   static member Encode(bytes: byte[]) : string =
       if bytes.Length = 0 then raise (invalidArg "bytes" "Cannot be empty")

       let numZeroes = 
           let rec countZeros index =
               if index >= bytes.Length then index
               elif bytes.[index] = 0uy then countZeros (index + 1)
               else index
           countZeros 0

       let outputLen = 
           let growthPercentage = 138
           numZeroes + ((bytes.Length - numZeroes) * growthPercentage / 100) + 1

       let output = Array.zeroCreate<char> outputLen
       let numCharsWritten = XrpBase58.internalEncode(ReadOnlySpan<byte>(bytes), (Span<char>(output)), numZeroes)

       if numCharsWritten > 0 then
           String(output, 0, numCharsWritten)
       else
           raise (InvalidOperationException("Output buffer with insufficient size generated"))
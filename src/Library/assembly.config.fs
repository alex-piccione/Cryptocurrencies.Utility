module assembly
open System.Runtime.CompilerServices

[<assembly: InternalsVisibleTo("UnitTests")>]

do () // This line is just to ensure the attribute is at the assembly level
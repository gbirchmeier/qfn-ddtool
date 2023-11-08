# qfn-ddtool
DataDictionary analyzer/codegen for QuickFIX/n


## In development - Not intended for use yet

Feel free to look around, but this is in early development.

When it's worth something, I'll announce it on the [QuickFIX/n mailing list](http://quickfixn.org/help/).

## To run

To parse DDs, but only analyze (not generate):  
`> dotnet run --project DDTool <ddFile> <ddFile>...`

To parse DDs and generate:  
`> dotnet run --project DDTool --outputdir <qfRepoDir> <ddFile> <ddFile>...`

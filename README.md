# README #

c# coding assignment

## How to use

This tool parses standard input and print results to standard output. Default output style is CSV.

The following option is available:
  "-xml" set XML output style.

Example usages:

"type infile.txt | rx > outfile.txt"
"type infile.txt | rx -xml > outfile.xml"
"rx -xml"

## Implementation notes

I chose Microsoft Tasks.Dataflow to perform async tasks chaining.
The sentence splitter is simple, it doesn't support multiline sentences. 
Despite it could be implemented easily by using simple streaming lexer.

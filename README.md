# File Transfer Tool for Hornetsecurity's code challenge

## Description
Simple console app used for copying the contents of a source file to a destination file.

## Features
- Path validation for source and destination
- Secure transfer using hashing algorithms to check each transferred block, as well as final files' checksum
- Utilization of multithreading to improve transfer operation performance
- RNG-based file generation script to produce testing samples
- Unit testing to ensure correct app functionality

## Testing
- For automated testing: Run FileTransferTool.Tests in VS Test Explorer
- For manual testing:
	- Configure FileTransferTool.TestFileGenerator as solitary startup project
	- Specify file size in respective Program.cs file
	- Run to produce test sample file on your local desktop
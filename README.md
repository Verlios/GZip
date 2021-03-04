# GZip
This is test task for show working GZip
# Description
For the successful implementation of this task, it was decided to use the Producer / Consumer pattern.
The program accepts an array of strings through the command line or the Visual Studio console. Validates the data and passes parameters to the appropriate method for compressing or decompressing the file. Compression and decompression occurs in a multi-threaded environment where the number of threads corresponds to the number of cores. During compression, the full length of the file is read, in the process the file is divided into blocks of the same size, which are transferred to the queue for further compression, in parallel identifying these same blocks with a unique id. The compression method extracts a block of bytes from the queue and compresses them, then writes them to another queue for writing to a file. Before directly writing the compressed data, we get and write the length of the compressed block. If this is not done, then we will not be able to decompress the files, because in fact the program will take blocks of random length and transfer them for decompression.

When uncompressing, the first 8 bytes are read, through which we find out how much more needs to be read before the end of the block. The block is read and transferred to decompression. The result is written to the output file. The iteration is repeated.

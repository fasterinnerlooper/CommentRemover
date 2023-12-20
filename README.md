# CommentRemover

The Comment Remover is designed specifically to remove comments from parquet files that contain C# code.
The code is specifically designed to process files found at hf.co/datasets/microsoft/LCC_csharp, but it could be altered to work with a parquet dataset of any type with minor modifications to the DataReader class, specifically lines 40 and 41.

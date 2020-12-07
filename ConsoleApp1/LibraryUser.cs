using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace ConsoleApp1
{
    public class LibraryUser
    {
        Setup set;

        public LibraryUser(Setup set)
        {
            this.set = set;
        }

        public async Task WriteTextAsync(TestInfo ti)
        {
            using (StreamWriter writer = new StreamWriter(this.set.outputPath+ti.fileName + "Test.cs"))
            {
                await writer.WriteAsync(ti.test);
            }
        }
        public async Task<string> ReadFileAsync(string src)
        {
            using (StreamReader reader = new StreamReader(src))
            {
                return await reader.ReadToEndAsync();
            }
        }
        public async Task gen()
        {
            DataflowLinkOptions linkOptions = new DataflowLinkOptions
            {
                PropagateCompletion = true
            };
            var readOptions = new ExecutionDataflowBlockOptions
            {MaxDegreeOfParallelism = set.paralelFilesLoaded};
            var writeOptions = new ExecutionDataflowBlockOptions
            {MaxDegreeOfParallelism = set.paralelFilesWriten};
            var processOptions = new ExecutionDataflowBlockOptions
            {MaxDegreeOfParallelism = set.paralelTasksProcessed};
            ItestGen itg = new TestGen();
            var readerBlock = new TransformBlock<string, Task<string>>(src => ReadFileAsync(src), readOptions);
            var generatorBlock = new TransformManyBlock<Task<string>, TestInfo>(src => itg.generate(src.Result), writeOptions);
            var saverBlock = new ActionBlock<TestInfo>(src => WriteTextAsync(src), processOptions);
            readerBlock.LinkTo(generatorBlock, linkOptions);
            generatorBlock.LinkTo(saverBlock, linkOptions);
            Console.WriteLine(readerBlock.Completion.Status + " " + generatorBlock.Completion.Status + " " + saverBlock.Completion.Status);
            foreach (string src in set.inputPath)
            {
                Console.WriteLine(src);
                await readerBlock.SendAsync(src);
            }
           
            Console.WriteLine(readerBlock.Completion.Status + " " + generatorBlock.Completion.Status + " " + saverBlock.Completion.Status);
            readerBlock.Complete();
      
            await saverBlock.Completion;


        }

    }
}

using System;
using BlazorInputFile;
using System.Threading.Tasks;

namespace Parser.Services
{
    public interface IFileOperations
    {
        Task Upload(IFileListEntry file);
        String Read(IFileListEntry file);
        String Parse(String rawContent);
        void Write(IFileListEntry file,String errorMessage);
    }
}

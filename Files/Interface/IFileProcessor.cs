using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Files.Interface
{
    public interface IFileProcessor
    {
        Task ProcessFilesAsync();
    }
}

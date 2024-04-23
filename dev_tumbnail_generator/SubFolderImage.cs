using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace dev_tumbnail_generator
{
    public class SubFolderImage
    {
        [FunctionName("SubFolderImage")]
        public static void Run([BlobTrigger("archive/{folder}/{subfolder}/image/new-{name}.{extension}", Connection = "connection_blob_dev_inprinty")] Stream imageStream,
            string folder,
            string subfolder,
            string name,
            string extension,
            ILogger log)
        {
            ImageHelper.ProcessImage(imageStream, folder, subfolder, name, extension, log);
        }
    }
}

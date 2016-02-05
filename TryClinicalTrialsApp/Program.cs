using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TryClinicalTrialsApp
{
    class Program
    {
        static void Main(string[] args)
        {
            RunAsync().Wait();
        }

        static async Task RunAsync()
        {
            DoFunThingsWithFiles(@"C:\Users\Ryan\AppData\Local\Temp\0ztc3jb0.cjn");
            return;
            var tmpDir = GetTemporaryDirectory();

            var search = @"https://clinicaltrials.gov/ct2/search?term=diabetes&recr=Open&rslt=&type=&cond=&intr=&titles=&outc=&spons=&lead=&id=&state1=NA%3AUS%3AIL&cntry1=&state2=&cntry2=&state3=&cntry3=&locn=&gndr=&rcv_s=&rcv_e=&lup_s=&lup_e=&studyxml=true";
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(search);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));

                HttpResponseMessage result = await client.GetAsync("");

                var zipPath = Path.Combine(tmpDir, Path.GetRandomFileName());
                using (var zipStream = await result.Content.ReadAsStreamAsync())
                {
                    using (var fileStream = new FileStream(zipPath, FileMode.Create))
                    {
                        await zipStream.CopyToAsync(fileStream);
                    }
                }

                ZipFile.ExtractToDirectory(zipPath, tmpDir);
                File.Delete(zipPath);

                DoFunThingsWithFiles(zipPath);

                Directory.EnumerateFiles(tmpDir);

                var x = 1 + 1;
                Console.WriteLine(x);
                //var trial = await result.Content.ReadAsAsync<clinical_study>();
            }

            Directory.Delete(tmpDir);
        }

        private static void DoFunThingsWithFiles(string directory)
        {
            var file = Directory.EnumerateFiles(directory).First();
            var serializer = new XmlSerializer(typeof(clinical_study));
            clinical_study study;
            using (var xmlStream = new StreamReader(file))
            {
                study = (clinical_study)new XmlSerializer(typeof(clinical_study)).Deserialize(xmlStream);                
            }
            Console.WriteLine(study.acronym);
            
        }

        static string GetTemporaryDirectory()
        {
            string tempDirectory;
            do
            {
                tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            } while (Directory.Exists(tempDirectory));
            Directory.CreateDirectory(tempDirectory);
            Console.WriteLine(tempDirectory);
            return tempDirectory;
        }
    }
}

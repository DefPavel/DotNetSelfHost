using DotNetSelfHost.WinForms.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace DotNetSelfHost.WinForms.Services
{
    public static class FileService
    {
        public static async Task<FileStatus> JsonPostWithToken(string token, string queryUrl, string httpMethod, string reportName)
        {

#pragma warning disable SYSLIB0014 // Тип или член устарел
            var req = (HttpWebRequest)WebRequest.Create("http://localhost:8080/api" + queryUrl);     // Создаём запрос
#pragma warning restore SYSLIB0014 // Тип или член устарел
            req.Method = httpMethod;                                            // Выбираем метод запроса
            req.Headers.Add("auth-token", token);
            req.Accept = "application/json";

            using var response = await req.GetResponseAsync();

            await using var responseStream = response.GetResponseStream();

            return SaveReport(responseStream, reportName);

        }

        private static FileStatus SaveReport(Stream inputStream, string reportName)
        {
            // Получить путь рабочего стола
            //var path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var path = "C:";

            var file = path + $"\\Документы-Программы\\{reportName.Replace(" ", "_")}_{DateTime.Now:yyyy-MM-dd-HH-mm-ss}.docx";
            if (Directory.Exists(path + $"\\Документы-Программы"))
            {
                //using FileStream outputFileStream = new(file, FileMode.Create);
                using var outputFileStream = new FileStream(file, FileMode.Create);
                inputStream.CopyTo(outputFileStream);
            }
            else
            {

                Directory.CreateDirectory(path + $"\\Документы-Программы");
                using var outputFileStream = new FileStream(file, FileMode.Create);
                inputStream.CopyTo(outputFileStream);

            }
            inputStream.Dispose();
            var docxUrl = EnumerateAllFiles("C:\\Program Files", "WINWORD.exe");
            if (docxUrl.Any())
            {
                _ = Process.Start(docxUrl.First(), file);
                return new FileStatus { Name = file , Status = true};

            }
            else return new FileStatus { Name= file , Status = false};

        }
        public static IEnumerable<string> EnumerateAllFiles(string path, string pattern)
        {
            IEnumerable<string> files = null;
            try { files = Directory.EnumerateFiles(path, pattern); }
            catch { }

            if (files != null)
            {
                foreach (var file in files) yield return file;
            }

            IEnumerable<string> directories = null;
            try { directories = Directory.EnumerateDirectories(path); }
            catch { }

            if (directories != null)
            {
                foreach (var file in directories.SelectMany(d => EnumerateAllFiles(d, pattern)))
                {
                    yield return file;
                }
            }
        }
    }
}

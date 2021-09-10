using System;
using System.Threading.Tasks;
using BlazorInputFile;
using Parser.Services;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Text;

namespace Parser.Data
{
    public class FileOperations : IFileOperations
    {
        private readonly IWebHostEnvironment _oWebHostEnvironment;
        public FileOperations(IWebHostEnvironment oWebHostEnvironment)
        {
            _oWebHostEnvironment = oWebHostEnvironment;
        }
        public async Task Upload(IFileListEntry file)
        {
            var path = Path.Combine(_oWebHostEnvironment.ContentRootPath, file.Name);
            var MemoryStream = new MemoryStream();
            await file.Data.CopyToAsync(MemoryStream);

            using (FileStream fileStream = new FileStream(path, FileMode.Create, FileAccess.ReadWrite))
            {
                MemoryStream.WriteTo(fileStream);
            }
        }
        public String Read(IFileListEntry file)
        {
            var path = Path.Combine(_oWebHostEnvironment.ContentRootPath, file.Name);
            return File.ReadAllText(path);
        }

        public String Parse(String rawContent)
        {
            StringBuilder parsedText = new StringBuilder();
            try
            {
                string[] lines = rawContent.Split("[EOL]");
                for (int j = 0; j < lines.Length - 1; j++)
                {
                    string[] eventDetails = lines[j].Split(';');
                    for (int i = 0; i < eventDetails.Length; i++)
                    {
                        switch (i)
                        {
                            case 0:
                                if (eventDetails[i].Length == 0 || eventDetails[i].Length > 32)
                                    throw new Exception(Constants.EVENT_NAME_EMPTY_OR_TOO_LONG);
                                break;
                            case 1:
                                if (eventDetails[i].Length == 0 || eventDetails[i].Length > 255)
                                    throw new Exception(Constants.EVENT_DESCRIPTION_EMPTY_OR_TOO_LONG);
                                break;
                            case int n when n == 2 || n == 3:
                                if (!ValidateDateFormat(eventDetails[i]))
                                    throw new Exception(Constants.INCORRECT_DATE_FORMAT);
                                break;
                            case int n when n > 3 && eventDetails[i].Length > 0:
                                throw new Exception(Constants.TOO_MANY_DETAILS);
                        }
                        parsedText.Append(eventDetails[i] + " ");
                    }
                    parsedText.AppendLine("\n");
                }
            }
            catch(Exception ex)
            {
                return Constants.ERROR + ex.ToString(); ;
            }
            return parsedText.ToString();
        }

        public void Write(IFileListEntry file, String errorMessage)
        {
            var path = Path.Combine(_oWebHostEnvironment.ContentRootPath, file.Name);
            File.WriteAllText(path, errorMessage);
        }

        public bool ValidateDateFormat(String date)
        {
            try
            {
                DateTime.ParseExact(date, "yyyy-MM-ddTHH:mm", System.Globalization.CultureInfo.InvariantCulture);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}

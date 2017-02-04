using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Hosting;

namespace DeliveryService.Helpers
{
    public class FilesHelper
    {
        readonly string _deleteUrl;
        readonly string _deleteType;
        readonly string _storageRoot;
        readonly string _urlBase;
        readonly string _tempPath;
        readonly string _serverMapPath ;
        public FilesHelper(FileUploadConfig config)
        {
            this._deleteUrl = config.DeleteUrl;
            this._deleteType = config.DeleteType;
            this._storageRoot = config.StorageRoot;
            this._urlBase = config.UrlBase;
            this._tempPath = config.TempPath;
            this._serverMapPath = config.ServerMapPath;
        }

        public void DeleteFiles(String pathToDelete)
        {

            string path = HostingEnvironment.MapPath(pathToDelete);

            System.Diagnostics.Debug.WriteLine(path);
            if (Directory.Exists(path))
            {
                DirectoryInfo di = new DirectoryInfo(path);
                foreach (FileInfo fi in di.GetFiles())
                {
                    System.IO.File.Delete(fi.FullName);
                    System.Diagnostics.Debug.WriteLine(fi.Name);
                }

                di.Delete(true);
            }
        }

        public string DeleteFile(string file)
        {
            System.Diagnostics.Debug.WriteLine("DeleteFile");
            System.Diagnostics.Debug.WriteLine(file);

            string fullPath = Path.Combine(_storageRoot, file);
            System.Diagnostics.Debug.WriteLine(fullPath);
            System.Diagnostics.Debug.WriteLine(System.IO.File.Exists(fullPath));
            string thumbPath = "/" + file + ".80x80.jpg";
            string partThumb1 = Path.Combine(_storageRoot, "thumbs");
            string partThumb2 = Path.Combine(partThumb1, file + ".80x80.jpg");

            System.Diagnostics.Debug.WriteLine(partThumb2);
            System.Diagnostics.Debug.WriteLine(System.IO.File.Exists(partThumb2));
            if (System.IO.File.Exists(fullPath))
            {
                //delete thumb 
                if (File.Exists(partThumb2))
                {
                    File.Delete(partThumb2);
                }
                File.Delete(fullPath);
                string succesMessage = "Ok";
                return succesMessage;
            }
            String failMessage = "Error Delete";
            return failMessage;
        }
        public JsonFiles GetFileList()
        {

            var r = new List<ViewDataUploadFilesResult>();

            String fullPath = Path.Combine(_storageRoot);
            if (Directory.Exists(fullPath))
            {
                DirectoryInfo dir = new DirectoryInfo(fullPath);
                foreach (FileInfo file in dir.GetFiles())
                {
                    int sizeInt = unchecked((int)file.Length);
                    r.Add(UploadResult(file.Name, sizeInt, file.FullName));
                }

            }
            JsonFiles files = new JsonFiles(r);

            return files;
        }

        public JsonFiles GetFile(string fileName)
        {

            var r = new List<ViewDataUploadFilesResult>();

            String fullPath = Path.Combine(_storageRoot);
            if (Directory.Exists(fullPath))
            {
                DirectoryInfo dir = new DirectoryInfo(fullPath);
                foreach (FileInfo file in dir.GetFiles())
                {
                    int sizeInt = unchecked((int)file.Length);
                    if (fileName == file.Name)
                        r.Add(UploadResult(file.Name, sizeInt, file.FullName));
                }

            }
            JsonFiles files = new JsonFiles(r);

            return files;
        }

        public void UploadAndShowResults(HttpContextBase contentBase, List<ViewDataUploadFilesResult> resultList)
        {
            var httpRequest = contentBase.Request;
            System.Diagnostics.Debug.WriteLine(Directory.Exists(_tempPath));

            String fullPath = Path.Combine(_storageRoot);
            Directory.CreateDirectory(fullPath);
            Directory.CreateDirectory(fullPath + "/thumbs/");

            foreach (String inputTagName in httpRequest.Files)
            {

                var headers = httpRequest.Headers;

                var file = httpRequest.Files[inputTagName];
                System.Diagnostics.Debug.WriteLine(file.FileName);

                if (string.IsNullOrEmpty(headers["X-File-Name"]))
                {

                    UploadWholeFile(contentBase, resultList);
                }
                else
                {

                    UploadPartialFile(headers["X-File-Name"], contentBase, resultList);
                }
            }
        }


        private void UploadWholeFile(HttpContextBase requestContext, List<ViewDataUploadFilesResult> statuses)
        {

            var request = requestContext.Request;
            for (int i = 0; i < request.Files.Count; i++)
            {
                var file = request.Files[i];
                String pathOnServer = Path.Combine(_storageRoot);
                var fullPath = Path.Combine(pathOnServer, Path.GetFileName(file.FileName));

                string newPath = "";
                if (File.Exists(fullPath))
                {
                    var splited = fullPath.Split('.');


                    for (int j = 0; j < splited.Length; j++)
                    {
                        if (j != splited.Length - 1)
                        {
                            newPath += splited[j];
                        }
                    }
                    newPath += DateTime.Now.Month.ToString() +
                                DateTime.Now.Day.ToString() +
                                DateTime.Now.Year.ToString() +
                                DateTime.Now.Hour.ToString() +
                                DateTime.Now.Minute.ToString() +
                                DateTime.Now.Second.ToString() + "." +
                                splited[splited.Length - 1];

                }
                if (!string.IsNullOrEmpty(newPath))
                    fullPath = newPath;

                file.SaveAs(fullPath);

                string currentFileName = fullPath.Split('\\')[fullPath.Split('\\').Length - 1];

                //Create thumb
                string[] imageArray = currentFileName.Split('.');
                if (imageArray.Length != 0)
                {
                    string extansion = imageArray[imageArray.Length - 1];
                    if (extansion != "jpg" && extansion != "png" && extansion != "jpeg")
                    {

                    }
                    else
                    {
                        var thumbfullPath = Path.Combine(pathOnServer, "thumbs");
                        string fileThumb = currentFileName + ".80x80.jpg";
                        var thumbfullPath2 = Path.Combine(thumbfullPath, fileThumb);
                        using (MemoryStream stream = new MemoryStream(File.ReadAllBytes(fullPath)))
                        {
                            var thumbnail = new WebImage(stream).Resize(200, 200);
                            thumbnail.Save(thumbfullPath2, "jpg");
                        }
                    }
                }
                statuses.Add(UploadResult(currentFileName, file.ContentLength, currentFileName));
            }
        }


        private void UploadPartialFile(string fileName, HttpContextBase requestContext, List<ViewDataUploadFilesResult> statuses)
        {
            var request = requestContext.Request;
            if (request.Files.Count != 1) throw new HttpRequestValidationException("Attempt to upload chunked file containing more than one fragment per request");
            var file = request.Files[0];
            var inputStream = file.InputStream;
            String patchOnServer = Path.Combine(_storageRoot);
            var fullName = Path.Combine(patchOnServer, Path.GetFileName(file.FileName));
            var ThumbfullPath = Path.Combine(fullName, Path.GetFileName(file.FileName + ".80x80.jpg"));
            ImageHandler handler = new ImageHandler();

            var ImageBit = ImageHandler.LoadImage(fullName);
            handler.Save(ImageBit, 80, 80, 10, ThumbfullPath);
            using (var fs = new FileStream(fullName, FileMode.Append, FileAccess.Write))
            {
                var buffer = new byte[1024];

                var l = inputStream.Read(buffer, 0, 1024);
                while (l > 0)
                {
                    fs.Write(buffer, 0, l);
                    l = inputStream.Read(buffer, 0, 1024);
                }
                fs.Flush();
                fs.Close();
            }
            statuses.Add(UploadResult(file.FileName, file.ContentLength, file.FileName));
        }
        public ViewDataUploadFilesResult UploadResult(String FileName, int fileSize, String FileFullPath)
        {
            String getType = System.Web.MimeMapping.GetMimeMapping(FileFullPath);
            var result = new ViewDataUploadFilesResult()
            {
                Name = FileName,
                Size = fileSize,
                Type = getType,
                Url = _urlBase + FileName,
                DeleteUrl = _deleteUrl + FileName,
                ThumbnailUrl = CheckThumb(getType, FileName),
                DeleteType = _deleteType,
            };
            return result;
        }

        public String CheckThumb(String type, String FileName)
        {
            var splited = type.Split('/');
            if (splited.Length == 2)
            {
                string extansion = splited[1];
                if (extansion.Equals("jpeg") || extansion.Equals("jpg") || extansion.Equals("png") || extansion.Equals("gif"))
                {
                    String thumbnailUrl = _urlBase + "/thumbs/" + FileName + ".80x80.jpg";
                    return thumbnailUrl;
                }
                else
                {
                    if (extansion.Equals("octet-stream")) //Fix for exe files
                    {
                        return "/Content/Free-file-icons/48px/exe.png";
                    }
                    if (extansion.Contains("zip")) //Fix for exe files
                    {
                        return "/Content/Free-file-icons/48px/zip.png";
                    }
                    String thumbnailUrl = "/Content/Free-file-icons/48px/" + extansion + ".png";
                    return thumbnailUrl;
                }
            }
            else
            {
                return _urlBase + "/thumbs/" + FileName + ".80x80.jpg";
            }

        }
        public List<String> FilesList()
        {

            List<String> Filess = new List<String>();
            string path = HostingEnvironment.MapPath(_serverMapPath);
            System.Diagnostics.Debug.WriteLine(path);
            if (Directory.Exists(path))
            {
                DirectoryInfo di = new DirectoryInfo(path);
                foreach (FileInfo fi in di.GetFiles())
                {
                    Filess.Add(fi.Name);
                    System.Diagnostics.Debug.WriteLine(fi.Name);
                }

            }
            return Filess;
        }
    }
    public class ViewDataUploadFilesResult
    {
        public string Name { get; set; }
        public int Size { get; set; }
        public string Type { get; set; }
        public string Url { get; set; }
        public string DeleteUrl { get; set; }
        public string ThumbnailUrl { get; set; }
        public string DeleteType { get; set; }
        public int DocumentId { get; set; }
    }
    public class JsonFiles
    {
        public ViewDataUploadFilesResult[] Files;
        public string TempFolder { get; set; }
        public JsonFiles(List<ViewDataUploadFilesResult> filesList)
        {
            Files = new ViewDataUploadFilesResult[filesList.Count];
            for (int i = 0; i < filesList.Count; i++)
            {
                Files[i] = filesList.ElementAt(i);
            }
        }
    }
}
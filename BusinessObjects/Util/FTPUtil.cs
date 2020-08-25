using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Threading;
using System.Collections;

namespace BusinessObjects.Util
{
    /// <summary>
    /// FTP
    /// </summary>
    public class FTPUtil
    {
        private string server;
        private int port;
        private string userID;
        private string password;
        private bool enableSSL;

        /// <summary>
        /// 合并地址
        /// </summary>
        /// <param name="path1">地址1</param>
        /// <param name="path2">地址2</param>
        /// <returns>合并后的地址</returns>
        public static string CombinePath(string path1, string path2)
        {
            return string.Format("{0}/{1}", path1.TrimEnd('/'), path2.TrimStart('/'));
        }

        /// <summary>
        /// FTPUtil
        /// </summary>
        /// <param name="ftpServer">服务器地址</param>
        /// <param name="ftpPort">接口</param>
        /// <param name="ftpUserID">用户</param>
        /// <param name="ftpPassword">密码</param>
        /// <param name="enableSSL">SSL</param>
        public FTPUtil(string ftpServer, int ftpPort, string ftpUserID, string ftpPassword, bool enableSSL)
        {
            this.server = ftpServer;
            this.port = ftpPort;
            this.userID = ftpUserID;
            this.password = ftpPassword;
            this.enableSSL = enableSSL;
        }

        /// <summary>
        /// 服务器地址
        /// </summary>
        /// <returns>服务器地址</returns>
        private string GetServerPath()
        {
            return string.Format("ftp://{0}:{1}", this.server, this.port);
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="remoteFolder">服务器地址</param>
        /// <param name="localFolder">文件夹地址</param>
        public void DownloadFileToLocal(string remoteFolder, string localFolder)
        {
            try
            {
                List<string> fileList = GetFileList(CombinePath(GetServerPath(), remoteFolder));

                foreach (string fileName in fileList)
                {
                    FtpWebRequest request = ConnectFTP(fileName);
                    request.Method = WebRequestMethods.Ftp.DownloadFile;

                    using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                    {
                        using (Stream stream = response.GetResponseStream())
                        {
                            using (FileStream fileStream = new FileStream(Path.Combine(localFolder, FileUtil.GetBackupFileName(new FileInfo(fileName))), FileMode.Create))
                            {
                                byte[] buffer = new byte[2048];
                                int read = 0;
                                do
                                {
                                    read = stream.Read(buffer, 0, buffer.Length);
                                    fileStream.Write(buffer, 0, read);
                                } while (read > 0);

                                fileStream.Flush();
                                fileStream.Close();
                            }

                            stream.Close();
                        }

                        response.Close();
                    }

                    DeleteFile(fileName);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Failed to download files from {0}", remoteFolder), ex);
            }
        }

        /// <summary>
        /// 上传文件到服务器
        /// </summary>
        /// <param name="remoteFolder">服务器上文件夹地址</param>
        /// <param name="fileName">文件名称</param>
        /// <param name="localFolder">本地文件夹地址</param>
        public void UploadFileToFTP(string remoteFolder, string fileName, string localFolder)
        {
            string fullFileName = Path.Combine(localFolder, fileName);

            try
            {
                FtpWebRequest request = ConnectFTP(CombinePath(CombinePath(GetServerPath(), remoteFolder), fileName));
                request.Method = WebRequestMethods.Ftp.UploadFile;

                using (FileStream fileStream = new FileStream(fullFileName, FileMode.Open))
                {
                    using (Stream stream = request.GetRequestStream())
                    {
                        byte[] buffer = new byte[2048];
                        int read = 0;
                        do
                        {
                            read = fileStream.Read(buffer, 0, buffer.Length);
                            stream.Write(buffer, 0, read);
                        } while (read > 0);

                        stream.Flush();
                        stream.Close();
                    }

                    fileStream.Close();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Failed to upload {0}", fullFileName), ex);
            }
        }

        /// <summary>
        /// 获取文件列表
        /// </summary>
        /// <param name="path">地址</param>
        /// <returns>文件列表</returns>
        private List<string> GetFileList(string path)
        {
            List<string> fileList = GetFileListFromFTP(path);
            List<string> fileListToDownload = new List<string>();

            if (fileList.Count > 0)
            {
                Dictionary<string, int> fileSizeDict = new Dictionary<string, int>();
                foreach (string fileName in fileList)
                {
                    int fileSize = GetFileSize(CombinePath(path, fileName));
                    fileSizeDict.Add(fileName, fileSize);
                }

                Thread.Sleep(5000);

                foreach (string fileName in fileSizeDict.Keys)
                {
                    if (fileSizeDict[fileName] > 0 && fileSizeDict[fileName] == GetFileSize(CombinePath(path, fileName)))
                    {
                        fileListToDownload.Add(fileName);
                    }
                }
            }

            return fileListToDownload;
        }

        /// <summary>
        /// 服务器上获取文件列表
        /// </summary>
        /// <param name="path">服务器地址</param>
        /// <returns>文件列表</returns>
        private List<string> GetFileListFromFTP(string path)
        {
            List<string> fileList = new List<string>();

            FtpWebRequest request = ConnectFTP(path);
            request.Method = WebRequestMethods.Ftp.ListDirectory;

            using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    string fileName = reader.ReadLine();

                    while (fileName != null)
                    {
                        if (fileName.LastIndexOf('/') > -1)
                            fileList.Add(fileName.Substring(fileName.LastIndexOf('/') + 1));
                        else
                            fileList.Add(fileName);

                        fileName = reader.ReadLine();
                    }

                    reader.Close();
                }

                response.Close();
            }

            return fileList;
        }

        /// <summary>
        /// 重命名文件
        /// </summary>
        /// <param name="fullFileName">文件地址</param>
        /// <param name="newFileName">新文件名</param>
        private void RenameFile(string fullFileName, string newFileName)
        {
            FtpWebRequest request = ConnectFTP(fullFileName);
            request.Method = WebRequestMethods.Ftp.Rename;
            request.RenameTo = newFileName;

            request.GetResponse();
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="fullFileName">文件地址</param>
        private void DeleteFile(string fullFileName)
        {
            FtpWebRequest request = ConnectFTP(fullFileName);
            request.Method = WebRequestMethods.Ftp.DeleteFile;

            request.GetResponse();
        }
        /// <summary>
        /// 检查文件是否存在
        /// </summary>
        /// <param name="fullFileName">文件地址</param>
        /// <returns>文件是否存在</returns>
        private bool CheckFileExist(string fullFileName)
        {
            int fileSize = GetFileSize(fullFileName);
            if (fileSize == 0)
            {
                NLog.LogManager.GetCurrentClassLogger().Info(string.Format("Can't find the file: {0} in FTP", fullFileName));
                return false;
            }

            Thread.Sleep(2000);

            int newFileSize = GetFileSize(fullFileName);
            if (fileSize != newFileSize)
            {
                NLog.LogManager.GetCurrentClassLogger().Info(string.Format("The file: {0} is still under transfer to FTP", fullFileName));
                return false;
            }

            return true;
        }

        /// <summary>
        /// 获取文件大小
        /// </summary>
        /// <param name="filePath">文件地址</param>
        /// <returns>文件大小</returns>
        private int GetFileSize(string filePath)
        {
            try
            {
                FtpWebRequest request = ConnectFTP(filePath);
                request.Method = WebRequestMethods.Ftp.GetFileSize;

                return (int)request.GetResponse().ContentLength;
            }
            catch (WebException ex)
            {

                if (((FtpWebResponse)ex.Response).StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
                    return 0;
                else
                    throw ex;
            }
        }

        /// <summary>
        /// 连接服务器
        /// </summary>
        /// <param name="path">文件地址</param>
        /// <returns>FtpWebRequest</returns>
        private FtpWebRequest ConnectFTP(string path)
        {
            FtpWebRequest request = (FtpWebRequest)FtpWebRequest.Create(path);
            request.UseBinary = true;
            request.KeepAlive = false;
            request.EnableSsl = this.enableSSL;
            request.Credentials = new NetworkCredential(this.userID, this.password);

            return request;
        }
    }
}

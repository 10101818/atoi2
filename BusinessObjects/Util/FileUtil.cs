using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BusinessObjects.Domain;

namespace BusinessObjects.Util
{
    /// <summary>
    /// 文件
    /// </summary>
    public class FileUtil
    {
        /// <summary>
        /// 根据文件地址和文件名获取文件列表
        /// </summary>
        /// <param name="filePath">文件地址</param>
        /// <param name="searchPattern">文件名</param>
        /// <returns>文件列表</returns>
        public static List<FileInfo> GetFileList(string filePath, string searchPattern)
        {
            List<FileInfo> fileList = new List<FileInfo>();

            // Gets the cache file list.
            IList<FileInfo> cachedList = new DirectoryInfo(filePath).GetFiles(searchPattern);

            //wait 5 seconds to get the file list again
            Thread.Sleep(5000);

            // Gets the current file list.
            IList<FileInfo> currentList = new DirectoryInfo(filePath).GetFiles(searchPattern);

            // Gets all the file entries that has the same name and size
            // FileInfo collection.
            foreach (FileInfo current in currentList)
            {
                foreach (FileInfo cached in cachedList)
                {
                    if (current.Name.ToUpper().Equals(cached.Name.ToUpper()))
                    {
                        if (current.Length.Equals(cached.Length))
                            fileList.Add(current);
                    }
                }
            }

            return fileList;
        }

        /// <summary>
        /// 判断文件是否使用中
        /// </summary>
        /// <param name="filePath">文件地址</param>
        /// <returns>文件是否使用中</returns>
        public static bool CheckFileInUse(string filePath)
        {
            try
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                {
                    fs.Close();
                }

                return false;
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Warn(String.Format("File: {0} is being used by another process. Error: {1} ", filePath, ex.Message));
                return true;
            }
        }

        /// <summary>
        /// 文件备份
        /// </summary>
        /// <param name="fileInfo">文件信息</param>
        /// <param name="folderName">文件夹名称</param>
        public static void BackupFile(FileInfo fileInfo, string folderName)
        {
            string newFilePath = Path.Combine(fileInfo.DirectoryName, folderName);

            if (Directory.Exists(newFilePath) == false)
            {
                Directory.CreateDirectory(newFilePath);
            }

            File.Copy(fileInfo.FullName, Path.Combine(newFilePath, GetBackupFileName(fileInfo)));
            File.Delete(fileInfo.FullName);
        }

        /// <summary>
        /// 移动文件
        /// </summary>
        /// <param name="fileInfo">文件信息</param>
        /// <param name="destFolder">新地址</param>
        public static void MoveFile(FileInfo fileInfo, string destFolder)
        {
            if (Directory.Exists(destFolder) == false)
            {
                Directory.CreateDirectory(destFolder);
            }

            File.Copy(fileInfo.FullName, Path.Combine(destFolder, GetBackupFileName(fileInfo)));
            File.Delete(fileInfo.FullName);
        }

        /// <summary>
        /// 判断文件夹是否存在
        /// </summary>
        /// <param name="folder">文件夹</param>
        public static void CheckDirectory(string folder)
        {
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);
        }
        /// <summary>
        /// 获取备份的文件名称
        /// </summary>
        /// <param name="fileInfo">文件信息</param>
        /// <returns>新的文件名</returns>
        public static string GetBackupFileName(FileInfo fileInfo)
        {
            return string.Format("{0}_{1}{2}", fileInfo.Name.Substring(0, fileInfo.Name.Length - fileInfo.Extension.Length), DateTime.Now.ToString("yyyyMMddHHmm"), fileInfo.Extension);
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="folderName">文件夹地址</param>
        /// <param name="fileName">文件名</param>
        public static void DeleteFile(string folderName, string fileName)
        {
            string filePath = Path.Combine(folderName, fileName);

            if (File.Exists(filePath))
                File.Delete(filePath);
        }

        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="bytes">文件内容</param>
        /// <param name="folder">文件夹地址</param>
        /// <param name="fileName">文件名</param>
        public static void SaveFile(byte[] bytes, string folder, string fileName)
        {
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            using (FileStream fw = new FileStream(Path.Combine(folder, fileName), FileMode.Create, FileAccess.Write))
            {
                fw.Write(bytes, 0, bytes.Length);
                fw.Flush();
            }
        }

        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="stream">文件内容</param>
        /// <param name="folder">文件夹地址</param>
        /// <param name="fileName">文件名</param>
        public static void SaveFile(Stream stream, string folder, string fileName)
        {
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            using (FileStream fw = new FileStream(Path.Combine(folder, fileName), FileMode.Create, FileAccess.Write))
            {
                stream.CopyTo(fw);
            }
        }

        /// <summary>
        /// 读取文件
        /// </summary>
        /// <param name="folder">文件夹地址</param>
        /// <param name="fileName">文件名</param>
        /// <returns>文件内容</returns>
        public static byte[] ReadFile( string folder, string fileName)
        {
            string filePath = Path.Combine(folder, fileName);
            if (!File.Exists(filePath)) return null;

            return File.ReadAllBytes(filePath);
        }

        /// <summary>
        /// 压缩文件夹
        /// </summary>
        /// <param name="streams">多文件内容</param>
        /// <returns>压缩包信息</returns>
        public static MemoryStream ZipFiles(List<FileStreamInfo> streams)
        {
            MemoryStream compressStream = new MemoryStream();

            using (ZipArchive zipArchive = new ZipArchive(compressStream, ZipArchiveMode.Create, true, Encoding.GetEncoding("gb2312")))
            {
                foreach (FileStreamInfo info in streams)
                {
                    ZipArchiveEntry fileEntry = zipArchive.CreateEntry(info.FileName);
                    using (Stream entryStream = fileEntry.Open())
                    {
                        info.Stream.CopyTo(entryStream);
                    }
                }
            }

            compressStream.Flush();
            compressStream.Position = 0;

            return compressStream;
        }

        /// <summary>
        /// 读取excel文件
        /// </summary>
        /// <param name="fileBytes">byte类型文件内容</param>
        /// <param name="dt">返回文件内容</param>
        /// <returns>是否读取成功</returns>
        public static bool ReadExcelFile(byte[] fileBytes, out DataTable dt)
        {
            string fileName = Path.GetTempFileName();
            File.WriteAllBytes(fileName, fileBytes);

            bool result = ReadExcelFile(fileName, out dt);

            File.Delete(fileName);

            return result;
        }

        /// <summary>
        /// 读取excel文件
        /// </summary>
        /// <param name="filePath">文件地址</param>
        /// <param name="dt">文件内容</param>
        /// <returns>是否读取成功</returns>
        public static bool ReadExcelFile(string filePath, out DataTable dt)
        {
            dt = new DataTable();

            try
            {
                using (OleDbConnection conn = new OleDbConnection(string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source='{0}';Extended Properties=\"Excel 12.0;HDR=YES;IMEX=1\"", filePath)))
                {
                    using (OleDbDataAdapter da = new OleDbDataAdapter("SELECT * FROM [Sheet1$]", conn))
                    {
                        da.Fill(dt);
                    }
                }
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                return false;
            }

            return true;
        }

        /// <summary>
        /// 读取csv文件
        /// </summary>
        /// <param name="fileInfo">文件信息</param>
        /// <param name="cols">列数</param>
        /// <param name="hasHeader">是否需要表头信息</param>
        /// <param name="dt">返回内容</param>
        /// <returns>是否读取成功</returns>
        public static bool ReadCSVFile(FileInfo fileInfo, int cols, bool hasHeader, out DataTable dt)
        {
            dt = new DataTable();

            try
            {
                ConstructSchema4Csv(fileInfo, cols, hasHeader);
                using (OleDbConnection conn = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileInfo.DirectoryName + ";;Extended Properties=\"Text;HDR=" + (hasHeader ? "Yes" : "No") + ";FORMAT=Delimited;IMEX=1;\""))
                {
                    using (OleDbDataAdapter da = new OleDbDataAdapter("SELECT * FROM [" + fileInfo.Name + "]", conn))
                    {
                        da.Fill(dt);
                    }
                }
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Warn(String.Format("GetDataFromCSV has error: {0}", ex.Message));
                return false;
            }
            finally
            {
                DeleteSchemaFile(fileInfo);
            }

            return true;
        }

        /// <summary>
        /// 创建Schema
        /// </summary>
        /// <param name="fileInfo">文件信息</param>
        /// <param name="cols">列数</param>
        /// <param name="hasHeader">是否需要表头信息</param>
        private static void ConstructSchema4Csv(FileInfo fileInfo, int cols, bool hasHeader)
        {
            StringBuilder schema = new StringBuilder();
            schema.AppendLine("[" + fileInfo.Name + "]");
            schema.AppendLine("ColNameHeader=" + hasHeader);
            for (int i = 0; i < cols; i++)
            {
                schema.AppendLine("col" + (i + 1).ToString() + "=" + "Column" + i + " Text");
            }
            string schemaFileName = fileInfo.DirectoryName + @"\Schema.ini";
            using (TextWriter tw = new StreamWriter(schemaFileName, false))
            {
                tw.WriteLine(schema.ToString());
                tw.Close();
            }
        }

        /// <summary>
        /// 删除Schema文件
        /// </summary>
        /// <param name="fileInfo">文件信息</param>
        private static void DeleteSchemaFile(FileInfo fileInfo)
        {
            try
            {
                string schemaFileName = fileInfo.DirectoryName + @"\Schema.ini";
                File.Delete(schemaFileName);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Warn(String.Format("DeleteSchemaFile has error: {0}", ex.Message));
            }
        }
    }
}

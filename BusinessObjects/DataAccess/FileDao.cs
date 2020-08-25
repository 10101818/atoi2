using BusinessObjects.Aspect;
using BusinessObjects.Domain;
using BusinessObjects.Manager;
using BusinessObjects.Util;
using PostSharp.Extensibility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.DataAccess
{
    /// <summary>
    /// 附件dao
    /// </summary>
    [LoggingAspect(AspectPriority = 1)]
    [ConnectionAspect(AspectPriority = 2, AttributeTargetTypeAttributes = MulticastAttributes.Public)]
    public class FileDao:BaseDao
    {
        #region "tblXXXXFile"
        /// <summary>
        /// 新增附件
        /// </summary>
        /// <param name="objectTypeId">附件对象类型ID</param>
        /// <param name="file">附件信息</param>
        /// <returns>附件信息</returns>
        public UploadFileInfo AddFile(int objectTypeId, UploadFileInfo file)
        {
            sqlStr = string.Format("INSERT INTO tbl{0}File ({0}ID,FileType,FileName,FileDesc,AddDate)" +
                " VALUES(@ObjectID,@FileType,@FileName,@FileDesc,@AddDate) SELECT @@IDENTITY", LookupManager.GetObjectTypeKey(objectTypeId));

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@ObjectID", SqlDbType.Int).Value = SQLUtil.ConvertInt(file.ObjectID);
                command.Parameters.Add("@FileType", SqlDbType.Int).Value = SQLUtil.ConvertInt(file.FileType);
                command.Parameters.Add("@FileName", SqlDbType.VarChar).Value = SQLUtil.TrimNull(file.FileName);
                command.Parameters.Add("@FileDesc", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(file.FileDesc);
                command.Parameters.Add("@AddDate", SqlDbType.DateTime).Value = DateTime.Now;

                file.ID = SQLUtil.ConvertInt(command.ExecuteScalar());
            }
            return file;
        }
        /// <summary>
        /// 更新附件
        /// </summary>
        /// <param name="objectTypeId">附件对象类型ID</param>
        /// <param name="file">附件信息</param>
        /// <returns>附件信息</returns>
        public UploadFileInfo UpdateFile(int objectTypeId, UploadFileInfo file)
        {
            sqlStr = string.Format(" UPDATE tbl{0}File SET FileName=@FileName, AddDate=@AddDate WHERE ID=@ID ", LookupManager.GetObjectTypeKey(objectTypeId));
            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@FileName", SqlDbType.VarChar).Value = SQLUtil.TrimNull(file.FileName);
                command.Parameters.Add("@AddDate", SqlDbType.DateTime).Value = DateTime.Now;
                command.Parameters.Add("@ID", SqlDbType.Int).Value = file.ID;
                command.ExecuteNonQuery();
            }
            return file;
        }
        /// <summary>
        /// 删除附件
        /// </summary>
        /// <param name="objectTypeId">附件对象类型ID</param>
        /// <param name="id">附件ID</param>
        public void DeleteFileByID(int objectTypeId, int id)
        {
            sqlStr = string.Format(" DELETE FROM tbl{0}File WHERE ID=@ID", LookupManager.GetObjectTypeKey(objectTypeId));
            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@ID", SqlDbType.Int).Value = id;
                command.ExecuteNonQuery();
            }
        }
        /// <summary>
        /// 根据附件关联ID获取第一条附件
        /// </summary>
        /// <param name="objectTypeId">附件对象类型ID</param>
        /// <param name="ObjectID">附件关联ID</param>
        /// <returns>附件信息</returns>
        public UploadFileInfo GetFile(int objectTypeId, int ObjectID)
        {
            sqlStr = "SELECT top(1) f.ID,f.FileName,f.FileType, @ObjectTypeId as ObjectTypeId,f.{0}ID ObjectID,f.FileDesc,f.AddDate, " + 
                    " ROW_NUMBER() OVER (PARTITION BY {0}ID, FileType ORDER By {0}ID, FileType, ID) as Seq" +
                    " FROM tbl{0}File f WHERE f.{0}ID=@ObjectID ";

            sqlStr = string.Format(sqlStr, LookupManager.GetObjectTypeKey(objectTypeId));

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@ObjectID", SqlDbType.Int).Value = ObjectID;
                command.Parameters.Add("@ObjectTypeId", SqlDbType.Int).Value = objectTypeId;

                DataRow dr = GetDataRow(command);
                if (dr != null)
                    return new UploadFileInfo(dr);
                else
                    return null;
            }
        }
        /// <summary>
        /// 根据附件ID获取第一条附件
        /// </summary>
        /// <param name="objectTypeId">附件对象类型ID</param>
        /// <param name="id">附件ID</param>
        /// <returns>附件信息</returns>
        public UploadFileInfo GetFileByID(int objectTypeId, int id)
        {
            sqlStr = "SELECT top(1) f.ID,f.FileName,f.FileType, @ObjectTypeId as ObjectTypeId, f.{0}ID ObjectID,f.FileDesc,f.AddDate, " +
                     " ROW_NUMBER() OVER (PARTITION BY {0}ID, FileType ORDER By {0}ID, FileType, ID) as Seq" +
                     " FROM tbl{0}File f WHERE f.ID=@ID ";

            sqlStr = string.Format(sqlStr, LookupManager.GetObjectTypeKey(objectTypeId));

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@ID", SqlDbType.Int).Value = id;
                command.Parameters.Add("@ObjectTypeId", SqlDbType.Int).Value = objectTypeId;

                DataRow dr = GetDataRow(command);

                if (dr != null)
                    return new UploadFileInfo(dr);
                else
                    return null;
            }
        }
        /// <summary>
        /// 根据附件关联ID获取附件
        /// </summary>
        /// <param name="objectTypeId">附件对象类型ID</param>
        /// <param name="ObjectID">附件关联ID</param>
        /// <returns>附件信息</returns>
        public List<UploadFileInfo> GetFiles(int objectTypeId, int ObjectID)
        {
            List<UploadFileInfo> infos = new List<UploadFileInfo>();

            sqlStr = "SELECT f.ID,f.FileName,f.FileType, @ObjectTypeId as ObjectTypeId,f.{0}ID ObjectID,f.FileDesc,f.AddDate, " +
                    " ROW_NUMBER() OVER (PARTITION BY {0}ID, FileType ORDER By {0}ID, FileType, ID) as Seq" +
                    " FROM tbl{0}File f WHERE f.{0}ID=@ObjectID order by f.ID";

            sqlStr = string.Format(sqlStr, LookupManager.GetObjectTypeKey(objectTypeId));

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@ObjectID", SqlDbType.Int).Value = ObjectID;
                command.Parameters.Add("@ObjectTypeId", SqlDbType.Int).Value = objectTypeId;

                using (DataTable dt = GetDataTable(command))
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        infos.Add(new UploadFileInfo(dr));
                    }
                }
            }
            return infos;
        }
        #endregion
    }
}

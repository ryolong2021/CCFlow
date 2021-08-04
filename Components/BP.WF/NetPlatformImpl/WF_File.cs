using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using BP.Web;
using Microsoft.AspNetCore.Http;

namespace BP.WF.NetPlatformImpl
{
    public class WF_File
    {
        /// <summary>
        /// 文件上传
        /// </summary>
        /// <param name="filePath"></param>
        public static void UploadFile(string filePath)
        {
            try
            {
                IFormFileCollection filelist = HttpContextHelper.RequestFiles();
                if (filelist == null || filelist.Count == 0)
                {
                    throw new NotImplementedException("アップロードされたファイルはありません");
                }
                var f = filelist[0];
                // 写入文件
                using (FileStream fs = System.IO.File.Create(filePath))
                {
                    f.CopyTo(fs);
                    fs.Flush();
                }
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message);
            }
        }
        /// <summary>
        /// 获取文件
        /// </summary>
        /// <returns></returns>
        public static IFormFile GetUploadFile(int index=0)
        {
            try
            {
                IFormFileCollection filelist = HttpContextHelper.RequestFiles();
                if (filelist == null || filelist.Count == 0)
                {
                    throw new NotImplementedException("アップロードされたファイルはありません");
                }
                return filelist[index];
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message);
            }
        }
        public static IFormFileCollection GetUploadFiles()
        {
            try
            {
                IFormFileCollection filelist = HttpContextHelper.RequestFiles();
                if (filelist == null || filelist.Count == 0)
                {
                    throw new NotImplementedException("アップロードされたファイルはありません");
                }
                return filelist;
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message);
            }
        }
        public static void UploadFile(IFormFile file,string filePath)
        {
            try
            {
                // 写入文件
                using (FileStream fs = System.IO.File.Create(filePath))
                {
                    file.CopyTo(fs);
                    fs.Flush();
                }
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message);
            }
        }
        public static long GetFileLength(IFormFile file)
        {
            try
            {
                return file.Length;
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message);
            }
        }
    }
}

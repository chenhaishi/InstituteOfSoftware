using BaiduBce;
using BaiduBce.Auth;
using BaiduBce.Services.Bos;
using BaiduBce.Services.Bos.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace SiliconValley.InformationSystem.Business.Cloudstorage_Business
{
    /// <summary>
    /// 百度云存储对象业务类
    /// </summary>
    public class CloudstorageBusiness
    {

        const string accessKeyId = "a43996ac0c6d40c69d3ebb47127909e9"; // 用户的Access Key ID
        const string secretAccessKey = "2cfdf8b1f0e548f28cafcfd1aafc9226"; // 用户的Secret Access Key
        const string endpoint = "http://bj.bcebos.com";
        // 初始化一个BosClient
        BceClientConfiguration config = new BceClientConfiguration();

        public List<BucketSummary> ListBuckets(BosClient client)
        {

            // 获取用户的Bucket列表
            List<BucketSummary> buckets = client.ListBuckets().Buckets;

            return buckets;

        }
        /// <summary>
        /// 配置存储对象信息
        /// </summary>
        /// <returns></returns>
        public BosClient BosClient()
        {
            config.Credentials = new DefaultBceCredentials(accessKeyId, secretAccessKey);
            config.Endpoint = endpoint;
            BosClient client = new BosClient(config);
            return client;
        }
        /// <summary>
        /// 递归列出Prefix目录下的所有文件
        /// </summary>
        /// <param name="bucketName">项目名称</param>
        /// <param name="Prefix">文件夹名称</param>
        /// <returns></returns>
        public List<BosObjectSummary> Listfiles(string bucketName, string Prefix)
        {
            var client = this.BosClient();

            // 获取用户的Bucket列表
            List<BucketSummary> buckets = client.ListBuckets().Buckets;

            /// client.DeleteBucket(bucketName);
            // 构造ListObjectsRequest请求
            ListObjectsRequest listObjectsRequest = new ListObjectsRequest() { BucketName = bucketName };

            // 递归列出fun目录下的所有文件
            listObjectsRequest.Prefix = Prefix;

            // List Objects
            ListObjectsResponse listObjectsResponse = client.ListObjects(listObjectsRequest);

            return listObjectsResponse.Contents;
        }

        /// <summary>
        /// 获取单张图片
        /// </summary>
        /// <param name="bucketName">项目名称</param>
        /// <param name="Prefix">文件路径</param>
        /// <param name="ImageName">图片名称</param>
        ///   /// <param name="expirationInSeconds">url有效时长</param>
        /// <returns></returns>
        public string ImagesFine(string bucketName, string Prefix, string ImageName, int expirationInSeconds)
        {
            try
            {
                var client = this.BosClient();
                GetObjectRequest getObjectRequest = new GetObjectRequest() { BucketName = bucketName, Key = Prefix + "/" + ImageName };
                Uri url = client.GeneratePresignedUrl(bucketName, Prefix + "/" + ImageName, expirationInSeconds);


                return url.AbsoluteUri;
            }
            catch (Exception ex)
            {

                return null;
            }
        }
        /// <summary>
        /// 上传图片，以文件流的方式
        /// </summary>
        /// <param name="bucketName">项目名称</param>
        /// <param name="Prefix">文件路径</param>
        /// <param name="objectKey">文件名</param>
        /// <param name="file">文件流</param>
        /// <returns></returns>
        public bool PutObject(String bucketName, string Prefix, String objectKey, Stream file)
        {
            var client = this.BosClient();
            try
            {
                // 以数据流形式上传Object
                PutObjectResponse putObjectResponseFromInputStream = client.PutObject(bucketName, Prefix + "/" + objectKey, file);
                return true;
            }
            catch (Exception ex)
            {
                return false;
                throw;
            }


            //// 以二进制串上传Object
            //PutObjectResponse putObjectResponseFromByte = client.PutObject(bucketName, objectKey, Encoding.Default.GetBytes("sampledata"));

            //// 以字符串上传Object
            //PutObjectResponse putObjectResponseFromString = client.PutObject(bucketName, objectKey, "sampledata");

            //// 打印ETag
            //Console.WriteLine(putObjectFromFileResponse.ETAG);

        }
        /// <summary>
        /// 提过文件路径获取子级
        /// </summary>
        /// <param name="Prefix">文件路径</param>
        /// <returns></returns>
        public List<string> Getchildren(string Prefix)
        {
            List<string> strname = new List<string>();
            var client = this.BosClient();
            // 构造ListObjectsRequest请求
            ListObjectsRequest listObjectsRequest = new ListObjectsRequest() { BucketName = "xinxihua" };

            // 1. 简单查询，列出Bucket下所有文件
            ListObjectsResponse listObjectsResponse = client.ListObjects(listObjectsRequest);
            // "/" 为文件夹的分隔符
            listObjectsRequest.Delimiter = "/";

            // 列出fun目录下的所有文件和文件夹
            listObjectsRequest.Prefix = Prefix;
            listObjectsResponse = client.ListObjects(listObjectsRequest);
            //获取文件夹
            foreach (var item in listObjectsResponse.CommonPrefixes)
            {
                strname.Add(Path.GetFileName(item.Prefix.Substring(0, item.Prefix.Length - 1)));
            }

            foreach (var item in listObjectsResponse.Contents)
            {
                if (item.Size > 0)
                {
                    strname.Add(Path.GetFileName(item.Key));
                }
            }

            return strname;
        }

        /// <summary>
        /// 保存文件，以文件流方式
        /// </summary>
        /// <param name="bucketName"></param>
        /// <param name="Prefix">文件路径（aaa/aaa/）</param>
        /// <param name="objectKey">文件名称</param>
        /// <param name="inputStream">文件流</param>
        /// <returns></returns>
        public int Savefile(String bucketName, string Prefix, string objectKey, Stream inputStream)
        {
            var client = this.BosClient();
            try
            {
                // 以数据流形式上传Object
                PutObjectResponse putObjectResponseFromInputStream = client.PutObject(bucketName, Prefix + objectKey, inputStream);
                return 1;
            }
            catch (Exception)
            {
                return 0;
                throw;
            }


        }
        /// <summary>
        /// 下载文 File(文件流.ObjectContent, "application/msword", 文件名);
        /// </summary>
        /// <param name="bucketName">项目名称</param>
        /// <param name="Prefix">路径</param>
        /// <param name="filename">文件名称</param>
        /// <returns></returns>
        public Stream DownloadFile(string bucketName, string Prefix, string filename)
        {
            var client = this.BosClient();
            // 新建GetObjectRequest
            GetObjectRequest getObjectRequest = new GetObjectRequest() { BucketName = bucketName, Key = Prefix + filename };

            // 下载Object到文件
            BosObject bosObject = client.GetObject(bucketName, Prefix + filename);


            return bosObject.ObjectContent;

            //ObjectMetadata objectMetadata = client.GetObject(getObjectRequest, new FileInfo("D:\\iazai"));

        }
        /// <summary>
        /// 删除文件或者图片
        /// </summary>
        /// <param name="client"></param>
        /// <param name="bucketName">项目名称</param>
        /// <param name="objectKey">路径+文件</param>
        public bool DeleteObject(string bucketName, string objectKey)
        {
            try
            {
        
        var client = this.BosClient();
                // 删除Object
                client.DeleteObject(bucketName, objectKey);
                return true;
            }
            catch (Exception)
            {

                throw;
                return false;
            }

        }

        ///// <summary>
        ///// 删除某个文件夹的所有文件
        ///// </summary>
        ///// <param name="FileName">文件</param>
        ///// <returns></returns>
        //public bool DeleteObject(string FileName)
        //{
        //    bool s = true;
        //    try
        //    {
              
        //         var client = this.BosClient();
        //        client.DeleteBucket(FileName);
        //    }
        //    catch (Exception)
        //    {
        //        s = false;
                
        //    }

        //    return s;
        //}
        public string text(string bucketName,string objectName)
        {
            var client = this.BosClient();
            // 生成url，并通过该url直接下载和打印对象内容
            string url = client.GeneratePresignedUrl(bucketName, objectName, 60).AbsoluteUri;
            using (WebClient webClient = new WebClient())
            {
                using (Stream stream = webClient.OpenRead(url))
                using (StreamReader streamReader = new StreamReader(stream))
                {
                    string response = streamReader.ReadToEnd();
                 return response;  // 您传入的<SampleData>
                }
            }
        }
    }
}

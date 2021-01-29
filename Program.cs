using System;
using Amazon.S3;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3.Model;
using Amazon.S3.Util;

namespace demo
{
    class Program
    {

        private const string accessKey = "E2R228B3YF1TF6IYBZJ1";
        private const string secretKey = "clsTQb6EtomIA6kZH54Xv0mrOnRjLyRCCNTTqiWr"; // do not store secret key hardcoded in your production source code!
        private const string endpointURL = "http://172.16.70.31:7480";

        static void Main(string[] args)
        {
            // Task.Run(MainAsync).GetAwaiter().GetResult();

            Task.Run(CreateBucketAsync).GetAwaiter().GetResult();
        }

        private static async Task MainAsync()
        {
            var config = new AmazonS3Config
            {
                RegionEndpoint = RegionEndpoint.USEast1, // MUST set this before setting ServiceURL and it should match the `MINIO_REGION` environment variable.
                ServiceURL = endpointURL, // replace http://localhost:9000 with URL of your MinIO server
                ForcePathStyle = true // MUST be true to work correctly with MinIO server
            };
            var amazonS3Client = new AmazonS3Client(accessKey, secretKey, config);

            // uncomment the following line if you like to troubleshoot communication with S3 storage and implement private void OnAmazonS3Exception(object sender, Amazon.Runtime.ExceptionEventArgs e)
            // amazonS3Client.ExceptionEvent += OnAmazonS3Exception;

            var listBucketResponse = await amazonS3Client.ListBucketsAsync();

            foreach (var bucket in listBucketResponse.Buckets)
            {
                Console.Out.WriteLine("bucket '" + bucket.BucketName + "' created at " + bucket.CreationDate);
            }
            if (listBucketResponse.Buckets.Count > 0)
            {
                var bucketName = listBucketResponse.Buckets[0].BucketName;

                var listObjectsResponse = await amazonS3Client.ListObjectsAsync(bucketName);

                foreach (var obj in listObjectsResponse.S3Objects)
                {
                    Console.Out.WriteLine("key = '" + obj.Key + "' | size = " + obj.Size + " | tags = '" + obj.ETag + "' | modified = " + obj.LastModified);
                }
            }
            if (listBucketResponse.Buckets.Count > 1)
            {
                var bucketName = listBucketResponse.Buckets[1].BucketName;

                var listObjectsResponse = await amazonS3Client.ListObjectsAsync(bucketName);

                foreach (var obj in listObjectsResponse.S3Objects)
                {
                    Console.Out.WriteLine("key = '" + obj.Key + "' | size = " + obj.Size + " | tags = '" + obj.ETag + "' | modified = " + obj.LastModified);
                }
            }
        }


        private const string bucketName = "Demo123123123";

        private static async Task CreateBucketAsync()
        {
            var config = new AmazonS3Config
            {
                RegionEndpoint = RegionEndpoint.USEast1, // MUST set this before setting ServiceURL and it should match the `MINIO_REGION` environment variable.
                ServiceURL = endpointURL, // replace http://localhost:9000 with URL of your MinIO server
                ForcePathStyle = true // MUST be true to work correctly with MinIO server
            };
            var amazonS3Client = new AmazonS3Client(accessKey, secretKey, config);

            try
            {
                var putBucketRequest = new PutBucketRequest
                {
                    BucketName = bucketName,
                    UseClientRegion = true
                };

                PutBucketResponse putBucketResponse = await amazonS3Client.PutBucketAsync(putBucketRequest);

                Console.Out.WriteLine("Status code = '" + putBucketResponse.HttpStatusCode);

            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine("Error encountered on server. Message:'{0}' when writing an object", e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Unknown encountered on server. Message:'{0}' when writing an object", e.Message);
            }

        }
    }
}

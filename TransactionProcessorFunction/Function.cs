using Amazon.Lambda.Core;
using Amazon.Lambda.S3Events;
using Amazon.S3;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;


// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace TransactionProcessorFunction
{
    public class TRFunctions
    {
        IAmazonS3 S3Client { get; set; }
        public TRFunctions()
        {
            S3Client = new AmazonS3Client();
        }

        public async Task<string> Handler(S3Event evnt, ILambdaContext context)
        {
            //01. Get the bucket name
            var s3Event = evnt.Records?[0].S3;
            var bucket = s3Event.Bucket.Name;

            //02. Get the file/key name
            var key = s3Event.Object.Key;

            try
            {
                //03. fetch the file from S3
                using (var response = await S3Client.GetObjectAsync(bucket, key))
                //04. Deserialize the file's content
                using (var sr = new StreamReader(response.ResponseStream))
                {
                    var text = await sr.ReadToEndAsync();
                    var data = JsonConvert.DeserializeObject<dynamic>(text);

                    //05. print the content
                    Console.WriteLine(data);

                    //06. parse and print the transactions
                    var transactions = data["transactions"];
                    foreach (var record in transactions)
                    {
                        Console.WriteLine(record["transType"]);
                    }
                    return "Success";
                }
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }
    }
}

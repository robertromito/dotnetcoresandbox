using System;
using System.Net.Http;
using Amazon.Runtime;
using Amazon.SQS;
using Amazon.SQS.Model;

namespace SimpleConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            new MyHttpClient().Run();
        }
    }

    class MyHttpClient
    {
        public void Run()
        {
            Console.WriteLine("I will connect to the HTTP of localstack!");
            var client = new HttpClient();

            const string uri = "http://localhost:4576/queue/rcb-affiliate";

            var amazonSQSClient = new AmazonSQSClient(
                new BasicAWSCredentials("access key", "secret key"),
                new AmazonSQSConfig
                {
                    ServiceURL = "http://localhost:4576/"
                });

            var sendMsgResp = amazonSQSClient
            .SendMessageAsync(
                new SendMessageRequest
                {
                    QueueUrl = uri,
                    MessageBody = "I am an SQS message!!!"
                })
            .Result;

            Console.WriteLine(sendMsgResp.HttpStatusCode);

            while (true)
            {
                Console.WriteLine($"[{DateTime.Now}] Waiting for messages");
                var recMsgResp = amazonSQSClient.ReceiveMessageAsync(
                    new ReceiveMessageRequest
                    {
                        QueueUrl = uri,
                        WaitTimeSeconds = 10,
                        MaxNumberOfMessages = 10
                    }
                )
                .Result;

                recMsgResp.Messages.ForEach(m =>
                {
                    Console.WriteLine(m.Body);
                    amazonSQSClient.DeleteMessageAsync(
                        new DeleteMessageRequest
                        {
                            QueueUrl = uri,
                            ReceiptHandle = m.ReceiptHandle
                        }
                    );
                });
            }
        }
    }
}

using System;
using System.Threading.Tasks;
using Amazon;
using Amazon.EC2;
using Amazon.EC2.Model;
using Amazon.Runtime;

class Program
{
    static async Task Main(string[] args)
    {
        string accessKeyFilePath = @"E:\2020039075_accessKeys.csv";

        try
        {
            string content = File.ReadAllText(accessKeyFilePath);

            content = content.Substring(content.IndexOf("\n") + 1);

            string[] keys = content.Split(',');
            keys[0] = keys[0].Trim();
            keys[1] = keys[1].Trim();

            var credentials = new BasicAWSCredentials(keys[0], keys[1]);

            using var ec2Client = new AmazonEC2Client(credentials, RegionEndpoint.APNortheast2);

            var request = new DescribeInstancesRequest();

            // EC2 인스턴스 리스트 가져오기
            var response = await ec2Client.DescribeInstancesAsync(request);

            // 인스턴스 정보 출력
            foreach (var reservation in response.Reservations)
            {
                foreach (var instance in reservation.Instances)
                {
                    Console.WriteLine($"Instance ID: {instance.InstanceId}, State: {instance.State.Name}");
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

    }
}

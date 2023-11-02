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
        const string accessKey = "";
        const string secretAccessKey = "";

        var credentials = new BasicAWSCredentials(accessKey, secretAccessKey);

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
}

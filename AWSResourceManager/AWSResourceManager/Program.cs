using System;
using System.Threading.Tasks;
using Amazon;
using Amazon.EC2;
using Amazon.EC2.Model;
using Amazon.Runtime;

class Program
{
    private static AmazonEC2Client ec2Client;

    private static async Task InitializeEC2Client()
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

            ec2Client = new AmazonEC2Client(credentials, RegionEndpoint.APNortheast2);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

    }

    static async Task Main(string[] args)
    {
        await InitializeEC2Client();

        while (true)
        {
            Console.WriteLine("                                                            ");
            Console.WriteLine("                                                            ");
            Console.WriteLine("------------------------------------------------------------");
            Console.WriteLine("           Amazon AWS Control Panel using SDK               ");
            Console.WriteLine("------------------------------------------------------------");
            Console.WriteLine("  1. list instance                2. available zones        ");
            Console.WriteLine("  3. start instance               4. available regions      ");
            Console.WriteLine("  5. stop instance                6. create instance        ");
            Console.WriteLine("  7. reboot instance              8. list images            ");
            Console.WriteLine("                                 99. quit                   ");
            Console.WriteLine("------------------------------------------------------------");

            Console.Write("Enter an integer: ");
            string menu = Console.ReadLine();

            if (int.TryParse(menu, out int menuId))
            {
                switch (menuId)
                {
                    case 1:
                        await ListInstances();
                        break;

                    case 2:
                        await ListAvailableZones();
                        break;

                    case 3:
                        Console.Write("Enter instance id: ");
                        string instanceId = Console.ReadLine();
                        if (instanceId != null)
                            await StartInstance(instanceId);
                        break;

                    case 4:
                        ListAvailableRegions();
                        break;

                    case 5:
                        StopInstance();
                        break;

                    case 6:
                        CreateInstance();
                        break;

                    case 7:
                        RebootInstance();
                        break;

                    case 8:
                        ListImages();
                        break;

                    case 99:
                        Console.WriteLine("Bye!");
                        return;

                    default:
                        Console.WriteLine("Concentration!");
                        break;
                }
            }
        }
    }

    public async static Task ListInstances()
    {
        Console.WriteLine("Listing instances......");

        var request = new DescribeInstancesRequest();

        // EC2 인스턴스 리스트 가져오기
        var response = await ec2Client.DescribeInstancesAsync(request);

        // 인스턴스 정보 출력
        foreach (var reservation in response.Reservations)
        {
            foreach (var instance in reservation.Instances)
            {
                Console.WriteLine($"[id]  {instance.InstanceId}, " +
                    $"[AMI] {instance.ImageId}, " +
                    $"[type] {instance.InstanceType}, " +
                    $"[state] {instance.State.Name}, " +
                    $"[monitoring state] {instance.Monitoring.State}");
            }
        }
    }

    public static async Task ListAvailableZones()
    {
        Console.WriteLine("Available zones......");

        try
        {
            var response = await ec2Client.DescribeAvailabilityZonesAsync();

            foreach (var availabilityZone in response.AvailabilityZones)
            {
                Console.WriteLine($"[id] {availabilityZone.ZoneId}, " +
                    $"[region] {availabilityZone.RegionName.PadLeft(15)}, " +
                    $"[zone] {availabilityZone.RegionName.PadLeft(15)}");
            }

            Console.WriteLine($"You have access to {response.AvailabilityZones.Count} Availability Zones.");
        }
        catch (AmazonServiceException e)
        {
            Console.WriteLine("Caught Exception: " + e.Message);
            Console.WriteLine("Response Status Code: " + e.StatusCode);
            Console.WriteLine("Error Code: " + e.ErrorCode);
            Console.WriteLine("Request ID: " + e.RequestId);
        }
    }

    public static async Task StartInstance(string instanceId)
    {
        Console.WriteLine($"Starting...... {instanceId}");

        var request = new StartInstancesRequest
        {
            InstanceIds = new List<string> { instanceId }
        };

        try
        {
            var response = await ec2Client.StartInstancesAsync(request);

            Console.WriteLine($"Successfully started instance {instanceId}");
        }
        catch (AmazonEC2Exception e)
        {
            Console.WriteLine($"Failed to start instance ({e.Message})");
        }
    }

    public static void ListAvailableRegions()
    {

    }

    public static void StopInstance()
    {

    }

    public static void CreateInstance()
    {

    }

    public static void RebootInstance()
    {

    }

    public static void ListImages()
    {

    }
}

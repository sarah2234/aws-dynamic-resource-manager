using System;
using System.Threading.Tasks;
using Amazon;
using Amazon.EC2;
using Amazon.EC2.Model;
using Amazon.Runtime;

class Program
{
    private static AmazonEC2Client ec2Client;

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

            ec2Client = new AmazonEC2Client(credentials, RegionEndpoint.APNortheast2);

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

        while (true)
        {
            Console.WriteLine("                                                            ");
            Console.WriteLine("                                                            ");
            Console.WriteLine("------------------------------------------------------------");
            Console.WriteLine("           Amazon AWS Control Panel using SDK               ");
            Console.WriteLine("------------------------------------------------------------");
            Console.WriteLine("  1. list instance                2. available zones        ");
            Console.WriteLine("  1. list instance                2. available zones        ");
            Console.WriteLine("  3. start instance               4. available regions      ");
            Console.WriteLine("  5. stop instance                6. create instance        ");
            Console.WriteLine("  7. reboot instance              8. list images            ");
            Console.WriteLine("                                 99. quit                   ");
            Console.WriteLine("------------------------------------------------------------");

            Console.WriteLine("Enter an integer: ");
            string menu = Console.ReadLine();

            if (int.TryParse(menu, out int menuId))
            {
                switch (menuId)
                {
                    case 1:
                        break;

                    case 2:
                        break;

                    case 3:
                        break;

                    case 4:
                        break;

                    case 5:
                        break;

                    case 6:
                        break;

                    case 7:
                        break;

                    case 8:
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
}

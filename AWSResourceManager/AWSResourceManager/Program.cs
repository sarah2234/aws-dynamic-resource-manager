﻿using System;
using System.Threading.Tasks;
using Amazon;
using Amazon.EC2;
using Amazon.EC2.Model;
using Amazon.Runtime;
using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;

class Program
{
    private static AmazonEC2Client ec2Client;
    private static BasicAWSCredentials credentials;
    private static RegionEndpoint regionEndpoint = RegionEndpoint.APNortheast2;

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

            credentials = new BasicAWSCredentials(keys[0], keys[1]);

            ec2Client = new AmazonEC2Client(credentials, regionEndpoint);
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
            Console.WriteLine("  -1. terminate instance          0. input command          ");
            Console.WriteLine("                                 99. quit                   ");
            Console.WriteLine("------------------------------------------------------------");

            Console.Write("Enter an integer: ");
            string menu = Console.ReadLine();

            if (int.TryParse(menu, out int menuId))
            {
                string instanceId;
                string amiId;

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
                        instanceId = Console.ReadLine();
                        if (!string.IsNullOrWhiteSpace(instanceId))
                            await StartInstance(instanceId);
                        break;

                    case 4:
                        await ListAvailableRegions();
                        break;

                    case 5:
                        Console.Write("Enter instance id: ");
                        instanceId = Console.ReadLine();
                        if (!string.IsNullOrWhiteSpace(instanceId))
                            await StopInstance(instanceId);
                        break;

                    case 6:
                        Console.Write("Enter AMI id: ");
                        amiId = Console.ReadLine();
                        if (!string.IsNullOrWhiteSpace(amiId))
                            await CreateInstance(amiId);
                        break;

                    case 7:
                        Console.Write("Enter instance id: ");
                        instanceId = Console.ReadLine();
                        if (!string.IsNullOrWhiteSpace(instanceId))
                            await RebootInstance(instanceId);
                        break;

                    case 8:
                        await ListImages();
                        break;

                    case -1:
                        Console.Write("Enter instance id: ");
                        instanceId = Console.ReadLine();
                        if (!string.IsNullOrWhiteSpace(instanceId))
                            await TerminateInstance(instanceId);
                        break;

                    case 0:
                        Console.Write("Enter instance id: ");
                        instanceId = Console.ReadLine();
                        if (!string.IsNullOrWhiteSpace(instanceId))
                            await ExecuteCommand(instanceId);
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

        var response = await ec2Client.DescribeInstancesAsync(request);

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
            Console.WriteLine($"Failed to start the instance {instanceId} ({e.Message})");
        }
    }

    public static async Task ListAvailableRegions()
    {
        Console.WriteLine("Available regions......");

        var response = await ec2Client.DescribeRegionsAsync();
        
        foreach (var region in response.Regions)
        {
            Console.WriteLine($"[region] {region.RegionName.PadLeft(15)}, " +
                $"[endpoint] {region.Endpoint}");
        }
    }

    public static async Task StopInstance(string instanceId)
    {
        var request = new StopInstancesRequest
        {
            InstanceIds = new List<string> { instanceId }
        };

        try
        {
            var response = await ec2Client.StopInstancesAsync(request);

            Console.WriteLine($"Successfully stop instance {instanceId}");
        }
        catch (AmazonEC2Exception e)
        {
            Console.WriteLine($"Failed to stop the instance {instanceId} ({e.Message})");
        }
    }

    public static async Task CreateInstance(string amiId)
    {
        var request = new RunInstancesRequest
        {
            ImageId = amiId,
            InstanceType = InstanceType.T2Micro,
            MaxCount = 1,
            MinCount = 1
        };

        try
        {
            var response = await ec2Client.RunInstancesAsync(request);
            var reservationId = response.Reservation.Instances[0].InstanceId;

            Console.WriteLine($"Successfully started EC2 instance {reservationId} based on AMI {amiId}");
        }
        catch (AmazonEC2Exception e)
        {
            Console.WriteLine($"Failed to create the instance ({e.Message})");
        }
    }

    public static async Task RebootInstance(string instanceId)
    {
        Console.WriteLine("Rebooting......" + instanceId);

        var request = new RebootInstancesRequest
        {
            InstanceIds = new List<string> { instanceId }
        };

        try
        {
            var response = await ec2Client.RebootInstancesAsync(request);

            Console.WriteLine("Successfully rebooted instance " + instanceId);
        }
        catch (AmazonEC2Exception e) 
        {
            Console.WriteLine($"Failed to reboot the instance {instanceId} ({e.Message})");
        }
    }

    public static async Task ListImages()
    {
        Console.WriteLine("Listing images......");

        var request = new DescribeImagesRequest
        {
            Filters = new List<Filter>
            {
                new Filter
                {
                    Name = "name",
                    Values = new List<string> { "aws-htcondor-slave" }
                }
            }
        };

        var response = await ec2Client.DescribeImagesAsync(request);

        foreach (var image in response.Images)
        {
            Console.WriteLine($"[image id] {image.ImageId}, [name] {image.Name}, [owner] {image.OwnerId}");
        }
    }

    public static async Task<Instance> FindInstance(string instanceId)
    {
        var request = new DescribeInstancesRequest
        { InstanceIds = new List<string> { instanceId } };

        try
        {
            var response = await ec2Client.DescribeInstancesAsync(request);

            return response.Reservations[0].Instances[0];
        }
        catch (Exception e)
        {
            Console.WriteLine($"Cannot find the instance {instanceId}");
        }

        return null;
    }

    public static async Task ExecuteCommand(string instanceId)
    {
        if (await FindInstance(instanceId) == null)
        {
            return;
        }    

        var ssmClient = new AmazonSimpleSystemsManagementClient(credentials, regionEndpoint);

        while (true)
        {
            Console.Write("$ ");
            string command = Console.ReadLine();

            if (command == "exit")
                break;

            var commandRequest = new SendCommandRequest
            {
                InstanceIds = new List<string> { instanceId },
                DocumentName = "AWS-RunShellScript",
                Parameters = new Dictionary<string, List<string>>
                {
                    { "commands", new List<string> { command } }
                }
            };

            try
            {
                var commandResponse = await ssmClient.SendCommandAsync(commandRequest);


                var commandInvocationsRequest = new ListCommandInvocationsRequest
                {
                    InstanceId = instanceId,
                    CommandId = commandResponse.Command.CommandId
                };

                await Task.Delay(1000);
                var commandInvocationsResponse = await ssmClient.ListCommandInvocationsAsync(commandInvocationsRequest);

                foreach (var invocations in commandInvocationsResponse.CommandInvocations)
                {
                    var outputRequest = new GetCommandInvocationRequest
                    {
                        CommandId = commandResponse.Command.CommandId,
                        InstanceId = invocations.InstanceId
                    };

                    var outputResponse = await ssmClient.GetCommandInvocationAsync(outputRequest);

                    Console.WriteLine($"{outputResponse.StandardOutputContent}");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed to execute the command ({e.Message})");
            }
        }
    }

    public static async Task TerminateInstance(string instanceId)
    {
        Console.WriteLine("<Warning!> Terminating " + instanceId);
        Console.WriteLine("To confirm that you want to terminate the instances, type 'yes'. " +
            "Instances with termination protection enabled will not be terminated. " +
            "Terminating the instance cannot be undone.");
        Console.Write(">> ");

        var confirmTermination = Console.ReadLine();
        if (confirmTermination != "yes")
        {
            Console.WriteLine($"\nIncorrect input '{confirmTermination}'. Stop termination");
            return;
        }

        var request = new TerminateInstancesRequest
        {
            InstanceIds = new List<string> { instanceId }
        };

        try
        {
            var response = await ec2Client.TerminateInstancesAsync(request);

            Console.WriteLine("Successfully terminated instance " + instanceId);
        }
        catch (AmazonEC2Exception e)
        {
            Console.WriteLine($"Failed to terminate the instance {instanceId} ({e.Message})");
        }
    }
}

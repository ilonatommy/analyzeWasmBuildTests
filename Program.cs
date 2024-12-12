using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;


public class Program
{
    private static readonly HttpClient client = new HttpClient();

    public static async Task Main(string[] args)
    {
        // these urls could be extracted from the url to "Send to Helix" on the job
        string workloadsUrl = "https://helix.dot.net/api/jobs/2ef17a17-89d8-49ff-8672-cdf5cf6c6935/workitems?api-version=2019-06-17";
        string noWebCilUrl = "https://helix.dot.net/api/jobs/22fad180-240c-4720-bedd-6ef21cd3a0a5/workitems?api-version=2019-06-17";
        List<WorkItemDetails> workloadTestsInfo = await GetTestDetails(workloadsUrl);
        List<WorkItemDetails> noWebCilTestsInfo = await GetTestDetails(noWebCilUrl);

        Dictionary<string, TestItem> testItems = new();
        testItems = await ProcessTestDetails(workloadTestsInfo, testItems);
        testItems = await ProcessTestDetails(noWebCilTestsInfo, testItems);

        var sortedTestItems = testItems.Values.OrderBy(t => t.Duration).ToList();

        foreach (var details in sortedTestItems)
        {
            Console.WriteLine(details);
        }
    }

    private static async Task<List<WorkItemDetails>> GetTestDetails(string url)
    {
        string jsonString = await client.GetStringAsync(url);

        List<WorkItem> workItems = JsonSerializer.Deserialize<List<WorkItem>>(jsonString);
        List<WorkItemDetails> workItemDetailsList = new();

        foreach (var item in workItems)
        {
            string detailsJsonString = await client.GetStringAsync(item.DetailsUrl);
            WorkItemDetails details = JsonSerializer.Deserialize<WorkItemDetails>(detailsJsonString);
            if (details != null)
                workItemDetailsList.Add(details);
        }
        return workItemDetailsList;
    }

    private static async Task<Dictionary<string, TestItem>> ProcessTestDetails(List<WorkItemDetails> workItemDetails, Dictionary<string, TestItem> testItems)
    {
        foreach (var details in workItemDetails)
        {
            string[] nameParts = details.Name.Split('-');
            if (nameParts.Length > 1)
            {
                string processedName = string.Join("-", nameParts, 1, nameParts.Length - 1);
                int totalTests = await GetNumberOfTests(details.ConsoleOutputUri);
                if (testItems.ContainsKey(processedName))
                {
                    testItems[processedName].Duration += details.Duration;
                    testItems[processedName].State &= (details.State == "Passed");
                    testItems[processedName].Details += $"; {nameParts[0]}: {details.Duration}";
                    testItems[processedName].NumberOfTests += totalTests;
                }
                else
                {
                    var newItem = new TestItem { 
                        Name = processedName,
                        Duration = details.Duration,
                        State = details.State == "Passed",
                        Details = $"{nameParts[0]}: {details.Duration}",
                        NumberOfTests = totalTests
                    };
                    testItems.Add(processedName, newItem);
                }
            }
        }
        return testItems;
    }

    private static async Task<int> GetNumberOfTests(string url)
    {
        string consoleOutput = await client.GetStringAsync(url);
        string[] lines = consoleOutput.Split('\n');
        bool readNextLine = false;
        foreach (var line in lines)
        {
            if (readNextLine)
            {
                readNextLine = false;
                string[] parts = line.Split(',');
                if (parts.Length != 0)
                {
                    string totalString = parts[0].Split("Total:")[1].Trim();
                    if (int.TryParse(totalString, out int total))
                    {
                        return total;
                    }
                }
                break;
            }
            if (line.StartsWith("=== TEST EXECUTION SUMMARY ==="))
            {
                readNextLine = true;
            }
        }
        return 0;
    }
}
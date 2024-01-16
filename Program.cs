using Newtonsoft.Json;
using System.Text;


var currentDirectory = Directory.GetCurrentDirectory();
var storesDirectory = Path.Combine(currentDirectory, "stores");
var salesTotalDir = Path.Combine(currentDirectory, "salesTotalDir");

Directory.CreateDirectory(salesTotalDir);

var salesFiles = FindFiles(storesDirectory);
var salesTotal = CalculateSalesTotal(salesFiles);

File.AppendAllText(Path.Combine(salesTotalDir, "totals.txt"), $"{salesTotal}{Environment.NewLine}");

// Call the new method and write the report to a new file
var salesReport = GenerateSalesReport(salesFiles);
File.WriteAllText(Path.Combine(salesTotalDir, "report.txt"), salesReport);

IEnumerable<string> FindFiles(string folderName)
{
    List<string> salesFiles = new List<string>();
    var foundFiles = Directory.EnumerateFiles(folderName, "*", SearchOption.AllDirectories);

    foreach (var file in foundFiles)
    {
        var extension = Path.GetExtension(file);
        if (extension == ".json")
        {
            salesFiles.Add(file);
        }
    }

    return salesFiles;
}

double CalculateSalesTotal(IEnumerable<string> salesFiles)
{
    double salesTotal = 0;

    // Loop over each file path in salesFiles
    foreach (var file in salesFiles)
    {
        // Read the contents of the file
        string salesJson = File.ReadAllText(file);

        // Parse the contents as JSON
        SalesData? data = JsonConvert.DeserializeObject<SalesData?>(salesJson);

        // Add the amount found in the Total field to the salesTotal variable
        salesTotal += data?.Total ?? 0;
    }

    return salesTotal;
}

// Create a new method that generates a sales summary report
string GenerateSalesReport(IEnumerable<string> salesFiles)
{
    // Create a StringBuilder object to store the report
    StringBuilder report = new StringBuilder();

    // Append the sales summary header
    report.AppendLine("Sales Summary");
    report.AppendLine("----------------------------");

    // Append the total sales
    report.AppendLine($"Total Sales: ${CalculateSalesTotal(salesFiles):N2}");

    // Append the details header
    report.AppendLine();
    report.AppendLine("Details:");

    // Loop over each file path in salesFiles
    foreach (var file in salesFiles)
    {
        // Read the contents of the file
        string salesJson = File.ReadAllText(file);

        // Parse the contents as JSON
        SalesData? data = JsonConvert.DeserializeObject<SalesData?>(salesJson);

        // Append the file name and the total sales for that file
        report.AppendLine($" {Path.GetFileName(file)}: ${data?.Total:N2}");
    }

    // Return the report as a string
    return report.ToString();
}

record SalesData (double Total);

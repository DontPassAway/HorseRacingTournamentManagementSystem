using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;

string dir = @"c:\Users\phamt\source\repos\HorseRacingTournamentManagementSystem\HorseRacing.Application\DTOs";
var files = Directory.GetFiles(dir, "*Dtos.cs", SearchOption.AllDirectories);

foreach (var file in files)
{
    string content = File.ReadAllText(file);
    
    var regex = new Regex(@"public record ([A-Za-z0-9_]+Dto)\s*\(([^)]+)\)\s*;", RegexOptions.Multiline | RegexOptions.Singleline);
    
    string newContent = regex.Replace(content, match =>
    {
        string name = match.Groups[1].Value;
        
        if (name.StartsWith("Create") || name.StartsWith("Update") || 
            name.StartsWith("Register") || name.StartsWith("Login") || 
            name.StartsWith("Respond") || name.StartsWith("Approve") || 
            name.StartsWith("Confirm") || name.StartsWith("Reject") ||
            name == "AuthResponseDto")
        {
            return match.Value;
        }
        
        string args = match.Groups[2].Value;
        var props = args.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(p => p.Trim())
                        .Select(p => 
                        {
                            int commentIdx = p.IndexOf("//");
                            if (commentIdx >= 0) p = p.Substring(0, commentIdx).Trim();
                            
                            var parts = p.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            if (parts.Length == 2)
                            {
                                string type = parts[0];
                                string propName = parts[1];
                                return $"    public {type} {propName} {{ get; set; }}";
                            }
                            return "";
                        }).Where(p => !string.IsNullOrEmpty(p));
        
        return $"public class {name}\n{{\n{string.Join("\n", props)}\n}}";
    });
    
    if (content != newContent)
    {
        File.WriteAllText(file, newContent);
        Console.WriteLine("Updated " + Path.GetFileName(file));
    }
}

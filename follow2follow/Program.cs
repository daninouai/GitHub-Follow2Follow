using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

class Program
{
    private static async Task Main(string[] args)
    {
        Console.Write("enter username: ");
        string username = Console.ReadLine();

        var httpClient = new HttpClient();

        try
        {
            var followersUrl = $"https://github.com/{username}?tab=followers";
            var followingUrl = $"https://github.com/{username}?tab=following";

            var followersHtml = await httpClient.GetStringAsync(followersUrl);
            var followingHtml = await httpClient.GetStringAsync(followingUrl);

            var followers = ExtractUsernames(followersHtml);
            var following = ExtractUsernames(followingHtml);

            var notFollowingBack = following.Except(followers).ToList();

            Console.WriteLine($"user {username} has {followers.Count} followers.");
            Console.WriteLine($"user {username} is following {following.Count} people.");
            Console.WriteLine($"people that {username} is following but are not following back:");

            foreach (var user in notFollowingBack)
            {
                Console.WriteLine(user);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"error: {ex.Message}");
        }
    }

    private static List<string> ExtractUsernames(string html)
    {
        var usernames = new List<string>();

        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        var nodes = doc.DocumentNode.SelectNodes("//a[@href]");

        if (nodes != null)
        {
            foreach (var node in nodes)
            {
                var href = node.GetAttributeValue("href", string.Empty);

                if (href.StartsWith("/") && !href.Contains("login"))
                {
                    var username = href.Substring(1);
                    usernames.Add(username);
                }
            }
        }

        return usernames.Distinct().ToList();
    }

}
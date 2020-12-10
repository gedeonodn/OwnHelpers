using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
namespace magsimpl
{

    class Program
    {
        static void Main(string[] args)
        {
            if (!new string[] { "-n", "--noemptyline" }.Intersect(args).Any() || args.Length == 0)
            {
                Console.WriteLine();
            }
            if (new string[] { "-h", "--help" }.Intersect(args).Any() || args.Length == 0)
            {
                PrintHelp();
                return;
            }
            else if (new string[] { "-d", "--decode" }.Intersect(args).Any())
            {
                Decode(args.Last());
                return;
            }

            else if (new string[] { "-f", "--to-first-bracket" }.Intersect(args).Any())
            {
                BeforeFirstBracket(args.Last());
                return;
            }
            else if (new string[] { "-l", "--to-last-bracket" }.Intersect(args).Any())
            {
                WithLastBracket(args.Last());
                return;
            }
            else
            {
                string delim = args
                    .Where(p => new Regex("-b=").IsMatch(p) || new Regex("--to-symbol-before=").IsMatch(p))
                    .FirstOrDefault()?
                    .Split('=')
                    .ElementAt(1)
                    .Trim();
                if (!string.IsNullOrWhiteSpace(delim))
                {
                    Before(HttpUtility.UrlEncode(delim), args.Last());
                    return;
                }

                delim = args
                    .Where(p => new Regex("-a=").IsMatch(p) || new Regex("--to-symbol-after=").IsMatch(p))
                    .FirstOrDefault()?
                    .Split('=')
                    .ElementAt(1)
                    .Trim();
                if (!string.IsNullOrWhiteSpace(delim))
                {
                    After(HttpUtility.UrlEncode(delim), args.Last());
                    return;
                }

                PrintHelp();
            }


        }

        static void Decode(string input)
        {
            input = new Regex("(?<=&dn=).*").Match(input).ToString();
            Console.WriteLine(HttpUtility.UrlDecode(input));
        }

        static void BeforeFirstBracket(string input)
        {
            Before("\\(", input);
        }

        static void WithLastBracket(string input)
        {
            After("\\)", input);
        }

        static void Before(string delimeter, string input)
        {
            delimeter = delimeter.Replace("+", "%20");
            input = new Regex($"^.*&dn=.*(?={delimeter.Trim('"')})", RegexOptions.IgnoreCase).Match(input).ToString().TrimEnd(new char[] { '%', '2', '0' });
            Console.WriteLine(input);
        }

        static void After(string delimeter, string input)
        {
            delimeter = delimeter.Replace("+", "%20");
            input = new Regex($"^.*&dn=.*{delimeter.Trim('"')}", RegexOptions.IgnoreCase).Match(input).ToString();
            Console.WriteLine(input);
        }

        static void PrintHelp()
        {
            Console.WriteLine("magsimple [Options] \"magnet url\"");
            Console.WriteLine();
            Console.WriteLine("Options:");
            Console.WriteLine("-h\t\t--help\t\t\t\tShow this help");
            Console.WriteLine("-d\t\t--decode\t\t\tShow decoded torrent name");
            Console.WriteLine("-f\t\t--to-first-bracket\t\t\tMagnet url with name cut to first (, not include");
            Console.WriteLine("-l\t\t--to-last-bracket\t\t\tMagnet url with name cut to last ), include");
            Console.WriteLine("-b=\"delimeter\"\t--to-symbol-before=\"delimeter\"\tMagnet url with name cut to delimeter, not include");
            Console.WriteLine("-a=\"delimeter\"\t--to-symbol-after=\"delimeter\"\tMagnet url with name cut to last delimeter, include");
            Console.WriteLine("-n\t\t--noemptyline\t\t\tDo not print first empty line");
        }
    }
}

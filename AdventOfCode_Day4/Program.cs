
using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using System.Text.RegularExpressions;

namespace AdventOfCode_Day4
{

    class Validator
    {
        static HashSet<string> _validEyeColors = new HashSet<string>()
        {
            "amb",
            "blu",
            "brn",
            "gry",
            "grn",
            "hzl",
            "oth"
        };

        static Dictionary<string, Tuple<int, int>> _validHeightsByUnit = new Dictionary<string, Tuple<int, int>>()
        {
            {
                "cm", new Tuple<int, int>(150, 193)
            },
            {
                "in", new Tuple<int, int>(59, 76)
            }
        };

        public static bool isNumberBetween(string value, int min, int max)
        {
            try
            {
                int number = Int32.Parse(value, NumberStyles.Integer);
                return min <= number && number <= max;
            } catch (FormatException)
            {
                Console.WriteLine("Unable to convert {0}.", value);
            } catch (OverflowException)
            {
                Console.WriteLine("Unable to convert {0}.", value);
            }

            return false;
        }

        public static bool isDateBetween(string date, int min, int max)
        {
            return date.Length == 4 && isNumberBetween(date, min, max);
        }

        public static bool isValidHeight(string height)
        {
            Regex rx = new Regex(@"(?<value>[0-9]+)(?<unit>[a-z]+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            Match match = rx.Match(height);

            if (!match.Success)
                return false;

            string unit = match.Groups["unit"].Value.ToLower();
            string value = match.Groups["value"].Value;

            return isNumberBetween(value, _validHeightsByUnit[unit].Item1, _validHeightsByUnit[unit].Item2);
        }

        public static bool isRegexMatch(string regex, string value)
        {
            return (new Regex(regex, RegexOptions.Compiled | RegexOptions.IgnoreCase)).IsMatch(value);
        }

        public static bool isValidEyeColor(string value)
        {
            return _validEyeColors.Contains(value);
        }
    }

    class PassportInformation
    {
        public string birth_year = null;
        public string issue_year = null;
        public string expiration_year = null;
        public string height = null;
        public string hair_color = null;
        public string eye_color = null;
        public string passport_id = null;
        public string country_id = null;

        public bool hasRequiredFields()
        {
            return birth_year != null && issue_year != null && expiration_year != null && height != null && hair_color != null && eye_color != null && passport_id != null;
        }

        public bool requiredFieldsValid()
        {
            if (!hasRequiredFields())
            {
                return false;
            }

            return Validator.isDateBetween(birth_year, 1920, 2002) &&
                Validator.isDateBetween(issue_year, 2010, 2020) &&
                Validator.isDateBetween(expiration_year, 2020, 2030) &&
                Validator.isValidHeight(height) &&
                Validator.isRegexMatch(@"#[0-9a-f]{6}", hair_color) &&
                Validator.isRegexMatch(@"[0-9]{9}", passport_id);
        }
    }

    class Parser
    {
        public List<PassportInformation> parse(string path)
        {
            List<PassportInformation> passports = new List<PassportInformation>();
            int linecount = 0;

            using (StreamReader sr = File.OpenText(path))
            {
                string line = "";
                string rawPassportData = "";

                while ((line = sr.ReadLine()) != null)
                {
                    if (line.Trim().Length == 0)
                    {
                        passports.Add(passportInformationFromInput(rawPassportData));
                        rawPassportData = "";
                        continue;
                    }

                    rawPassportData = String.Concat(rawPassportData, " ", line);
                }

                passports.Add(passportInformationFromInput(rawPassportData));
            }

            return passports;
        }

        PassportInformation passportInformationFromInput(string input)
        {
            PassportInformation info = new PassportInformation();
            string[] parts = input.Trim().Split(" ");

            foreach (string part in parts)
            {
                string[] kvp = part.Split(":");
                kvp[0] = kvp[0].Trim();
                kvp[1] = kvp[1].Trim();

                switch (kvp[0])
                {
                    case "byr":
                        info.birth_year = kvp[1];
                        break;
                    case "iyr":
                        info.issue_year = kvp[1];
                        break;
                    case "eyr":
                        info.expiration_year = kvp[1];
                        break;
                    case "hgt":
                        info.height = kvp[1];
                        break;
                    case "hcl":
                        info.hair_color = kvp[1];
                        break;
                    case "ecl":
                        info.eye_color = kvp[1];
                        break;
                    case "pid":
                        info.passport_id = kvp[1];
                        break;
                    case "cid":
                        info.country_id = kvp[1];
                        break;
                }
            }

            return info;
        }
    }

    class PartOne
    {
        Parser _parser = new Parser();

        public void solve()
        {
            int count = 0;
            List<PassportInformation> passports = _parser.parse("solution_input.txt");

            foreach (PassportInformation pi in passports)
            {
                if (pi.hasRequiredFields())
                    count++;
            }

            Console.WriteLine($"Count: {count:D}");
        }
    }

    class PartTwo
    {
        Parser _parser = new Parser();

        public void solve()
        {
            int count = 0;
           
            foreach (PassportInformation pi in _parser.parse("solution_input.txt"))
            {
                if (pi.requiredFieldsValid())
                {
                    count++;
                }
            }

            Console.WriteLine("Count: {0}", count);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            PartOne po = new PartOne();
            po.solve();

            Console.WriteLine();

            PartTwo pt = new PartTwo();
            pt.solve();
        }
    }
}

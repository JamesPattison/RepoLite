using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace RepoLite.GeneratorEngine.Generators.BaseParsers.Base
{
    public abstract class BaseClassParser
    {
        private const string VariablePostFix = "@@@";
        private const string EndIdentifier = "//";
        private string _elseIdentifier = "@@@ELSE";
        private readonly string[] _badWords = { "ELSE" };

        private readonly Func<string, string> _closingKey = s => $"{s.Substring(0, VariablePostFix.Length)}{EndIdentifier}{s.Substring(VariablePostFix.Length)}";
        private readonly BaseClassParseOptions _parseOptions;
        private readonly List<string> _validKeys;

        protected BaseClassParser(List<string> validKeys) : this(validKeys, new BaseClassParseOptions()) { }
        protected BaseClassParser(List<string> validKeys, BaseClassParseOptions parseOptions)
        {
            _parseOptions = parseOptions;
            _validKeys = validKeys;
        }

        public abstract string BuildBaseRepository();
        public abstract string BuildBaseModel();

        public string Parse(string template)
        {
            var templateLines = Regex.Split(template, "\r\n|\r|\n").ToList();

            if (_validKeys.Any(x => _badWords.Any(x.Contains)))
                throw new Exception($"Keys can not be any of: {string.Join(", ", _badWords)}");
            if (templateLines.Any(x => _validKeys.Any(x.Contains)))
            {
                foreach (var line in templateLines)
                {
                    var line1 = line;
                    if (!_validKeys.Any(x => line1.Contains(x)))
                        continue;

                    if (line.Contains("||") || line.Contains("&&"))
                    {
                        if (line.Contains("||"))
                        {
                            //We've got an or combined logic case
                            var split = line.Replace("@@@", string.Empty).Replace("||", "|").Split('|')
                                .Select(x => x.TrimEnd().TrimStart()).ToArray();

                            if (!split.Any(_validKeys.Contains))
                                continue;
                        }
                        else if (line.Contains("&&"))
                        {
                            //We've got an and combined logic case
                            var split = line.Replace("@@@", string.Empty).Replace("&&", "&").Split('&')
                                .Select(x => x.TrimEnd().TrimStart()).ToArray();

                            if (!split.All(_validKeys.Contains))
                                continue;
                        }

                        ValidClauseRemoveElse(templateLines, line);
                        return Parse(string.Join(Environment.NewLine, templateLines));
                    }

                    ValidClauseRemoveElse(templateLines, line);
                    return Parse(string.Join(Environment.NewLine, templateLines));
                }
            }

            RemoveUnsatisfiedKeys(ref templateLines);

            if (_parseOptions.RemoveMultipleBlankLines)
            {
                for (var i = templateLines.Count - 1; i > 0; i--)
                {
                    var line = templateLines[i];
                    if (!string.IsNullOrWhiteSpace(line))
                        continue;

                    var nextLine = templateLines[i - 1];
                    if (string.IsNullOrWhiteSpace(nextLine))
                        templateLines.RemoveAt(i);
                }
            }

            return string.Join(Environment.NewLine, templateLines);
        }

        private void ValidClauseRemoveElse(List<string> templateLines, string line)
        {
            var openingLineIndex = templateLines.IndexOf(line);
            var elseIndex = -1;
            var closingLineIndex = templateLines.IndexOf(_closingKey(line));

            for (var j = openingLineIndex; j < closingLineIndex; j++)
            {
                if (!templateLines[j].Contains($"{_elseIdentifier} [{line.Replace(VariablePostFix, string.Empty)}]"))
                    continue;
                elseIndex = j;
                break;
            }

            if (elseIndex > -1)
            {
                for (var j = closingLineIndex; j >= elseIndex; j--)
                {
                    templateLines.RemoveAt(j);
                }

                templateLines.RemoveAt(openingLineIndex);
            }
            else
            {
                templateLines.RemoveAt(closingLineIndex);
                templateLines.RemoveAt(openingLineIndex);
            }
        }

        private void RemoveUnsatisfiedKeys(ref List<string> templateLines)
        {
            var unsatisfiedKeys = templateLines.Where(x => x.Contains(VariablePostFix) && !x.Contains(EndIdentifier)).ToArray();

            foreach (var unsatisfiedKey in unsatisfiedKeys.Where(x => !x.StartsWith(_elseIdentifier)))
            {
                var openingLineIndex = templateLines.IndexOf(unsatisfiedKey);
                var elseIndex = -1;
                var closingLineIndex = templateLines.IndexOf(_closingKey(unsatisfiedKey));

                for (var i = openingLineIndex; i < closingLineIndex; i++)
                {
                    if (!templateLines[i].Contains($"{_elseIdentifier} [{unsatisfiedKey.Replace(VariablePostFix, string.Empty)}]"))
                        continue;
                    elseIndex = i;
                    break;
                }

                if (closingLineIndex == -1)
                    throw new Exception($"Key {unsatisfiedKey} has no closing key");

                if (elseIndex > -1)
                {
                    templateLines.RemoveAt(closingLineIndex);
                    for (var i = elseIndex; i >= openingLineIndex; i--)
                    {
                        templateLines.RemoveAt(i);
                    }
                }
                else
                {
                    //In reverse as removing by index
                    for (var i = closingLineIndex; i >= openingLineIndex; i--)
                    {
                        templateLines.RemoveAt(i);
                    }
                }
            }
        }
    }
}

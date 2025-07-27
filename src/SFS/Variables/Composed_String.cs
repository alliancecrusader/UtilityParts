using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using SFS.Variables;

namespace SFS.Variables
{
    [Serializable]
    public class Composed_String : Composed<string>
    {
        public string input;
        private Dictionary<string, string> variableCache;
        private List<string> usedVariables;

        protected override string GetResult(bool initialize)
        {
            if (initialize)
            {
                // Parse the input string to find variable references
                usedVariables = ExtractVariableNames(input);
                variableCache = new Dictionary<string, string>();

                // Register change callbacks for all used variables
                foreach (string variableName in usedVariables)
                {
                    // Register callbacks for different variable types
                    if (variables.stringVariables.Has(variableName))
                    {
                        variables.stringVariables.RegisterOnVariableChange(base.Recalculate, variableName);
                    }
                    else if (variables.doubleVariables.Has(variableName))
                    {
                        variables.doubleVariables.RegisterOnVariableChange(base.Recalculate, variableName);
                    }
                    else if (variables.boolVariables.Has(variableName))
                    {
                        variables.boolVariables.RegisterOnVariableChange(base.Recalculate, variableName);
                    }
                }
            }

            return ProcessString(input);
        }

        protected override bool Equals(string a, string b)
        {
            return string.Equals(a, b);
        }

        public Composed_String(string input)
        {
            this.input = input ?? string.Empty;
        }

        /// <summary>
        /// Extracts variable names from the input string using both {variableName} and direct variable patterns
        /// </summary>
        private List<string> ExtractVariableNames(string text)
        {
            List<string> variables = new List<string>();

            // Extract bracketed variables {variableName}
            Regex bracketedRegex = new Regex(@"\{([^}]+)\}");
            MatchCollection bracketedMatches = bracketedRegex.Matches(text);
            foreach (Match match in bracketedMatches)
            {
                string variableName = match.Groups[1].Value;
                if (!variables.Contains(variableName))
                {
                    variables.Add(variableName);
                }
            }

            // Extract direct variable references (word boundaries, letters/numbers/underscore)
            Regex directRegex = new Regex(@"\b([a-zA-Z_][a-zA-Z0-9_]*)\b");
            MatchCollection directMatches = directRegex.Matches(text);
            foreach (Match match in directMatches)
            {
                string potentialVariable = match.Groups[1].Value;

                // Check if this is actually a variable that exists
                if (IsValidVariable(potentialVariable) && !variables.Contains(potentialVariable))
                {
                    variables.Add(potentialVariable);
                }
            }

            return variables;
        }

        /// <summary>
        /// Checks if a string is a valid variable name that exists in the variables module
        /// </summary>
        private bool IsValidVariable(string name)
        {
            return variables.stringVariables.Has(name) ||
                   variables.doubleVariables.Has(name) ||
                   variables.boolVariables.Has(name);
        }

        /// <summary>
        /// Processes the string by replacing both bracketed and direct variable references with actual values
        /// </summary>
        private string ProcessString(string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            string result = text;

            // First, replace bracketed variable placeholders {variableName}
            Regex bracketedRegex = new Regex(@"\{([^}]+)\}");
            result = bracketedRegex.Replace(result, match =>
            {
                string variableName = match.Groups[1].Value;
                return GetVariableValue(variableName);
            });

            // Then, replace direct variable references (but only if they're actual variables)
            Regex directRegex = new Regex(@"\b([a-zA-Z_][a-zA-Z0-9_]*)\b");
            result = directRegex.Replace(result, match =>
            {
                string potentialVariable = match.Groups[1].Value;
                if (IsValidVariable(potentialVariable))
                {
                    return GetVariableValue(potentialVariable);
                }
                return match.Value; // Keep original text if not a variable
            });

            return result;
        }

        /// <summary>
        /// Gets the string representation of a variable value
        /// </summary>
        private string GetVariableValue(string variableName)
        {
            // Try string variables first
            if (variables.stringVariables.Has(variableName))
            {
                return variables.stringVariables.GetValue(variableName) ?? string.Empty;
            }

            // Try double variables
            if (variables.doubleVariables.Has(variableName))
            {
                double value = variables.doubleVariables.GetValue(variableName);
                return value.ToString("G"); // General format, removes unnecessary zeros
            }

            // Try bool variables
            if (variables.boolVariables.Has(variableName))
            {
                bool value = variables.boolVariables.GetValue(variableName);
                return value.ToString();
            }

            // Variable not found, return empty string for bracketed, original for direct
            return string.Empty;
        }

        /// <summary>
        /// Sets a new input string and triggers recalculation
        /// </summary>
        public void SetInput(string newInput)
        {
            if (input != newInput)
            {
                input = newInput ?? string.Empty;

                // Clear cache and re-initialize if already initialized
                if (usedVariables != null)
                {
                    variableCache?.Clear();
                    usedVariables = null;
                    Recalculate();
                }
            }
        }

        /// <summary>
        /// Appends text to the current input
        /// </summary>
        public void Append(string text)
        {
            SetInput(input + text);
        }

        /// <summary>
        /// Prepends text to the current input
        /// </summary>
        public void Prepend(string text)
        {
            SetInput(text + input);
        }

        /// <summary>
        /// Gets a list of variable names used in this composed string
        /// </summary>
        public List<string> GetUsedVariables()
        {
            return ExtractVariableNames(input);
        }
    }
}

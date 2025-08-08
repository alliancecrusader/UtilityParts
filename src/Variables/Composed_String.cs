using System;
using System.Collections.Generic;
using SFS.Variables;

namespace UtilityParts.Variables
{
    [Serializable]
    public class Composed_String : Composed<string>
    {
        public string input;
        private List<string> usedVariables;

        protected override string GetResult(bool initialize)
        {
            if (initialize)
            {
                usedVariables = GetVariablesUsed(input);
                foreach (string item in usedVariables)
                {
                    variables.stringVariables.RegisterOnVariableChange(base.Recalculate, item);
                }
            }
            
            return ProcessInput(input);
        }

        protected override bool Equals(string a, string b)
        {
            return a == b;
        }

        public Composed_String(string a)
        {
            input = a ?? string.Empty;
        }

        private List<string> GetVariablesUsed(string text)
        {
            List<string> vars = new List<string>();

            if (variables.stringVariables.Has(text))
            {
                vars.Add(text);
            }
            
            return vars;
        }

        private string ProcessInput(string text)
        {
            if (variables.stringVariables.Has(text))
            {
                return variables.stringVariables.GetValue(text);
            }
            
            return text;
        }
    }   
}
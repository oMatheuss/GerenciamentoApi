using System.Text.RegularExpressions;

namespace GerenciamentoAPI.Helpers
{
    public static class Regexes
    {
        public static readonly Regex Email = new("^\\S+@\\S+$");
    }
}

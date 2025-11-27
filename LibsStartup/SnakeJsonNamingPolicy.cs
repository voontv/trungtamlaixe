using System.Linq;
using System.Text.Json;

namespace Ttlaixe.LibsStartup
{
    public class SnakeJsonNamingPolicy : JsonNamingPolicy
    {
        public override string ConvertName(string name)
        {
            return string.Concat(name.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x : x.ToString())).ToLower();
        }
    }
}
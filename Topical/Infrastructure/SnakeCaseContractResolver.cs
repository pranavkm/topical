using System;
using System.Text;
using Newtonsoft.Json.Serialization;

namespace Topical.Infrastructure
{
    public class SnakeCaseContractResolver : DefaultContractResolver
    {
        protected override string ResolvePropertyName(string propertyName)
        {
            var builder = new StringBuilder(propertyName.Length);

            for (int i = 0; i < propertyName.Length; i++)
            {
                char current = propertyName[i];
                if (Char.IsUpper(current))
                {
                    if (i == 0)
                    {
                        builder.Append(Char.ToLowerInvariant(current));
                    }
                    else
                    {
                        builder.Append('_')
                               .Append(Char.ToLowerInvariant(current));
                    }
                }
                else
                {
                    builder.Append(current);
                }
            }

            return builder.ToString();
        }
    }


}
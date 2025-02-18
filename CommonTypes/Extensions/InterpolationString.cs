using System.Text;

namespace CommonTypes.Extensions
{
    internal class InterpolationString : IInterpolationString
    {
        private IEnumerable<InterpolationPart> _parts;
        public string Generate(Func<string, string> reslove)
        {
            var sb = new StringBuilder();
            foreach (var part in _parts)
            {
                if (part.Type == InterpolationPartType.Text)
                    sb.Append(part.Value);
                else
                    sb.Append(reslove(part.Value));
            }
            return sb.ToString();
        }
        public async ValueTask<string> Generate(Func<string, ValueTask<string>> reslove)
        {
            var sb = new StringBuilder();
            foreach (var part in _parts)
            {
                if (part.Type == InterpolationPartType.Text)
                    sb.Append(part.Value);
                else
                    sb.Append(await reslove(part.Value));
            }
            return sb.ToString();
        }
        public InterpolationString(IEnumerable<InterpolationPart> parts)
        {
            _parts = parts;
        }
    }
}

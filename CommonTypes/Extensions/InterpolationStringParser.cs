using System.Collections.Concurrent;

namespace CommonTypes.Extensions
{
    internal class InterpolationStringParser
    {
        private static ConcurrentDictionary<string, IInterpolationString> _staticCache = new ConcurrentDictionary<string, IInterpolationString>();
        private static InnerInterpolationStringParser _innerParser = new InnerInterpolationStringParser();

        private static IEnumerable<InterpolationPart> _parseArguments(string value)
        {
            return _innerParser.Visit(value);
        }
        public static IInterpolationString Parse(string value)
        {
            return _staticCache.GetOrAdd(value, (key) =>
            {
                return new InterpolationString(_parseArguments(key));
            });
        }
        private InterpolationStringParser()
        {

        }
    }
    internal enum InterpolationPartType
    {
        /// <summary>
        /// text
        /// </summary>
        Text = 0,
        /// <summary>
        /// argument
        /// </summary>
        Argu = 1
    }
    internal struct InterpolationPart
    {
        public InterpolationPartType Type;
        public string Value;
    }
}

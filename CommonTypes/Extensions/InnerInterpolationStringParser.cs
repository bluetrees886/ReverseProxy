using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonTypes.Extensions
{
    internal class InnerInterpolationStringParser : BaseParser<IEnumerable<InterpolationPart>>
    {
        private enum InterpolationState
        {
            None = 0,
            InterPending = 1,
            Inter = 2,
            Error = 3
        }
        private static string _buildInterpolation(ReadOnlySpan<char> text, int begin, int end)
        {
            return new string(text.Slice(begin, end - begin));
        }
        private static (char[] Buf, int Len) _compressText(ReadOnlySpan<char> text, int begin, int end)
        {
            var ret = new char[end - begin];
            var len = 0;
            var state = 0;
            for (int i = begin; i < end; i++)
            {
                var ch = text[i];
                if (state == 0)
                {
                    if (ch == '{')
                        state = 1;
                    ret[len++] = ch;
                }
                else
                {
                    state = 0;
                }
            }
            return (ret, len);
        }
        private static string _buildText(ReadOnlySpan<char> text, int begin, int end)
        {
            var cp = _compressText(text, begin, end);
            return new string(cp.Buf, 0, cp.Len);
        }
        private static string _buildText(ReadOnlySpan<char> text, int begin)
        {
            var cp = _compressText(text, begin, text.Length);
            return new string(cp.Buf, 0, cp.Len);
        }
        private static void _processFinal(ReadOnlySpan<char> text, ref VisitArgument<IEnumerable<InterpolationPart>> argument)
        {
            if (text.Length > argument.Begin)
            {
                argument.Result = argument.Result.Append(new InterpolationPart()
                {
                    Type = InterpolationPartType.Text,
                    Value = _buildText(text, argument.Begin)
                });
            }
        }
        private static void _visitNone(ReadOnlySpan<char> text, scoped ref VisitArgument<IEnumerable<InterpolationPart>> argument)
        {
            var ch = text[argument.Position];
            if (ch == '{')
            {
                argument.State = (int)InterpolationState.InterPending;
            }
            argument.Position++;
        }
        private static void _visitInterPending(ReadOnlySpan<char> text, scoped ref VisitArgument<IEnumerable<InterpolationPart>> argument)
        {
            var ch = text[argument.Position];
            if (ch == '{')
            {
                argument.State = (int)InterpolationState.None;
            }
            else
            {
                var pos = argument.Position - 1;
                if (pos > argument.Begin)
                {
                    var ret = new InterpolationPart()
                    {
                        Type = InterpolationPartType.Text,
                        Value = _buildText(text, argument.Begin, pos),
                    };
                    argument.Result = argument.Result.Append(ret);
                }
                argument.State = (int)InterpolationState.Inter;
                argument.Begin = argument.Position;
            }
            argument.Position++;
        }
        private static void _visitInter(ReadOnlySpan<char> text, scoped ref VisitArgument<IEnumerable<InterpolationPart>> argument)
        {
            var ch = text[argument.Position];
            if (ch == '}')
            {
                var pos = argument.Position;
                if (pos > argument.Begin)
                {
                    var ret = new InterpolationPart()
                    {
                        Type = InterpolationPartType.Argu,
                        Value = _buildInterpolation(text, argument.Begin, pos),
                    };
                    argument.Result = argument.Result.Append(ret);
                }
                argument.State = (int)InterpolationState.None;
                argument.Begin = argument.Position + 1;
            }
            argument.Position++;
        }

        protected override IEnumerable<InterpolationPart> getDefaultResult()
        {
            return Enumerable.Empty<InterpolationPart>();
        }
        protected override void finalVisit(ReadOnlySpan<char> text, scoped ref VisitArgument<IEnumerable<InterpolationPart>> argument)
        {
            _processFinal(text, ref argument);
        }
        public InnerInterpolationStringParser()
        {
            addVisit((int)InterpolationState.None, _visitNone);
            addVisit((int)InterpolationState.InterPending, _visitInterPending);
            addVisit((int)InterpolationState.Inter, _visitInter);
        }
    }
}

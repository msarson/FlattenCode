using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace FlattenCode
{
    /// <summary>
    /// Flattens Clarion line continuations (|) into single logical lines.
    ///
    /// Rules:
    ///   1. A | not inside a string literal marks a continuation.
    ///      Everything from | to end-of-line is stripped (the compiler treats
    ///      that trailing text as a comment), and the next line is joined.
    ///   2. After joining, adjacent string literals are collapsed:
    ///      'abc' & 'def'  →  'abcdef'
    ///      This handles the pattern where a string was split across lines as
    ///      'part1' & '|' & 'part2'  (after join becomes 'part1' & 'part2').
    /// </summary>
    public static class Flattener
    {
        public static string Flatten(string input)
        {
            int dummy;
            return Flatten(input, -1, out dummy);
        }

        /// <summary>
        /// Finds the 0-based line index of the start of the continuation group
        /// that contains <paramref name="caretLine"/> (0-based).
        /// Walk backwards while the preceding line ends with a continuation pipe.
        /// </summary>
        public static int FindGroupStartLine(string input, int caretLine)
        {
            string[] lines = input.Split(new[] { "\r\n", "\r", "\n" }, System.StringSplitOptions.None);
            int i = caretLine;
            if (i < 0 || i >= lines.Length) return 0;
            // Walk backwards while the line above us has a continuation pipe
            // (meaning 'i' is a continuation of the previous line)
            while (i > 0 && FindContinuationPipe(lines[i - 1]) >= 0)
                i--;
            return i;
        }

        /// <summary>
        /// Flattens the input and maps inputCaretLine to the corresponding output line number.
        /// </summary>
        public static string Flatten(string input, int inputCaretLine, out int outputCaretLine)
        {
            outputCaretLine = inputCaretLine;
            string[] lines = input.Split(new[] { "\r\n", "\r", "\n" }, System.StringSplitOptions.None);
            string lineEnding = DetectLineEnding(input);

            var result = new List<string>();
            var lineMap = new int[lines.Length];
            int i = 0;
            while (i < lines.Length)
            {
                string current = lines[i];
                int outputLine = result.Count;
                lineMap[i] = outputLine;

                // Keep joining as long as this line has a continuation pipe
                while (true)
                {
                    int pipePos = FindContinuationPipe(current);
                    if (pipePos < 0) break;

                    // Strip from | to end of line
                    string before = current.Substring(0, pipePos).TrimEnd();

                    if (i + 1 >= lines.Length)
                    {
                        current = before;
                        break;
                    }

                    i++;
                    if (i < lines.Length) lineMap[i] = outputLine;
                    string next = lines[i].TrimStart();
                    current = (before.Length > 0 && next.Length > 0)
                        ? before + " " + next
                        : before + next;
                }

                // Collapse 'x' & 'y' -> 'xy' (may need multiple passes for chains)
                current = CollapseStringConcatenation(current);
                result.Add(current);
                i++;
            }

            if (inputCaretLine >= 0 && inputCaretLine < lineMap.Length)
                outputCaretLine = lineMap[inputCaretLine];

            return string.Join(lineEnding, result);
        }

        /// <summary>
        /// Returns the index of the first | that is outside a string literal,
        /// or -1 if none found.
        /// Handles Clarion's doubled-quote escape: '' inside a string is a
        /// literal quote character, not the end of the string.
        /// </summary>
        private static int FindContinuationPipe(string line)
        {
            bool inString = false;
            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];
                if (c == '\'')
                {
                    if (inString && i + 1 < line.Length && line[i + 1] == '\'')
                        i++; // doubled quote '' inside string — skip both, stay in string
                    else
                        inString = !inString;
                }
                else if (c == '|' && !inString)
                    return i;
            }
            return -1;
        }

        /// <summary>
        /// Removes ' & ' between adjacent string literals so 'a' & 'b' becomes 'ab'.
        /// Loops until stable to handle chains: 'a' & 'b' & 'c' -> 'abc'.
        /// </summary>
        private static string CollapseStringConcatenation(string line)
        {
            string prev = null;
            while (prev != line)
            {
                prev = line;
                line = Regex.Replace(line, @"'\s*&\s*'", "");
            }
            return line;
        }

        private static string DetectLineEnding(string text)
        {
            if (text.Contains("\r\n")) return "\r\n";
            if (text.Contains("\r")) return "\r";
            return "\n";
        }
    }
}

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.SharpDevelop.Gui;

namespace FlattenCode
{
    /// <summary>
    /// Shows the Flatten Code menu item only when the caret is inside or adjacent
    /// to a continuation block (a line that ends with | or whose predecessor does).
    /// </summary>
    public class HasContinuationCondition : IConditionEvaluator
    {
        public bool IsValid(object caller, Condition condition)
        {
            if (WorkbenchSingleton.Workbench == null) return false;
            var provider = WorkbenchSingleton.Workbench.ActiveContent as ITextEditorControlProvider;
            if (provider?.TextEditorControl?.ActiveTextAreaControl == null) return false;

            var doc  = provider.TextEditorControl.ActiveTextAreaControl.Document;
            int line = provider.TextEditorControl.ActiveTextAreaControl.Caret.Line;

            string[] lines = doc.TextContent.Split(new[] { "\r\n", "\r", "\n" },
                                System.StringSplitOptions.None);

            // Show if caret line has a pipe, or the line before it does
            return (line   < lines.Length && Flattener.FindContinuationPipe(lines[line])     >= 0)
                || (line-1 >= 0           && Flattener.FindContinuationPipe(lines[line - 1]) >= 0);
        }
    }
}

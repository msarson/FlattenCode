using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;

namespace FlattenCode
{
    public class FlattenCodeCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            IWorkbenchWindow window = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
            if (window == null) return;

            ITextEditorControlProvider provider = window.ActiveViewContent as ITextEditorControlProvider;
            if (provider == null) return;

            TextEditorControl tec = provider.TextEditorControl;
            IDocument doc = tec.Document;
            TextAreaControl area = tec.ActiveTextAreaControl;

            string originalText;
            int replaceOffset;
            int replaceLength;
            int caretLineBefore = area.Caret.Line;

            if (area.SelectionManager.HasSomethingSelected)
            {
                // Operate on selected text only
                ISelection sel = area.SelectionManager.SelectionCollection[0];
                originalText = sel.SelectedText;
                replaceOffset = sel.Offset;
                replaceLength = sel.Length;
                // caret line relative to selection start for mapping
                int selStartLine = doc.OffsetToPosition(replaceOffset).Y;
                caretLineBefore = caretLineBefore - selStartLine;
            }
            else
            {
                // Operate on entire document; jump to start of the group the caret is in
                originalText = doc.TextContent;
                replaceOffset = 0;
                replaceLength = doc.TextLength;
                caretLineBefore = Flattener.FindGroupStartLine(originalText, caretLineBefore);
            }

            int outputCaretLine;
            string flattened = Flattener.Flatten(originalText, caretLineBefore, out outputCaretLine);

            if (flattened == originalText)
            {
                MessageService.ShowMessage("No line continuations found to flatten.");
                return;
            }

            doc.Replace(replaceOffset, replaceLength, flattened);

            // Position caret at column 0 of the output line the cursor was on
            int targetLine = doc.OffsetToPosition(replaceOffset).Y + outputCaretLine;
            if (targetLine >= doc.TotalNumberOfLines)
                targetLine = doc.TotalNumberOfLines - 1;
            area.Caret.Position = new TextLocation(0, targetLine);
            area.ScrollToCaret();
        }
    }
}

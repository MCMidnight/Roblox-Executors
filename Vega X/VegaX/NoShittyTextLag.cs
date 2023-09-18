using System;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;

namespace VegaX
{
	public class NoShittyTextLag : VisualLineElementGenerator
	{
		public override int GetFirstInterestedOffset(int startOffset)
		{
			DocumentLine lastDocumentLine = base.CurrentContext.VisualLine.LastDocumentLine;
			if (lastDocumentLine.Length > 2000)
			{
				int num = lastDocumentLine.Offset + 2000 - 100 - "< Expand >".Length;
				if (startOffset <= num)
				{
					return num;
				}
			}
			return -1;
		}

		public override VisualLineElement ConstructElement(int offset)
		{
			return new FormattedTextElement("< Expand >", base.CurrentContext.VisualLine.LastDocumentLine.EndOffset - offset - 100);
		}

		const int maxLength = 2000;

		const string ellipsis = "< Expand >";

		const int charactersAfterEllipsis = 100;
	}
}

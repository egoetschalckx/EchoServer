using System;

namespace Tree
{
	public class TextInBox : ITreeNode
	{
		public long Id { get; private set; }
		public readonly String text;
		public double Height { get; private set; }
		public double Width { get; private set; }

		public TextInBox(long id, String text, int width, int height)
		{
			if (id < 0)
			{
				throw new ArgumentOutOfRangeException("id", "id must be greater than or equal to 0");
			}

			Id = id;
			this.text = text;
			Width = width;
			Height = height;
		}

		public override string ToString()
		{
			return String.Format("[{0}] {1} : ({2}, {3})", Id, text, Width, Height);
		}

		public bool Equals(TextInBox other)
		{
			return !ReferenceEquals(null, other) && Id == other.Id;
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode();
		}
	}
}

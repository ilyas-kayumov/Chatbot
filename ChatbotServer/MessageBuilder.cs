using System.Text;

public class MessageBuilder
{
    private readonly StringBuilder message = new StringBuilder();
    public const string NewLine = "$";

    public MessageBuilder AppendLine(string value)
    {
        message.Append(value).Append(NewLine);
        return this;
    }

    public override string ToString()
    {
        return message.ToString();
    }
}
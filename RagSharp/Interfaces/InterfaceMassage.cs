namespace RagSharp;


public class InterfaceMessage
{
    public string Sender { get; set; }
    public string? Recipient { get; set; }
    public string Content { get; set; }

    public override string ToString()
    {
        string result = $"{Sender} said to ";
        if(Recipient != null)
        {
            result += Recipient;
        }
        else{
            result += "the group";
        }
        result += $": \"{Content}\"";
        return result;
    }
}
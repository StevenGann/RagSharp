namespace RagSharp;

public interface IInterfaceProvider
{
    public void Setup();
    public void Update();
    public InterfaceMessage[]? GetMessages();
    public void Prompt(string prompt);
}
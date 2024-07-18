namespace RagSharp;

public interface ILLMProvider
{
    public void SetSystemPrompt(string prompt);
    public void SetContext(string[] context);
    public string Prompt(string prompt);
}
